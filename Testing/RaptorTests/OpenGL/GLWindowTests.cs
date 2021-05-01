// <copyright file="GLWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0001 // Name can be simplified
#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.OpenGL
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using Raptor;
    using Raptor.Content;
    using Raptor.Hardware;
    using Raptor.Input;
    using Raptor.NativeInterop;
    using Raptor.Observables;
    using Raptor.OpenGL;
    using Raptor.Services;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
    using RaptorKeyboardState = Raptor.Input.KeyboardState;
    using RaptorMouseButton = Raptor.Input.MouseButton;
    using RaptorMouseState = Raptor.Input.MouseState;
    using SysVector2 = System.Numerics.Vector2;
    using TKMouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

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
        private readonly Mock<ITaskService> mockTaskService;
        private readonly Mock<IKeyboardInput<KeyCode, RaptorKeyboardState>> mockKeyboard;
        private readonly Mock<IMouseInput<RaptorMouseButton, RaptorMouseState>> mockMouse;
        private readonly Mock<OpenGLObservable> mockGLObservable;

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
            this.mockTaskService = new Mock<ITaskService>();
            this.mockKeyboard = new Mock<IKeyboardInput<KeyCode, RaptorKeyboardState>>();
            this.mockMouse = new Mock<IMouseInput<RaptorMouseButton, RaptorMouseState>>();
            this.mockGLObservable = new Mock<OpenGLObservable>();
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
            }, "The parameter must not be null. (Parameter 'platform')");
        }

        [Fact]
        public void Ctor_WithNullTaskService_ThrowsException()
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
                    null,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
            }, "The parameter must not be null. (Parameter 'taskService')");
        }

        [Fact]
        public void Ctor_WithNullKeyboard_ThrowsException()
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
                    this.mockTaskService.Object,
                    null,
                    this.mockMouse.Object,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
            }, "The parameter must not be null. (Parameter 'keyboard')");
        }

        [Fact]
        public void Ctor_WithNullMouse_ThrowsException()
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    null,
                    this.mockContentLoader.Object,
                    this.mockGLObservable.Object);
            }, "The parameter must not be null. (Parameter 'mouse')");
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
                    this.mockTaskService.Object,
                    this.mockKeyboard.Object,
                    this.mockMouse.Object,
                    null,
                    this.mockGLObservable.Object);
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

            window.CachedIntProps[nameof(window.Width)].IsCaching = false;

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

            window.CachedIntProps[nameof(window.Height)].IsCaching = false;

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

            window.CachedTypeOfBorder.IsCaching = false;

            // Act
            window.TypeOfBorder = BorderType.Fixed;
            var actual = window.TypeOfBorder;

            // Assert
            Assert.Equal(BorderType.Fixed, actual);
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
        public void Show_WhenInvoked_SetsUpLoadEvent()
        {
            /* NOTE: The way testing is done here does not use the
             * Assert.Raises<T>() assertion because due to OpenTK, we are
             * stuck with events that do not use the generic EventHandler<T>
             * type of events.  Assert.Raises<T>() only works with EventHandler<T>.
             */
            // Arrange
            var loadInvoked = false;
            var window = CreateWindow();

            /* NOTE: this is a local function to act as a handler to the event
             * This also allows the unregistration of the event as well to prevent
             * memory leaks during test execution
             */
            void TestHandler() => loadInvoked = true;

            window.Initialize += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Load += null);
            window.Initialize -= TestHandler; // Prevent memory leak

            // Assert
            Assert.True(loadInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpUnloadEvent()
        {
            // Arrange
            var unloadInvoked = false;
            var window = CreateWindow();

            void TestHandler() => unloadInvoked = true;

            window.Uninitialize += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Unload += null);
            window.Uninitialize -= TestHandler;

            // Assert
            Assert.True(unloadInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpUpdateFrameEvent()
        {
            // Arrange
            var updateInvoked = false;
            var window = CreateWindow();

            void TestHandler(FrameTime e) => updateInvoked = true;

            window.Update += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.UpdateFrame += null, It.IsAny<FrameEventArgs>());
            window.Update -= TestHandler;

            // Assert
            Assert.True(updateInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpRenderFrameEvent()
        {
            // Arrange
            var renderInvoked = false;
            var window = CreateWindow();

            void TestHandler(FrameTime e) => renderInvoked = true;

            window.Draw += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.RenderFrame += null, It.IsAny<FrameEventArgs>());
            window.Draw -= TestHandler;

            // Assert
            Assert.True(renderInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpResizeEvent()
        {
            // Arrange
            var resizeInvoked = false;
            var window = CreateWindow();

            void TestHandler() => resizeInvoked = true;

            window.WinResize += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Resize += null, It.IsAny<ResizeEventArgs>());
            window.WinResize -= TestHandler;

            // Assert
            Assert.True(resizeInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpKeyDownEvent()
        {
            // Arrange
            var window = CreateWindow();
            var keyboardKeyEventArgs = new KeyboardKeyEventArgs(
                Keys.T,
                It.IsAny<int>(),
                It.IsAny<KeyModifiers>(),
                It.IsAny<bool>());

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.KeyDown += null, keyboardKeyEventArgs);

            // Assert
            this.mockKeyboard.Verify(m => m.SetState(KeyCode.T, true), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpKeyUpEvent()
        {
            // Arrange
            var window = CreateWindow();
            var keyboardKeyEventArgs = new KeyboardKeyEventArgs(
                Keys.H,
                It.IsAny<int>(),
                It.IsAny<KeyModifiers>(),
                It.IsAny<bool>());

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.KeyUp += null, keyboardKeyEventArgs);

            // Assert
            this.mockKeyboard.Verify(m => m.SetState(KeyCode.H, false), Times.Once());
        }

        [Theory]
        [InlineData(TKMouseButton.Button4)]
        [InlineData(TKMouseButton.Button5)]
        [InlineData(TKMouseButton.Button6)]
        [InlineData(TKMouseButton.Button7)]
        [InlineData(TKMouseButton.Button8)]
        public void Show_WithInvalidGLMouseButton_ThrowsException(
            TKMouseButton from)
        {
            // Arrange
            var window = CreateWindow();
            var mouseButtonEventArgs = new MouseButtonEventArgs(
                from,
                It.IsAny<InputAction>(),
                It.IsAny<KeyModifiers>());

            // Act & Assert
            window.Show();

            Assert.ThrowsWithMessage<Exception>(() =>
            {
                this.mockWindowFacade.Raise(m => m.MouseDown += null, mouseButtonEventArgs);
            }, "Unrecognized OpenGL mouse button.");
        }

        [Theory]
        [InlineData(TKMouseButton.Left, RaptorMouseButton.LeftButton)]
        [InlineData(TKMouseButton.Right, RaptorMouseButton.RightButton)]
        [InlineData(TKMouseButton.Middle, RaptorMouseButton.MiddleButton)]
        public void Show_WhenInvoked_ProperlyMapsMouseButtons(
            TKMouseButton from, RaptorMouseButton to)
        {
            // Arrange
            var window = CreateWindow();
            var mouseButtonEventArgs = new MouseButtonEventArgs(
                from,
                It.IsAny<InputAction>(),
                It.IsAny<KeyModifiers>());

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.MouseDown += null, mouseButtonEventArgs);

            // Assert
            this.mockMouse.Verify(m => m.SetState(to, true), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpMouseDownEvent()
        {
            // Arrange
            var window = CreateWindow();
            var mouseButtonEventArgs = new MouseButtonEventArgs(
                TKMouseButton.Right,
                It.IsAny<InputAction>(),
                It.IsAny<KeyModifiers>());

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.MouseDown += null, mouseButtonEventArgs);

            // Assert
            this.mockMouse.Verify(m => m.SetState(RaptorMouseButton.RightButton, true), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpMouseUpEvent()
        {
            // Arrange
            var window = CreateWindow();

            var mouseButtonEventArgs = new MouseButtonEventArgs(
                TKMouseButton.Left,
                It.IsAny<InputAction>(),
                It.IsAny<KeyModifiers>());

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.MouseUp += null, mouseButtonEventArgs);

            // Assert
            this.mockMouse.Verify(m => m.SetState(RaptorMouseButton.LeftButton, false), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpMouseMoveEvent()
        {
            // Arrange
            var window = CreateWindow();
            var mouseMoveEventArgs = new MouseMoveEventArgs(11, 22, 33, 44);

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.MouseMove += null, mouseMoveEventArgs);

            // Assert
            this.mockMouse.Verify(m => m.SetXPos(11), Times.Once());
            this.mockMouse.Verify(m => m.SetYPos(22), Times.Once());
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpCloseEvent()
        {
            // Arrange
            var unloadInvoked = false;
            var window = CreateWindow();

            void TestHandler() => unloadInvoked = true;

            window.Uninitialize += TestHandler;

            // Act
            window.Show();
            this.mockWindowFacade.Raise(m => m.Closed += null);
            window.Uninitialize -= TestHandler;

            // Assert
            Assert.True(unloadInvoked);
        }

        [Fact]
        public void Show_WhenInvoked_SetsUpOpenGLErrorCallback()
        {
            // Arrange
            const string testMessage = "test-message";
            const string testUserParam = "test-param";

            var testMessagePtr = Marshal.StringToHGlobalAnsi(testMessage);
            var testUserParamPtr = Marshal.StringToHGlobalAnsi(testUserParam);

            DebugProc? testCallback = null;
            var window = CreateWindow();
            this.mockGLInvoker.Setup(m => m.DebugMessageCallback(It.IsAny<DebugProc>(), It.IsAny<IntPtr>()))
                .Callback<DebugProc, IntPtr>((callback, userParam) =>
                {
                    testCallback = callback;
                });

            // Act
            window.Show();

            Assert.ThrowsWithMessage<Exception>(() =>
            {
                testCallback(
                    DebugSource.DebugSourceOther,
                    DebugType.DebugTypePerformance,
                    1234,
                    DebugSeverity.DebugSeverityHigh,
                    456,
                    testMessagePtr,
                    testUserParamPtr);
            }, "test-message\n\tSrc: DebugSourceOther\n\tType: DebugTypePerformance\n\tID: 1234\n\tSeverity: DebugSeverityHigh\n\tLength: 456\n\tUser Param: test-param");

            // Assert
            this.mockGLInvoker.Verify(m => m.DebugMessageCallback(It.IsAny<DebugProc>(), It.IsAny<IntPtr>()), Times.Once());
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
            this.mockWindowFacade.Raise(m => m.Unload += null);
            this.mockWindowFacade.Raise(m => m.UpdateFrame += null, It.IsAny<FrameEventArgs>());
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
            this.mockWindowFacade.Raise(m => m.Unload += null);
            this.mockWindowFacade.Raise(m => m.RenderFrame += null, It.IsAny<FrameEventArgs>());
            window.Draw -= TestHandler;

            // Assert
            Assert.False(renderInvoked);
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
            await window.ShowAsync(It.IsAny<Action>());

            // Assert
            this.mockWindowFacade.Verify(m => m.Init(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
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
            await window.ShowAsync(It.IsAny<Action>());

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
            await window.ShowAsync(It.IsAny<Action>());

            // Assert
            this.mockTaskService.Verify(m => m.Start(), Times.Once());
        }

        [Fact]
        public async void ShowAsync_WhenTaskIsFinished_InvokesDisposeParam()
        {
            // Arrange
            var disposeInvoked = false;
            this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });

            this.mockTaskService.Setup(m => m.ContinueWith(
                It.IsAny<Action<Task>>(),
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default))
                    .Callback<Action<Task>, TaskContinuationOptions, TaskScheduler>(
                        (action, options, schedular) =>
                        {
                            action(Task.CompletedTask);
                        });

            var window = CreateWindow();

            // Act
            await window.ShowAsync(() => disposeInvoked = true);

            // Assert
            Assert.True(disposeInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersLoadEvent()
        {
            // Arrange
            var loadInvoked = false;

            var window = CreateWindow();

            void LoadHandler() => loadInvoked = true;

            window.Initialize += LoadHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.Load += null);

            window.Initialize -= LoadHandler;

            // Assert
            Assert.False(loadInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersUnloadEvent()
        {
            // Arrange
            var unloadInvoked = false;

            var window = CreateWindow();

            void UnloadHandler() => unloadInvoked = true;

            window.Uninitialize += UnloadHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.Unload += null);

            window.Uninitialize -= UnloadHandler;

            // Assert
            Assert.False(unloadInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersUpdateFrameEvent()
        {
            // Arrange
            var updateFrameInvoked = false;

            var window = CreateWindow();

            void UpdateHandler(FrameTime e) => updateFrameInvoked = true;

            window.Update += UpdateHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.UpdateFrame += null);

            window.Update -= UpdateHandler;

            // Assert
            Assert.False(updateFrameInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersRenderFrameEvent()
        {
            // Arrange
            var drawFrameInvoked = false;

            var window = CreateWindow();

            void DrawHandler(FrameTime e) => drawFrameInvoked = true;

            window.Draw += DrawHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.RenderFrame += null);

            window.Draw -= DrawHandler;

            // Assert
            Assert.False(drawFrameInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersResizeEvent()
        {
            // Arrange
            var winResizeFrameInvoked = false;

            var window = CreateWindow();

            void WinResizeHandler() => winResizeFrameInvoked = true;

            window.WinResize += WinResizeHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.Resize += null);

            window.WinResize -= WinResizeHandler;

            // Assert
            Assert.False(winResizeFrameInvoked);
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersKeyDownEvent()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.KeyDown += null, It.IsAny<KeyboardKeyEventArgs>());

            // Assert
            this.mockKeyboard.Verify(m => m.SetState(It.IsAny<KeyCode>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersKeyUpEvent()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.KeyUp += null, It.IsAny<KeyboardKeyEventArgs>());

            // Assert
            this.mockKeyboard.Verify(m => m.SetState(It.IsAny<KeyCode>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersMouseDownEvent()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.MouseDown += null, It.IsAny<MouseButtonEventArgs>());

            // Assert
            this.mockMouse.Verify(m => m.SetState(It.IsAny<RaptorMouseButton>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersMouseUpEvent()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.MouseUp += null, It.IsAny<MouseButtonEventArgs>());

            // Assert
            this.mockMouse.Verify(m => m.SetState(It.IsAny<RaptorMouseButton>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersMouseMoveEvent()
        {
            // Arrange
            var window = CreateWindow();
            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.MouseMove += null, It.IsAny<MouseMoveEventArgs>());

            // Assert
            this.mockMouse.Verify(m => m.SetXPos(It.IsAny<int>()), Times.Never());
            this.mockMouse.Verify(m => m.SetYPos(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenInvoked_UnregistersCloseEvent()
        {
            // Arrange
            var uninitializeInvoked = false;

            var window = CreateWindow();

            void UninitializeHandler() => uninitializeInvoked = true;

            window.Uninitialize += UninitializeHandler;

            window.Show();

            // Act
            window.Dispose();
            this.mockWindowFacade.Raise(m => m.Closed += null);

            window.Uninitialize -= UninitializeHandler;

            // Assert
            Assert.False(uninitializeInvoked);
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
            this.mockTaskService.Verify(m => m.Dispose(), Times.Once());
            this.mockWindowFacade.Verify(m => m.Dispose(), Times.Once());
            this.mockGLObservable.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="GLWindow"/> for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>The instance to test.</returns>
        private GLWindow CreateWindow(int width = 10, int height = 20)
            => new (
                width,
                height,
                this.mockGLInvoker.Object,
                this.mockMonitorService.Object,
                this.mockWindowFacade.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockKeyboard.Object,
                this.mockMouse.Object,
                this.mockContentLoader.Object,
                this.mockGLObservable.Object);
    }
}
