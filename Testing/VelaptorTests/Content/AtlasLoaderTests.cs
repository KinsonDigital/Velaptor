// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Newtonsoft.Json;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasLoader"/> class.
    /// </summary>
    public class AtlasLoaderTests
    {
        private const string AtlasContentName = "test-atlas";
        private readonly string atlasDataFilePath;
        private readonly string atlasImageFilePath;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly JsonSerializerSettings jsonSettings = new ()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
        };
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPathResolver> mockAtlasPathResolver;
        private readonly Mock<IImageService> mockImageService;
        private readonly AtlasSubTextureData[] atlasSpriteData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
        /// </summary>
        public AtlasLoaderTests()
        {
            const string atlasDirPath = @"C:\temp\Content\Atlas\";
            this.atlasDataFilePath = $@"{atlasDirPath}{AtlasContentName}.json";
            this.atlasImageFilePath = $@"{atlasDirPath}{AtlasContentName}.png";

            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockGLInvoker.Name = nameof(this.mockGLInvoker);

            this.mockAtlasPathResolver = new Mock<IPathResolver>();
            this.mockAtlasPathResolver.Setup(m => m.ResolveDirPath()).Returns(atlasDirPath);

            this.mockImageService = new Mock<IImageService>();
            this.mockImageService.Name = nameof(this.mockImageService);

            this.atlasSpriteData = new[]
            {
                new AtlasSubTextureData()
                {
                    Name = "sub-texture|0",
                    Bounds = new Rectangle(11, 22, 33, 44),
                },
                new AtlasSubTextureData()
                {
                    Name = "sub-texture|1",
                    Bounds = new Rectangle(55, 66, 77, 88),
                },
            };

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath))
                .Returns(() => JsonConvert.SerializeObject(this.atlasSpriteData, this.jsonSettings));
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsTextureAtlasData()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(AtlasContentName);

            // Assert
            Assert.Equal(this.atlasSpriteData[0], actual[0]);
            Assert.Equal(this.atlasSpriteData[1], actual[1]);
            this.mockAtlasPathResolver.Verify(m => m.ResolveDirPath(), Times.Once());
            this.mockFile.Verify(m => m.ReadAllText(this.atlasDataFilePath), Times.Once());
            this.mockImageService.Verify(m => m.Load(this.atlasImageFilePath), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingSameDataThatIsDisposed_RemovesDataBeforeAdding()
        {
            // Arrange
            var loader = CreateLoader();
            var loadedData = loader.Load(AtlasContentName);

            // Set the font as not being pooled. This will allow us to dispose of
            // the font to get the font into the disposed state for testing
            loadedData.IsPooled = false;
            loadedData.Dispose();

            // Act
            var actual = loader.Load(AtlasContentName);

            // Assert
            Assert.NotSame(loadedData, actual);
        }

        [Fact]
        public void Load_WithAlreadyLoadedAtlasData_ReturnsAlreadyLoadedAtlasData()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(AtlasContentName);

            // Act
            var actual = loader.Load(AtlasContentName);

            // Assert
            Assert.Equal(this.atlasSpriteData[0], actual[0]);
            Assert.Equal(this.atlasSpriteData[1], actual[1]);
            Assert.Equal(AtlasContentName, actual.Name);
            this.mockFile.Verify(m => m.ReadAllText(this.atlasDataFilePath), Times.Once());
            this.mockImageService.Verify(m => m.Load(this.atlasImageFilePath), Times.Once());
        }

        [Fact]
        public void Load_WithIssuesDeserializingJSONAtlasData_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath))
                .Returns(() => "invalid-data");

            var loader = CreateLoader();
            const string newtonsoftErrorMsg = "Unexpected character encountered while parsing value: i. Path '', line 0, position 0.";

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(AtlasContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.atlasDataFilePath}'.\n{newtonsoftErrorMsg}");
        }

        [Fact]
        public void Load_WithNullDeserializeResult_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath))
                .Returns(() => string.Empty);

            var loader = CreateLoader();
            const string exceptionMessage = "Deserialized atlas sub texture data is null.";

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(AtlasContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.atlasDataFilePath}'.\n{exceptionMessage}");
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsAtlas()
        {
            // Arrange
            var loader = CreateLoader();
            var atlasData = loader.Load(AtlasContentName);

            // Act
            loader.Unload(AtlasContentName);

            // Assert
            Assert.True(atlasData.IsDisposed);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTextures()
        {
            // Arrange
            var loader = CreateLoader();
            var atlasData = loader.Load(AtlasContentName);

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            Assert.True(atlasData.IsDisposed);
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private AtlasLoader CreateLoader()
            => new (
                this.mockGLInvoker.Object,
                this.mockImageService.Object,
                this.mockAtlasPathResolver.Object,
                this.mockFile.Object);
    }
}
