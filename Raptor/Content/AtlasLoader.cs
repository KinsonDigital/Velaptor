// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Raptor.Graphics;

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public class AtlasLoader : ILoader<IAtlasData>
    {
        private readonly AtlasRepository atlasRepo;
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
            this.atlasRepo = AtlasRepository.Instance;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.textureLoader = textureLoader;
            this.file = file;
        }

        /// <inheritdoc/>
        public IAtlasData Load(string name)
        {
            if (this.atlasRepo.IsAtlasLoaded(name))
            {
                return this.atlasRepo.GetAtlasData(name);
            }

            var atlasDataFilePath = this.atlasDataPathResolver.ResolveFilePath(name);

            var rawData = this.file.ReadAllText($"{atlasDataFilePath}");
            var atlasSpriteData = JsonConvert.DeserializeObject<AtlasSubTextureData[]>(rawData);

            var atlasTexture = this.textureLoader.Load(name);

            var atlasData = new AtlasData(atlasSpriteData, atlasTexture, name, atlasDataFilePath);

            this.atlasRepo.AddAtlasData(name, atlasData);

            return atlasData;
        }
    }
}
