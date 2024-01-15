// <copyright file="ShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using OpenGL.Services;
using OpenGL.Shaders;

/// <summary>
/// Creates instance of type <see cref="IShaderProgram"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal sealed class ShaderFactory : IShaderFactory
{
    private readonly IShaderProgram textureShader;
    private readonly IShaderProgram fontShader;
    private readonly IShaderProgram shapeShader;
    private readonly IShaderProgram lineShader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderFactory"/> class.
    /// </summary>
    public ShaderFactory()
    {
        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService>();
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();

        this.textureShader = new TextureShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactableFactory);

        this.fontShader = new FontShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactableFactory);

        this.shapeShader = new ShapeShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactableFactory);

        this.lineShader = new LineShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactableFactory);
    }

    /// <inheritdoc/>
    public IShaderProgram CreateTextureShader() => this.textureShader;

    /// <inheritdoc/>
    public IShaderProgram CreateFontShader() => this.fontShader;

    /// <inheritdoc/>
    public IShaderProgram CreateShapeShader() => this.shapeShader;

    /// <inheritdoc/>
    public IShaderProgram CreateLineShader() => this.lineShader;
}
