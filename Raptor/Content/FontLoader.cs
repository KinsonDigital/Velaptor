// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using Raptor.Services;

    /// <summary>
    /// Loads font content for rendering text.
    /// </summary>
    public class FontLoader : ILoader<IFont>
    {
        private readonly ConcurrentDictionary<string, IFont> fonts = new ConcurrentDictionary<string, IFont>();
        private readonly IGLInvoker gl;
        private readonly IFontAtlasService fontAtlasService;
        private readonly IPathResolver fontPathResolver;
        private readonly IFile file;
        private bool isDiposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Reolves paths to JSON font data files.</param>
        [ExcludeFromCodeCoverage]
        public FontLoader(IFontAtlasService fontAtlasService, IPathResolver fontPathResolver)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = IoC.Container.GetInstance<IFile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="gl">Makes calls to OpenGL.</param>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Reolves paths to JSON font data files.</param>
        /// <param name="file">Peforms file related operations.</param>
        internal FontLoader(IGLInvoker gl, IFontAtlasService fontAtlasService, IPathResolver fontPathResolver, IFile file)
        {
            this.gl = gl;
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = file;
        }

        /// <inheritdoc/>
        public IFont Load(string name)
        {
            var fontsDirPath = this.fontPathResolver.ResolveDirPath();
            var filePathNoExtension = $"{fontsDirPath}{name}";

            return this.fonts.GetOrAdd(filePathNoExtension, (path) =>
            {
                var fontDataFilePath = $"{path}.json";
                var fontFilePath = $"{path}.ttf";

                var rawData = this.file.ReadAllText(fontDataFilePath);

                FontSettings? fontSettings;

                try
                {
                    fontSettings = JsonConvert.DeserializeObject<FontSettings>(rawData);

                    if (fontSettings is null)
                    {
                        throw new Exception("Deserialized font settings are null.");
                    }
                }
                catch (Exception ex)
                {
                    throw new LoadContentException($"There was an issue deserializing the JSON atlas data file at '{fontDataFilePath}'.\n{ex.Message}");
                }

                var (fontAtlasImage, atlasData) = this.fontAtlasService.CreateFontAtlas(fontFilePath, fontSettings.Size);

                this.fontAtlasService.CreateFontAtlas(path, 14);

                var fontAtlasTexture = new Texture(this.gl, name, path, fontAtlasImage);

                return new Font(fontAtlasTexture, atlasData, name, path, fontSettings);
            });
        }

        /// <inheritdoc/>
        public void Unload(string name)
        {
            var filePathNoExtension = $"{this.fontPathResolver.ResolveDirPath()}{name}";

            if (this.fonts.TryRemove(filePathNoExtension, out var font))
            {
                font.Dispose();
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
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDiposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var font in this.fonts.Values)
                {
                    font.Dispose();
                }
            }

            this.isDiposed = true;
        }
    }
}
