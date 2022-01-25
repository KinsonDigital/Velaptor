// <copyright file="GPUBufferFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates singleton instances of <see cref="TextureGPUBuffer"/> and <see cref="FontGPUBuffer"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class GPUBufferFactory
    {
        private static IGPUBuffer<SpriteBatchItem>? textureBuffer;
        private static IGPUBuffer<SpriteBatchItem>? fontBuffer;

        /// <summary>
        /// Creates an instance of the <see cref="TextureGPUBuffer"/> class.
        /// </summary>
        /// <returns>A gpu buffer class.</returns>
        /// <remarks>
        ///     The instance is a singleton.  Every call to this method will return the same instance.
        /// </remarks>
        public static IGPUBuffer<SpriteBatchItem> CreateTextureGPUBuffer()
        {
            if (textureBuffer is not null)
            {
                return textureBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();

            textureBuffer = new TextureGPUBuffer(glInvoker, glInvokerExtensions, glInitObservable, shutDownObservable);

            return textureBuffer;
        }

        /// <summary>
        /// Creates an instance of the <see cref="FontGPUBuffer"/> class.
        /// </summary>
        /// <returns>A gpu buffer class.</returns>
        /// <remarks>
        ///     The instance is a singleton.  Every call to this method will return the same instance.
        /// </remarks>
        public static IGPUBuffer<SpriteBatchItem> CreateFontGPUBuffer()
        {
            if (fontBuffer is not null)
            {
                return fontBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();

            fontBuffer = new FontGPUBuffer(glInvoker, glInvokerExtensions, glInitObservable, shutDownObservable);

            return fontBuffer;
        }
    }
}
