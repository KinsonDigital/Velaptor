// <copyright file="GLWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0001 // Name can be simplified
#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.OpenGL
{
    using System;
    using System.Runtime.InteropServices;
    using Moq;
    using Raptor;
    using Raptor.Content;
    using Raptor.Hardware;
    using Raptor.NativeInterop;
    using Raptor.OpenGL;
    using Raptor.Services;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
    using SysVector2 = System.Numerics.Vector2;

    /// <summary>
    /// Tests the <see cref="GLWindow"/> class.
    /// </summary>
    public class GLWindowTests
    {
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly Mock<ISystemMonitorService> mockMonitorService;
        private readonly Mock<IGameWindowFacade> mockWindowFacade;
        private readonly Mock<IPlatform> mockPlatform;
        private readonly Mock<IContentLoader> mockContentLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindowTests"/> class.
        /// </summary>
        public GLWindowTests()
        {
            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockMonitorService = new Mock<ISystemMonitorService>();
            this.mockWindowFacade = new Mock<IGameWindowFacade>();
            this.mockPlatform = new Mock<IPlatform>();
            this.mockContentLoader = new Mock<IContentLoader>();
        }

        #region Contructor Tests
        [Fact]
        public void Ctor_WithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    null,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockContentLoader.Object);
            }, "The parameter must not be null. (Parameter 'glInvoker')");
        }

        [Fact]
        public void Ctor_WithNullSystemMonitorService_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    this.mockGLInvoker.Object,
                    null,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockContentLoader.Object);
            }, "The parameter must not be null. (Parameter 'systemMonitorService')");
        }

        [Fact]
        public void Ctor_WithNullWindowFacade_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    this.mockGLInvoker.Object,
                    this.mockMonitorService.Object,
                    null,
                    this.mockPlatform.Object,
                    this.mockContentLoader.Object);
            }, "The parameter must not be null. (Parameter 'windowFacade')");
        }

        [Fact]
        public void Ctor_WithNullPlatform_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    this.mockGLInvoker.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    null,
                    this.mockContentLoader.Object);
            }, "The parameter must not be null. (Parameter 'platform')");
        }

        [Fact]
        public void Ctor_WithNullContentLoader_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    this.mockGLInvoker.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'contentLoader')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Width_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow(100, 200);

            // Act
            var actual = window.Width;

            // Assert
            Assert.Equal(100, actual);
        }

        [Fact]
        public void Width_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Size);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.Width = 111;
            var actual = window.Width;

            // Assert
            Assert.Equal(111, actual);
        }

        [Fact]
        public void Height_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow(100, 200);

            // Act
            var actual = window.Height;

            // Assert
            Assert.Equal(200, actual);
        }

        [Fact]
        public void Height_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Size);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.Height = 111;
            var actual = window.Height;

            // Assert
            Assert.Equal(111, actual);
        }

        [Fact]
        public void Title_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.Title;

            // Assert
            Assert.Equal("Raptor Application", actual);
        }

        [Fact]
        public void Title_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Title);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.Title = "test-title";
            var actual = window.Title;

            // Assert
            Assert.Equal("test-title", actual);
        }

        [Fact]
        public void Position_WhenCachingValueOnOSXPlatform_ReturnsCorrectResult()
        {
            // Arrange
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.OSX);

            this.mockMonitorService.SetupGet(p => p.MainMonitor)
                .Returns(() =>
                {
                    return new SystemMonitor(this.mockPlatform.Object)
                    {
                        HorizontalScale = 1f,
                        VerticalScale = 1f,
                        Width = 2000,
                        Height = 1000,
                    };
                });

            var window = CreateWindow(100, 200);

            // Act
            var actual = window.Position;

            // Assert
            Assert.Equal(new SysVector2(950, 400), actual);
        }

        [Fact]
        public void Position_WhenCachingValueOnWindowsPlatform_ReturnsCorrectResult()
        {
            // Arrange
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

            this.mockMonitorService.SetupGet(p => p.MainMonitor)
                .Returns(() =>
                {
                    return new SystemMonitor(this.mockPlatform.Object)
                    {
                        HorizontalScale = 1f,
                        VerticalScale = 1f,
                        Width = 2000,
                        Height = 1000,
                    };
                });

            var window = CreateWindow(100, 200);

            // Act
            var actual = window.Position;

            // Assert
            Assert.Equal(new SysVector2(950, 400), actual);
        }

        [Fact]
        public void Position_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Location);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.Position = new SysVector2(123, 456);
            var actual = window.Position;

            // Assert
            Assert.Equal(new SysVector2(123, 456), actual);
        }

        [Fact]
        public void UpdateFrequency_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.UpdateFrequency;

            // Assert
            Assert.Equal(60, actual);
        }

        [Fact]
        public void UpdateFrequency_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.UpdateFrequency);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.UpdateFrequency = 30;
            var actual = window.UpdateFrequency;

            // Assert
            Assert.Equal(30, actual);
        }

        [Fact]
        public void MouseCursorVisible_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.MouseCursorVisible;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void MouseCursorVisible_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.CursorVisible);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.MouseCursorVisible = false;
            var actual = window.MouseCursorVisible;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void WindowState_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.WindowState;

            // Assert
            Assert.Equal(StateOfWindow.Normal, actual);
        }

        [Fact]
        public void WindowState_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.WindowState);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.WindowState = StateOfWindow.FullScreen;
            var actual = window.WindowState;

            // Assert
            Assert.Equal(StateOfWindow.FullScreen, actual);
        }

        [Fact]
        public void TypeOfBorder_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.TypeOfBorder;

            // Assert
            Assert.Equal(BorderType.Resizable, actual);
        }

        [Fact]
        public void TypeOfBorder_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.WindowBorder);

            var window = CreateWindow();

            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            window.TypeOfBorder = BorderType.Fixed;
            var actual = window.TypeOfBorder;

            // Assert
            Assert.Equal(BorderType.Fixed, actual);
        }

        [Fact]
        public void Initialized_WhenInvoked_RegistersToOpenGLInitEvent()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            IGLInvoker.SetOpenGLAsInitialized();

            // Assert
            Assert.True(window.Initialized);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="GLWindow"/> for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>The instance to test.</returns>
        private GLWindow CreateWindow(int width = 10, int height = 20)
            => new GLWindow(
                width,
                height,
                this.mockGLInvoker.Object,
                this.mockMonitorService.Object,
                this.mockWindowFacade.Object,
                this.mockPlatform.Object,
                this.mockContentLoader.Object);
    }
}
