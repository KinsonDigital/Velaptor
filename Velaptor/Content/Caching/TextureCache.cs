// <copyright file="TextureCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
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
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCache"/> class.
        /// </summary>
        /// <param name="imageService">Provides image related services.</param>
        /// <param name="textureFactory">Creates <see cref="ITexture"/> objects.</param>
        /// <param name="fontAtlasService">Provides font atlas services.</param>
        /// <param name="path">Provides path related services.</param>
        public TextureCache(
            IImageService imageService,
            ITextureFactory textureFactory,
            IFontAtlasService fontAtlasService,
            IPath path)
        {
            this.imageService = imageService;
            this.textureFactory = textureFactory;
            this.fontAtlasService = fontAtlasService;
            this.path = path;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TextureCache"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~TextureCache() => Dispose(false);

        /// <inheritdoc/>
        public int TotalCachedItems => this.textures.Count;

        /// <summary>
        /// Gets a cached texture that matches the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The texture(.png) or font(.ttf) file to cache as a texture.</param>
        /// <returns>The cached item.</returns>
        /// <remarks>
        /// <para>If the item does not already exist in the cached, it is created then cached.</para>
        /// <para>If the item does already exist in the cache, then that cached item is returned.</para>
        /// <para>If the item is a font(.ttf) file, a texture atlas is created of all the font glyphs and cached.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="CachingMetaDataException">
        /// Thrown for the following reasons:
        /// <list type="bullet">
        ///     <item>
        ///         Thrown if the given <paramref name="filePath"/> is a font(.ttf) file and no metadata exists.
        ///     </item>
        ///
        ///     <item>
        ///         Thrown if the given <paramref name="filePath"/> is a font(.ttf) file and the metadata is invalid.
        ///     </item>
        /// </list>
        /// </exception>
        /// <exception cref="LoadTextureException">
        ///     Thrown if the given <paramref name="filePath"/> to the content file does not exist.
        /// </exception>
        /// <exception cref="CachingException">
        ///     Thrown if the given <paramref name="filePath"/> to the content is not a
        ///     texture(.png) or font(.ttf) file.
        /// </exception>
        public ITexture GetItem(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "The parameter must not be null or empty");
            }

            var (invalid, fontSize, containsMetaData, metaData) = ParseMetaData(filePath);

            // If the required metadata is missing
            if (invalid)
            {
                if (containsMetaData)
                {
                    var exceptionMsg = "Font metadata required when caching fonts.";
                    exceptionMsg += string.IsNullOrEmpty(metaData)
                        ? string.Empty
                        : $"\nCurrent metadata: '{metaData}'";
                    exceptionMsg += "Required metadata syntax: '|size:<number-here>'";

                    throw new CachingMetaDataException(exceptionMsg);
                }
                else
                {
                    var exceptionMsg = "Font metadata required when caching fonts.";
                    exceptionMsg += "Required metadata syntax: '|size:<number-here>'";
                    exceptionMsg += "\nIf the '|' character is missing, it signifies that no metadata exists.";

                    throw new CachingMetaDataException(exceptionMsg);
                }
            }

            // If the required meta data is all or partly there but invalid
            filePath = containsMetaData
                ? filePath.Replace(metaData, string.Empty)
                : filePath;

            var isTextureFile = this.path.GetExtension(filePath) == TextureFileExtension;
            var isFontFile = this.path.GetExtension(filePath) == FontFileExtension;

            if (filePath.IsValidFilePath() is false)
            {
                throw new LoadTextureException($"The texture file path '{filePath}' is not a valid path.");
            }

            // If the requested texture is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var texture in this.textures)
            {
                if (texture.Key != filePath || !texture.Value.IsDisposed)
                {
                    continue;
                }

                this.textures.TryRemove(texture);
                break;
            }

            return this.textures.GetOrAdd(filePath, textureOrFontFilePath =>
            {
                ImageData imageData;

                if (isTextureFile && isFontFile is false)
                {
                    imageData = this.imageService.Load(textureOrFontFilePath);
                }
                else if (isFontFile && isTextureFile is false)
                {
                    var (atlasImageData, _) = this.fontAtlasService.CreateFontAtlas(textureOrFontFilePath, fontSize);
                    imageData = atlasImageData;
                }
                else
                {
                    var exceptionMsg = "Texture Caching Error:";
                    exceptionMsg += $"\nWhen caching textures, the only file types allowed";
                    exceptionMsg += $" are '{TextureFileExtension}' and '{FontFileExtension}' files.";
                    exceptionMsg += "\nFont files are converted into texture atlases for the font glyphs.";

                    throw new CachingException(exceptionMsg);
                }

                var name = this.path.GetFileNameWithoutExtension(textureOrFontFilePath);

                return this.textureFactory.Create(name, textureOrFontFilePath, imageData, true);
            });
        }

        /// <inheritdoc/>
        public void Unload(string cacheKey)
        {
            this.textures.TryRemove(cacheKey, out var texture);

            if (texture is null)
            {
                return;
            }

            texture.IsPooled = false;
            texture.Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

                /// <summary>
        /// Parses the given value for font meta data and returns a result.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>A number of values representing the parsing results.</returns>
        private static (bool invalid, int fontSize, bool containsMetaData, string metaData) ParseMetaData(string value)
        {
            const int noFontSize = -1;
            const bool isValid = false;
            const bool isInvalid = true;
            const string emptyMetaData = "";
            var pipeIndex = value.IndexOf('|');
            var pipeSections = value.Split('|');
            var filePath = pipeSections[0];
            var varAndValue = pipeSections.Length >= 2 ? pipeSections[1] : string.Empty;

            var isFontFile = filePath.Contains(FontFileExtension);

            // If no pipe exists, then we must assume there is no metadata
            if ((pipeIndex <= -1 || value.Length < 3) && isFontFile)
            {
                return (isInvalid, noFontSize, false, emptyMetaData);
            }

            // If not a font file, then metadata is not processed
            if (isFontFile is false)
            {
                return (isValid, noFontSize, false, emptyMetaData);
            }

            var consecutivePipes = value.Contains("||");
            var moreThanTwoPipesAndEndsWithPipe = value.Count(c => c == '|') >= 2 && value.EndsWith('|');
            var endsWithSinglePipe = value.EndsWith('|');

            if (consecutivePipes || moreThanTwoPipesAndEndsWithPipe || endsWithSinglePipe)
            {
                return (isInvalid, noFontSize, true, emptyMetaData);
            }

            var colonIndex = varAndValue.IndexOf(':');

            if (colonIndex == -1 || varAndValue.StartsWith(':') || varAndValue.EndsWith(':'))
            {
                return (isInvalid, noFontSize, true, $"|{varAndValue}");
            }

            if (varAndValue.Contains("::"))
            {
                return (isInvalid, noFontSize, true, emptyMetaData);
            }

            var sizeIndex = varAndValue.IndexOf("size", StringComparison.Ordinal);
            var missingSizeVar = sizeIndex == -1 ||
                                      sizeIndex + "size".Length - 1 > colonIndex;

            var sizeAndValueSections = varAndValue.Split(':');
            var parseFailure = int.TryParse(sizeAndValueSections[1], out var size) is false;

            if (missingSizeVar || parseFailure)
            {
                return (isInvalid, noFontSize, true, $"|{varAndValue}");
            }

            return (isValid, size, true, $"|{varAndValue}");
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
                foreach (var texture in this.textures.Values)
                {
                    texture.IsPooled = false;
                    texture.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}
