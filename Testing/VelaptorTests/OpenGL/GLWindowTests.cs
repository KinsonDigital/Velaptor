// <copyright file="GLWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Velaptor;
using Velaptor.Content;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.Hardware;
using Velaptor.Input;
using Velaptor.Input.Exceptions;
using Velaptor.NativeInterop.GLFW;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Velaptor.Services;
using Xunit;
using IWindow = Velaptor.UI.IWindow;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkWindow = Silk.NET.Windowing.IWindow;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using SysVector2 = System.Numerics.Vector2;
using VelaptorKeyboardState = Velaptor.Input.KeyboardState;
using VelaptorMouseButton = Velaptor.Input.MouseButton;
using VelaptorMouseState = Velaptor.Input.MouseState;
using VelaptorWindowBorder = Velaptor.WindowBorder;

/// <summary>
/// Tests the <see cref="GLWindow"/> class.
/// </summary>
public class GLWindowTests
{
    private readonly Mock<IAppService> mockAppService;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IGlfwInvoker> mockGlfw;
    private readonly Mock<IGLContext> mockGLContext;
    private readonly Mock<ISystemDisplayService> mockDisplayService;
    private readonly Mock<IPlatform> mockPlatform;
    private readonly Mock<IContentLoader> mockContentLoader;
    private readonly Mock<ISceneManager> mockSceneManager;
    private readonly Mock<ITaskService> mockTaskService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IPushReactable<MouseStateData>> mockMouseReactable;
    private readonly Mock<IPushReactable<KeyboardKeyStateData>> mockKeyboardReactable;
    private readonly Mock<IPushReactable<WindowSizeData>> mockWinSizeReactable;
    private readonly Mock<IPushReactable<GL>> mockGLReactable;
    private readonly Mock<SilkWindow> mockSilkWindow;
    private readonly Mock<IWindowFactory> mockWindowFactory;
    private readonly Mock<ITimerService> mockTimerService;
    private Mock<INativeInputFactory>? mockNativeInputFactory;
    private Mock<IInputContext>? mockSilkInputContext;
    private Mock<IKeyboard>? mockSilkKeyboard;
    private Mock<IMouse>? mockSilkMouse;
    private Mock<ICursor>? mockMouseCursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLWindowTests"/> class.
    /// </summary>
    public GLWindowTests()
    {
        this.mockAppService = new Mock<IAppService>();
        this.mockGLContext = new Mock<IGLContext>();
        this.mockSilkWindow = new Mock<SilkWindow>();
        this.mockSilkWindow.SetupGet(p => p.GLContext).Returns(this.mockGLContext.Object);

        MockSystemSilkInput();

        this.mockWindowFactory = new Mock<IWindowFactory>();
        this.mockWindowFactory.Setup(m => m.CreateSilkWindow()).Returns(this.mockSilkWindow.Object);

        this.mockGL = new Mock<IGLInvoker>();
        this.mockGlfw = new Mock<IGlfwInvoker>();
        this.mockDisplayService = new Mock<ISystemDisplayService>();
        this.mockPlatform = new Mock<IPlatform>();
        this.mockContentLoader = new Mock<IContentLoader>();
        this.mockSceneManager = new Mock<ISceneManager>();
        this.mockTaskService = new Mock<ITaskService>();

        this.mockPushReactable = new Mock<IPushReactable>();
        this.mockMouseReactable = new Mock<IPushReactable<MouseStateData>>();
        this.mockKeyboardReactable = new Mock<IPushReactable<KeyboardKeyStateData>>();
        this.mockGLReactable = new Mock<IPushReactable<GL>>();

        var mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        this.mockWinSizeReactable = new Mock<IPushReactable<WindowSizeData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(this.mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateMouseReactable()).Returns(this.mockMouseReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateKeyboardReactable()).Returns(this.mockKeyboardReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateGLReactable()).Returns(this.mockGLReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable()).Returns(mockViewPortReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateWindowSizeReactable()).Returns(this.mockWinSizeReactable.Object);

        this.mockTimerService = new Mock<ITimerService>();
    }

