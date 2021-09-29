// <copyright file="ButtonTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using VelaptorTests.Helpers;

namespace VelaptorTests.UI
{
    using Moq;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Button"/> class.
    /// </summary>
    public class ButtonTests
    {
        private readonly Mock<IContentLoader> mockContentLoader;
        private readonly Mock<ITexture> mockTexture;
        private readonly Mock<ControlBase> mockLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTests"/> class.
        /// </summary>
        public ButtonTests()
        {
            this.mockTexture = new Mock<ITexture>();
            this.mockTexture.SetupGet(p => p.Width).Returns(200);
            this.mockTexture.SetupGet(p => p.Height).Returns(100);

            this.mockLabel = new Mock<ControlBase>();

            this.mockContentLoader = new Mock<IContentLoader>();
            this.mockContentLoader.Setup(m => m.Load<ITexture>("button-face"))
                .Returns(this.mockTexture.Object);
        }

        #region Prop Tests
        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 200)]
        public void Width_WhenGettingValueBeforeLoadingContent_ReturnsCorrectResult(bool loadContent, int expected)
        {
            // Arrange
            var button = CreateButton();

            if (loadContent)
            {
                button.LoadContent();
            }

            // Act
            var actual = button.Width;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 100)]
        public void Height_WhenGettingValueBeforeLoadingContent_ReturnsCorrectResult(bool loadContent, int expected)
        {
            // Arrange
            var button = CreateButton();

            if (loadContent)
            {
                button.LoadContent();
            }

            // Act
            var actual = button.Height;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvoked_LoadsContent()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.LoadContent();

            // Assert
            this.mockContentLoader.Verify(m => m.Load<ITexture>("button-face"), Times.Once);
            this.mockLabel.Verify(m => m.LoadContent(), Times.Once);
        }

        [Fact]
        public void Render_WithNoLoadedContentButVisible_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Visible = true;

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                             It.IsAny<int>(),
                             It.IsAny<int>(),
                             It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WithLoadedContentButInvisible_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Visible = false;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WithNullTexture_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();

            this.mockContentLoader.Setup(m => m.Load<ITexture>("button-face"))
                .Returns(() =>
                {
                    ITexture? nullTexture = null;

#pragma warning disable 8603
                    return nullTexture;
#pragma warning restore 8603
                });
            var button = CreateButton();
            button.Visible = true;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvoked_RendersTexture()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Position = new Point(10, 20);
            button.Visible = true;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(this.mockTexture.Object,
                    110,
                    70,
                    Color.White), Times.Once);

            this.mockLabel.Verify(m =>
                m.Render(mockSpriteBatch.Object), Times.Once);
        }

        [Fact]
        public void Dispose_WithNoLoadedContent_DoesNotThrowException()
        {
            // Arrange
            var button = CreateButton();

            // Act & Assert
            AssertExtensions.DoesNotThrow<NullReferenceException>(() =>
            {
                button.Dispose();
            });

            this.mockTexture.Verify(m => m.Dispose(), Times.Never);
        }

        [Fact]
        public void Dispose_WithLoadedContent_DisposesOfTexture()
        {
            // Arrange
            var button = CreateButton();
            button.LoadContent();

            // Act & Assert
            button.Dispose();
            button.Dispose();

            this.mockTexture.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new button for the purpose of testing.
        /// </summary>
        /// <returns>The button instance to test.</returns>
        private Button CreateButton()
            => new Button(this.mockContentLoader.Object, this.mockLabel.Object);
    }
}
