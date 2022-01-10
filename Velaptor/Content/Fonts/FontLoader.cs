// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Content.Factories;

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
        private readonly IFontAtlasService fontAtlasService;
        private readonly IPathResolver fontPathResolver;
        private readonly IFontFactory fontFactory;
        private readonly IPath path;
        private readonly IImageService imageService;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public FontLoader()
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            this.fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
            this.fontPathResolver = PathResolverFactory.CreateFontPathResolver();
            this.imageService = IoC.Container.GetInstance<IImageService>();
            this.path = IoC.Container.GetInstance<IPath>();
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
        /// <param name="fontFactory">Generates new <see cref="IFont"/> instances.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal FontLoader(
            // TODO: Inject TextureCache
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IFontAtlasService fontAtlasService,
            IPathResolver fontPathResolver,
            IImageService imageService,
            IFontFactory fontFactory,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.imageService = imageService;
            this.fontFactory = fontFactory;
            this.path = path;
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

            // TODO: This code below should be replace with a TextureCache call.  The returning texture is what will
            // get injected into the Font() ctor.


            return this.fonts.GetOrAdd(fontFilePath, filePath =>
            {
                // TODO: this will be removed.  This is going to be dealt with in the TextureCache class
                var (fontAtlasImage, atlasData) = this.fontAtlasService.CreateFontAtlas(filePath, size);

                // OpenGL origin Y is at the bottom instead of the top.  This means
                // that the current image data which is vertically oriented correctly, needs
                // to be flipped vertically before being sent to OpenGL.
                // TODO: this will be removed.  This is going to be dealt with in the TextureCache class
                fontAtlasImage = this.imageService.FlipVertically(fontAtlasImage);

                // TODO: this will be removed.  This is going to be returned by TextureCache class
                var fontAtlasTexture = new Texture(this.gl, this.glExtensions, name, filePath, fontAtlasImage) { IsPooled = true };

                // TODO: When storing the font texture atlas into the texture cache, the key needs to be a full font file path with metadata

                return this.fontFactory.Create(fontAtlasTexture, name, filePath, size, atlasData);
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
