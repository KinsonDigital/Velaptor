// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Holds data relating to a texture atlas.
    /// </summary>
    public class AtlasData : IAtlasData
    {
        private readonly AtlasSubTextureData[] atlasSprites;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasData"/> class.
        /// </summary>
        /// <param name="atlasSubTexutureData">The sub texture data of all of the sub textures in the atlas.</param>
        /// <param name="texture">The texture data of the atlas.</param>
        /// <param name="atlasName">The name of the atlas.</param>
        public AtlasData(AtlasSubTextureData[] atlasSubTexutureData, ITexture texture, string atlasName)
        {
            this.atlasSprites = atlasSubTexutureData.OrderBy(data => data.FrameIndex).ToArray();
            Texture = texture;
            Name = atlasName;
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
                var allNames = this.atlasSprites.Select(item => item.Name).ToArray();

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
        /// Gets the texture of the atlas.
        /// </summary>
        public ITexture Texture { get; }

        /// <summary>
        /// Gets the width of the atlas.
        /// </summary>
        public int Width => Texture.Width;

        /// <summary>
        /// Gets the height of the atlas.
        /// </summary>
        public int Height => Texture.Height;

        /// <summary>
        /// Returns the value at the specified key.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The atlas sprite data.</returns>
        public AtlasSubTextureData this[int index] => this.atlasSprites[index];

        /// <inheritdoc/>
        public AtlasSubTextureData[] GetFrames(string subTextureID)
            => (from s in this.atlasSprites
                where s.Name == subTextureID
                select s).ToArray();

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
                Texture.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
