// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private const string TextureContentName = "test-texture";
        private const string AtlasContentName = "test-atlas";
        private const string SoundContentName = "test-sound";
        private const string FontContentName = "test-font";
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;
        private readonly Mock<ILoader<IAtlasData>> mockAtlasLoader;
        private readonly Mock<ILoader<IFont>> mockFontLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoaderTests"/> class.
        /// </summary>
        public ContentLoaderTests()
        {
            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();
            this.mockAtlasLoader = new Mock<ILoader<IAtlasData>>();
            this.mockFontLoader = new Mock<ILoader<IFont>>();
        }

        #region Method Tests
        [Fact]
        public void Load_WhenLoadingTextures_LoadsTexture()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadTexture(TextureContentName);

            // Assert
            this.mockTextureLoader.Verify(m => m.Load(TextureContentName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingSounds_LoadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadSound(SoundContentName);

            // Assert
            this.mockSoundLoader.Verify(m => m.Load(SoundContentName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingAtlasData_LoadsAtlasData()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadAtlas(AtlasContentName);

            // Assert
            this.mockAtlasLoader.Verify(m => m.Load(AtlasContentName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingFonts_LoadsFont()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadFont(FontContentName, 12);

            // Assert
            this.mockFontLoader.Verify(m => m.Load($"{FontContentName}|size:12"), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingTextures_UnloadsTexture()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.FilePath).Returns(TextureContentName);

            this.mockTextureLoader.Setup(m => m.Load(TextureContentName)).Returns(mockTexture.Object);

            var loader = CreateContentLoader();
            var texture = loader.LoadTexture(TextureContentName);

            // Act
            loader.UnloadTexture(texture);

            // Assert
            this.mockTextureLoader.Verify(m => m.Unload(TextureContentName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingSounds_UnloadsSound()
        {
            // Arrange
            var mockSound = new Mock<ISound>();
            mockSound.SetupGet(p => p.FilePath).Returns(SoundContentName);

            this.mockSoundLoader.Setup(m => m.Load(SoundContentName)).Returns(mockSound.Object);

            var loader = CreateContentLoader();
            var sound = loader.LoadSound(SoundContentName);

            // Act
            loader.UnloadSound(sound);

            // Assert
            this.mockSoundLoader.Verify(m => m.Unload(SoundContentName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingAtlasData_UnloadsAtlasData()
        {
            // Arrange
            var mockAtlasData = new Mock<IAtlasData>();
            mockAtlasData.SetupGet(p => p.FilePath).Returns(AtlasContentName);

            this.mockAtlasLoader.Setup(m => m.Load(AtlasContentName)).Returns(mockAtlasData.Object);

            var loader = CreateContentLoader();
            var atlasData = loader.LoadAtlas(AtlasContentName);

            // Act
            loader.UnloadAtlas(atlasData);

            // Assert
            this.mockAtlasLoader.Verify(m => m.Unload(AtlasContentName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingFonts_UnloadsFont()
        {
            // Arrange
            const int fontSize = 12;
            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FilePath).Returns(FontContentName);
            mockFont.SetupGet(p => p.Size).Returns(fontSize);

            this.mockFontLoader.Setup(m => m.Load($"{FontContentName}|size:{fontSize}")).Returns(mockFont.Object);

            var loader = CreateContentLoader();
            var font = loader.LoadFont(FontContentName, fontSize);

            // Act
            loader.UnloadFont(font);

            // Assert
            this.mockFontLoader.Verify(m => m.Unload($"{FontContentName}|size:{fontSize}"), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfLoaders()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.mockTextureLoader.Verify(m => m.Dispose(), Times.Once());
            this.mockAtlasLoader.Verify(m => m.Dispose(), Times.Once());
            this.mockSoundLoader.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Returns a new instance of a content loader.
        /// </summary>
        /// <returns>A content loader instance to use for testing.</returns>
        private ContentLoader CreateContentLoader()
            => new (this.mockTextureLoader.Object,
                    this.mockSoundLoader.Object,
                    this.mockAtlasLoader.Object,
                    this.mockFontLoader.Object);
    }
}
