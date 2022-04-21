// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Shaders;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

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
            var shaderManager = new ShaderManager(shaderFactory);
            var textureBuffer = GPUBufferFactory.CreateTextureGPUBuffer();
            var fontBuffer = GPUBufferFactory.CreateFontGPUBuffer();
            var rectBuffer = GPUBufferFactory.CreateRectGPUBuffer();
            var batchServiceManager = IoC.Container.GetInstance<IBatchServiceManager>();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            renderer = new Renderer(
                glInvoker,
                openGLService,
                shaderManager,
                textureBuffer,
                fontBuffer,
                rectBuffer,
                batchServiceManager,
                glInitReactor,
                shutDownReactor);

            renderer.RenderSurfaceWidth = renderSurfaceWidth;
            renderer.RenderSurfaceHeight = renderSurfaceHeight;

            return renderer;
        }
    }
}
