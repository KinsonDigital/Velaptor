// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.IO;
    using Raptor.Audio;
    using Raptor.Exceptions;
    using Raptor.Graphics;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<ISound> soundLoader;
        private readonly ILoader<IAtlasData> atlasLoader;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="soundLoader">Loads sounds.</param>
        /// <param name="atlasLoader">Loads a texture atlas.</param>
        public ContentLoader(ILoader<ITexture> textureLoader, ILoader<ISound> soundLoader, ILoader<IAtlasData> atlasLoader)
        {
            this.textureLoader = textureLoader;
            this.soundLoader = soundLoader;
            this.atlasLoader = atlasLoader;
        }

        /// <inheritdoc/>
        public T Load<T>(string name)
            where T : class, IContent
        {
            name = Path.GetFileNameWithoutExtension(name);

            if (typeof(T) == typeof(ITexture))
            {
                return (T)this.textureLoader.Load(name);
            }

            if (typeof(T) == typeof(ISound))
            {
                return (T)this.soundLoader.Load(name);
            }

            if (typeof(T) == typeof(IAtlasData))
            {
                return (T)this.atlasLoader.Load(name);
            }

            throw new UnknownContentException($"Content of type '{typeof(T)}' invalid.  Content types must inherit from interface '{nameof(IContent)}'.");
        }

        /// <inheritdoc/>
        public void Unload<T>(string name)
            where T : class, IContent
        {
            name = Path.GetFileNameWithoutExtension(name);

            if (typeof(T) == typeof(ITexture))
            {
                this.textureLoader.Unload(name);
                return;
            }

            if (typeof(T) == typeof(IAtlasData))
            {
                this.atlasLoader.Unload(name);
                return;
            }

            if (typeof(T) == typeof(ISound))
            {
                this.soundLoader.Unload(name);
                return;
            }

            throw new UnknownContentException($"The content of type '{typeof(T)}' is unknown.");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureLoader.Dispose();
                this.soundLoader.Dispose();
                this.atlasLoader.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
