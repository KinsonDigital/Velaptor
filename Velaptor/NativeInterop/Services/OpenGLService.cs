// <copyright file="OpenGLService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.Services;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Carbonate.Fluent;
using Carbonate.OneWay;
using Exceptions;
using OpenGL;
using Silk.NET.OpenGL;
using Velaptor.OpenGL;
using Velaptor.Services;

/// <summary>
/// Provides OpenGL helper methods to improve OpenGL related operations.
/// </summary>
internal sealed class OpenGLService : IOpenGLService
{
    // ReSharper disable InconsistentNaming
#pragma warning disable SA1310
    private const int API_ID_RECOMPILE_FRAGMENT_SHADER = 2;
    private const int API_ID_RECOMPILE_VERTEX_SHADER = 131218;
#pragma warning restore SA1310

    // ReSharper restore InconsistentNaming
    private readonly IGLInvoker glInvoker;
    private readonly IDotnetService dotnetService;
    private readonly ILoggingService loggingService;
    private DebugProc? debugCallback;

    // ReSharper disable once MemberInitializerValueIgnored
    private GL gl = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGLService"/> class.
    /// </summary>
    /// <param name="glInvoker">Invokes OpenGL functions.</param>
    /// <param name="glReactable">Sends and receives push notifications.</param>
    /// <param name="dotnetService">Invokes Dotnet functions.</param>
    /// <param name="loggingService">Logs messages to the console and files.</param>
    public OpenGLService(IGLInvoker glInvoker, IPushReactable<GL> glReactable, IDotnetService dotnetService, ILoggingService loggingService)
    {
        ArgumentNullException.ThrowIfNull(glInvoker);
        ArgumentNullException.ThrowIfNull(glReactable);
        ArgumentNullException.ThrowIfNull(loggingService);
        ArgumentNullException.ThrowIfNull(dotnetService);

        this.glInvoker = glInvoker;
        this.dotnetService = dotnetService;
        this.loggingService = loggingService;

        var glContextSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.GLContextCreatedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.GLContextCreatedId)))
            .BuildOneWayReceive<GL>(glObj =>
            {
                this.gl = glObj ??
                          throw new PushNotificationException(
                              $"{nameof(GLInvoker)}.Constructor()",
                              PushNotifications.GLContextCreatedId);
            });

        Console.WriteLine(this.gl);

        glReactable.Subscribe(glContextSubscription);
    }

    /// <inheritdoc/>
    public event EventHandler<GLErrorEventArgs>? GLError;

    /// <inheritdoc/>
    public bool IsVBOBound { get; private set; }

    /// <inheritdoc/>
    public bool IsEBOBound { get; private set; }

    /// <inheritdoc/>
    public bool IsVAOBound { get; private set; }

    /// <inheritdoc/>
    public Size GetViewPortSize()
    {
        /*
         * [0] = X
         * [1] = Y
         * [3] = Width
         * [4] = Height
         */
        var data = new int[4];

        this.glInvoker.GetInteger(GLGetPName.Viewport, data);

        return new Size(data[2], data[3]);
    }

    /// <inheritdoc/>
    public void SetViewPortSize(Size size)
    {
        /*
         * [0] = X
         * [1] = Y
         * [3] = Width
         * [4] = Height
         */
        var data = new int[4];

        this.glInvoker.GetInteger(GLGetPName.Viewport, data);

        this.glInvoker.Viewport(data[0], data[1], (uint)size.Width, (uint)size.Height);
    }

    /// <inheritdoc/>
    public Vector2 GetViewPortPosition()
    {
        /*
       * [0] = X
       * [1] = Y
       * [3] = Width
       * [4] = Height
       */
        var data = new int[4];

        this.glInvoker.GetInteger(GLGetPName.Viewport, data);

        return new Vector2(data[0], data[1]);
    }

    /// <inheritdoc/>
    public void BindVBO(uint vbo)
    {
        this.glInvoker.BindBuffer(GLBufferTarget.ArrayBuffer, vbo);
        IsVBOBound = true;
    }

    /// <inheritdoc/>
    public void UnbindVBO()
    {
        this.glInvoker.BindBuffer(GLBufferTarget.ArrayBuffer, 0u);
        IsVBOBound = false;
    }

    /// <inheritdoc/>
    public void BindEBO(uint ebo)
    {
        this.glInvoker.BindBuffer(GLBufferTarget.ElementArrayBuffer, ebo);
        IsEBOBound = true;
    }

    /// <inheritdoc/>
    public void UnbindEBO()
    {
        if (IsVAOBound)
        {
            throw new InvalidOperationException("The VAO object must be unbound before unbinding an EBO object.");
        }

        this.glInvoker.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);
        IsEBOBound = false;
    }

    /// <inheritdoc/>
    public void BindVAO(uint vao)
    {
        this.glInvoker.BindVertexArray(vao);
        IsVAOBound = true;
    }

    /// <inheritdoc/>
    public void UnbindVAO()
    {
        this.glInvoker.BindVertexArray(0);
        IsVAOBound = false;
    }

    /// <inheritdoc/>
    public void BindTexture2D(uint textureId) => this.glInvoker.BindTexture(GLTextureTarget.Texture2D, textureId);

    /// <inheritdoc/>
    public void UnbindTexture2D() => this.glInvoker.BindTexture(GLTextureTarget.Texture2D, 0u);

    /// <inheritdoc/>
    public bool ProgramLinkedSuccessfully(uint programId)
    {
        var programParams = this.glInvoker.GetProgram(programId, GLProgramParameterName.LinkStatus);

        return programParams >= 1;
    }

    /// <inheritdoc/>
    public bool ShaderCompiledSuccessfully(uint shaderId)
    {
        var shaderParams = this.glInvoker.GetShader(shaderId, GLShaderParameter.CompileStatus);

        return shaderParams >= 1;
    }

    /// <inheritdoc/>
    public void BeginGroup(string label)
        =>
            this.glInvoker.PushDebugGroup(
                GLDebugSource.DebugSourceApplication,
                100,
                (uint)label.Length,
                label);

    /// <inheritdoc/>
    public void EndGroup() => this.glInvoker.PopDebugGroup();

    /// <inheritdoc/>
    public void LabelShader(uint shaderId, string label)
        => this.glInvoker.ObjectLabel(GLObjectIdentifier.Shader, shaderId, (uint)label.Length, label);

    /// <inheritdoc/>
    public void LabelShaderProgram(uint shaderId, string label)
        => this.glInvoker.ObjectLabel(GLObjectIdentifier.Program, shaderId, (uint)label.Length, label);

    /// <inheritdoc/>
    public void LabelVertexArray(uint vertexArrayId, string label)
    {
        label = string.IsNullOrEmpty(label)
            ? "NOT SET"
            : label;

        var newLabel = $"{label} VAO";

        this.glInvoker.ObjectLabel(GLObjectIdentifier.VertexArray, vertexArrayId, (uint)newLabel.Length, newLabel);
    }

    /// <inheritdoc/>
    public void LabelBuffer(uint bufferId, string label, OpenGLBufferType bufferType)
    {
        label = string.IsNullOrEmpty(label)
            ? "NOT SET"
            : label;

        var bufferTypeAcronym = bufferType switch
        {
            OpenGLBufferType.VertexBufferObject => "VBO",
            OpenGLBufferType.IndexArrayObject => "EBO",
            _ => throw new InvalidEnumArgumentException(nameof(bufferType), (int)bufferType, typeof(OpenGLBufferType))
        };

        var newLabel = $"{label} {bufferTypeAcronym}";

        this.glInvoker.ObjectLabel(GLObjectIdentifier.Buffer, bufferId, (uint)newLabel.Length, newLabel);
    }

    /// <inheritdoc/>
    public void LabelTexture(uint textureId, string label)
    {
        label = string.IsNullOrEmpty(label)
            ? "NOT SET"
            : label;

        this.glInvoker.ObjectLabel(GLObjectIdentifier.Texture, textureId, (uint)label.Length, label);
    }

    /// <inheritdoc/>
    public void SetupErrorCallback()
    {
        if (this.debugCallback is not null)
        {
            return;
        }

        this.debugCallback = DebugCallback;

        /*NOTE:
         * This is here to help prevent an issue with an obscure System.ExecutionException from occurring.
         * The garbage collector performs a collect on the delegate passed into GL.DebugMessageCallback()
         * without the native system knowing about it which causes this exception. The GC.KeepAlive()
         * method tells the garbage collector to not collect the delegate to prevent this from happening.
         */
        this.dotnetService.GCKeepAlive(this.debugCallback);

        this.gl.DebugMessageCallback(this.debugCallback, this.dotnetService.MarshalStringToHGlobalAnsi(string.Empty));
    }

    /// <summary>
    /// Invoked when there is an OpenGL related error.
    /// </summary>
    /// <param name="source">The debug source.</param>
    /// <param name="type">The debug type.</param>
    /// <param name="id">The ID of the error or message.</param>
    /// <param name="severity">The severity of the message.</param>
    /// <param name="length">The length of the message.</param>
    /// <param name="message">The error message.</param>
    /// <param name="userParam">The OpenGL parameter related to the error.</param>
    private void DebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
    {
        var openGLMessage = this.dotnetService.MarshalPtrToStringAnsi(message);

        openGLMessage += $"{Environment.NewLine}\tSrc: {source}";
        openGLMessage += $"{Environment.NewLine}\tType: {type}";
        openGLMessage += $"{Environment.NewLine}\tID: {id}";
        openGLMessage += $"{Environment.NewLine}\tSeverity: {severity}";
        openGLMessage += $"{Environment.NewLine}\tLength: {length}";
        openGLMessage += $"{Environment.NewLine}\tUser Param: {this.dotnetService.MarshalPtrToStringAnsi(userParam)}";

        // Ignore warnings about shader recompilation
        if (id is API_ID_RECOMPILE_VERTEX_SHADER or API_ID_RECOMPILE_FRAGMENT_SHADER)
        {
            return;
        }

        if (severity == GLEnum.NoError)
        {
            this.loggingService.Warning(openGLMessage);
        }
        else
        {
            if (severity != GLEnum.DebugSeverityNotification)
            {
                this.loggingService.Error(openGLMessage);
                this.GLError?.Invoke(this, new GLErrorEventArgs(openGLMessage));
            }
        }
    }
}
