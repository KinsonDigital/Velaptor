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
        private const string AtlasFileNameWithoutExtension = "test-atlas";
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
            this.atlasFilePath = $@"{this.atlasDirPath}{AtlasFileNameWithoutExtension}.png";

            this.mockAtlasPathResolver = new Mock<IPathResolver>();
            this.mockAtlasPathResolver.Setup(m => m.ResolveFilePath(AtlasFileNameWithoutExtension)).Returns(this.atlasFilePath);

            this.mockTexture = new Mock<ITexture>();

            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockTextureLoader.Setup(m => m.Load(AtlasFileNameWithoutExtension))
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
            var loader = new AtlasLoader(this.mockTextureLoader.Object, this.mockAtlasPathResolver.Object, this.mockFile.Object);

            // Act
            var actual = loader.Load(AtlasFileNameWithoutExtension);

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
            var loader = new AtlasLoader(this.mockTextureLoader.Object, this.mockAtlasPathResolver.Object, this.mockFile.Object);
            loader.Load(AtlasFileNameWithoutExtension);

            // Act
            var actual = loader.Load(AtlasFileNameWithoutExtension);

            // Assert
            Assert.Equal(this.atlasSpriteData[0], actual[0]);
            Assert.Equal(this.atlasSpriteData[1], actual[1]);
            Assert.Same(this.mockTexture.Object, actual.Texture);
            Assert.Equal(AtlasFileNameWithoutExtension, actual.Name);
        }
        #endregion
    }
}
