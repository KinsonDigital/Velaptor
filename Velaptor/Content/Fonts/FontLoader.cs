// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Factories;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;

    // ReS harper restore RedundantNameQualifier

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
        private readonly IFontStatsService fontStatsService;
        private readonly IPathResolver fontPathResolver;
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
        [ExcludeFromCodeCoverage]
        public FontLoader()
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            this.freeTypeInvoker = IoC.Container.GetInstance<IFreeTypeInvoker>();
            this.freeTypeExtensions = IoC.Container.GetInstance<IFreeTypeExtensions>();
            this.fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
            this.fontStatsService = IoC.Container.GetInstance<IFontStatsService>();
            this.fontPathResolver = PathResolverFactory.CreateFontPathResolver();
            this.path = IoC.Container.GetInstance<IPath>();
            this.imageService = IoC.Container.GetInstance<IImageService>();

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
        /// <param name="fontStatsService">Used to gather stats about content or system fonts.</param>
        /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
        /// <param name="imageService">Manipulates image data.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal FontLoader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            IFontAtlasService fontAtlasService,
            IFontStatsService fontStatsService,
            IPathResolver fontPathResolver,
            IImageService imageService,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.fontAtlasService = fontAtlasService;
            this.fontStatsService = fontStatsService;
            this.fontPathResolver = fontPathResolver;
            this.path = path;
            this.imageService = imageService;

            this.fontAtlasService.SetAvailableCharacters(this.glyphChars);
        }

        /// <inheritdoc/>
        public IFont Load(string name)
        {
            var size = 12;

            if (name.Contains('|') &&
                name.NotStartsWith('|') &&
                name.NotEndsWith('|'))
            {
                var sections = name.Split('|');

                // TODO: Convert to uint
                var success = int.TryParse(sections[1], out var parsedSize);

                if (success)
                {
                    size = parsedSize;
                }

                name = sections[0];
            }

            name = this.path.HasExtension(name)
                ? this.path.GetFileNameWithoutExtension(name)
                : name;

            var fontFilePath = this.fontPathResolver.ResolveFilePath(name);

            return this.fonts.GetOrAdd(fontFilePath, filePath =>
            {
                var (fontAtlasImage, atlasData) = this.fontAtlasService.CreateFontAtlas(filePath, size);

                // OpenGL origin Y is at the bottom instead of the top.  This means
                // that the current image data which is vertically oriented correctly, needs
                // to be flipped vertically before being sent to OpenGL.
                fontAtlasImage = this.imageService.FlipVertically(fontAtlasImage);

                var fontAtlasTexture = new Texture(this.gl, this.glExtensions, name, filePath, fontAtlasImage) { IsPooled = true };

                return new Font(
                    fontAtlasTexture,
                    this.freeTypeInvoker,
                    this.freeTypeExtensions,
                    this.fontStatsService,
                    atlasData,
                    name,
                    filePath,
                    size)
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
