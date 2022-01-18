// <copyright file="TextureCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Graphics;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Caches <see cref="ITexture"/> objects for performant retrieval at a later time.
    /// </summary>
    internal sealed class TextureCache : IDisposableItemCache<string, ITexture>
    {
        private const string TextureFileExtension = ".png";
        private const string FontFileExtension = ".ttf";
        private readonly ConcurrentDictionary<string, ITexture> textures = new ();
        private readonly IImageService imageService;
        private readonly ITextureFactory textureFactory;
        private readonly IFontAtlasService fontAtlasService;
        private readonly IFontMetaDataParser fontMetaDataParser;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCache"/> class.
        /// </summary>
        /// <param name="imageService">Provides image related services.</param>
        /// <param name="textureFactory">Creates <see cref="ITexture"/> objects.</param>
        /// <param name="fontAtlasService">Provides font atlas services.</param>
        /// <param name="fontMetaDataParser">Parses metadata that might be attached to the file path.</param>
        /// <param name="path">Provides path related services.</param>
        public TextureCache(
            IImageService imageService,
            ITextureFactory textureFactory,
            IFontAtlasService fontAtlasService,
            IFontMetaDataParser fontMetaDataParser,
            IPath path)
        {
            this.imageService = imageService;
            this.textureFactory = textureFactory;
            this.fontAtlasService = fontAtlasService;
            this.fontMetaDataParser = fontMetaDataParser;
            this.path = path;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TextureCache"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~TextureCache() => Dispose(false);

        /// <inheritdoc/>
        public int TotalCachedItems => this.textures.Count;

        /// <inheritdoc/>
        public ReadOnlyCollection<string> CacheKeys => new (this.textures.Keys.ToArray());

        /// <summary>
        /// Gets a texture using the given <paramref name="textureFilePath"/>.
        /// <para>
        ///     If the given <paramref name="textureFilePath"/> is a <c>.ttf</c> font file, then the texture will be a font atlas
        ///     texture created from the font.  Font file paths must include metadata.
        /// </para>
        /// <para>
        ///     If the item has not been retrieved before, it will create it and then cache it for fast retrieval
        ///     on the next call.
        /// </para>
        /// </summary>
        /// <param name="textureFilePath">
        ///     The full file path to a <c>Texture(.png)</c> or <c>Font(.ttf)</c> file.
        /// </param>
        /// <returns>A standard texture or font atlas texture created from a font.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="textureFilePath"/> is null or empty.
        /// </exception>
        /// <exception cref="CachingException">
        ///     Thrown for the following reasons:
        /// <list type="bullet">
        ///     <item>If a full file path is used with metadata that is not a <c>Font(.ttf)</c> file type.</item>
        ///     <item>If a <c>Font</c> path with metadata is not a fully qualified <c>Font</c> file path.</item>
        ///     <item>If a fully qualified <c>Font</c> file path has no metadata attached.</item>
        ///     <item>If a fully qualified <c>Texture</c> file path is not a <c>Texture(.png)</c> file type.</item>
        ///     <item>If a <c>Texture</c> path is not a fully qualified <c>Texture</c> file path.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CachingMetaDataException">
        ///     Thrown if the metadata attached to the end of the path is invalid.
        /// </exception>
        public ITexture GetItem(string textureFilePath)
        {
            if (string.IsNullOrEmpty(textureFilePath))
            {
                throw new ArgumentNullException(nameof(textureFilePath), "The parameter must not be null or empty.");
            }

            var parseResult = this.fontMetaDataParser.Parse(textureFilePath);

            string fullFilePath;
            var isFontFile = false;
            string cacheKey;

            if (parseResult.ContainsMetaData)
            {
                // If the metadata is valid and the prefix is a full qualified file path
                if (parseResult.IsValid)
                {
                    if (parseResult.MetaDataPrefix.HasValidFullFilePathSyntax())
                    {
                        if (this.path.GetExtension(parseResult.MetaDataPrefix).ToLower() != FontFileExtension)
                        {
                            throw new CachingException($"Font caching must be a '{FontFileExtension}' file type.");
                        }

                        fullFilePath = parseResult.MetaDataPrefix;
                        cacheKey = $"{parseResult.MetaDataPrefix}|size:{parseResult.FontSize}";
                        isFontFile = true;
                    }
                    else
                    {
                        var exceptionMsg = $"The font file path '{parseResult.MetaDataPrefix}' must be a";
                        exceptionMsg += $" fully qualified file path of type '{FontFileExtension}'.";
                        throw new CachingException(exceptionMsg);
                    }
                }
                else
                {
                    var exceptionMsg = $"The metadata '{parseResult.MetaData}' is invalid and is required";
                    exceptionMsg += $" for font files of type '{FontFileExtension}'.";
                    throw new CachingMetaDataException(exceptionMsg);
                }
            }
            else
            {
                if (textureFilePath.HasValidFullFilePathSyntax())
                {
                    var fileExtension = this.path.GetExtension(textureFilePath).ToLower();

                    if (fileExtension == FontFileExtension)
                    {
                        var exceptionMsg = "Font file paths must include metadata.";
                        exceptionMsg += "\nFont Content Path MetaData Syntax: <file-path>|size:<font-size>";
                        exceptionMsg += @"\nExample: C:\Windows\Fonts\my-font.ttf|size:12";

                        throw new CachingMetaDataException(exceptionMsg);
                    }

                    if (fileExtension != TextureFileExtension)
                    {
                        throw new CachingException($"Texture caching must be a '{TextureFileExtension}' file type.");
                    }

                    fullFilePath = textureFilePath;
                    cacheKey = textureFilePath;
                }
                else
                {
                    var exceptionMsg = $"The texture file path '{textureFilePath}' must be a";
                    exceptionMsg += $" fully qualified file path of type '{TextureFileExtension}'.";
                    throw new CachingException(exceptionMsg);
                }
            }

            // If the requested texture is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var texture in this.textures)
            {
                if (texture.Key != cacheKey || !texture.Value.IsDisposed)
                {
                    continue;
                }

                this.textures.TryRemove(texture);
                break;
            }

            return this.textures.GetOrAdd(cacheKey, _ =>
            {
                ImageData imageData;

                if (isFontFile)
                {
                    var (atlasImageData, _) = this.fontAtlasService.CreateFontAtlas(fullFilePath, parseResult.FontSize);

                    atlasImageData = this.imageService.FlipVertically(atlasImageData);
                    imageData = atlasImageData;
                }
                else
                {
                    imageData = this.imageService.Load(fullFilePath);
                }

                var contentName = this.path.GetFileNameWithoutExtension(fullFilePath);

                return this.textureFactory.Create(contentName, fullFilePath, imageData, false);
            });
        }

        /// <inheritdoc/>
        public void Unload(string cacheKey)
        {
            this.textures.TryRemove(cacheKey, out var texture);
            texture?.Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
                var cacheKeys = this.textures.Keys.ToArray();

                // Dispose of all textures no matter what type
                foreach (var cacheKey in cacheKeys)
                {
                    this.textures.TryRemove(cacheKey, out var texture);
                    texture?.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}