    #region Contructor Tests
    [Fact]
    public void Ctor_WithNullAppServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                null,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'appService')");
    }

    [Fact]
    public void Ctor_WithNullWindowFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                null,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'windowFactory')");
    }

    [Fact]
    public void Ctor_WithNullNativeInputFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                null,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'nativeInputFactory')");
    }

    [Fact]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                null,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'glInvoker')");
    }

    [Fact]
    public void Ctor_WithNullGLFWInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                null,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                null,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'systemDisplayService')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                null,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'platform')");
    }

    [Fact]
    public void Ctor_WithNullTaskServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                null,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'taskService')");
    }

    [Fact]
    public void Ctor_WithNullContentLoaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                null,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'contentLoader')");
    }

    [Fact]
    public void Ctor_WithNullSceneManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                null,
                this.mockReactableFactory.Object,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'sceneManager')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                null,
                this.mockTimerService.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullTimerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                this.mockAppService.Object,
                this.mockWindowFactory.Object,
                this.mockNativeInputFactory.Object,
                this.mockGL.Object,
                this.mockGlfw.Object,
                this.mockDisplayService.Object,
                this.mockPlatform.Object,
                this.mockTaskService.Object,
                this.mockContentLoader.Object,
                this.mockSceneManager.Object,
                this.mockReactableFactory.Object,
                null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'timerService')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Width_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(100, 200);

        // Act
        var actual = sut.Width;

        // Assert
        actual.Should().Be(100);
    }

    [Fact]
    public void Width_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        this.mockSilkWindow.SetupProperty(p => p.Size);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedUIntProps[nameof(sut.Width)].IsCaching = false;

        // Act
        sut.Width = 111;
        var actual = sut.Width;

        // Assert
        actual.Should().Be(111u);
    }

    [Fact]
    public void Height_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(100, 200);

        // Act
        var actual = sut.Height;

        // Assert
        actual.Should().Be(200u);
    }

    [Fact]
    public void Height_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        this.mockSilkWindow.SetupProperty(p => p.Size);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedUIntProps[nameof(sut.Height)].IsCaching = false;

        // Act
        sut.Height = 111;
        var actual = sut.Height;

        // Assert
        actual.Should().Be(111u);
    }

    [Fact]
    public void Title_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Title;

        // Assert
        actual.Should().Be("Velaptor Application");
    }

    [Fact]
    public void Title_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        this.mockSilkWindow.SetupProperty(p => p.Title);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedStringProps[nameof(sut.Title)].IsCaching = false;

        // Act
        sut.Title = "test-title";
        var actual = sut.Title;

        // Assert
        actual.Should().Be("test-title");
    }

    [Fact]
    public void Position_WhenCachingValueOnOSXPlatform_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.OSX);

        this.mockDisplayService.SetupGet(p => p.MainDisplay)
            .Returns(() => new SystemDisplay(this.mockPlatform.Object)
            {
                HorizontalScale = 1f,
                VerticalScale = 1f,
                Width = 2000,
                Height = 1000,
            });

        var sut = CreateSystemUnderTest(100, 200);

        // Act
        var actual = sut.Position;

        // Assert
        actual.Should().Be(new SysVector2(950, 400));
    }

    [Fact]
    public void Position_WhenCachingValueOnWindowsPlatform_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

        this.mockDisplayService.SetupGet(p => p.MainDisplay)
            .Returns(() => new SystemDisplay(this.mockPlatform.Object)
            {
                HorizontalScale = 1f,
                VerticalScale = 1f,
                Width = 2000,
                Height = 1000,
            });

        var sut = CreateSystemUnderTest(100, 200);

        // Act
        var actual = sut.Position;

        // Assert
        actual.Should().Be(new SysVector2(950, 400));
    }

    [Fact]
    public void Position_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        this.mockSilkWindow.SetupProperty(p => p.Position);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedPosition.IsCaching = false;

        // Act
        sut.Position = new SysVector2(123, 456);
        var actual = sut.Position;

        // Assert
        actual.Should().Be(new SysVector2(123, 456));
    }

    [Fact]
    public void UpdateFrequency_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.UpdateFrequency;

        // Assert
        actual.Should().Be(60);
    }

    [Fact]
    public void UpdateFrequency_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        this.mockSilkWindow.SetupProperty(p => p.UpdatesPerSecond);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedIntProps[nameof(sut.UpdateFrequency)].IsCaching = false;

        // Act
        sut.UpdateFrequency = 30;
        var actual = sut.UpdateFrequency;

        // Assert
        actual.Should().Be(30);
    }

    [Fact]
    public void ContentLoader_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockOtherContentLoader = new Mock<IContentLoader>();
        var sut = CreateSystemUnderTest();

        // Act
        sut.ContentLoader = mockOtherContentLoader.Object;
        var actual = sut.ContentLoader;

        // Assert
        actual.Should().BeSameAs(mockOtherContentLoader.Object);
    }

    [Fact]
    public void MouseCursorVisible_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.MouseCursorVisible;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void MouseCursorVisible_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedBoolProps[nameof(sut.MouseCursorVisible)].IsCaching = false;

        // Act
        sut.MouseCursorVisible = false;
        var actual = sut.MouseCursorVisible;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void WindowState_WhenGettingInvalidValue_ThrowsException()
    {
        // Arrange
        var expected = $"The value of the enum '{nameof(WindowState)}' used in the class '{nameof(GLWindow)}' and";
        expected += " method 'SetupOtherPropCaches' is invalid and out of range.";

        this.mockSilkWindow.SetupGet(p => p.WindowState).Returns((WindowState)1234);
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => _ = sut.WindowState;

        // Assert
        act.Should().Throw<EnumOutOfRangeException<WindowState>>()
            .WithMessage(expected);
    }

    [Fact]
    public void WindowState_WhenSettingInvalidValue_ThrowsException()
    {
        // Arrange
        var expected = $"The value of the enum '{nameof(StateOfWindow)}' used in the class '{nameof(GLWindow)}' and";
        expected += " method 'SetupOtherPropCaches' is invalid and out of range.";

        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => sut.WindowState = (StateOfWindow)1234;

        // Assert
        act.Should().Throw<EnumOutOfRangeException<StateOfWindow>>()
            .WithMessage(expected);
    }

    [Fact]
    public void WindowState_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.WindowState;

        // Assert
        actual.Should().Be(StateOfWindow.Normal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WindowState_WhenSettingValueAndNotCaching_ReturnsCorrectResult(int sutStateValue)
    {
        // Arrange
        var sutState = (StateOfWindow)sutStateValue;
        this.mockSilkWindow.SetupProperty(p => p.WindowState);
        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedWindowState.IsCaching = false;

        // Act
        sut.WindowState = sutState;
        var actual = sut.WindowState;

        // Assert
        actual.Should().Be(sutState);
    }

    [Fact]
    public void Initialize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var initAction = new Action(() => { });

        // Act
        sut.Initialize = initAction;

        // Assert
        sut.Initialize.Should().BeSameAs(initAction);
    }

    [Fact]
    public void Uninitialize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var unInitAction = new Action(() => { });

        // Act
        sut.Uninitialize = unInitAction;

        // Assert
        sut.Uninitialize.Should().BeSameAs(unInitAction);
    }

    [Fact]
    public void Initialized_WhenWindowIsInitialized_ReturnsTrue()
    {
        // Arrange
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        sut.Initialized.Should().BeTrue();
    }

    [Fact]
    public void TypeOfBorder_WhenGettingInvalidValue_ThrowsException()
    {
        // Arrange
        var expected = $"The value of the enum '{nameof(Silk.NET.Windowing.WindowBorder)}' used in the class '{nameof(GLWindow)}' and";
        expected += " method 'SetupOtherPropCaches' is invalid and out of range.";

        this.mockSilkWindow.SetupGet(p => p.WindowBorder).Returns((SilkWindowBorder)1234);
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => _ = sut.TypeOfBorder;

        // Assert
        act.Should().Throw<EnumOutOfRangeException<Silk.NET.Windowing.WindowBorder>>()
            .WithMessage(expected);
    }

    [Fact]
    public void TypeOfBorder_WhenSettingInvalidValue_ThrowsException()
    {
        // Arrange
        var expected = $"The value of the enum '{nameof(Velaptor.WindowBorder)}' used in the class '{nameof(GLWindow)}' and";
        expected += " method 'SetupOtherPropCaches' is invalid and out of range.";

        this.mockSilkWindow.SetupGet(p => p.WindowBorder).Returns((SilkWindowBorder)1234);
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => sut.TypeOfBorder = (VelaptorWindowBorder)1234;

        // Assert
        act.Should().Throw<EnumOutOfRangeException<Velaptor.WindowBorder>>()
            .WithMessage(expected);
    }

    [Fact]
    public void TypeOfBorder_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.TypeOfBorder;

        // Assert
        actual.Should().Be(VelaptorWindowBorder.Resizable);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TypeOfBorder_WhenSettingValueAndNotCaching_ReturnsCorrectResult(int sutBorderValue)
    {
        // Arrange
        var sutBorder = (VelaptorWindowBorder)sutBorderValue;
        this.mockSilkWindow.SetupProperty(p => p.WindowBorder);

        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedTypeOfBorder.IsCaching = false;

        // Act
        sut.TypeOfBorder = sutBorder;
        var actual = sut.TypeOfBorder;

        // Assert
        actual.Should().Be(sutBorder);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Show_WithNoSystemKeyboards_ThrowsException()
    {
        // Arrange
        this.mockSilkInputContext.Setup(p => p.Keyboards)
            .Returns(Array.Empty<IKeyboard>().AsReadOnly());
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Show();

        // Assert
        act.Should().Throw<NoKeyboardException>()
            .WithMessage("Input Exception: No connected keyboards available.");
    }

    [Fact]
    public void Show_WithNoSystemMice_ThrowsException()
    {
        // Arrange
        this.mockSilkInputContext.Setup(p => p.Mice)
            .Returns(Array.Empty<IMouse>().AsReadOnly());
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Show();

        // Assert
        act.Should().Throw<NoMouseException>()
            .WithMessage("Input Exception: No connected mice available.");
    }

    [Fact]
    public void Show_WhenInvoked_SubscribesToEvents()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        this.mockSilkWindow.VerifyAddOnce(e => e.Load += It.IsAny<Action>());
        this.mockSilkWindow.VerifyAddOnce(e => e.Closing += It.IsAny<Action>());
        this.mockSilkWindow.VerifyAddOnce(e => e.Resize += It.IsAny<Action<Vector2D<int>>>());
        this.mockSilkWindow.VerifyAddOnce(e => e.Update += It.IsAny<Action<double>>());
        this.mockSilkWindow.VerifyAddOnce(e => e.Render += It.IsAny<Action<double>>());
    }

    [Fact]
    public void Show_WhenInvoked_SetsUpOpenGLErrorCallback()
    {
        // Arrange
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => this.mockGL.Raise(i => i.GLError += null, new GLErrorEventArgs("gl-error"));

        act.Should().Throw<Exception>().WithMessage("gl-error");

        // Assert
        this.mockGL.VerifyAdd(i => i.GLError += It.IsAny<EventHandler<GLErrorEventArgs>>(), Times.Once());
    }

    [Fact]
    public async Task ShowAsync_WhenInvoked_StartsInternalShowTask()
    {
        // Arrange
        this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
            .Callback<Action>(action =>
            {
                action();
            });

        var sut = CreateSystemUnderTest();

        // Act
        await sut.ShowAsync();

        // Assert
        this.mockTaskService.Verify(m => m.Start(), Times.Once());
    }

    [Fact]
    public async Task ShowAsync_WhenAfterStartParamIsNotNull_ExecutesAtCorrectTime()
    {
        // Arrange
        var taskServiceSetActionInvoked = false;
        var taskServiceStartInvoked = false;
        this.mockTaskService.Setup(m => m.SetAction(It.IsAny<Action>()))
            .Callback<Action>(_ => taskServiceSetActionInvoked = true);
        this.mockTaskService.Setup(m => m.Start())
            .Callback(() => taskServiceStartInvoked = true);

        var sut = CreateSystemUnderTest();

        // Act
        await sut.ShowAsync(() => { });

        // Assert
        taskServiceSetActionInvoked.Should().BeTrue(
            $"the {nameof(ITaskService)}.{nameof(ITaskService.SetAction)}() method must be executed before the 'afterStart` parameter");
        taskServiceStartInvoked.Should().BeTrue(
            $"the {nameof(ITaskService)}.{nameof(ITaskService.Start)}() method must be executed before the 'afterStart` parameter");
    }

    [Fact]
    public async Task ShowAsync_WhenAfterUnloadParamIsNotNull_ExecutesActionParamAfterWindowUnloads()
    {
        // Arrange
        this.mockSilkWindow.Setup(m => m.Close())
            .Callback(() =>
            {
                this.mockSilkWindow.Raise(e => e.Closing += null);
            });

        var afterUnloadExecuted = false;
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        await sut.ShowAsync(null, () => afterUnloadExecuted = true);
        sut.Close();

        // Assert
        afterUnloadExecuted.Should().BeTrue("the 'afterUnload` parameter must be executed after the sut unloads.");
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfWindow()
    {
        // Arrange
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockPushReactable.VerifyOnce(m => m.UnsubscribeAll());
        this.mockGL.VerifyRemoveOnce(e => e.GLError -= It.IsAny<EventHandler<GLErrorEventArgs>>(), $"Unsubscription of the '{nameof(IGLInvoker.GLError)}' event did not occur.");
        this.mockSilkWindow.VerifyRemoveOnce(e => e.Load -= It.IsAny<Action>(), $"Unsubscription of the '{nameof(SilkWindow.Load)}' event did not occur.");
        this.mockSilkWindow.VerifyRemoveOnce(s => s.Update -= It.IsAny<Action<double>>(), $"Unsubscription of the '{nameof(SilkWindow.Update)}' event did not occur.");
        this.mockSilkWindow.VerifyRemoveOnce(s => s.Render -= It.IsAny<Action<double>>(), $"Unsubscription of the '{nameof(SilkWindow.Render)}' event did not occur.");
        this.mockSilkWindow.VerifyRemoveOnce(s => s.Resize -= It.IsAny<Action<Vector2D<int>>>(), $"Unsubscription of the '{nameof(SilkWindow.Resize)}' event did not occur.");
        this.mockSilkWindow.VerifyRemoveOnce(s => s.Closing -= It.IsAny<Action>(), $"Unsubscription of the '{nameof(SilkWindow.Closing)}' event did not occur.");
        this.mockTaskService.Verify(m => m.Dispose(), Times.Once());
        this.mockGL.Verify(m => m.Dispose(), Times.Once());
        this.mockGlfw.Verify(m => m.Dispose(), Times.Once());
    }

    [Fact]
    public void GLWindow_WhenLoading_LoadsWindow()
    {
        // Arrange
        var initializeInvoked = false;
        MockWindowLoadEvent();
        var sut = CreateSystemUnderTest();
        sut.Initialize += () => initializeInvoked = true;

        // Act
        sut.Show();

        // Assert
        this.mockGLReactable.VerifyOnce(m => m.Push(It.Ref<GL>.IsAny, PushNotifications.GLContextCreatedId));
        this.mockGLReactable.VerifyOnce(m => m.Unsubscribe(PushNotifications.GLContextCreatedId));

        this.mockSilkKeyboard.VerifyAddOnce(m => m.KeyDown += It.IsAny<Action<IKeyboard, Key, int>>());
        this.mockSilkKeyboard.VerifyAddOnce(m => m.KeyUp += It.IsAny<Action<IKeyboard, Key, int>>());

        this.mockSilkMouse.VerifyAddOnce(m => m.MouseDown += It.IsAny<Action<IMouse, SilkMouseButton>>());
        this.mockSilkMouse.VerifyAddOnce(m => m.MouseUp += It.IsAny<Action<IMouse, SilkMouseButton>>());
        this.mockSilkMouse.VerifyAddOnce(m => m.MouseMove += It.IsAny<Action<IMouse, SysVector2>>());
        this.mockSilkMouse.VerifyAddOnce(m => m.Scroll += It.IsAny<Action<IMouse, ScrollWheel>>());

        this.mockGL.VerifyOnce(m => m.SetupErrorCallback());
        this.mockGL.VerifyOnce(m => m.Enable(GLEnableCap.DebugOutput));
        this.mockGL.VerifyOnce(m => m.Enable(GLEnableCap.DebugOutputSynchronous));
        this.mockGL.VerifyAddOnce(e => e.GLError += It.IsAny<EventHandler<GLErrorEventArgs>>());

        this.mockPushReactable.VerifyOnce(m => m.Push(PushNotifications.GLInitializedId));
        this.mockPushReactable.VerifyOnce(m => m.Unsubscribe(PushNotifications.GLInitializedId));

        initializeInvoked.Should().BeTrue($"the action '{nameof(IWindow)}.{nameof(IWindow.Initialize)}' must be invoked");

        this.mockAppService.VerifyOnce(m => m.Init());
    }

    [Fact]
    public void GLWindow_WhenWindowResizes_SetsGLViewportAndTriggersResizeEvent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var actualSize = default(SizeU);
        sut.WinResize = u => actualSize = u;
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Resize += It.IsAny<Action<Vector2D<int>>>(), new Vector2D<int>(11, 22));

        // Assert
        this.mockGL.Verify(m => m.Viewport(0, 0, 11, 22));
        this.mockWinSizeReactable
            .VerifyOnce(m =>
                m.Push(new WindowSizeData { Width = 11u, Height = 22u }, PushNotifications.WindowSizeChangedId));
        actualSize.Width.Should().Be(11u);
        actualSize.Height.Should().Be(22u);
    }

    [Fact]
    public void GLWindow_WhenUpdatingWhileShuttingDown_DoesNotUpdateAnything()
    {
        // Arrange
        var sutUpdateInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Show();
        sut.Update = _ => sutUpdateInvoked = true;

        // Act
        this.mockSilkWindow.Raise(e => e.Closing += null);
        this.mockSilkWindow.Raise(e => e.Update += It.IsAny<Action<double>>(), 0.016);

        // Assert
        sutUpdateInvoked.Should().BeFalse($"{nameof(GLWindow.Update)} should not of been invoked during sut shutdown.");
        this.mockMouseReactable.VerifyNever(m
            => m.Push(It.IsAny<MouseStateData>(), PushNotifications.MouseStateChangedId));
    }

    [Fact]
    public void GLWindow_WhileUpdatingWhenNotShuttingDown_PerformsUpdate()
    {
        // Arrange
        var expected = new MouseStateData
        {
            ScrollDirection = MouseScrollDirection.None,
            ScrollWheelValue = 0,
        };

        var sutUpdateInvoked = false;

        MouseStateData? actual = null;
        this.mockMouseReactable.Setup(m =>
                m.Push(It.Ref<MouseStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((ref MouseStateData data, Guid _) => { actual = data; });

        var sut = CreateSystemUnderTest();
        sut.Show();
        sut.Update = time =>
        {
            sutUpdateInvoked = true;

            time.ElapsedTime.Days.Should().Be(0);
            time.ElapsedTime.Hours.Should().Be(0);
            time.ElapsedTime.Minutes.Should().Be(0);
            time.ElapsedTime.Seconds.Should().Be(0);
            time.ElapsedTime.Milliseconds.Should().Be(16);
        };

        // Act
        this.mockSilkWindow.Raise(e => e.Update += It.IsAny<Action<double>>(), 0.016);

        // Assert
        this.mockTimerService.VerifyOnce(m => m.Start());
        sutUpdateInvoked.Should().BeTrue($"{nameof(GLWindow.Update)} was not invoked.");
        this.mockMouseReactable.VerifyOnce(m =>
            m.Push(It.Ref<MouseStateData>.IsAny, PushNotifications.MouseStateChangedId));

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameWithAutoClearEnabled_ClearsGLBuffer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Render += It.IsAny<Action<double>>(), 0.016);

        // Assert
        this.mockGL.Verify(m => m.Clear(GLClearBufferMask.ColorBufferBit), Times.Once);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameWithAutoClearDisabled_ClearsGLBuffer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoClearBuffer = false;
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Render += It.IsAny<Action<double>>(), 0.016);

        // Assert
        this.mockGL.Verify(m => m.Clear(It.IsAny<GLClearBufferMask>()), Times.Never);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrame_InvokesDrawAndSwapsBuffer()
    {
        // Arrange
        this.mockTimerService.SetupGet(p => p.MillisecondsPassed).Returns(4);
        var drawInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Draw = time =>
        {
            drawInvoked = true;
            time.ElapsedTime.Should().Be(new TimeSpan(0, 0, 0, 0, 16));
        };
        sut.AutoClearBuffer = false;
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Render += It.IsAny<Action<double>>(), 0.016);

        // Assert
        drawInvoked.Should().BeTrue($"the '{nameof(GLWindow.Draw)}()' method should of been invoked.");
        this.mockGLContext.VerifyOnce(m => m.SwapBuffers());
        this.mockTimerService.VerifyOnce(m => m.Stop());
        sut.Fps.Should().Be(250);
        this.mockTimerService.VerifyOnce(m => m.Reset());
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameDuringShutdown_DoesNotPerformRenderProcess()
    {
        // Arrange
        var drawInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Draw = _ =>
        {
            drawInvoked = true;
        };
        sut.AutoClearBuffer = true;
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Closing += null);
        this.mockSilkWindow.Raise(e => e.Render += It.IsAny<Action<double>>(), 0.016);

        // Assert
        this.mockGL.VerifyNever(m => m.Clear(It.IsAny<GLClearBufferMask>()));
        drawInvoked.Should().BeFalse($"the '{nameof(GLWindow.Draw)}()' method should not of been invoked.");
        this.mockGLContext.VerifyNever(m => m.SwapBuffers());
    }

    [Fact]
    public void GLWindow_WhenClosingWindow_ShutsDownWindow()
    {
        // Arrange
        var uninitializeInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Uninitialize += () => uninitializeInvoked = true;
        sut.Show();

        // Act
        this.mockSilkWindow.Raise(e => e.Closing += null);

        // Assert
        uninitializeInvoked.Should().BeTrue();
        this.mockSceneManager.VerifyOnce(m => m.UnloadContent());
        this.mockPushReactable.VerifyOnce(m => m.Push(PushNotifications.SystemShuttingDownId));
    }

    [Fact]
    public void GLWindow_WhenKeyboardKeyIsPressedDown_UpdatesKeyboardState()
    {
        // Arrange
        var expected = new KeyboardKeyStateData { Key = KeyCode.Space, IsDown = true };

        KeyboardKeyStateData? actual = null;

        MockWindowLoadEvent();

        this.mockKeyboardReactable.Setup(m =>
                m.Push(It.Ref<KeyboardKeyStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((in KeyboardKeyStateData data, Guid _) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");
                actual = data;
            });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkKeyboard.Raise(e => e.KeyDown += It.IsAny<Action<IKeyboard, Key, int>>(),
            null,
            Key.Space,
            0);

        // Assert
        this.mockKeyboardReactable.VerifyOnce(m =>
            m.Push(It.Ref<KeyboardKeyStateData>.IsAny, PushNotifications.KeyboardStateChangedId));
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhenKeyboardKeyIsReleased_UpdatesKeyboardState()
    {
        // Arrange
        var expected = new KeyboardKeyStateData { Key = KeyCode.K, IsDown = false };

        KeyboardKeyStateData? actual = null;

        MockWindowLoadEvent();

        this.mockKeyboardReactable.Setup(m =>
                m.Push(It.Ref<KeyboardKeyStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((in KeyboardKeyStateData data, Guid _) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");
                actual = data;
            });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkKeyboard.Raise(e => e.KeyUp += It.IsAny<Action<IKeyboard, Key, int>>(),
            null,
            Key.K,
            0);

        // Assert
        this.mockKeyboardReactable.VerifyOnce(m =>
            m.Push(It.Ref<KeyboardKeyStateData>.IsAny, PushNotifications.KeyboardStateChangedId));
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhenMouseButtonIsPressedDown_UpdatesMouseInputState()
    {
        // Arrange
        var expected = new MouseStateData
        {
            Button = VelaptorMouseButton.LeftButton,
            ButtonIsDown = true,
        };

        MockWindowLoadEvent();

        MouseStateData? actual = null;

        this.mockMouseReactable.Setup(m =>
                m.Push(It.Ref<MouseStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((ref MouseStateData data, Guid _) => { actual = data; });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkMouse.Raise(e => e.MouseDown += It.IsAny<Action<IMouse, SilkMouseButton>>(),
            null,
            SilkMouseButton.Left);

        // Assert
        this.mockMouseReactable.VerifyOnce(m =>
            m.Push(It.Ref<MouseStateData>.IsAny, PushNotifications.MouseStateChangedId));

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhenMouseButtonIsReleased_UpdatesMouseInputState()
    {
        // Arrange
        var expected = new MouseStateData
        {
            Button = VelaptorMouseButton.RightButton,
            ButtonIsDown = false,
        };

        MockWindowLoadEvent();

        MouseStateData? actual = null;
        this.mockMouseReactable.Setup(m =>
                m.Push(It.Ref<MouseStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((ref MouseStateData data, Guid _) => { actual = data; });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkMouse.Raise(e => e.MouseUp += It.IsAny<Action<IMouse, SilkMouseButton>>(),
            null,
            SilkMouseButton.Right);

        // Assert
        this.mockMouseReactable.VerifyOnce(m =>
            m.Push(It.Ref<MouseStateData>.IsAny, PushNotifications.MouseStateChangedId));

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(123, MouseScrollDirection.ScrollUp)]
    [InlineData(-123, MouseScrollDirection.ScrollDown)]
    [InlineData(0, MouseScrollDirection.None)]
    public void GLWindow_WhenMouseIsScrolled_UpdatesMouseInputState(int wheelValue, MouseScrollDirection expected)
    {
        // Arrange
        var expectedStateData = new MouseStateData
        {
            ScrollDirection = expected,
            ScrollWheelValue = wheelValue,
        };

        MockWindowLoadEvent();

        MouseStateData? actual = null;
        this.mockMouseReactable.Setup(m =>
                m.Push(It.Ref<MouseStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((ref MouseStateData data, Guid _) => { actual = data; });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkMouse.Raise(e => e.Scroll += It.IsAny<Action<IMouse, ScrollWheel>>(),
            null,
            new ScrollWheel(0, wheelValue));

        // Assert
        this.mockMouseReactable.VerifyOnce(m =>
            m.Push(It.Ref<MouseStateData>.IsAny, PushNotifications.MouseStateChangedId));

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expectedStateData);
    }

    [Fact]
    public void GLWindow_WhenMouseMoves_UpdatesMouseInputState()
    {
        // Arrange
        var expected = new MouseStateData { X = 11, Y = 22 };
        MockWindowLoadEvent();

        MouseStateData? actual = null;

        this.mockMouseReactable.Setup(m =>
                m.Push(It.Ref<MouseStateData>.IsAny, It.IsAny<Guid>()))
            .Callback((ref MouseStateData data, Guid _) => { actual = data; });

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkMouse.Raise(e => e.MouseMove += It.IsAny<Action<IMouse, SysVector2>>(),
            null,
            new SysVector2(11f, 22f));

        // Assert
        this.mockMouseReactable.VerifyOnce(m =>
            m.Push(It.Ref<MouseStateData>.IsAny, PushNotifications.MouseStateChangedId));

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Close_WhenInvoked_ClosesWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        sut.Close();

        // Assert
        this.mockSilkWindow.VerifyOnce(m => m.Close());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GLWindow"/> for the purpose of testing.
    /// </summary>
    /// <param name="width">The width of the sut.</param>
    /// <param name="height">The height of the sut.</param>
    /// <returns>The instance to test.</returns>
    private GLWindow CreateSystemUnderTest(uint width = 10, uint height = 20)
        => new (width,
            height,
            this.mockAppService.Object,
            this.mockWindowFactory.Object,
            this.mockNativeInputFactory.Object,
            this.mockGL.Object,
            this.mockGlfw.Object,
            this.mockDisplayService.Object,
            this.mockPlatform.Object,
            this.mockTaskService.Object,
            this.mockContentLoader.Object,
            this.mockSceneManager.Object,
            this.mockReactableFactory.Object,
            this.mockTimerService.Object);

    private void MockWindowLoadEvent()
    {
        this.mockSilkWindow.Setup(m => m.Run(It.IsAny<Action>()))
            .Callback<Action>(_ =>
            {
                // Mock the behavior of the load event being invoked when the app is ran
                this.mockSilkWindow.Raise(e => e.Load += null);
            });
    }

    /// <summary>
    /// Mocks the following:
    /// <list type="bullet">
    ///     <item><see cref="Silk"/>.<see cref="IInputContext"/></item>
    ///     <item><see cref="Velaptor"/>.<see cref="INativeInputFactory"/></item>
    ///     <item><see cref="Silk"/>.<see cref="IKeyboard"/></item>
    ///     <item><see cref="Silk"/>.<see cref="IMouse"/></item>
    /// </list>
    /// </summary>
    private void MockSystemSilkInput()
    {
        this.mockSilkInputContext = new Mock<IInputContext>();
        this.mockNativeInputFactory = new Mock<INativeInputFactory>();
        this.mockNativeInputFactory.Setup(m => m.CreateInput()).Returns(this.mockSilkInputContext.Object);

        MockSilkKeyboard();
        MockSilkMouse();
    }

    /// <summary>
    /// Mocks the <see cref="Silk"/> keyboard input.
    /// </summary>
    private void MockSilkKeyboard()
    {
        this.mockSilkKeyboard = new Mock<IKeyboard>();
        var keyboards = new List<IKeyboard> { this.mockSilkKeyboard.Object };
        this.mockSilkInputContext.Setup(p => p.Keyboards)
            .Returns(keyboards.AsReadOnly());
    }

    /// <summary>
    /// Mocks the <see cref="Silk"/> mouse input.
    /// </summary>
    private void MockSilkMouse()
    {
        this.mockMouseCursor = new Mock<ICursor>();
        this.mockMouseCursor.SetupProperty(p => p.CursorMode);

        this.mockSilkMouse = new Mock<IMouse>();
        this.mockSilkMouse.SetupGet(p => p.Cursor).Returns(this.mockMouseCursor.Object);

        var mice = new List<IMouse> { this.mockSilkMouse.Object };

        this.mockSilkInputContext.Setup(p => p.Mice)
            .Returns(mice.AsReadOnly());
    }
}
