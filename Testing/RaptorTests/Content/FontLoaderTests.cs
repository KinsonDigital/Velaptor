// <copyright file="FontLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Newtonsoft.Json;
    using Raptor.Content;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using Raptor.Services;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontLoader"/> class.
    /// </summary>
    public class FontLoaderTests
    {
        private const string FontContentName = "test-font";
        private readonly string fontsDirPath;
        private readonly string fontDataFilepath;
        private readonly string fontFilepath;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly GlyphMetrics[] glyphMetricData;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
        };
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPathResolver> mockFontPathResolver;
        private readonly FontSettings fontSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
        /// </summary>
        public FontLoaderTests()
        {
            this.fontsDirPath = @"C:\temp\Content\Fonts\";
            this.fontDataFilepath = $@"{this.fontsDirPath}{FontContentName}.json";
            this.fontFilepath = $@"{this.fontsDirPath}{FontContentName}.ttf";

            this.mockGLInvoker = new Mock<IGLInvoker>();

            this.glyphMetricData = new GlyphMetrics[]
                {
                    GenerateMetricData(0),
                    GenerateMetricData(10),
                };

            this.mockFontAtlasService = new Mock<IFontAtlasService>();
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(this.fontFilepath, It.IsAny<int>()))
                .Returns(() =>
                {
                    return (default(ImageData), this.glyphMetricData);
                });

            this.mockFontPathResolver = new Mock<IPathResolver>();
            this.mockFontPathResolver.Setup(m => m.ResolveDirPath()).Returns(this.fontsDirPath);

            this.fontSettings = new FontSettings
            {
                Size = 12,
                Style = FontStyle.Regular,
            };

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.ReadAllText(this.fontDataFilepath)).Returns(() =>
            {
                return JsonConvert.SerializeObject(this.fontSettings, this.jsonSettings);
            });
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsFont()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(FontContentName);

            // Assert
            this.mockFile.Verify(m => m.ReadAllText(this.fontDataFilepath), Times.Once());
            Assert.Equal(this.fontSettings.Size, actual.Size);
            Assert.Equal(this.fontSettings.Style, actual.Style);
            Assert.Equal(FontContentName, actual.Name);
            Assert.Equal($"{this.fontsDirPath}{FontContentName}", actual.Path);
            Assert.Equal(this.glyphMetricData.Length, actual.Length);
        }

        [Fact]
        public void Load_WithAlreadyLoadedAtlasData_ReturnsAlreadyLoadedAtlasData()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(FontContentName);

            // Act
            var actual = loader.Load(FontContentName);

            // Assert
            this.mockFile.Verify(m => m.ReadAllText(this.fontDataFilepath), Times.Once());
            Assert.Equal(FontContentName, actual.Name);
        }

        [Fact]
        public void Load_WithIssuesDeserializingJSONAtlasData_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.ReadAllText(this.fontDataFilepath)).Returns(() =>
            {
                return "invalid-data";
            });

            var loader = CreateLoader();
            var newtonsoftErrorMsg = "Unexpected character encountered while parsing value: i. Path '', line 0, position 0.";

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(FontContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.fontDataFilepath}'.\n{newtonsoftErrorMsg}");
        }

        [Fact]
        public void Load_WithNullDeserializeResult_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.ReadAllText(this.fontDataFilepath)).Returns(() =>
            {
                return string.Empty;
            });

            var loader = CreateLoader();
            var exceptionMessage = "Deserialized font settings are null.";

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(FontContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.fontDataFilepath}'.\n{exceptionMessage}");
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsFonts()
        {
            // Arrange
            var loader = CreateLoader();
            var font = loader.Load(FontContentName);

            // Act
            loader.Unload(FontContentName);

            // Assert
            Assert.True(font.Unloaded);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTextures()
        {
            // Arrange
            var loader = CreateLoader();
            var font = loader.Load(FontContentName);

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            Assert.True(font.Unloaded);
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
                AtlasBounds = new Rectangle(start + 13, start + 14, start + 15, start + 16),
            };
        }

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpoase of testing.
        /// </summary>
        /// <returns>The instnace to test.</returns>
        private FontLoader CreateLoader() => new FontLoader(
            this.mockGLInvoker.Object,
            this.mockFontAtlasService.Object,
            this.mockFontPathResolver.Object,
            this.mockFile.Object);
    }
}
