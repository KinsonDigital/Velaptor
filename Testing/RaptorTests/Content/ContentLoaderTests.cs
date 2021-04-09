// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using RaptorTests.Fakes;
    using RaptorTests.Helpers;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private const string TextureName = "test-texture";
        private const string AtlasName = "test-atlas";
        private const string SoundName = "test-sound";
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;
        private readonly Mock<ILoader<IAtlasData>> mockAtlasLoader;
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
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsTexture()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.Load<ITexture>("test-texture");

            // Assert
            this.mockTextureLoader.Verify(m => m.Load("test-texture"), Times.Once());
        }

        [Fact]
        public void Load_WhenInvoked_LoadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act
            loader.Load<ISound>("test-sound");

            // Assert
            this.mockSoundLoader.Verify(m => m.Load("test-sound"), Times.Once());
        }

        [Fact]
        public void Load_WhenLoadingUnknownContent_ThrowsException()
        {
            // Arrange
            var loader = CreateContentLoader();

            // Act & Assert
            Assert.ThrowsWithMessage<UnknownContentException>(() =>
            {
                loader.Load<IInvalidContent>("test-texture");
            }, "Content of type 'RaptorTests.Fakes.IInvalidContent' invalid.  Content types must inherit from interface 'IContent'.");
        }

        [Fact]
        public void Unload_WhenUnloadingTexture_UnloadsTexture()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.Load<ITexture>(TextureName);

            // Act
            loader.Unload<ITexture>(TextureName);

            // Assert
            this.mockTextureLoader.Verify(m => m.Unload(TextureName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingAtlasData_UnloadsAtlasData()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.Load<IAtlasData>(AtlasName);

            // Act
            loader.Unload<IAtlasData>(AtlasName);

            // Assert
            this.mockAtlasLoader.Verify(m => m.Unload(AtlasName), Times.Once());
        }

        [Fact]
        public void Unload_WhenUnloadingSound_UnloadsSound()
        {
            // Arrange
            var loader = CreateContentLoader();
            loader.Load<ISound>(SoundName);

            // Act
            loader.Unload<ISound>(SoundName);

            // Assert
            this.mockSoundLoader.Verify(m => m.Unload(SoundName), Times.Once());
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
            => new ContentLoader(this.mockTextureLoader.Object,
                                 this.mockSoundLoader.Object,
                                 this.mockAtlasLoader.Object);
    }
}
