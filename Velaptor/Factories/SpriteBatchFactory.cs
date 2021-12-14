// <copyright file="SpriteBatchFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.NativeInterop.FreeType;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Observables;
using Velaptor.Services;

namespace Velaptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;

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
            var glInvokerExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            var freeTypeInvoker = IoC.Container.GetInstance<IFreeTypeInvoker>();
            var textureShader = ShaderFactory.CreateTextureShader();
            var fontShader = ShaderFactory.CreateFontShader();
            var textureBuffer = GPUBufferFactory.CreateTextureGPUBuffer();
            var fontBuffer = GPUBufferFactory.CreateFontGPUBuffer();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();

            spriteBatch = new SpriteBatch(
                glInvoker,
                glInvokerExtensions,
                freeTypeInvoker,
                textureShader,
                fontShader,
                textureBuffer,
                fontBuffer,
                new TextureBatchService(),
                new FontBatchService(),
                glInitObservable);

            spriteBatch.RenderSurfaceWidth = renderSurfaceWidth;
            spriteBatch.RenderSurfaceHeight = renderSurfaceHeight;

            return spriteBatch;
        }
    }
}
