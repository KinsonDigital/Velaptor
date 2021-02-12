// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System;
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Newtonsoft.Json;
    using Raptor.Content;
    using Raptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasDataLoader{T}"/> class.
    /// </summary>
    public class AtlasLoaderTests
    {
        private const string AtlasContentName = "test-atlas";
        private readonly string atlasDirPath;
        private readonly string atlasFilePath;
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
        };
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPathResolver> mockAtlasPathResolver;
        private readonly Mock<ITexture> mockTexture;
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly AtlasSubTextureData[] atlasSpriteData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
        /// </summary>
        public AtlasLoaderTests()
        {
            this.atlasDirPath = @"C:\temp\Content\Atlas\";
            this.atlasFilePath = $@"{this.atlasDirPath}{AtlasContentName}.png";

            this.mockAtlasPathResolver = new Mock<IPathResolver>();
            this.mockAtlasPathResolver.Setup(m => m.ResolveFilePath(AtlasContentName)).Returns(this.atlasFilePath);

            this.mockTexture = new Mock<ITexture>();

            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockTextureLoader.Setup(m => m.Load(AtlasContentName))
                .Returns(this.mockTexture.Object);

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
            this.mockFile.Setup(m => m.ReadAllText(this.atlasFilePath)).Returns(() =>
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
            this.mockFile.Verify(m => m.ReadAllText(this.atlasFilePath), Times.Once());
            Assert.Equal(this.atlasSpriteData[0], actual[0]);
            Assert.Equal(this.atlasSpriteData[1], actual[1]);
            Assert.Same(this.mockTexture.Object, actual.Texture);
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
            Assert.Same(this.mockTexture.Object, actual.Texture);
            Assert.Equal(AtlasContentName, actual.Name);
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsAtlas()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(AtlasContentName);

            // Act
            loader.Unload(AtlasContentName);

            // Assert
            this.mockTexture.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTextures()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(AtlasContentName);

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.mockTexture.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpoase of testing.
        /// </summary>
        /// <returns>The instnace to test.</returns>
        private AtlasLoader CreateLoader() => new AtlasLoader(this.mockTextureLoader.Object, this.mockAtlasPathResolver.Object, this.mockFile.Object);
    }
}
