// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Holds data relating to a texture atlas.
    /// </summary>
    public class AtlasData : IAtlasData
    {
        private readonly AtlasSubTextureData[] subTextures;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasData"/> class.
        /// </summary>
        /// <param name="texture">The texture data of the atlas.</param>
        /// <param name="atlasSubTexutureData">The sub texture data of all of the sub textures in the atlas.</param>
        /// <param name="atlasName">The name of the atlas.</param>
        /// <param name="path">The path to the content.</param>
        public AtlasData(ITexture texture, AtlasSubTextureData[] atlasSubTexutureData, string atlasName, string path)
        {
            if (texture is null)
            {
                throw new ArgumentNullException(nameof(Texture), "The parameter must not be null.");
            }

            this.subTextures = atlasSubTexutureData.OrderBy(data => data.FrameIndex).ToArray();
            Texture = texture;
            Name = atlasName;
            Path = path;
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
        public string Path { get; }

        /// <inheritdoc/>
        public ITexture Texture { get; private set; }

        /// <inheritdoc/>
        public int Width => Texture.Width;

        /// <inheritdoc/>
        public int Height => Texture.Height;

        /// <inheritdoc/>
        public bool Unloaded { get; private set; }

        /// <inheritdoc/>
        public AtlasSubTextureData this[int index] => this.subTextures[index];

        /// <inheritdoc/>
        public AtlasSubTextureData GetFrame(string subTextureID)
        {
            var foundFrmae = (from s in this.subTextures
                              where s.Name == subTextureID
                              select s).FirstOrDefault();

            if (foundFrmae is null)
            {
                // TODO: Create a custom exception named TextureAtlasException and implement here
                throw new Exception($"The frame '{subTextureID}' was not found in the atlas '{Name}'.");
            }

            return foundFrmae;
        }

        /// <inheritdoc/>
        public AtlasSubTextureData[] GetFrames(string subTextureID)
            => (from s in this.subTextures
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
            if (Unloaded)
            {
                return;
            }

            if (disposing)
            {
                Texture.Dispose();
            }

            Unloaded = true;
        }
    }
}
