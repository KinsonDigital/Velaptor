// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using Newtonsoft.Json;
    using Velaptor.Content.Exceptions;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;

    /// <summary>
    /// Loads font content for rendering text.
    /// </summary>
    public sealed class FontLoader : ILoader<IFont>
    {
        private readonly ConcurrentDictionary<string, IFont> fonts = new ();
        private readonly IGLInvoker gl;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFontAtlasService fontAtlasService;
        private readonly IPathResolver fontPathResolver;
        private readonly IFile file;
        private readonly IImageService imageService;
        private readonly char[] glyphChars =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Reolves paths to JSON font data files.</param>
        /// <param name="imageService">Manipulates image data.</param>
        [ExcludeFromCodeCoverage]
        public FontLoader(IFontAtlasService fontAtlasService, IPathResolver fontPathResolver, IImageService imageService)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.freeTypeInvoker = IoC.Container.GetInstance<IFreeTypeInvoker>();
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = IoC.Container.GetInstance<IFile>();
            this.imageService = imageService;

            this.fontAtlasService.SetAvailableCharacters(this.glyphChars);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="gl">Makes native calls to OpenGL.</param>
        /// <param name="freeTypeInvoker">Makes calls to the native free type library.</param>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Reolves paths to JSON font data files.</param>
        /// <param name="file">Peforms file related operations.</param>
        /// <param name="imageService">Manipulates image data.</param>
        internal FontLoader(
            IGLInvoker gl,
            IFreeTypeInvoker freeTypeInvoker,
            IFontAtlasService fontAtlasService,
            IPathResolver fontPathResolver,
            IFile file,
            IImageService imageService)
        {
            this.gl = gl;
            this.freeTypeInvoker = freeTypeInvoker;
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = file;
            this.imageService = imageService;

            this.fontAtlasService.SetAvailableCharacters(this.glyphChars);
        }

        /// <inheritdoc/>
        public IFont Load(string name)
        {
            var fontsDirPath = this.fontPathResolver.ResolveDirPath();
            var filePathNoExtension = $"{fontsDirPath}{name}";

            // If the requested font is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var font in this.fonts)
            {
                if (font.Key != filePathNoExtension || !font.Value.IsDisposed)
                {
                    continue;
                }

                this.fonts.TryRemove(font);
                break;
            }

            return this.fonts.GetOrAdd(filePathNoExtension, path =>
            {
                var fontDataFilePath = $"{path}.json";
                var fontFilePath = $"{path}.ttf";

                if (this.file.Exists(fontDataFilePath) is false)
                {
                    throw new FileNotFoundException($"The JSON data file '{fontDataFilePath}' describing the font settings for font content '{name}' is missing.");
                }

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

                // OpenGL origin Y is at the bottom instead of the top.  This menas
                // that the current image data which is vertically oriented correctly, needs
                // to be flipped vertically before being sent to OpenGL.
                fontAtlasImage = this.imageService.FlipVertically(fontAtlasImage);

                var fontAtlasTexture = new Texture(this.gl, name, path, fontAtlasImage) { IsPooled = true };

                return new Font(fontAtlasTexture, atlasData, fontSettings, this.glyphChars, name, path)
                {
                    IsPooled = true,
                    HasKerning = this.freeTypeInvoker.FT_Has_Kerning(),
                };
            });
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string name)
        {
            var filePathNoExtension = $"{this.fontPathResolver.ResolveDirPath()}{name}";

            if (this.fonts.TryRemove(filePathNoExtension, out var font))
            {
                font.IsPooled = false;
                font.Dispose();
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var font in this.fonts.Values)
                {
                    font.IsPooled = false;
                    font.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}
