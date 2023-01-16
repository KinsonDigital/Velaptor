// <copyright file="GPUBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using ReactableData;
using Velaptor.Exceptions;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Base functionality for managing buffer data in the GPU.
/// </summary>
/// <typeparam name="TData">The type of data in the GPU buffer.</typeparam>
internal abstract class GPUBufferBase<TData> : IGPUBuffer<TData>
    where TData : struct
{
    private readonly IDisposable shutDownUnsubscriber;
    private readonly IDisposable glInitUnsubscriber;
    private readonly IDisposable viewPortSizeUnsubscriber;
    private uint ebo; // Element Buffer Object
    private uint[] indices = Array.Empty<uint>();
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GPUBufferBase{TData}"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    internal GPUBufferBase(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(reactableFactory);

        GL = gl;
        OpenGLService = openGLService;

        var pushReactable = reactableFactory.CreateNoDataReactable();
        var portSizeReactable = reactableFactory.CreateViewPortReactable();

        var glInitName = this.GetExecutionMemberName(nameof(PushNotifications.GLInitializedId));
        this.glInitUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.GLInitializedId,
            name: glInitName,
            onReceive: Init,
            onUnsubscribe: () => this.glInitUnsubscriber?.Dispose()));

        var shutDownName = this.GetExecutionMemberName(nameof(PushNotifications.SystemShuttingDownId));
        this.shutDownUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.SystemShuttingDownId,
            name: shutDownName,
            onReceive: ShutDown));

        var viewPortName = this.GetExecutionMemberName(nameof(PushNotifications.ViewPortSizeChangedId));
        this.viewPortSizeUnsubscriber = portSizeReactable.Subscribe(new ReceiveReactor<ViewPortSizeData>(
            eventId: PushNotifications.ViewPortSizeChangedId,
            name: viewPortName,
            onReceiveMsg: msg =>
            {
                var data = msg.GetData();

                if (data is null)
                {
                    throw new PushNotificationException(
                        $"{nameof(GPUBufferBase<TData>)}.Constructor()",
                        PushNotifications.ViewPortSizeChangedId);
                }

                ViewPortSize = new SizeU(data.Width, data.Height);
            }, onUnsubscribe: () => this.viewPortSizeUnsubscriber?.Dispose()));

        ProcessCustomAttributes();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GPUBufferBase{TData}"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "De-constructors cannot be unit tested.")]
    ~GPUBufferBase()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        ShutDown();
    }

    /// <summary>
    /// Gets or sets the size of the batch.
    /// </summary>
    protected internal uint BatchSize { get; protected set; } = 100;

    /// <summary>
    /// Gets a value indicating whether or not the buffer has been initialized.
    /// </summary>
    protected internal bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the name of the buffer.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
    protected internal string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the size of the viewport.
    /// </summary>
    protected SizeU ViewPortSize { get; private set; }

    /// <summary>
    /// Gets the invoker that makes OpenGL calls.
    /// </summary>
    private protected IGLInvoker GL { get; }

    /// <summary>
    /// Gets the OpenGL service that provides helper methods for OpenGL related operations.
    /// </summary>
    private protected IOpenGLService OpenGLService { get; }

    /// <summary>
    /// Gets the ID of the vertex array object.
    /// </summary>
    private protected uint VAO { get; private set; }

    /// <summary>
    /// Gets the ID of the vertex buffer object.
    /// </summary>
    private protected uint VBO { get; private set; }

    /// <summary>
    /// Updates GPU buffer with the given <paramref name="data"/> at the given <paramref name="batchIndex"/>.
    /// </summary>
    /// <param name="data">The data to send to the GPU.</param>
    /// <param name="batchIndex">The index of the batch of data to update.</param>
    public void UploadData(TData data, uint batchIndex)
    {
        PrepareForUpload();
        UploadVertexData(data, batchIndex);
    }

    /// <summary>
    /// Updates the vertex data in the GPU.
    /// </summary>
    /// <param name="data">The data to upload.</param>
    /// <param name="batchIndex">The index location of the data in a batch of data to upload.</param>
    protected internal abstract void UploadVertexData(TData data, uint batchIndex);

    /// <summary>
    /// Prepares anything that might need to be done before sending data to the GPU.
    /// </summary>
    /// <summary>
    ///     This could be binding buffers, setting state, etc.
    /// </summary>
    protected internal abstract void PrepareForUpload();

    /// <summary>
    /// Generates the data to be sent to the GPU.
    /// </summary>
    /// <returns>The data to be sent to the GPU.</returns>
    protected internal abstract float[] GenerateData();

    /// <summary>
    /// Sets up the vertex array object to describe to the GPU how
    /// the vertex array data is laid out and how it should be used.
    /// </summary>
    protected internal abstract void SetupVAO();

    /// <summary>
    /// Generates the data for the indices that are used in the vertex array object.
    /// </summary>
    /// <returns>The indices data.</returns>
    protected internal abstract uint[] GenerateIndices();

    /// <summary>
    /// Initializes the GPU buffer.
    /// </summary>
    private void Init()
    {
        // Generate the VAO and VBO with only 1 object each
        VAO = GL.GenVertexArray();
        OpenGLService.BindVAO(VAO);
        OpenGLService.LabelVertexArray(VAO, Name);

        VBO = GL.GenBuffer();
        OpenGLService.BindVBO(VBO);
        OpenGLService.LabelBuffer(VBO, Name, OpenGLBufferType.VertexBufferObject);

        this.ebo = GL.GenBuffer();
        OpenGLService.BindEBO(this.ebo);
        OpenGLService.LabelBuffer(this.ebo, Name, OpenGLBufferType.IndexArrayObject);

        OpenGLService.BeginGroup($"Setup {Name} Data");
        OpenGLService.BeginGroup($"Upload {Name} Vertex Data");

        IsInitialized = true;

        var vertBufferData = GenerateData();

        GL.BufferData(GLBufferTarget.ArrayBuffer, vertBufferData, GLBufferUsageHint.DynamicDraw);

        OpenGLService.EndGroup();

        OpenGLService.BeginGroup($"Upload {Name} Indices Data");

        this.indices = GenerateIndices();

        // Configure the Vertex Attribute so that OpenGL knows how to read the VBO
        GL.BufferData(GLBufferTarget.ElementArrayBuffer, this.indices, GLBufferUsageHint.StaticDraw);
        OpenGLService.EndGroup();

        SetupVAO();

        OpenGLService.UnbindVBO();
        OpenGLService.UnbindVAO();
        OpenGLService.UnbindEBO();
        OpenGLService.EndGroup();
    }

    /// <summary>
    /// Looks for and pulls settings out of various attributes to help set the state of the buffer.
    /// </summary>
    private void ProcessCustomAttributes()
    {
        Attribute[]? attributes = null;
        var currentType = GetType();

        if (currentType == typeof(TextureGPUBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(TextureGPUBuffer));
        }
        else if (currentType == typeof(FontGPUBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(FontGPUBuffer));
        }
        else if (currentType == typeof(RectGPUBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(RectGPUBuffer));
        }
        else if (currentType == typeof(LineGPUBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(LineGPUBuffer));
        }
        else
        {
            Name = "UNKNOWN BUFFER";
        }

        if (attributes is null || attributes.Length <= 0)
        {
            return;
        }

        foreach (var attribute in attributes)
        {
            if (attribute is GPUBufferNameAttribute nameAttribute)
            {
                Name = nameAttribute.Name;
            }
        }
    }

    /// <summary>
    /// Shuts down the application by disposing of resources.
    /// </summary>
    private void ShutDown()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.shutDownUnsubscriber.Dispose();
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(this.ebo);

        this.isDisposed = true;
    }
}
