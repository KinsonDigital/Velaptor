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
    using System.Linq;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds data relating to a texture atlas.
    /// </summary>
    public sealed class AtlasData : IAtlasData
    {
        private readonly AtlasSubTextureData[] subTextures;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasData"/> class.
        /// </summary>
        /// <param name="texture">The texture data of the atlas.</param>
        /// <param name="atlasSubTextureData">The sub texture data of all of the sub textures in the atlas.</param>
        /// <param name="atlasName">The name of the atlas.</param>
        /// <param name="path">The path to the content.</param>
        public AtlasData(ITexture texture, AtlasSubTextureData[] atlasSubTextureData, string atlasName, string path)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture), "The parameter must not be null.");

            this.subTextures = atlasSubTextureData.OrderBy(data => data.FrameIndex).ToArray();

            Name = atlasName;
            FilePath = path;
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

        /// <inheritdoc/>
        public string FilePath { get; }

        /// <inheritdoc/>
        public ITexture Texture { get; private set; }

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
                Texture.IsPooled = false;
                Texture.Dispose();
                IsDisposed = true;
            }
        }
    }
}
