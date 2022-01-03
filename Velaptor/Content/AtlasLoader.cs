// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    public sealed class AtlasLoader : ILoader<IAtlasData>
    {
        private readonly ConcurrentDictionary<string, IAtlasData> atlases = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IImageService imageService;
        private readonly IPathResolver atlasDataPathResolver;
        private readonly IFile file;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="imageService">Loads image data from disk.</param>
        /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
        [ExcludeFromCodeCoverage]
        public AtlasLoader(
            IImageService imageService,
            IPathResolver atlasDataPathResolver)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            this.imageService = imageService;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="gl">Makes calls to OpenGL.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="imageService">Loads image data from disk.</param>
        /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        /// <param name="path">Processes directory and file paths.</param>
        internal AtlasLoader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IImageService imageService,
            IPathResolver atlasDataPathResolver,
            IFile file,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.imageService = imageService;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.file = file;
            this.path = path;
        }

        /// <inheritdoc/>
        public IAtlasData Load(string name)
        {
            name = this.path.HasExtension(name)
                ? this.path.GetFileNameWithoutExtension(name)
                : name;

            var atlasDataPathNoExtension = $"{this.atlasDataPathResolver.ResolveDirPath()}{name}";

            // If the requested texture atlas is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var font in this.atlases)
            {
                if (font.Key != atlasDataPathNoExtension || !font.Value.IsDisposed)
                {
                    continue;
                }

                this.atlases.TryRemove(font);
                break;
            }

            return this.atlases.GetOrAdd(atlasDataPathNoExtension, (filePath) =>
            {
                var atlasDataFilePath = $"{filePath}.json";
                var atlasImageFilePath = $"{filePath}.png";

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

                var data = this.imageService.Load(atlasImageFilePath);

                var atlasTexture = new Texture(this.gl, this.glExtensions, name, filePath, data) { IsPooled = true };

                return new AtlasData(atlasTexture, atlasSpriteData, name, filePath) { IsPooled = true };
            });
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string name)
        {
            var filePathNoExtension = $"{this.atlasDataPathResolver.ResolveDirPath()}{name}";

            if (this.atlases.TryRemove(filePathNoExtension, out var atlas))
            {
                atlas.IsPooled = false;
                atlas.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/></param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var atlas in this.atlases.Values)
                {
                    atlas.IsPooled = false;
                    atlas.Dispose();
                }

                this.atlases.Clear();
            }

            this.isDisposed = true;
        }
    }
}
