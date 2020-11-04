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

    /// <summary>
    /// Tests the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoaderTests"/> class.
        /// </summary>
        public ContentLoaderTests()
        {
            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();
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
            AssertHelpers.ThrowsWithMessage<UnknownContentException>(() =>
            {
                loader.Load<IInvalidContent>("test-texture");
            }, "Content of type 'RaptorTests.Fakes.IInvalidContent' invalid.  Content types must inherit from interface 'IContent'.");
        }
        #endregion

        /// <summary>
        /// Returns a new instance of a content loader.
        /// </summary>
        /// <returns>A content loader instance to use for testing.</returns>
        private ContentLoader CreateContentLoader()
            => new ContentLoader(this.mockTextureLoader.Object,
                                 this.mockSoundLoader.Object);
    }
}
