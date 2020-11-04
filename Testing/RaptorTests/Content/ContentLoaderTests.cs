// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Graphics;
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
        public void LoadTexture_WhenInvoked_LoadsTexture()
        {
            // Act
            var loader = CreateContentLoader();

            loader.Load<ITexture>("test-texture");

            // Assert
            this.mockTextureLoader.Verify(m => m.Load("test-texture"), Times.Once());
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
