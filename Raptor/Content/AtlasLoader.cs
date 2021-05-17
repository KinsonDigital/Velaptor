// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Raptor.Content.Exceptions;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using Raptor.Services;

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    public class AtlasLoader : ILoader<IAtlasData>
    {
        private readonly ConcurrentDictionary<string, IAtlasData> atlases = new ();
        private readonly IGLInvoker gl;
        private readonly IImageService imageService;
        private readonly IPathResolver atlasDataPathResolver;
        private readonly IFile file;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="imageService">Loads image data from disk.</param>
        /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        [ExcludeFromCodeCoverage]
        public AtlasLoader(
            IImageService imageService,
            IPathResolver atlasDataPathResolver,
            IFile file)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.imageService = imageService;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.file = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="gl">Makes calls to OpenGL.</param>
        /// <param name="imageService">Loads image data from disk.</param>
        /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        internal AtlasLoader(
            IGLInvoker gl,
            IImageService imageService,
            IPathResolver atlasDataPathResolver,
            IFile file)
        {
            this.gl = gl;
            this.imageService = imageService;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.file = file;
        }

        /// <inheritdoc/>
        public IAtlasData Load(string name)
        {
            var atlasDataDirPath = this.atlasDataPathResolver.ResolveDirPath();

            return this.atlases.GetOrAdd($"{atlasDataDirPath}{name}", (path) =>
            {
                var atlasDataFilePath = $"{path}.json";
                var atlasImageFilePath = $"{path}.png";

                var rawData = this.file.ReadAllText(atlasDataFilePath);

                AtlasSubTextureData[]? atlasSpriteData;

                try
                {
                    atlasSpriteData = JsonConvert.DeserializeObject<AtlasSubTextureData[]>(rawData);

                    if (atlasSpriteData is null)
                    {
                        throw new Exception($"Deserialized atlas sub texture data is null.");
                    }
                }
                catch (Exception ex)
                {
                    throw new LoadContentException($"There was an issue deserializing the JSON atlas data file at '{atlasDataFilePath}'.\n{ex.Message}");
                }

                ImageData data = this.imageService.Load(atlasImageFilePath);

                var atlasTexture = new Texture(this.gl, name, path, data);

                return new AtlasData(atlasTexture, atlasSpriteData, name, path);
            });
        }

        /// <inheritdoc/>
        public void Unload(string name)
        {
            var filePathNoExtension = $"{this.atlasDataPathResolver.ResolveDirPath()}{name}";

            if (this.atlases.TryRemove(filePathNoExtension, out var atlas))
            {
                atlas.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var atlas in this.atlases.Values)
                {
                    atlas.Dispose();
                }

                this.atlases.Clear();
            }

            this.isDisposed = true;
        }
    }
}
