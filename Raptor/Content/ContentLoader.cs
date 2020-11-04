// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
namespace Raptor.Content
{
    using System;
    using Raptor.Audio;
    using Raptor.Graphics;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<ISound> soundLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="contentSource">Manages the source of where content is located.</param>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="soundLoader">Loads sounds.</param>
        public ContentLoader(
            ILoader<ITexture> textureLoader,
            ILoader<ISound> soundLoader)
        {
            this.textureLoader = textureLoader;
            this.soundLoader = soundLoader;
        }

        /// <inheritdoc/>
        public T Load<T>(string name)
            where T : class, IContent
        {
            if (typeof(T) == typeof(ITexture))
            {
                return (T)this.textureLoader.Load(name);
            }

            if (typeof(T) == typeof(ISound))
            {
                return (T)this.soundLoader.Load(name);
            }

            // TODO: Create custom unknown content exception
            throw new Exception($"Content of type '{typeof(T)}' invalid.  Must inherit from type '{nameof(IContent)}'");
        }
    }
}
