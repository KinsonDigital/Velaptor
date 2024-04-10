﻿// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Exceptions;
using Factories;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using Services;

/// <inheritdoc/>
internal abstract class ShaderProgram : IShaderProgram
{
    private readonly IShaderLoaderService shaderLoaderService;
    private readonly IDisposable glInitReactorUnsubscriber;
    private readonly IDisposable shutDownReactorUnsubscriber;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    /// <exception cref="ShaderCompileException">Thrown when the shader has issues compiling.</exception>
    /// <exception cref="ShaderLinkException">Thrown when the shader has issues linking.</exception>
    private protected ShaderProgram(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService shaderLoaderService,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(gl);
        ArgumentNullException.ThrowIfNull(openGLService);
        ArgumentNullException.ThrowIfNull(shaderLoaderService);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        GL = gl;
        OpenGLService = openGLService;
        this.shaderLoaderService = shaderLoaderService;

        var reactable = reactableFactory.CreateNoDataPushReactable();

        ProcessCustomAttributes();

        this.glInitReactorUnsubscriber = reactable.CreateNonReceiveOrRespond(
            PushNotifications.GLInitializedId,
            Init,
            () => this.glInitReactorUnsubscriber?.Dispose(),
            callerMemberName: Name);

        this.shutDownReactorUnsubscriber = reactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.shutDownReactorUnsubscriber?.Dispose(),
            callerMemberName: Name);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ShaderProgram"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "De-constructors cannot be unit tested.")]
    ~ShaderProgram()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        ShutDown();
    }

    /// <inheritdoc/>
    public uint ShaderId { get; private set; }

    /// <inheritdoc/>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the batch.
    /// </summary>
    public uint BatchSize { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ShaderProgram"/> is disposed.
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "MemberCanBePrivate.Global",
        Justification = "Left of inheriting members to use.")]
    protected bool IsDisposed { get; set; }

    /// <summary>
    /// Gets invokes OpenGL functions.
    /// </summary>
    private protected IGLInvoker GL { get; }

    /// <summary>
    /// Gets the OpenGL service that provides helper methods for OpenGL related operations.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Intended to be available in classes inheriting this class.")]
    private protected IOpenGLService OpenGLService { get; }

    /// <summary>
    /// <inheritdoc cref="IShaderProgram.Use"/>
    /// </summary>
    /// <exception cref="ShaderNotInitializedException">
    ///     Thrown when invoked without the shader being initialized.
    /// </exception>
    public virtual void Use()
    {
        if (!this.isInitialized)
        {
            throw new ShaderNotInitializedException("The shader has not been initialized.");
        }

        GL.UseProgram(ShaderId);
    }

    /// <summary>
    /// Shuts down the application by disposing of any resources.
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "VirtualMemberNeverOverridden.Global",
        Justification = "Will be used in the future.")]
    protected virtual void ShutDown()
    {
        if (IsDisposed)
        {
            return;
        }

        GL.DeleteProgram(ShaderId);

        IsDisposed = true;
    }

    /// <summary>
    /// Initializes the shader program by compiling the vertex and fragment shaders.
    /// </summary>
    /// <exception cref="ShaderCompileException">
    ///     Thrown if the shader compilation throws an error.
    /// </exception>
    private void Init()
    {
        if (this.isInitialized)
        {
            return;
        }

        OpenGLService.BeginGroup($"Load {Name} Vertex Shader");

        var vertShaderSrc = this.shaderLoaderService.LoadVertSource(Name);
        var vertShaderId = GL.CreateShader(GLShaderType.VertexShader);

        OpenGLService.LabelShader(vertShaderId, $"{Name} Vertex Shader");

        GL.ShaderSource(vertShaderId, vertShaderSrc);
        GL.CompileShader(vertShaderId);

        // Checking the shader for compilation errors.
        var infoLog = GL.GetShaderInfoLog(vertShaderId);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            var exceptionMsg = $"Error compiling vertex shader '{Name}' with shader ID '{vertShaderId}'.{Environment.NewLine}{infoLog}";
            throw new ShaderCompileException(exceptionMsg);
        }

        OpenGLService.EndGroup();

        OpenGLService.BeginGroup($"Load {Name} Fragment Shader");

        var fragShaderSrc = this.shaderLoaderService.LoadFragSource(Name);
        var fragShaderId = GL.CreateShader(GLShaderType.FragmentShader);

        OpenGLService.LabelShader(fragShaderId, $"{Name} Fragment Shader");

        GL.ShaderSource(fragShaderId, fragShaderSrc);
        GL.CompileShader(fragShaderId);

        // Checking the shader for compilation errors.
        infoLog = GL.GetShaderInfoLog(fragShaderId);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            var exceptionMsg = $"Error compiling fragment shader '{Name}' with shader ID '{fragShaderId}'.{Environment.NewLine}{infoLog}";
            throw new ShaderCompileException(exceptionMsg);
        }

        OpenGLService.EndGroup();

        CreateProgram(Name, vertShaderId, fragShaderId);
        CleanShadersIfReady(Name, vertShaderId, fragShaderId);

        this.isInitialized = true;
    }

    /// <summary>
    /// Creates an <c>OpenGL</c> shader program using the given <paramref name="shaderName"/>,
    /// <paramref name="vertShaderId"/>, and <paramref name="fragShaderId"/>.
    /// </summary>
    /// <param name="shaderName">The name to give the shader program.</param>
    /// <param name="vertShaderId">The <c>OpenGL</c> vertex shader ID.</param>
    /// <param name="fragShaderId">The <c>OpenGL</c> fragment shader ID.</param>
    /// <exception cref="ShaderLinkException">
    ///     Thrown if the linking process during shader compilation throws an error.
    /// </exception>
    private void CreateProgram(string shaderName, uint vertShaderId, uint fragShaderId)
    {
        OpenGLService.BeginGroup($"Create {shaderName} Shader Program");

        // Combining the shaders under one shader program.
        ShaderId = GL.CreateProgram();

        OpenGLService.LabelShaderProgram(ShaderId, $"{shaderName} Shader Program");

        GL.AttachShader(ShaderId, vertShaderId);
        GL.AttachShader(ShaderId, fragShaderId);

        // Link and check for for errors.
        GL.LinkProgram(ShaderId);
        var status = GL.GetProgram(ShaderId, GLProgramParameterName.LinkStatus);
        if (status == 0)
        {
            var exceptionMsg = $"Error linking shader with ID '{ShaderId}'{Environment.NewLine}{GL.GetProgramInfoLog(ShaderId)}";
            throw new ShaderLinkException(exceptionMsg);
        }

        OpenGLService.EndGroup();
    }

    /// <summary>
    /// Performs cleanup after the vertex and fragments shaders are compiled, linked,
    /// and sent to the GPU.
    /// </summary>
    /// <param name="name">The name of the shader program.</param>
    /// <param name="vertShaderId">The <c>OpenGL</c> vertex shader ID.</param>
    /// <param name="fragShaderId">The <c>OpenGL</c> fragment shader ID.</param>
    /// <remarks>
    ///     The vertex and fragment shader code is not needed on the CPU side
    ///     once the GPU receives the shader code result.
    /// </remarks>
    private void CleanShadersIfReady(string name, uint vertShaderId, uint fragShaderId)
    {
        OpenGLService.BeginGroup($"Clean Up {name} Vertex Shader");

        GL.DetachShader(ShaderId, vertShaderId);
        GL.DeleteShader(vertShaderId);

        OpenGLService.EndGroup();

        OpenGLService.BeginGroup($"Clean Up {name} Fragment Shader");

        // Delete the no longer useful individual shaders
        GL.DetachShader(ShaderId, fragShaderId);
        GL.DeleteShader(fragShaderId);

        OpenGLService.EndGroup();
    }

    /// <summary>
    /// Processes any custom attributes that may or may not be on the implementing class.
    /// </summary>
    private void ProcessCustomAttributes()
    {
        Attribute[]? attributes = null;
        var currentType = GetType();

        if (currentType == typeof(TextureShader))
        {
            attributes = Attribute.GetCustomAttributes(typeof(TextureShader));
        }
        else if (currentType == typeof(FontShader))
        {
            attributes = Attribute.GetCustomAttributes(typeof(FontShader));
        }
        else if (currentType == typeof(ShapeShader))
        {
            attributes = Attribute.GetCustomAttributes(typeof(ShapeShader));
        }
        else if (currentType == typeof(LineShader))
        {
            attributes = Attribute.GetCustomAttributes(typeof(LineShader));
        }
        else
        {
            Name = "UNKNOWN";
        }

        if (attributes is null || attributes.Length <= 0)
        {
            return;
        }

        foreach (var attribute in attributes)
        {
            if (attribute is ShaderNameAttribute shaderNameAttribute)
            {
                Name = shaderNameAttribute.Name;
            }
        }
    }
}
