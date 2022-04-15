// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
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
            var textureShader = ShaderFactory.CreateTextureShader();
            var fontShader = ShaderFactory.CreateFontShader();
            var rectShader = ShaderFactory.CreateRectShader();
            var textureBuffer = GPUBufferFactory.CreateTextureGPUBuffer();
            var fontBuffer = GPUBufferFactory.CreateFontGPUBuffer();
            var rectBuffer = GPUBufferFactory.CreateRectGPUBuffer();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();
            var textureBatchingService = IoC.Container.GetInstance<IBatchingService<TextureBatchItem>>();
            var fontBatchingService = IoC.Container.GetInstance<IBatchingService<FontGlyphBatchItem>>();
            var rectBatchingService = IoC.Container.GetInstance<IBatchingService<RectShape>>();

            renderer = new Renderer(
                glInvoker,
                openGLService,
                textureShader,
                fontShader,
                rectShader,
                textureBuffer,
                fontBuffer,
                rectBuffer,
                textureBatchingService,
                fontBatchingService,
                rectBatchingService,
                glInitReactor,
                shutDownReactor);

            renderer.RenderSurfaceWidth = renderSurfaceWidth;
            renderer.RenderSurfaceHeight = renderSurfaceHeight;

            return renderer;
        }
    }
}
