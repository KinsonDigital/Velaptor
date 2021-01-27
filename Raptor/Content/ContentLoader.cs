// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
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
    }
}
