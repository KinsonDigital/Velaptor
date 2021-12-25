// <copyright file="LabelTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Label"/> class.
    /// </summary>
    public class LabelTests
    {
        private const string TextValue = "hello world";
        private readonly Mock<IContentLoader> mockContentLoader;
        private readonly Mock<IFont> mockFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTests"/> class.
        /// </summary>
        public LabelTests()
        {
            this.mockFont = new Mock<IFont>();
            MockGlyphs(TextValue);

            this.mockContentLoader = new Mock<IContentLoader>();
            this.mockContentLoader.Setup(m => m.Load<IFont>(It.IsAny<string>()))
                .Returns(this.mockFont.Object);
        }

        #region Prop Tests
        [Fact]
        public void Text_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Text = "test-value";
            var actual = label.Text;

            // Assert
            Assert.Equal("test-value", actual);
        }

        [Fact]
        public void Left_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();
            label.Position = new Point(400, 300);
            label.Width = 100;

            // Act
            label.Left = 50;
            var actual = label.Left;

            // Assert
            Assert.Equal(50, actual);
            Assert.Equal(100, label.Position.X);
        }

        [Fact]
        public void Right_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();
            label.Position = new Point(400, 300);
            label.Width = 100;

            // Act
            label.Right = 300;
            var actual = label.Right;

            // Assert
            Assert.Equal(300, actual);
            Assert.Equal(250, label.Position.X);
        }

        [Fact]
        public void Top_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();
            label.Position = new Point(400, 300);
            label.Height = 100;

            // Act
            label.Top = 300;
            var actual = label.Top;

            // Assert
            Assert.Equal(300, actual);
            Assert.Equal(350, label.Position.Y);
        }

        [Fact]
        public void Bottom_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();
            label.Position = new Point(400, 300);
            label.Height = 100;

            // Act
            label.Bottom = 300;
            var actual = label.Bottom;

            // Assert
            Assert.Equal(300, actual);
            Assert.Equal(250, label.Position.Y);
        }

        [Fact]
        public void Width_WhenSettingTextProp_CalculatesCorrectWidth()
        {
            // Arrange
            this.mockFont.Setup(m => m.Measure(TextValue))
                .Returns(new SizeF(10f, 0f));

            var label = CreateLabel();

            label.Text = TextValue;
            label.LoadContent();

            // Act
            var actual = label.Width;

            // Assert
            Assert.Equal(10u, actual);
        }

        [Fact]
        public void Height_WhenSettingTextProp_CalculatesCorrectHeight()
        {
            // Arrange
            this.mockFont.Setup(m => m.Measure(TextValue))
                .Returns(new SizeF(0f, 20f));

            var label = CreateLabel();

            label.Text = TextValue;
            label.LoadContent();

            // Act
            var actual = label.Height;

            // Assert
            Assert.Equal(20u, actual);
        }

        [Fact]
        public void WidthAndHeight_WhenTextIsEmpty_CalculatesCorrectResults()
        {
            // Arrange
            var label = CreateLabel();

            label.Text = string.Empty;
            label.LoadContent();

            // Act
            var actualWidth = label.Width;
            var actualHeight = label.Height;

            // Assert
            Assert.Equal(0u, actualWidth);
            Assert.Equal(0u, actualHeight);
        }

        [Fact]
        public void Style_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Style = FontStyle.Bold;
            var actual = label.Style;

            // Assert
            Assert.Equal(FontStyle.Bold, actual);
        }

        [Fact]
        public void Color_WhenNotGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            var actual = label.Color;

            // Assert
            Assert.Equal(Color.Black, actual);
        }

        [Fact]
        public void Color_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Color = Color.FromArgb(11, 22, 33, 44);
            var actual = label.Color;

            // Assert
            Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvoked_LoadsCorrectFont()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.LoadContent();
            label.LoadContent();

            // Assert
            this.mockContentLoader.Verify(m => m.Load<IFont>("TimesNewRoman"), Times.Once);
        }

        [Fact]
        public void UnloadContent_WhenLoadedAndNotDisposed_UnloadsContent()
        {
            // Arrange
            var label = CreateLabel();
            label.LoadContent();

            // Act
            label.UnloadContent();

            // Assert
            this.mockFont.Verify(m => m.Dispose(), Times.Once);
        }

        [Fact]
        public void UnloadContent_WhenNotLoaded_DoesNotUnloadContent()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.UnloadContent();

            // Assert
            this.mockFont.Verify(m => m.Dispose(), Times.Never);
        }

        [Fact]
        public void UnloadContent_WhenLoadedAndDisposed_DoesNotUnloadContent()
        {
            // Arrange
            var label = CreateLabel();
            label.LoadContent();
            label.Dispose();

            // Act
            label.UnloadContent();

            // Assert
            this.mockFont.Verify(m => m.Dispose(), Times.Once);
        }

        [Fact]
        public void Render_WithNullSpriteBatch_ThrowsException()
        {
            // Arrange
            var label = CreateLabel();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                label.Render(null);
            }, "The parameter must not be null. (Parameter 'spriteBatch')");
        }

        [Fact]
        public void Render_WhenInvokedWithoutLoadedContent_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var label = CreateLabel();
            label.Visible = true;

            // Act
            label.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m => m.Render(It.IsAny<IFont>(),
                                                               It.IsAny<string>(),
                                                               It.IsAny<int>(),
                                                               It.IsAny<int>(),
                                                               It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvokedWhileNotVisible_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var label = CreateLabel();
            label.Visible = false;

            // Act
            label.LoadContent();
            label.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<IFont>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvoked_RendersText()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();

            var label = CreateLabel();
            label.Text = TextValue;
            label.Position = new Point(100, 200);
            label.Visible = true;
            label.Color = Color.FromArgb(11, 22, 33, 44);
            label.LoadContent();

            // Act
            label.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(this.mockFont.Object,
                    TextValue,
                    100,
                    200,
                    Color.FromArgb(11, 22, 33, 44)), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvokedWithoutLoadingContent_DoesNotThrowException()
        {
            // Arrange
            var label = CreateLabel();

            // Act & Assert
            AssertExtensions.DoesNotThrow<NullReferenceException>(() =>
            {
                label.Dispose();
            });
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfFont()
        {
            // Arrange
            var label = CreateLabel();
            label.LoadContent();

            // Act
            label.Dispose();
            label.Dispose();

            // Assert
            this.mockFont.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Mocks the glyphs for the given <param name="text"></param>.
        /// </summary>
        /// <param name="text">The text glyphs to mock for the font object.</param>
        private void MockGlyphs(string text)
        {
            this.mockFont.SetupGet(p => p.Metrics)
                .Returns(() =>
                {
                    var result = new List<GlyphMetrics>();

                    for (var i = 0; i < text.Length; i++)
                    {
                        var charIndex = i;
                        var alreadyAdded = result.Any(g => g.Glyph == text[charIndex]);

                        if (alreadyAdded is false)
                        {
                            result.Add(new GlyphMetrics { Glyph = text[charIndex], GlyphWidth = charIndex, GlyphHeight = charIndex * 10 });
                        }
                    }

                    return new ReadOnlyCollection<GlyphMetrics>(result);
                });
        }

        /// <summary>
        /// Creates a new label for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private Label CreateLabel() => new (this.mockContentLoader.Object);
    }
}
