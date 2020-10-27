// <copyright file="WindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests
{
    using System;
    using System.Numerics;
    using Moq;
    using Raptor;
    using Raptor.Content;
    using RaptorTests.Fakes;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Window"/> class.
    /// </summary>
    public class WindowTests
    {
        private readonly Mock<IWindow> mockWindow;
        private readonly Mock<IContentLoader> mockContentLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTests"/> class.
        /// </summary>
        public WindowTests()
        {
            this.mockWindow = new Mock<IWindow>();
            this.mockContentLoader = new Mock<IContentLoader>();
        }

        #region Prop Tests
        [Fact]
        public void Title_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            this.mockWindow.SetupProperty(p => p.Title);

            var window = CreateWindow();

            window.Title = "test-title";

            // Act
            var actual = window.Title;

            // Assert
            Assert.Equal("test-title", actual);
        }

        [Fact]
        public void Position_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            this.mockWindow.SetupProperty(p => p.Position);
            var expected = new Vector2(11, 22);
            var window = CreateWindow();

            // Act
            window.Position = new Vector2(11, 22);
            var actual = window.Position;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Width_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            this.mockWindow.SetupProperty(p => p.Width);

            var window = CreateWindow();
            window.Width = 1234;

            // Act
            var actual = window.Width;

            // Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void Height_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            this.mockWindow.SetupProperty(p => p.Height);

            var window = CreateWindow();
            window.Height = 1234;

            // Act
            var actual = window.Height;

            // Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void UpdateFrequency_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            this.mockWindow.SetupProperty(p => p.UpdateFreq);

            var window = CreateWindow();
            window.UpdateFrequency = 1234;

            // Act
            var actual = window.UpdateFrequency;

            // Assert
            Assert.Equal(1234, actual);
        }
        #endregion

        #region Method tests
        [Fact]
        public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullWindow_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var window = new WindowFake(null, this.mockContentLoader.Object);
            }, "Window must not be null. (Parameter 'window')");
        }

        [Fact]
        public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullLoader_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var window = new WindowFake(this.mockWindow.Object, null);
            }, "Content loader must not be null. (Parameter 'contentLoader')");
        }

        [Fact]
        public void Ctor_WhenUsingOverloadWithLoaderAndWidthAndHeightWithNullLoader_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var window = new WindowFake(window: null, contentLoader: null);
            }, "Window must not be null. (Parameter 'window')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithWindowAndContentLoader_SetsWindowAndContentLoader()
        {
            // Act
            var window = CreateWindow();

            // Assert
            Assert.Equal(this.mockContentLoader.Object, window.ContentLoader);
        }

        [Fact]
        public void Show_WhenInvoked_ShowsWindow()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Show();

            // Assert
            this.mockWindow.Verify(m => m.Show(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Dispose();
            window.Dispose();

            // Assert
            this.mockWindow.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="WindowFake"/> for the purpose
        /// of testing the abstract <see cref="Window"/> class.
        /// </summary>
        /// <returns>The instance used for testing.</returns>
        private WindowFake CreateWindow() => new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object);
    }
}
