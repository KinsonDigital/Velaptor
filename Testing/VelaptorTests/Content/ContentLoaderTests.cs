// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using VelaptorTests.Fakes;
    using Xunit;
    using Assert = Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private const string TextureName = "test-texture";
        private const string AtlasName = "test-atlas";
        private const string SoundName = "test-sound";
        private const string FontName = "test-font";
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;
        private readonly Mock<ILoader<IAtlasData>> mockAtlasLoader;
        private readonly Mock<ILoader<IFont>> mockFontLoader;
        private readonly Mock<ITexture> mockTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoaderTests"/> class.
        /// </summary>
        public ContentLoaderTests()
        {
            this.mockTexture = new Mock<ITexture>();
            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockTextureLoader.Setup(m => m.Load(TextureName)).Returns(this.mockTexture.Object);

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
            loader.LoadTexture(TextureName);

            // Assert
            this.mockTextureLoader.Verify(m => m.Load(TextureName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingSounds_LoadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadSound(SoundName);

            // Assert
            this.mockSoundLoader.Verify(m => m.Load(SoundName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingAtlasData_LoadsAtlasData()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadAtlas(AtlasName);

            // Assert
            this.mockAtlasLoader.Verify(m => m.Load(AtlasName), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingFonts_LoadsFont()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.LoadFont(FontName, 12);

            // Assert
            this.mockFontLoader.Verify(m => m.Load(FontName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingTextures_UnloadsTexture()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.LoadTexture(TextureName);

            // Act
            loader.Unload<ITexture>(TextureName);

            // Assert
            this.mockTextureLoader.Verify(m => m.Unload(TextureName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingSounds_UnloadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.LoadSound(SoundName);

            // Act
            loader.Unload<ISound>(SoundName);

            // Assert
            this.mockSoundLoader.Verify(m => m.Unload(SoundName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingAtlasData_UnloadsAtlasData()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.LoadAtlas(AtlasName);

            // Act
            loader.Unload<IAtlasData>(AtlasName);

            // Assert
            this.mockAtlasLoader.Verify(m => m.Unload(AtlasName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingFonts_UnloadsFont()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.LoadFont(FontName, 12);

            // Act
            loader.Unload<IFont>(FontName);

            // Assert
            this.mockFontLoader.Verify(m => m.Unload(FontName), Times.Once());
        }

        [Fact]
        public void Unload_IfUnloadingUnknownContentType_ThrowException()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act & Assert
            Assert.ThrowsWithMessage<UnknownContentException>(() =>
            {
                loader.Unload<IUnknownContentItem>("unknown-content");
            }, $"The content of type '{typeof(IUnknownContentItem)}' is unknown.");
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
