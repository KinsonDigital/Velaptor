// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates <see cref="ITexture"/> objects for rendering.
    /// </summary>
    internal class TextureFactory : ITextureFactory
    {
        private readonly IGLInvoker gl;
        private readonly IOpenGLService mockGLService;
        private readonly IReactable<DisposeTextureData> disposeTexturesReactable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureFactory"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public TextureFactory()
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.mockGLService = IoC.Container.GetInstance<IOpenGLService>();
            this.disposeTexturesReactable = IoC.Container.GetInstance<IReactable<DisposeTextureData>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureFactory"/> class.
        /// </summary>
        /// <param name="gl">Provides access to OpenGL functions.</param>
        /// <param name="openGLService">Provides OpenGL related helper methods.</param>
        /// <param name="disposeTexturesReactable">Sends push notifications to dispose of textures.</param>
        internal TextureFactory(IGLInvoker gl, IOpenGLService openGLService, IReactable<DisposeTextureData> disposeTexturesReactable)
        {
            this.gl = gl ?? throw new ArgumentNullException(nameof(gl), "The parameter must not be null.");
            this.mockGLService =
                openGLService ??
                throw new ArgumentNullException(nameof(openGLService), "The parameter must not be null.");
            this.disposeTexturesReactable =
                disposeTexturesReactable ??
                throw new ArgumentNullException(nameof(disposeTexturesReactable), "The parameter must not be null.");
        }

        /// <inheritdoc/>
        public ITexture Create(string name, string filePath, ImageData imageData)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The parameter must not be null or empty.");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "The parameter must not be null or empty.");
            }

            return new Texture(this.gl, this.mockGLService, this.disposeTexturesReactable, name, filePath, imageData);
        }
    }
}
