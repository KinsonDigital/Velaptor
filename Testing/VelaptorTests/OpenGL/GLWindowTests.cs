// <copyright file="GLWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using System.Runtime.InteropServices;
    using Moq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Hardware;
    using Velaptor.NativeInterop.GLFW;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;
    using SysVector2 = System.Numerics.Vector2;
    using VelaptorKeyboardState = Velaptor.Input.KeyboardState;
    using VelaptorMouseButton = Velaptor.Input.MouseButton;
    using VelaptorMouseState = Velaptor.Input.MouseState;

    /// <summary>
    /// Tests the <see cref="GLWindow"/> class.
    /// </summary>
    public class GLWindowTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLFWInvoker> mockGLFW;
        private readonly Mock<ISystemMonitorService> mockMonitorService;
        private readonly Mock<IGameWindowFacade> mockWindowFacade;
        private readonly Mock<IPlatform> mockPlatform;
        private readonly Mock<IContentLoader> mockContentLoader;
        private readonly Mock<ITaskService> mockTaskService;
        private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
        private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindowTests"/> class.
        /// </summary>
        public GLWindowTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLFW = new Mock<IGLFWInvoker>();
            this.mockMonitorService = new Mock<ISystemMonitorService>();
            this.mockWindowFacade = new Mock<IGameWindowFacade>();
            this.mockPlatform = new Mock<IPlatform>();
            this.mockContentLoader = new Mock<IContentLoader>();
            this.mockTaskService = new Mock<ITaskService>();
            this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
        }

        #region Contructor Tests
        [Fact]
        public void Ctor_WithNullGLInvokerParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    null,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'glInvoker')");
        }

        [Fact]
        public void Ctor_WithNullSystemMonitorServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    null,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'systemMonitorService')");
        }

        [Fact]
        public void Ctor_WithNullWindowFacadeParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    null,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'windowFacade')");
        }

        [Fact]
        public void Ctor_WithNullPlatformParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    null,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'platform')");
        }

        [Fact]
        public void Ctor_WithNullTaskServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    null,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'taskService')");
        }

        [Fact]
        public void Ctor_WithNullContentLoaderParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    null,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'contentLoader')");
        }

        [Fact]
        public void Ctor_WithNullGLInitReactorParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    null,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'glInitReactable')");
        }

        [Fact]
        public void Ctor_WithNullShutDownReactorParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLWindow(
                    It.IsAny<uint>(),
                    It.IsAny<uint>(),
                    this.mockGL.Object,
                    this.mockGLFW.Object,
                    this.mockMonitorService.Object,
                    this.mockWindowFacade.Object,
                    this.mockPlatform.Object,
                    this.mockTaskService.Object,
                    this.mockContentLoader.Object,
                    this.mockGLInitReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
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
            Assert.Equal(100u, actual);
        }

        [Fact]
        public void Width_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Size);

            var window = CreateWindow();

            window.CachedUIntProps[nameof(window.Width)].IsCaching = false;

            // Act
            window.Width = 111;
            var actual = window.Width;

            // Assert
            Assert.Equal(111u, actual);
        }

        [Fact]
        public void Height_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow(100, 200);

            // Act
            var actual = window.Height;

            // Assert
            Assert.Equal(200u, actual);
        }

        [Fact]
        public void Height_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Size);

            var window = CreateWindow();

            window.CachedUIntProps[nameof(window.Height)].IsCaching = false;

            // Act
            window.Height = 111;
            var actual = window.Height;

            // Assert
            Assert.Equal(111u, actual);
        }

        [Fact]
        public void Title_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.Title;

            // Assert
            Assert.Equal("Velaptor Application", actual);
        }

        [Fact]
        public void Title_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.Title);

            var window = CreateWindow();

            window.CachedStringProps[nameof(window.Title)].IsCaching = false;

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
                .Returns(() => new SystemMonitor(this.mockPlatform.Object)
                {
                    HorizontalScale = 1f,
                    VerticalScale = 1f,
                    Width = 2000,
                    Height = 1000,
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
                .Returns(() => new SystemMonitor(this.mockPlatform.Object)
                {
                    HorizontalScale = 1f,
                    VerticalScale = 1f,
                    Width = 2000,
                    Height = 1000,
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

            window.CachedPosition.IsCaching = false;

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

            window.CachedIntProps[nameof(window.UpdateFrequency)].IsCaching = false;

            // Act
            window.UpdateFrequency = 30;
            var actual = window.UpdateFrequency;

            // Assert
            Assert.Equal(30, actual);
        }

        [Fact]
        public void ContentLoader_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var mockOtherContentLoader = new Mock<IContentLoader>();
            var window = CreateWindow();

            // Act
            window.ContentLoader = mockOtherContentLoader.Object;
            var actual = window.ContentLoader;

            // Assert
            Assert.Same(mockOtherContentLoader.Object, actual);
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

            window.CachedBoolProps[nameof(window.MouseCursorVisible)].IsCaching = false;

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

            window.CachedWindowState.IsCaching = false;

            // Act
            window.WindowState = StateOfWindow.FullScreen;
            var actual = window.WindowState;

            // Assert
            Assert.Equal(StateOfWindow.FullScreen, actual);
        }

        [Fact]
        public void Initialize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();
            var initAction = new Action(() => { });

            // Act
            window.Initialize = initAction;

            // Assert
            Assert.Equal(initAction, window.Initialize);
        }

        [Fact]
        public void Uninitialize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();
            var unInitAction = new Action(() => { });

            // Act
            window.Uninitialize = unInitAction;

            // Assert
            Assert.Equal(unInitAction, window.Uninitialize);
        }

        [Fact]
        public void Initialized_WhenWindowIsInitialized_ReturnsTrue()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            this.mockWindowFacade.Raise(e => e.Load += null, EventArgs.Empty);

            // Assert
            Assert.True(window.Initialized);
        }

        [Fact]
        public void TypeOfBorder_WhenCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            var actual = window.TypeOfBorder;

            // Assert
            Assert.Equal(Velaptor.WindowBorder.Resizable, actual);
        }

        [Fact]
        public void TypeOfBorder_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
        {
            // Arrange
            this.mockWindowFacade.SetupProperty(p => p.WindowBorder);

            var window = CreateWindow();

            window.CachedTypeOfBorder.IsCaching = false;

            // Act
            window.TypeOfBorder = Velaptor.WindowBorder.Fixed;
            var actual = window.TypeOfBorder;

            // Assert
            Assert.Equal(Velaptor.WindowBorder.Fixed, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Show_WhenInvoked_InitializesWindowFacade()
        {
            // Arrange
            var window = CreateWindow(123, 456);

            // Act
            window.Show();
            this.mockWindowFacade.Raise(w => w.Load += null, EventArgs.Empty);

            // Assert
            this.mockWindowFacade.Verify(m => m.Init(123, 456), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_RunsWindowFacade()
        {
            // Arrange
            var window = CreateWindow(123, 456);

            // Act
            window.Show();

            // Assert
            this.mockWindowFacade.Verify(m => m.Show(), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpOpenGLErrorCallback()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            this.mockWindowFacade.Raise(i => i.Load += null, EventArgs.Empty);

            // Act
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.mockGL.Raise(i => i.GLError += null, new GLErrorEventArgs("gl-error"));
            }, "gl-error");

            // Assert
            this.mockGL.VerifyAdd(i => i.GLError += It.IsAny<EventHandler<GLErrorEventArgs>>(), Times.Once());
        }

        [Fact]
        public void Show_WhenUpdatingFrameWhileShuttingDown_DoesNotInvokeUpdateEvent()
        {
            // Arrange
            var updateInvoked = false;
            var window = CreateWindow();

            void TestHandler(FrameTime e) => updateInvoked = true;

            window.Update += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Unload += null, EventArgs.Empty);
            this.mockWindowFacade.Raise(m => m.UpdateFrame += null, new FrameTimeEventArgs(123));
            window.Update -= TestHandler;

            // Assert
            Assert.False(updateInvoked);
        }

        [Fact]
        public void Show_WhenRenderingFrameWhileShuttingDown_DoesNotInvokeRenderEvent()
        {
            // Arrange
            var renderInvoked = false;
            var window = CreateWindow();

            void TestHandler(FrameTime e) => renderInvoked = true;

            window.Draw += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Unload += null, EventArgs.Empty);
            this.mockWindowFacade.Raise(m => m.RenderFrame += null, new FrameTimeEventArgs(234));
            window.Draw -= TestHandler;

            // Assert
            Assert.False(renderInvoked);
        }

        [Fact]
        public async void ShowAsync_WhenInvoked_SubscribesToWindowEvents()
        {
            // Arrange
            this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });

            var window = CreateWindow();

            // Act
            await window.ShowAsync();

            // Assert
            this.mockWindowFacade.VerifyAdd(s => s.Load += It.IsAny<EventHandler<EventArgs>>(), Times.Once(), $"Subscription of the '{nameof(IGameWindowFacade.Load)}' event did not occur.");
            this.mockWindowFacade.VerifyAdd(s => s.Unload += It.IsAny<EventHandler<EventArgs>>(), Times.Once(), $"Subscription of the '{nameof(IGameWindowFacade.Unload)}' event did not occur.");
            this.mockWindowFacade.VerifyAdd(s => s.UpdateFrame += It.IsAny<EventHandler<FrameTimeEventArgs>>(), Times.Once(), $"Subscription of the '{nameof(IGameWindowFacade.UpdateFrame)}' event did not occur.");
            this.mockWindowFacade.VerifyAdd(s => s.RenderFrame += It.IsAny<EventHandler<FrameTimeEventArgs>>(), Times.Once(), $"Subscription of the '{nameof(IGameWindowFacade.RenderFrame)}' event did not occur.");
            this.mockWindowFacade.VerifyAdd(s => s.Resize += It.IsAny<EventHandler<WindowSizeEventArgs>>(), Times.Once(), $"Subscription of the '{nameof(IGameWindowFacade.Resize)}' event did not occur.");
        }

        [Fact]
        public async void ShowAsync_WhenInvoked_InitWindowFacade()
        {
            // Arrange
            this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });

            var window = CreateWindow();

            // Act
            await window.ShowAsync();
            this.mockWindowFacade.Raise(w => w.Load += null, EventArgs.Empty);

            // Assert
            this.mockWindowFacade.Verify(m => m.Init(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once());
        }

        [Fact]
        public async void ShowAsync_WhenInvoked_ExecutesWindowShow()
        {
            // Arrange
            this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });

            var window = CreateWindow();

            // Act
            await window.ShowAsync();

            // Assert
            this.mockWindowFacade.Verify(m => m.Show(), Times.Once());
        }

        [Fact]
        public async void ShowAsync_WhenInvoked_StartsInternalShowTask()
        {
            // Arrange
            this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });

            var window = CreateWindow();

            // Act
            await window.ShowAsync();

            // Assert
            this.mockTaskService.Verify(m => m.Start(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfWindowFacade()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            window.Dispose();

            // Assert
            this.mockWindowFacade.VerifyRemove(s => s.Load -= It.IsAny<EventHandler<EventArgs>>(), Times.Once(), $"Unsubscription of the '{nameof(IGameWindowFacade.Load)}' event did not occur.");
            this.mockWindowFacade.VerifyRemove(s => s.Unload -= It.IsAny<EventHandler<EventArgs>>(), Times.Once(), $"Unsubscription of the '{nameof(IGameWindowFacade.Unload)}' event did not occur.");
            this.mockWindowFacade.VerifyRemove(s => s.UpdateFrame -= It.IsAny<EventHandler<FrameTimeEventArgs>>(), Times.Once(), $"Unsubscription of the '{nameof(IGameWindowFacade.UpdateFrame)}' event did not occur.");
            this.mockWindowFacade.VerifyRemove(s => s.RenderFrame -= It.IsAny<EventHandler<FrameTimeEventArgs>>(), Times.Once(), $"Unsubscription of the '{nameof(IGameWindowFacade.RenderFrame)}' event did not occur.");
            this.mockWindowFacade.VerifyRemove(s => s.Resize -= It.IsAny<EventHandler<WindowSizeEventArgs>>(), Times.Once(), $"Unsubscription of the '{nameof(IGameWindowFacade.Resize)}' event did not occur.");
            this.mockGL.VerifyRemove(e => e.GLError -= It.IsAny<EventHandler<GLErrorEventArgs>>(), Times.Once, $"Unsubscription of the '{nameof(IGLInvoker.GLError)}' event did not occur.");
            this.mockWindowFacade.Verify(m => m.Dispose(), Times.Once());
            this.mockTaskService.Verify(m => m.Dispose(), Times.Once());
            this.mockGL.Verify(m => m.Dispose(), Times.Once());
            this.mockGLFW.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void GLWindow_WhenWindowResizes_SetsGLViewportAndTriggersResizeEvent()
        {
            // Arrange
            var window = CreateWindow();
            var actualSize = default(SizeU);
            window.WinResize = u => actualSize = u;
            window.Show();

            // Act
            this.mockWindowFacade.Raise(w => w.Resize += null, null, new WindowSizeEventArgs(30, 40));

            // Assert
            this.mockGL.Verify(m => m.Viewport(0, 0, 30, 40));
            Assert.Equal(30u, actualSize.Width);
            Assert.Equal(40u, actualSize.Height);
        }

        [Fact]
        public void GLWindow_WhenRenderingFrameWithAutoClearEnabled_ClearsGLBuffer()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            this.mockWindowFacade.Raise(e => e.RenderFrame += null, null, new FrameTimeEventArgs(16));

            // Assert
            this.mockGL.Verify(m => m.Clear(GLClearBufferMask.ColorBufferBit), Times.Once);
        }

        [Fact]
        public void GLWindow_WhenRenderingFrameWithAutoClearDisabled_ClearsGLBuffer()
        {
            // Arrange
            var window = CreateWindow();
            window.AutoClearBuffer = false;
            window.Show();

            // Act
            this.mockWindowFacade.Raise(e => e.RenderFrame += null, null, new FrameTimeEventArgs(16));

            // Assert
            this.mockGL.Verify(m => m.Clear(It.IsAny<GLClearBufferMask>()), Times.Never);
        }

        [Fact]
        public void GLWindow_WhenRenderingFrame_InvokesDrawAndSwapsBuffer()
        {
            // Arrange
            var drawInvoked = false;
            var window = CreateWindow();
            window.Draw = time =>
            {
                drawInvoked = true;
                Assert.Equal(time.ElapsedTime, new TimeSpan(0, 0, 0, 16));
            };
            window.AutoClearBuffer = false;
            window.Show();

            // Act
            this.mockWindowFacade.Raise(e => e.RenderFrame += null, null, new FrameTimeEventArgs(16));

            // Assert
            this.mockWindowFacade.Verify(m => m.SwapBuffers(), Times.Once);
            Assert.True(drawInvoked);
        }

        [Fact]
        public void GLWindow_WhenUnloadingWindow_PushesShutDownNotification()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            this.mockWindowFacade.Raise(m => m.Unload += null, EventArgs.Empty);

            // Assert
            this.mockShutDownReactable.Verify(m => m.PushNotification(default, true), Times.Once);
            this.mockShutDownReactable.Verify(m => m.Dispose(), Times.Once);
        }

        [Fact]
        public void Close_WhenInvoked_ClosesInternalWindowFacade()
        {
            // Arrange
            var window = CreateWindow();

            // Act
            window.Close();

            // Assert
            this.mockWindowFacade.Verify(m => m.Close(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="GLWindow"/> for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>The instance to test.</returns>
        private GLWindow CreateWindow(uint width = 10, uint height = 20)
            => new (
                width,
                height,
                this.mockGL.Object,
                this.mockGLFW.Object,
                this.mockMonitorService.Object,
                this.mockWindowFacade.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockGLInitReactable.Object,
                this.mockShutDownReactable.Object);
    }
}
