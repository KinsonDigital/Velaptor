// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO.Abstractions;
    using System.Linq;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasLoader"/> class.
    /// </summary>
    public class AtlasLoaderTests
    {
        private const string TextureExtension = ".png";
        private const string AtlasDataExtension = ".json";
        private const string DirPath = @"C:\Content\Atlas\";
        private const string AtlasContentName = "test-atlas";
        private const string FakeJSONData = "fake-json-data";
        private readonly string atlasImageFilePath = $"{DirPath}{AtlasContentName}{TextureExtension}";
        private readonly string atlasDataFilePath = $"{DirPath}{AtlasContentName}{AtlasDataExtension}";
        private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
        private readonly Mock<IAtlasDataFactory> mockAtlasDataFactory;
        private readonly Mock<IPathResolver> mockAtlasPathResolver;
        private readonly Mock<IJSONService> mockJSONService;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPath> mockPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
        /// </summary>
        public AtlasLoaderTests()
        {
            this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
            this.mockAtlasDataFactory = new Mock<IAtlasDataFactory>();

            this.mockAtlasPathResolver = new Mock<IPathResolver>();
            this.mockAtlasPathResolver.Setup(m => m.ResolveDirPath()).Returns(DirPath);

            this.mockJSONService = new Mock<IJSONService>();

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.ReadAllText(this.atlasDataFilePath))
                .Returns(FakeJSONData);
            this.mockFile.Setup(m => m.Exists(this.atlasDataFilePath)).Returns(true);
            this.mockFile.Setup(m => m.Exists(this.atlasImageFilePath)).Returns(true);

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetDirectoryName(this.atlasImageFilePath)).Returns(DirPath);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.atlasImageFilePath)).Returns(AtlasContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AtlasContentName)).Returns(AtlasContentName);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullTextureCacheParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    null,
                    this.mockAtlasDataFactory.Object,
                    this.mockAtlasPathResolver.Object,
                    this.mockJSONService.Object,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'textureCache')");
        }

        [Fact]
        public void Ctor_WithNullAtlasDataFactoryParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    this.mockTextureCache.Object,
                    null,
                    this.mockAtlasPathResolver.Object,
                    this.mockJSONService.Object,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'atlasDataFactory')");
        }

        [Fact]
        public void Ctor_WithNullAtlasDataPathResolverParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    this.mockTextureCache.Object,
                    this.mockAtlasDataFactory.Object,
                    null,
                    this.mockJSONService.Object,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'atlasDataPathResolver')");
        }

        [Fact]
        public void Ctor_WithNullJSONServiceParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    this.mockTextureCache.Object,
                    this.mockAtlasDataFactory.Object,
                    this.mockAtlasPathResolver.Object,
                    null,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'jsonService')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    this.mockTextureCache.Object,
                    this.mockAtlasDataFactory.Object,
                    this.mockAtlasPathResolver.Object,
                    this.mockJSONService.Object,
                    null,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new AtlasLoader(
                    this.mockTextureCache.Object,
                    this.mockAtlasDataFactory.Object,
                    this.mockAtlasPathResolver.Object,
                    this.mockJSONService.Object,
                    this.mockFile.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'path')");
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Load_WithNullOrEmptyValue_ThrowsException(string value)
        {
            // Arrange
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                loader.Load(value);
            }, "The string parameter must not be null or empty. (Parameter 'contentNameOrPath')");
        }

        [Fact]
        public void Load_WithInvalidFullFilePathExtensions_ThrowsException()
        {
            // Arrange
            const string extension = ".txt";
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadAtlasException>(() =>
            {
                loader.Load($"{DirPath}{AtlasContentName}{extension}");
            }, "When performing full content file path loads, the files must be a '.png' or '.json' extension.");
        }

        [Fact]
        public void Load_WhenUsingFullFilePath_LoadsTextureAtlasData()
        {
            // Arrange
            var mockAtlasData = new Mock<IAtlasData>();

            var loader = CreateLoader();

            this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(TextureExtension);

            var atlasData = MockAtlasJSONData().ToArray();
            MockAtlasDataFactory(mockAtlasData.Object, atlasData, DirPath, $"{AtlasContentName}");

            // Act
            var actual = loader.Load(this.atlasImageFilePath);

            // Assert
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.atlasImageFilePath), Times.Once);
            this.mockPath.Verify(m => m.GetDirectoryName(this.atlasImageFilePath), Times.Once);
            this.mockFile.Verify(m => m.ReadAllText(this.atlasDataFilePath), Times.Once());
            this.mockJSONService.Verify(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData), Times.Once);
            this.mockAtlasDataFactory.Verify(m => m.Create(atlasData, DirPath, $"{AtlasContentName}"));
            Assert.Same(mockAtlasData.Object, actual);
        }

        [Fact]
        public void Load_WhenAtlasJSONDataFileDoNotExist_ThrowsException()
        {
            // Arrange
            const string dirPath = @"C:\app-dir\content\atlas\";
            const string contentName = "missing-json-file";
            var invalidFilePath = $"{dirPath}{contentName}{AtlasDataExtension}";
            var loader = CreateLoader();

            var expected = $"The atlas data directory '{dirPath}' does not contain the";
            expected += $" required '{dirPath}{contentName}{AtlasDataExtension}' atlas data file.";

            this.mockFile.Setup(m => m.Exists(invalidFilePath)).Returns(false);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(invalidFilePath)).Returns(contentName);
            this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(AtlasDataExtension);
            this.mockPath.Setup(m => m.GetDirectoryName(invalidFilePath)).Returns(dirPath);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadAtlasException>(() =>
            {
                loader.Load(invalidFilePath);
            }, expected);
        }

        [Fact]
        public void Load_WhenAtlasImageDataFileDoNotExist_ThrowsException()
        {
            // Arrange
            const string dirPath = @"C:\app-dir\content\atlas\";
            const string jsonContentName = "missing-file";
            const string imageContentName = "missing-file";
            var validJSONFilePath = $"{dirPath}{jsonContentName}{AtlasDataExtension}";
            var invalidImageFilePath = $"{dirPath}{imageContentName}{TextureExtension}";
            var loader = CreateLoader();

            var expected = $"The atlas data directory '{dirPath}' does not contain the";
            expected += $" required '{dirPath}{imageContentName}{TextureExtension}' atlas image file.";

            this.mockFile.Setup(m => m.Exists(validJSONFilePath)).Returns(true);
            this.mockFile.Setup(m => m.Exists(invalidImageFilePath)).Returns(false);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(invalidImageFilePath)).Returns(imageContentName);
            this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(TextureExtension);
            this.mockPath.Setup(m => m.GetDirectoryName(invalidImageFilePath)).Returns(dirPath);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadAtlasException>(() =>
            {
                loader.Load(invalidImageFilePath);
            }, expected);
        }

        [Fact]
        public void Load_WhenUsingJustContentName_LoadsTextureAtlasData()
        {
            // Arrange
            var mockAtlasData = new Mock<IAtlasData>();

            var loader = CreateLoader();

            var atlasData = MockAtlasJSONData().ToArray();
            MockAtlasDataFactory(mockAtlasData.Object, atlasData, DirPath, AtlasContentName);

            // Act
            var actual = loader.Load(AtlasContentName);

            // Assert
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(AtlasContentName), Times.Once);
            this.mockAtlasPathResolver.Verify(m => m.ResolveDirPath(), Times.Once);
            this.mockFile.Verify(m => m.ReadAllText(this.atlasDataFilePath), Times.Once());
            this.mockJSONService.Verify(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData), Times.Once);
            this.mockAtlasDataFactory.Verify(m => m.Create(atlasData, DirPath, AtlasContentName));
            Assert.Same(mockAtlasData.Object, actual);
        }

        [Fact]
        public void Load_WhenNullJSONDataDeserializationResult_ThrowsException()
        {
            // Arrange
            this.mockJSONService.Setup(m => m.Deserialize<AtlasSubTextureData[]>(It.IsAny<string>()))
                .Returns(() => null);

            var loader = CreateLoader();

            // Act & Assert1
            AssertExtensions.ThrowsWithMessage<LoadContentException>(() =>
            {
                loader.Load(AtlasContentName);
            }, $"There was an issue deserializing the JSON atlas data file at '{this.atlasDataFilePath}'.");
        }

        [Fact]
        public void Load_WithDirPath_ThrowsException()
        {
            // Arrange
            var expected = $"Directory paths not allowed when loading texture atlas data.\n";
            expected += "Relative and fully qualified directory paths not valid.\n";
            expected += "The path must be a fully qualified file path or content item name\n";
            expected += @"located in the application's './Content/Atlas' directory.";
            var mockAtlasData = new Mock<IAtlasData>();

            var loader = CreateLoader();

            var atlasData = MockAtlasJSONData().ToArray();
            MockAtlasDataFactory(mockAtlasData.Object, atlasData, DirPath, AtlasContentName);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadAtlasException>(() =>
            {
                loader.Load(DirPath);
            }, expected);
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsAtlas()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(AtlasContentName);

            // Assert
            this.mockTextureCache.Verify(m => m.Unload(AtlasContentName));
        }
        #endregion

        /// <summary>
        /// Creates atlas sub texture data for the purpose of testing.
        /// </summary>
        /// <returns>The data to use for testing.</returns>
        private static AtlasSubTextureData[] CreateAtlasSubTextureData()
        {
            var items = new List<AtlasSubTextureData>();

            for (var i = 0; i < 2; i++)
            {
                items.Add(new AtlasSubTextureData
                {
                    Name = $"sub-texture{i}",
                    Bounds = new Rectangle(i + 1, i + 2, i + 3, i + 4),
                    FrameIndex = i,
                });
            }

            return items.ToArray();
        }

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private AtlasLoader CreateLoader()
            => new (
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
                this.mockFile.Object,
                this.mockPath.Object);

        /// <summary>
        /// Mocks the atlas data factory for the purpose of testing.
        /// </summary>
        /// <param name="dataIfInvokedCorrectly">The mock data to return if the mock is invoked correctly.</param>
        /// <param name="subTextureData">The sub texture data to return if the mock is invoked correctly.</param>
        /// <param name="dirPath">The test directory path.</param>
        /// <param name="atlasName">The test atlas name.</param>
        private void MockAtlasDataFactory(
            IAtlasData dataIfInvokedCorrectly,
            AtlasSubTextureData[] subTextureData,
            string dirPath,
            string atlasName)
        {
            this.mockAtlasDataFactory.Setup(m =>
                    m.Create(subTextureData,
                        dirPath,
                        atlasName))
                .Returns(dataIfInvokedCorrectly);
        }

        /// <summary>
        /// Mocks the JSON data deserialization process.
        /// </summary>
        /// <returns>The test data to use if the mock is invoked correctly.</returns>
        private IEnumerable<AtlasSubTextureData> MockAtlasJSONData()
        {
            var data = CreateAtlasSubTextureData();

            this.mockJSONService.Setup(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData))
                .Returns(data);

            return data;
        }
    }
}
