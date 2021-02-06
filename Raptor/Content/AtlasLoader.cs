// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Collections.Concurrent;
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Raptor.Graphics;

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public class AtlasLoader : ILoader<IAtlasData>
    {
        private readonly ConcurrentDictionary<string, IAtlasData> atlases = new ConcurrentDictionary<string, IAtlasData>();
        private readonly IPathResolver atlasDataPathResolver;
        private readonly ILoader<ITexture> textureLoader;
        private readonly IFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The texture loader.</param>
        /// <param name="atlasDataPathResolver">The source of the atlas JSON data.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        public AtlasLoader(ILoader<ITexture> textureLoader, IPathResolver atlasDataPathResolver, IFile file)
        {
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.textureLoader = textureLoader;
            this.file = file;
        }

        /// <inheritdoc/>
        public IAtlasData Load(string name)
        {
            var atlasDataFilePath = this.atlasDataPathResolver.ResolveFilePath(name);

            return this.atlases.GetOrAdd(atlasDataFilePath, (key) =>
            {
                var rawData = this.file.ReadAllText($"{atlasDataFilePath}");
                var atlasSpriteData = JsonConvert.DeserializeObject<AtlasSubTextureData[]>(rawData);

                var atlasTexture = this.textureLoader.Load(name);

                return new AtlasData(atlasSpriteData, atlasTexture, name, atlasDataFilePath);
            });
        }
    }
}
