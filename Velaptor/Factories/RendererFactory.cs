// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Graphics;
using Velaptor.NativeInterop.OpenGL;
using OpenGL.Buffers;
using OpenGL.Shaders;
using Services;

/// <summary>
/// Creates instances of the type <see cref="Renderer"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class RendererFactory
{
    private static IRenderer? renderer;

    /// <summary>
    /// Initializes and instance of a <see cref="IRenderer"/>.
    /// </summary>
    /// <param name="renderSurfaceWidth">The width of the render surface.</param>
    /// <param name="renderSurfaceHeight">The height of the render surface.</param>
    /// <returns>A Velaptor implemented renderer.</returns>
    public static IRenderer CreateRenderer(uint renderSurfaceWidth, uint renderSurfaceHeight)
    {
        if (renderer is not null)
        {
            return renderer;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var openGLService = IoC.Container.GetInstance<IOpenGLService>();
        var shaderFactory = IoC.Container.GetInstance<IShaderFactory>();
        var bufferFactory = IoC.Container.GetInstance<IGPUBufferFactory>();
        var shaderManager = new ShaderManager(shaderFactory);
        var bufferManager = new BufferManager(bufferFactory);

        var batchServiceManager = IoC.Container.GetInstance<IBatchServiceManager>();
        var reactable = IoC.Container.GetInstance<IReactable>();

        renderer = new Renderer(
            glInvoker,
            openGLService,
            shaderManager,
            bufferManager,
            batchServiceManager,
            reactable);

        renderer.RenderSurfaceWidth = renderSurfaceWidth;
        renderer.RenderSurfaceHeight = renderSurfaceHeight;

        return renderer;
    }
}
