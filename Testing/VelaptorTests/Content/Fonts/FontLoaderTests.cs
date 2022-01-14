// <copyright file="FontLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.IO;

namespace VelaptorTests.Content.Fonts
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontLoader"/> class.
    /// </summary>
    public class FontLoaderTests
    {
        private const int FontSize = 12;
        private const string DirPath = @"C:\fonts\";
        private const string FontContentName = "test-font";
        private const string FontExtension = ".ttf";
        private readonly string metaData = $"size:{FontSize}";
        private readonly string fontFilePath;
        private readonly string filePathWithMetaData;
        private readonly string contentNameWithMetaData;
        private readonly GlyphMetrics[] glyphMetricData;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly Mock<IPathResolver> mockFontPathResolver;
        private readonly Mock<IDisposableItemCache<string, ITexture>> mockTextureCache;
        private readonly Mock<IFontFactory> mockFontFactory;
        private readonly Mock<IFontMetaDataParser> mockFontMetaDataParser;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<ITexture> mockFontAtlasTexture;
        private readonly Mock<IFont> mockFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
        /// </summary>
        public FontLoaderTests()
        {
            this.fontFilePath = $@"{DirPath}{FontContentName}{FontExtension}";
            this.filePathWithMetaData = $"{this.fontFilePath}|{this.metaData}";
            this.contentNameWithMetaData = $"{FontContentName}|{this.metaData}";

            this.mockFontAtlasTexture = new Mock<ITexture>();
            this.mockFont = new Mock<IFont>();

            this.glyphMetricData = new[]
                {
                    GenerateMetricData(0),
                    GenerateMetricData(10),
                };

            this.mockFontAtlasService = new Mock<IFontAtlasService>();
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(this.fontFilePath, FontSize))
                .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

            // Mock for full file paths with metadata
            this.mockFontPathResolver = new Mock<IPathResolver>();
            this.mockFontPathResolver.Setup(m => m.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);

            // Mock for both full file paths and content names with metadata
            this.mockTextureCache = new Mock<IDisposableItemCache<string, ITexture>>();
            this.mockTextureCache.Setup(m => m.GetItem(this.filePathWithMetaData))
                .Returns(this.mockFontAtlasTexture.Object);

            // Mock for both full file paths and content names with metadata
            this.mockFontFactory = new Mock<IFontFactory>();
            this.mockFontFactory.Setup(m =>
                    m.Create(this.mockFontAtlasTexture.Object, FontContentName, this.fontFilePath, FontSize, this.glyphMetricData))
                .Returns(this.mockFont.Object);

            this.mockFontMetaDataParser = new Mock<IFontMetaDataParser>();
            // Mock for full file paths with metadata
            this.mockFontMetaDataParser.Setup(m => m.Parse(this.filePathWithMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    this.fontFilePath,
                    this.metaData,
                    FontSize));

            // Mock for content names with metadata
            this.mockFontMetaDataParser.Setup(m => m.Parse(this.contentNameWithMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    FontContentName,
                    this.metaData,
                    FontSize));

            // Mock for both full file paths and content names with metadata
            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}"))
                .Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}{FontExtension}"))
                .Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.fontFilePath))
                .Returns(FontContentName);
        }

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Load_WithNullOrEmptyParam_ThrowsException(string contentName)
        {
            // Arrange
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                loader.Load(contentName);
            }, "The parameter must not be null. (Parameter 'contentWithMetaData')");
        }

        [Fact]
        public void Load_WithInvalidMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "invalid-metadata";
            const string invalidMetaData = "size-12";

            var expected = $"The metadata '{invalidMetaData}' is invalid when loading '{contentName}'.";
            expected += "\n\tExpected MetaData Syntax: size:<font-size>";
            expected += "\n\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    true,
                    false,
                    string.Empty,
                    invalidMetaData,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Load(contentName);
            }, expected);
        }

        [Fact]
        public void Load_WithNoMetaData_ThrowsException()
        {
            // Arrange
            this.mockFontMetaDataParser.Setup(m => m.Parse(It.IsAny<string>()))
                .Returns(new FontMetaDataParseResult(
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    -1));

            var expected = "The font content item 'missing-metadata' must have metadata post fixed to the";
            expected += " end of a content name or full file path";

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Load("missing-metadata");
            }, expected);
        }

        [Fact]
        public void Load_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            const string appFontContentDirPath = @"C:\AppDir\Content\Fonts\";
            this.mockFontPathResolver.Setup(m => m.ResolveDirPath()).Returns(appFontContentDirPath);
            this.mockFontPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>())).Returns(string.Empty);

            var expected = $"The font content item '{FontContentName}' does not exist.";
            expected += $"\nCheck the applications font content directory '{appFontContentDirPath}' to see if it exists.";

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadFontException>(() =>
            {
                loader.Load(this.contentNameWithMetaData);
            }, expected);
        }

        [Fact]
        public void Load_WithFileNameAndExtensionOnly_LoadsFontFromContentDirectory()
        {
            // Arrange
            var fileNameWithExt = $"{FontContentName}{FontExtension}";
            var fileNameWithExtAndMetaData = $"{fileNameWithExt}|size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(fileNameWithExtAndMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    fileNameWithExt,
                    this.metaData,
                    FontSize));
            this.mockFontPathResolver.Setup(m => m.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExt)).Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExtAndMetaData)).Returns(FontContentName);
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(this.fontFilePath, FontSize))
                .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

            var loader = CreateLoader();

            // Act
            var actual = loader.Load(fileNameWithExtAndMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(fileNameWithExtAndMetaData), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(fileNameWithExt), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
            this.mockFontFactory.Verify(m =>
                    m.Create(this.mockFontAtlasTexture.Object, FontContentName, this.fontFilePath, FontSize, this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Load_WhenUsingFullFilePathWithMetaData_LoadsFont()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.filePathWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
            this.mockFontFactory.Verify(m =>
                    m.Create(this.mockFontAtlasTexture.Object, FontContentName, this.fontFilePath, FontSize, this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Load_WhenUsingContentNameWithMetaData_LoadsFont()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.contentNameWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
            this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);

            this.mockFontFactory.Verify(m =>
                    m.Create(this.mockFontAtlasTexture.Object, FontContentName, this.fontFilePath, FontSize, this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Unload_WithInvalidMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "invalid-metadata";
            const string invalidMetaData = "size-12";

            var expected = $"The metadata '{invalidMetaData}' is invalid when unloading '{contentName}'.";
            expected += "\n\tExpected MetaData Syntax: size:<font-size>";
            expected += "\n\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    true,
                    false,
                    string.Empty,
                    invalidMetaData,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Unload(contentName);
            }, expected);
        }

        [Fact]
        public void Unload_WithNoMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "missing-metadata";

            var expected = "When unloading fonts, the name of or the full file path of the font";
            expected += " must be supplied with valid metadata syntax.";
            expected += $"\n\tExpected MetaData Syntax: size:<font-size>";
            expected += "\n\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Unload(contentName);
            }, expected);
        }

        [Fact]
        public void Unload_WhenUnloadingWithFullFilePathAndMetaData_UnloadsFonts()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.filePathWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
            this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
        }

        [Fact]
        public void Unload_WhenUnloadingWithContentNameAndMetaData_UnloadsFonts()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.contentNameWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
            this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
            this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTextures()
        {
            // Arrange
            const string texturePath = @"C:\Textures\test-texture.png";
            const string fontPath = @"C:\Fonts\test-font.ttf";
            var fontPathWithMetaData = $"{fontPath}|{this.metaData}";

            this.mockTextureCache.SetupGet(p => p.CacheKeys)
                .Returns(() => new ReadOnlyCollection<string>(new[]
                {
                    texturePath,
                    fontPathWithMetaData,
                }));

            this.mockFontMetaDataParser.Setup(m => m.Parse(texturePath))
                .Returns(new FontMetaDataParseResult(
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    -1));

            this.mockFontMetaDataParser.Setup(m => m.Parse(fontPathWithMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    fontPath,
                    this.metaData,
                    -1));

            var loader = CreateLoader();

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(texturePath), Times.Once);
            this.mockFontMetaDataParser.Verify(m => m.Parse(fontPathWithMetaData), Times.Once);
            this.mockTextureCache.Verify(m => m.Unload(texturePath), Times.Never);
            this.mockTextureCache.Verify(m => m.Unload(fontPathWithMetaData), Times.Once);
        }
        #endregion

        /// <summary>
        /// Generates fake glyph metric data for testing.
        /// </summary>
        /// <param name="start">The start value of all of the metric data.</param>
        /// <returns>The glyph metric data to be tested.</returns>
        /// <remarks>
        ///     The start value is a metric value start and incremented for each metric.
        /// </remarks>
        private static GlyphMetrics GenerateMetricData(int start)
        {
            return new GlyphMetrics()
            {
                Ascender = start,
                Descender = start + 1,
                CharIndex = (uint)start + 2,
                GlyphWidth = start + 3,
                GlyphHeight = start + 4,
                HoriBearingX = start + 5,
                HoriBearingY = start + 6,
                XMin = start + 7,
                XMax = start + 8,
                YMin = start + 9,
                YMax = start + 10,
                HorizontalAdvance = start + 11,
                Glyph = (char)(start + 12),
                GlyphBounds = new RectangleF(start + 13, start + 14, start + 15, start + 16),
            };
        }

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private FontLoader CreateLoader() => new (
            this.mockFontAtlasService.Object,
            this.mockFontPathResolver.Object,
            this.mockTextureCache.Object,
            this.mockFontFactory.Object,
            this.mockFontMetaDataParser.Object,
            this.mockPath.Object);
    }
}
