// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
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

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads font content for rendering text.
    /// </summary>
    public sealed class FontLoader : ILoader<IFont>
    {
        private readonly ConcurrentDictionary<string, IFont> fonts = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFreeTypeExtensions freeTypeExtensions;
        private readonly IFontAtlasService fontAtlasService;
        private readonly IPathResolver fontPathResolver;
        private readonly IFile file;
        private readonly IPath path;
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
        /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
        /// <param name="imageService">Manipulates image data.</param>
        [ExcludeFromCodeCoverage]
        public FontLoader(IFontAtlasService fontAtlasService, IPathResolver fontPathResolver, IImageService imageService)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            this.freeTypeInvoker = IoC.Container.GetInstance<IFreeTypeInvoker>();
            this.freeTypeExtensions = IoC.Container.GetInstance<IFreeTypeExtensions>();
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();
            this.imageService = imageService;

            this.fontAtlasService.SetAvailableCharacters(this.glyphChars);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="gl">Makes native calls to OpenGL.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="freeTypeInvoker">Makes calls to the native free type library.</param>
        /// <param name="freeTypeExtensions">Provides extensions/helpers to free type library functionality.</param>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
        /// <param name="imageService">Manipulates image data.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal FontLoader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            IFontAtlasService fontAtlasService,
            IPathResolver fontPathResolver,
            IImageService imageService,
            IFile file,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.file = file;
            this.path = path;
            this.imageService = imageService;

            this.fontAtlasService.SetAvailableCharacters(this.glyphChars);
        }

        /// <inheritdoc/>
        public IFont Load(string name)
        {
            name = this.path.HasExtension(name)
                ? this.path.GetFileNameWithoutExtension(name)
                : name;

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

            return this.fonts.GetOrAdd(filePathNoExtension, filePath =>
            {
                var fontDataFilePath = $"{filePath}.json";
                var fontFilePath = $"{filePath}.ttf";

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

                // OpenGL origin Y is at the bottom instead of the top.  This means
                // that the current image data which is vertically oriented correctly, needs
                // to be flipped vertically before being sent to OpenGL.
                fontAtlasImage = this.imageService.FlipVertically(fontAtlasImage);

                var fontAtlasTexture = new Texture(this.gl, this.glExtensions, name, filePath, fontAtlasImage) { IsPooled = true };

                return new Font(fontAtlasTexture, this.freeTypeInvoker, this.freeTypeExtensions, atlasData, fontSettings, this.glyphChars, name, filePath)
                {
                    IsPooled = true,
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
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
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
