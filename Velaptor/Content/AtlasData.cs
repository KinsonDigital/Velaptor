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
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds data relating to a texture atlas.
    /// </summary>
    public sealed class AtlasData : IAtlasData
    {
        private const string AtlasDataExtension = ".json";
        private const string TextureExtension = ".png";
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly AtlasSubTextureData[] subTextures;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasData"/> class.
        /// </summary>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="path">Performs path related operations.</param>
        /// <param name="atlasSubTextureData">The sub texture data of all of the sub textures in the atlas.</param>
        /// <param name="dirPath">The path to the content.</param>
        /// <param name="atlasName">The name of the atlas.</param>
        public AtlasData(
            IDisposableItemCache<string, ITexture> textureCache,
            IPath path,
            IEnumerable<AtlasSubTextureData> atlasSubTextureData,
            string dirPath,
            string atlasName)
        {
            this.textureCache = textureCache;
            this.subTextures = atlasSubTextureData.OrderBy(data => data.FrameIndex).ToArray();

            // Throw exception if the path is not a directory path
            if (string.IsNullOrEmpty(path.GetDirectoryName(dirPath)))
            {
                throw new Exception("The path must be a valid directory path.");
            }

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
                var allNames = this.subTextures.Select(item => item.Name).ToArray();

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
        public bool IsPooled { get; set; }

        /// <inheritdoc/>
        public AtlasSubTextureData this[int index] => this.subTextures[index];

        /// <inheritdoc/>
        public AtlasSubTextureData GetFrame(string subTextureId)
        {
            var foundFrame = (from s in this.subTextures
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
            => (from s in this.subTextures
                where s.Name == subTextureId
                select s).ToArray();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsPooled)
            {
                throw new PooledDisposalException();
            }

            if (disposing)
            {
                this.textureCache.Unload(FilePath);
            }

            IsDisposed = true;
        }
    }
}
