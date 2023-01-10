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
    private static IShaderProgram? textureShader;
    private static IShaderProgram? fontShader;
    private static IShaderProgram? rectShader;
    private static IShaderProgram? lineShader;

    /// <inheritdoc/>
    public IShaderProgram CreateTextureShader()
    {
        if (textureShader is not null)
        {
            return textureShader;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
        var reactable = IoC.Container.GetInstance<IPushReactable>();

        textureShader = new TextureShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        return textureShader;
    }

    /// <inheritdoc/>
    public IShaderProgram CreateFontShader()
    {
        if (fontShader is not null)
        {
            return fontShader;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
        var reactable = IoC.Container.GetInstance<IPushReactable>();

        fontShader = new FontShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        return fontShader;
    }

    /// <inheritdoc/>
    public IShaderProgram CreateRectShader()
    {
        if (rectShader is not null)
        {
            return rectShader;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
        var reactable = IoC.Container.GetInstance<IPushReactable>();

        rectShader = new RectangleShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        return rectShader;
    }

    /// <inheritdoc/>
    public IShaderProgram CreateLineShader()
    {
        if (lineShader is not null)
        {
            return lineShader;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
        var reactable = IoC.Container.GetInstance<IPushReactable>();

        lineShader = new LineShader(
            glInvoker,
            glInvokerExtensions,
            shaderLoaderService,
            reactable);

        return lineShader;
    }
}
