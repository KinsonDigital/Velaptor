﻿// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.IO;
    using Velaptor.Content.Exceptions;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads content.
    /// </summary>
    public sealed class ContentLoader : IContentLoader
    {
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<ISound> soundLoader;
        private readonly ILoader<IAtlasData> atlasLoader;
        private readonly ILoader<IFont> fontLoader;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="soundLoader">Loads sounds.</param>
        /// <param name="atlasLoader">Loads a texture atlas.</param>
        /// <param name="fontLoader">Loads fonts for rendering test.</param>
        public ContentLoader(
            ILoader<ITexture> textureLoader,
            ILoader<ISound> soundLoader,
            ILoader<IAtlasData> atlasLoader,
            ILoader<IFont> fontLoader)
        {
            this.textureLoader = textureLoader;
            this.soundLoader = soundLoader;
            this.atlasLoader = atlasLoader;
            this.fontLoader = fontLoader;
        }

        /// <inheritdoc/>
        public ITexture LoadTexture(string nameOrFilePath) => this.textureLoader.Load(nameOrFilePath);

        /// <inheritdoc/>
        public ISound LoadSound(string nameOrFilePath) => this.soundLoader.Load(Path.GetFileNameWithoutExtension(nameOrFilePath));

        /// <inheritdoc/>
        public IAtlasData LoadAtlas(string nameOrFilePath) => this.atlasLoader.Load(nameOrFilePath);

        /// <inheritdoc/>
        public IFont LoadFont(string nameOrFilePath, int size) => this.fontLoader.Load($"{nameOrFilePath}|size:{size}");

        /// <inheritdoc/>
        public void Unload<T>(string nameOrFilePath)
            where T : class, IContent
        {
            nameOrFilePath = Path.GetFileNameWithoutExtension(nameOrFilePath);

            if (typeof(T) == typeof(ITexture))
            {
                this.textureLoader.Unload(nameOrFilePath);
                return;
            }

            if (typeof(T) == typeof(IAtlasData))
            {
                this.atlasLoader.Unload(nameOrFilePath);
                return;
            }

            if (typeof(T) == typeof(ISound))
            {
                this.soundLoader.Unload(nameOrFilePath);
                return;
            }

            if (typeof(T) == typeof(IFont))
            {
                this.fontLoader.Unload(nameOrFilePath);
                return;
            }

            throw new UnknownContentException($"The content of type '{typeof(T)}' is unknown.");
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        private void Dispose(bool disposing)
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
                this.fontLoader.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
