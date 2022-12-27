// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Graphics;
using NativeInterop.OpenGL;
using OpenGL.Buffers;
using OpenGL.Shaders;
using Services;

/// <summary>
/// Creates instances of the type <see cref="Renderer"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due direct interaction with IoC container.")]
public static class RendererFactory
{
    private static IRenderer? renderer;

    /// <summary>
    /// Initializes and instance of a <see cref="IRenderer"/>.
    /// </summary>
    /// <returns>A Velaptor implemented renderer.</returns>
    public static IRenderer CreateRenderer()
    {
        if (renderer is not null)
        {
            return renderer;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var openGLService = IoC.Container.GetInstance<IOpenGLService>();
        var shaderManager = IoC.Container.GetInstance<IShaderManager>();
        var bufferManager = IoC.Container.GetInstance<IBufferManager>();

        var batchServiceManager = IoC.Container.GetInstance<IBatchServiceManager>();
        var reactable = IoC.Container.GetInstance<IReactable>();

        renderer = new Renderer(
            glInvoker,
            openGLService,
            shaderManager,
            bufferManager,
            batchServiceManager,
            reactable);

        return renderer;
    }
}
