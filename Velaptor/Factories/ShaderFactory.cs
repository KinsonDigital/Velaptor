// <copyright file="ShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Carbonate;
using NativeInterop.OpenGL;
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
    private readonly IShaderProgram rectShader;
    private readonly IShaderProgram lineShader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderFactory"/> class.
    /// </summary>
    public ShaderFactory()
    {
        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
        var reactable = IoC.Container.GetInstance<IPushReactable>();

        this.textureShader = new TextureShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        this.fontShader = new FontShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        this.rectShader = new RectangleShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        this.lineShader = new LineShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);
    }

    /// <inheritdoc/>
    public IShaderProgram CreateTextureShader() => this.textureShader;

    /// <inheritdoc/>
    public IShaderProgram CreateFontShader() => this.fontShader;

    /// <inheritdoc/>
    public IShaderProgram CreateRectShader() => this.rectShader;

    /// <inheritdoc/>
    public IShaderProgram CreateLineShader() => this.lineShader;
}
