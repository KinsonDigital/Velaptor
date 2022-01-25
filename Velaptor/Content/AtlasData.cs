// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Caching;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds data relating to a texture atlas.
    /// </summary>
    public sealed class AtlasData : IAtlasData
    {
        private const string AtlasDataExtension = ".json";
        private const string TextureExtension = ".png";
        private readonly IItemCache<string, ITexture> textureCache;
        private readonly AtlasSubTextureData[] subTexturesData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasData"/> class.
        /// </summary>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="path">Performs path related operations.</param>
        /// <param name="atlasSubTextureData">The sub texture data of all sub textures in the atlas.</param>
        /// <param name="dirPath">The path to the content.</param>
        /// <param name="atlasName">The name of the atlas.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if any of the constructor parameters are null.
        /// </exception>
        public AtlasData(
            IItemCache<string, ITexture> textureCache,
            IPath path,
            IEnumerable<AtlasSubTextureData> atlasSubTextureData,
            string dirPath,
            string atlasName)
        {
            // Throw exception if the path is not a directory path
            if (string.IsNullOrEmpty(dirPath))
            {
                throw new ArgumentNullException(nameof(dirPath), "The parameters must not be null or empty.");
            }

            // Throw exception if the path is not a directory path
            if (string.IsNullOrEmpty(atlasName))
            {
                throw new ArgumentNullException(nameof(atlasName), "The parameters must not be null or empty.");
            }

            this.textureCache = textureCache ?? throw new ArgumentNullException(nameof(textureCache), "The parameters must not be null or empty.");

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path), "The parameters must not be null or empty.");
            }

            if (atlasSubTextureData is null)
            {
                throw new ArgumentNullException(nameof(atlasSubTextureData), "The parameters must not be null or empty.");
            }

            this.subTexturesData = atlasSubTextureData.OrderBy(data => data.FrameIndex).ToArray();

            atlasName = path.GetFileNameWithoutExtension(atlasName);

            Name = atlasName;
            FilePath = $"{dirPath}{atlasName}{TextureExtension}";
            AtlasDataFilePath = $"{dirPath}{atlasName}{AtlasDataExtension}";
            Texture = this.textureCache.GetItem(FilePath);
        }

        /// <summary>
        /// Gets a list of unique sub texture names.
        /// </summary>
        /// <remarks>
        ///     Will not return duplicate names of animating sub textures.
        ///     Animating sub textures will have identical names.
        /// </remarks>
        public ReadOnlyCollection<string> SubTextureNames
        {
            get
            {
                var result = new List<string>();
                var allNames = this.subTexturesData.Select(item => item.Name).ToArray();

                foreach (var name in allNames)
                {
                    if (!result.Contains(name))
                    {
                        result.Add(name);
                    }
                }

                return result.ToReadOnlyCollection();
            }
        }

        /// <summary>
        /// Gets the name of the atlas.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the path to the texture.
        /// </summary>
        public string FilePath { get; }

        /// <inheritdoc/>
        public string AtlasDataFilePath { get; }

        /// <inheritdoc/>
        public ITexture Texture { get; }

        /// <inheritdoc/>
        public uint Width => Texture.Width;

        /// <inheritdoc/>
        public uint Height => Texture.Height;

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public AtlasSubTextureData this[int index] => this.subTexturesData[index];

        /// <inheritdoc/>
        public AtlasSubTextureData GetFrame(string subTextureId)
        {
            var foundFrame = (from s in this.subTexturesData
                              where s.Name == subTextureId
                              select s).FirstOrDefault();

            if (foundFrame is null)
            {
                // TODO: Create a custom exception named TextureAtlasException and implement here
                throw new Exception($"The frame '{subTextureId}' was not found in the atlas '{Name}'.");
            }

            return foundFrame;
        }

        /// <inheritdoc/>
        public AtlasSubTextureData[] GetFrames(string subTextureId)
            => (from s in this.subTexturesData
                where s.Name == subTextureId
                select s).ToArray();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureCache.Unload(FilePath);
            }

            IsDisposed = true;
        }
    }
}
