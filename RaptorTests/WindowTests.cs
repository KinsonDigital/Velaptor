using Moq;
using Raptor;
using Raptor.Content;
using RaptorTests.Fakes;
using RaptorTests.Helpers;
using System;
using Xunit;

namespace RaptorTests
{
    public class WindowTests
    {
        private readonly Mock<IWindow> mockWindow;
        private readonly Mock<IContentLoader> mockContentLoader;

        public WindowTests()
        {
            this.mockWindow = new Mock<IWindow>();
            this.mockContentLoader = new Mock<IContentLoader>();
        }

        [Fact]
        public void Title_WhenSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            this.mockWindow.SetupProperty(p => p.Title);

            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object)
            {
                Title = "test-title"
            };

            //Act
            var actual = window.Title;

            //Assert
            Assert.Equal("test-title", actual);
        }

        [Fact]
        public void Width_WhenSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            this.mockWindow.SetupProperty(p => p.Width);

            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object)
            {
                Width = 1234
            };

            //Act
            var actual = window.Width;

            //Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void Height_WhenSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            this.mockWindow.SetupProperty(p => p.Height);

            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object)
            {
                Height = 1234
            };

            //Act
            var actual = window.Height;

            //Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void UpdateFrequency_WhenSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            this.mockWindow.SetupProperty(p => p.UpdateFreq);

            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object)
            {
                UpdateFrequency = 1234
            };

            //Act
            var actual = window.UpdateFrequency;

            //Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullWindow_ThrowsException()
        {
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                var window = new WindowFake(null, this.mockContentLoader.Object);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }, "Window must not be null. (Parameter 'window')");
        }

        [Fact]
        public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullLoader_ThrowsException()
        {
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                var window = new WindowFake(this.mockWindow.Object, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }, "Content loader must not be null. (Parameter 'contentLoader')");
        }

        [Fact]
        public void Ctor_WhenUsingOverloadWithLoaderAndWidthAndHeightWithNullLoader_ThrowsException()
        {
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                var window = new WindowFake(window: null, contentLoader: null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }, "Window must not be null. (Parameter 'window')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithWindowAndContentLoader_SetsWindowAndContentLoader()
        {
            //Act
            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object);

            //Assert
            Assert.Equal(this.mockContentLoader.Object, window.ContentLoader);
        }

        [Fact]
        public void Show_WhenInvoked_ShowsWindow()
        {
            //Arrange
            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object);

            //Act
            window.Show();

            //Assert
            this.mockWindow.Verify(m => m.Show(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            //Arrange
            var window = new WindowFake(this.mockWindow.Object, this.mockContentLoader.Object);

            //Act
            window.Dispose();
            window.Dispose();

            //Assert
            this.mockWindow.Verify(m => m.Dispose(), Times.Once());
        }
    }
}
