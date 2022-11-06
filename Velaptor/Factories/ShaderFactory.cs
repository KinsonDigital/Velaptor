// <copyright file="ShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.Factories;

/// <summary>
/// Creates instance of type <see cref="IShaderProgram"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ShaderFactory : IShaderFactory
{
    private static IShaderProgram? textureShader;
    private static IShaderProgram? fontShader;
    private static IShaderProgram? rectShader;

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
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        textureShader = new TextureShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitReactor, shutDownReactor);

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
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        fontShader = new FontShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitReactor, shutDownReactor);

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
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        rectShader = new RectangleShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitReactor, shutDownReactor);

        return rectShader;
    }
}
