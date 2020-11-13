// <copyright file="WindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Desktop
{
    using System;
    using System.Numerics;
    using Moq;
    using Raptor;
    using Raptor.Content;
    using Raptor.Desktop;
    using RaptorTests.Fakes;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Window"/> class.
    /// </summary>
    public class WindowTests
    {
        // TODO: Convert all propers that use pure interface invokes to use MOQ verifies
        private readonly Mock<IWindow> mockWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTests"/> class.
        /// </summary>
        public WindowTests() => this.mockWindow = new Mock<IWindow>();

        #region Prop Tests
        [Fact]
        public void Title_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Title = "test-title";
            _ = window.Title;

            // Assert
            this.mockWindow.VerifySet(p => p.Title = "test-title", Times.Once());
            this.mockWindow.VerifyGet(p => p.Title, Times.Once());
        }

        [Fact]
        public void Position_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Position = new Vector2(11, 22);
            _ = window.Position;

            // Assert
            this.mockWindow.VerifySet(p => p.Position = new Vector2(11, 22), Times.Once());
            this.mockWindow.VerifyGet(p => p.Position, Times.Once());
        }

        [Fact]
        public void Width_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Width = 1234;
            _ = window.Width;

            // Assert
            this.mockWindow.VerifySet(p => p.Width = 1234, Times.Once());
            this.mockWindow.VerifyGet(p => p.Width, Times.Once());
        }

        [Fact]
        public void Height_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();
            window.Height = 1234;

            // Act
            _ = window.Height;

            // Assert
            this.mockWindow.VerifySet(p => p.Height = 1234, Times.Once());
            this.mockWindow.VerifyGet(p => p.Height, Times.Once());
        }

        [Fact]
        public void AutoClearBuffer_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.AutoClearBuffer = true;
            _ = window.AutoClearBuffer;

            // Assert
            this.mockWindow.VerifySet(p => p.AutoClearBuffer = true, Times.Once());
            this.mockWindow.VerifyGet(p => p.AutoClearBuffer, Times.Once());
        }

        [Fact]
        public void MouseCursorVisible_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.MouseCursorVisible = true;
            _ = window.MouseCursorVisible;

            // Assert
            this.mockWindow.VerifySet(p => p.MouseCursorVisible = true, Times.Once());
            this.mockWindow.VerifyGet(p => p.MouseCursorVisible, Times.Once());
        }

        [Fact]
        public void UpdateFrequency_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.UpdateFrequency = 1234;
            _ = window.UpdateFrequency;

            // Assert
            this.mockWindow.VerifySet(p => p.UpdateFrequency = 1234, Times.Once());
            this.mockWindow.VerifyGet(p => p.UpdateFrequency, Times.Once());
        }

        [Fact]
        public void WindowState_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.WindowState = StateOfWindow.FullScreen;
            _ = window.WindowState;

            // Assert
            this.mockWindow.VerifySet(p => p.WindowState = StateOfWindow.FullScreen, Times.Once());
            this.mockWindow.VerifyGet(p => p.WindowState, Times.Once());
        }

        [Fact]
        public void TypeOfBorder_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.TypeOfBorder = BorderType.Resizable;
            _ = window.TypeOfBorder;

            // Assert
            this.mockWindow.VerifySet(p => p.TypeOfBorder = BorderType.Resizable, Times.Once());
            this.mockWindow.VerifyGet(p => p.TypeOfBorder, Times.Once());
        }

        [Fact]
        public void ContentLoader_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var mockContentLoader = new Mock<IContentLoader>();
            var window = CreateWindow();

            // Act
            window.ContentLoader = mockContentLoader.Object;
            _ = window.ContentLoader;

            // Assert
            this.mockWindow.VerifySet(p => p.ContentLoader = mockContentLoader.Object, Times.Once());
            this.mockWindow.VerifyGet(p => p.ContentLoader, Times.Once());
        }
        #endregion

        #region Method tests
        [Fact]
        public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullWindow_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var window = new WindowFake(null);
            }, "Window must not be null. (Parameter 'window')");
        }

        [Fact]
        public void Ctor_WhenInvoked_ByDefaultContentLoaderIsNull()
        {
            // Act
            var window = CreateWindow();

            // Assert
            Assert.Null(window.ContentLoader);
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
        private WindowFake CreateWindow() => new WindowFake(this.mockWindow.Object);
    }
}
