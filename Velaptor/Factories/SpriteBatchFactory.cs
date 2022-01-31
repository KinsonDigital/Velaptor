// <copyright file="SpriteBatchFactory.cs" company="KinsonDigital">
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
    /// Creates instances of the type <see cref="SpriteBatch"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SpriteBatchFactory
    {
        private static ISpriteBatch? spriteBatch;

        /// <summary>
        /// Initializes and instance of a <see cref="ISpriteBatch"/>.
        /// </summary>
        /// <param name="renderSurfaceWidth">The width of the render surface.</param>
        /// <param name="renderSurfaceHeight">The height of the render surface.</param>
        /// <returns>A Velaptor implemented sprite batch.</returns>
        public static ISpriteBatch CreateSpriteBatch(uint renderSurfaceWidth, uint renderSurfaceHeight)
        {
            if (spriteBatch is not null)
            {
                return spriteBatch;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
            var textureShader = ShaderFactory.CreateTextureShader();
            var fontShader = ShaderFactory.CreateFontShader();
            var textureBuffer = GPUBufferFactory.CreateTextureGPUBuffer();
            var fontBuffer = GPUBufferFactory.CreateFontGPUBuffer();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();
            var textureBatchService = IoC.Container.GetInstance<IBatchManagerService<SpriteBatchItem>>();
            var fontBatchService = IoC.Container.GetInstance<IBatchManagerService<SpriteBatchItem>>();

            spriteBatch = new SpriteBatch(
                glInvoker,
                glInvokerExtensions,
                textureShader,
                fontShader,
                textureBuffer,
                fontBuffer,
                textureBatchService,
                fontBatchService,
                glInitReactor,
                shutDownReactor);

            spriteBatch.RenderSurfaceWidth = renderSurfaceWidth;
            spriteBatch.RenderSurfaceHeight = renderSurfaceHeight;

            return spriteBatch;
        }
    }
}
