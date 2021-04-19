// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
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
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="AtlasDataLoader{T}"/> class.
    /// </summary>
    public class AtlasLoaderTests
    {
        private const string AtlasContentName = "test-atlas";
        private readonly string atlasDirPath;
        private readonly string atlasDataFilePath;
        private readonly string atlasImageFilePath;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
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
            this.atlasDirPath = @"C:\temp\Content\Atlas\";
            this.atlasDataFilePath = $@"{this.atlasDirPath}{AtlasContentName}.json";
            this.atlasImageFilePath = $@"{this.atlasDirPath}{AtlasContentName}.png";

            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockGLInvoker.Name = nameof(this.mockGLInvoker);

            this.mockAtlasPathResolver = new Mock<IPathResolver>();
            this.mockAtlasPathResolver.Setup(m => m.ResolveDirPath()).Returns(this.atlasDirPath);

            this.mockImageService = new Mock<IImageService>();
            this.mockImageService.Name = nameof(this.mockImageService);

            this.atlasSpriteData = new AtlasSubTextureData[]
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
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath)).Returns(() =>
            {
                return JsonConvert.SerializeObject(this.atlasSpriteData, this.jsonSettings);
            });
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
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath)).Returns(() =>
            {
                return "invalid-data";
            });

            var loader = CreateLoader();
            var newtonsoftErrorMsg = "Unexpected character encountered while parsing value: i. Path '', line 0, position 0.";

            // Act & Assert
            Assert.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(AtlasContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.atlasDataFilePath}'.\n{newtonsoftErrorMsg}");
        }

        [Fact]
        public void Load_WithNullDeserializeResult_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath)).Returns(() =>
            {
                return string.Empty;
            });

            var loader = CreateLoader();
            var exceptionMessage = "Deserialized atlas sub texture data is null.";

            // Act & Assert
            Assert.ThrowsWithMessage<LoadContentException>(() =>
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
            Assert.True(atlasData.Unloaded);
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
            Assert.True(atlasData.Unloaded);
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpoase of testing.
        /// </summary>
        /// <returns>The instnace to test.</returns>
        private AtlasLoader CreateLoader()
            => new AtlasLoader(
                this.mockGLInvoker.Object,
                this.mockImageService.Object,
                this.mockAtlasPathResolver.Object,
                this.mockFile.Object);
    }
}
