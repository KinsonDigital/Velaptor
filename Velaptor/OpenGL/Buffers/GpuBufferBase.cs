// <copyright file="GpuBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Factories;
using NativeInterop.OpenGL;
using NativeInterop.Services;

/// <summary>
/// Base functionality for managing buffer data in the GPU.
/// </summary>
/// <typeparam name="TData">The type of data in the GPU buffer.</typeparam>
internal abstract class GpuBufferBase<TData> : IGpuBuffer<TData>
    where TData : struct
{
    private readonly IDisposable initUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly IDisposable portSizeUnsubscriber;
    private uint ebo; // Element Buffer Object

    /// <summary>
    /// Initializes a new instance of the <see cref="GpuBufferBase{TData}"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    private protected GpuBufferBase(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(gl);
        ArgumentNullException.ThrowIfNull(openGLService);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        GL = gl;
        OpenGLService = openGLService;

        var reactable = reactableFactory.CreateNoDataPushReactable();

        // Subscribe to GL initialized signal
        this.initUnsubscriber = reactable.CreateNonReceiveOrRespond(
            PushNotifications.GLInitializedId,
            Init,
            () => this.initUnsubscriber?.Dispose());

        // Subscribe to the shut-down signal
        this.shutDownUnsubscriber = reactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.shutDownUnsubscriber?.Dispose());

        // Subscribe to port size change notifications
        var portSizeReactable = reactableFactory.CreateViewPortReactable();

        this.portSizeUnsubscriber = portSizeReactable.CreateOneWayReceive(
            PushNotifications.ViewPortSizeChangedId,
            (data) =>
            {
                ViewPortSize = new SizeU(data.Width, data.Height);
            },
            () => this.portSizeUnsubscriber?.Dispose());

        ProcessCustomAttributes();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GpuBufferBase{TData}"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "De-constructors cannot be unit tested.")]
    ~GpuBufferBase()
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
    /// Gets a value indicating whether the buffer has been initialized.
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
    /// Gets or sets a value indicating whether the buffer has been disposed.
    /// </summary>
    private bool IsDisposed { get; set; }

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
    /// Resizes the batch of data in the GPU.
    /// </summary>
    protected void ResizeBatch()
    {
        OpenGLService.BindVAO(VAO);

        OpenGLService.BeginGroup($"Set size of {Name} Vertex Data");

        var vertBufferData = GenerateData();

        OpenGLService.BindVBO(VBO);
        GL.BufferData(GLBufferTarget.ArrayBuffer, vertBufferData, GLBufferUsageHint.DynamicDraw);

        OpenGLService.EndGroup();

        OpenGLService.BeginGroup($"Set size of {Name} Indices Data");

        var indices = GenerateIndices();

        // Configure the Vertex Attribute so that OpenGL knows how to read the VBO
        OpenGLService.BindEBO(this.ebo);
        GL.BufferData(GLBufferTarget.ElementArrayBuffer, indices, GLBufferUsageHint.StaticDraw);

        OpenGLService.UnbindVBO();
        OpenGLService.UnbindVAO();
        OpenGLService.UnbindEBO();

        OpenGLService.EndGroup();
    }

    /// <summary>
    /// Shuts down the application by disposing of resources.
    /// </summary>
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Kept for future use.")]
    protected virtual void ShutDown()
    {
        if (IsDisposed)
        {
            return;
        }

        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(this.ebo);

        IsDisposed = true;
    }

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

        IsInitialized = true;

        SetupVAO();

        ResizeBatch();

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

        if (currentType == typeof(TextureGpuBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(TextureGpuBuffer));
        }
        else if (currentType == typeof(FontGpuBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(FontGpuBuffer));
        }
        else if (currentType == typeof(ShapeGpuBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(ShapeGpuBuffer));
        }
        else if (currentType == typeof(LineGpuBuffer))
        {
            attributes = Attribute.GetCustomAttributes(typeof(LineGpuBuffer));
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
            if (attribute is GpuBufferNameAttribute nameAttribute)
            {
                Name = nameAttribute.Name;
            }
        }
    }
}
