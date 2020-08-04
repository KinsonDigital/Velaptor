// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using Raptor;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private readonly string rootDir = @"C:\Content\";
        private readonly string graphicsDirName = "Graphics";
        private readonly string soundsDirName = "Sounds";
        private readonly string atlasDirName = "Atlas";
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<AtlasRegionRectangle[]>> mockAtlasDataLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;
        private readonly Mock<IContentSource> mockContentSrc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoaderTests"/> class.
        /// </summary>
        public ContentLoaderTests()
        {
            this.mockContentSrc = new Mock<IContentSource>();
            this.mockContentSrc.SetupGet(p => p.ContentRootDirectory).Returns(this.rootDir);
            this.mockContentSrc.SetupGet(p => p.GraphicsDirectoryName).Returns(this.graphicsDirName);
            this.mockContentSrc.SetupGet(p => p.SoundsDirectoryName).Returns(this.soundsDirName);
            this.mockContentSrc.SetupGet(p => p.AtlasDirectoryName).Returns(this.atlasDirName);

            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockAtlasDataLoader = new Mock<ILoader<AtlasRegionRectangle[]>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();
        }

        #region Method Tests
        [Fact]
        public void LoadTexture_WhenInvoked_LoadsTexture()
        {
            // Act
            var loader = CreateContentLoader();

            loader.LoadTexture("test-texture");

            // Assert
            this.mockTextureLoader.Verify(m => m.Load("test-texture"), Times.Once());
        }

        [Fact]
        public void LoadSound_WhenInvoked_LoadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadSound("sound.ogg");

            // Assert
            this.mockSoundLoader.Verify(m => m.Load("sound.ogg"), Times.Once());
        }

        [Fact]
        public void LoadAtlasData_WhenInvoked_LoadsAtlasData()
        {
            // Act
            this.mockContentSrc.Setup(m => m.GetContentPath(ContentType.Atlas, "test-atlas.json"))
                .Returns("test-path");
            var loader = CreateContentLoader();
            loader.LoadAtlasData("test-atlas.json");

            // Assert
            this.mockContentSrc.Verify(m => m.GetContentPath(ContentType.Atlas, "test-atlas.json"), Times.Once());
            this.mockAtlasDataLoader.Verify(m => m.Load("test-path"), Times.Once());
        }
        #endregion

        /// <summary>
        /// Returns a new instance of a content loader.
        /// </summary>
        /// <returns>A content loader instance to use for testing.</returns>
        private ContentLoader CreateContentLoader()
            => new ContentLoader(this.mockContentSrc.Object,
                                 this.mockTextureLoader.Object,
                                 this.mockAtlasDataLoader.Object,
                                 this.mockSoundLoader.Object);
    }
}
