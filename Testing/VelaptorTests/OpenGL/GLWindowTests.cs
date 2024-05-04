// <copyright file="GLWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Hardware;
using Velaptor.Input;
using Velaptor.Input.Exceptions;
using Velaptor.NativeInterop.GLFW;
using Velaptor.NativeInterop.ImGui;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Exceptions;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Velaptor.Services;
using Xunit;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkIWindow = Silk.NET.Windowing.IWindow;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using SysVector2 = System.Numerics.Vector2;
using VelaptorMouseButton = Velaptor.Input.MouseButton;
using VelaptorWindowBorder = Velaptor.WindowBorder;
using WindowBorder = Velaptor.WindowBorder;
using GLObjectsReactable = Carbonate.OneWay.IPushReactable<Velaptor.ReactableData.GLObjectsData>;

/// <summary>
/// Tests the <see cref="GLWindow"/> class.
/// </summary>
public class GLWindowTests : TestsBase
{
    private readonly IAppService mockAppService;
    private readonly IGLInvoker mockGL;
    private readonly IGlfwInvoker mockGlfw;
    private readonly IGLContext mockGLContext;
    private readonly ISystemDisplayService mockDisplayService;
    private readonly IPlatform mockPlatform;
    private readonly ISceneManager mockSceneManager;
    private readonly ITaskService mockTaskService;
    private readonly IStatsWindowService mockStatsWindowService;
    private readonly IImGuiFacade mockImGuiFacade;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable mockPushReactable;
    private readonly IPushReactable<MouseStateData> mockMouseReactable;
    private readonly IPushReactable<KeyboardKeyStateData> mockKeyboardReactable;
    private readonly IPushReactable<WindowSizeData> mockPushWinSizeReactable;
    private readonly IPullReactable<WindowSizeData> mockPullWinSizeReactable;
    private readonly IPushReactable<GL> mockGLReactable;
    private readonly GLObjectsReactable mockGLObjectsReactable;
    private readonly SilkIWindow mockSilkWindow;
    private readonly ITimerService mockTimerService;
    private readonly INativeInputFactory? mockNativeInputFactory;
    private readonly IInputContext? mockSilkInputContext;
    private readonly IKeyboard? mockSilkKeyboard;
    private readonly IMouse? mockSilkMouse;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLWindowTests"/> class.
    /// </summary>
    public GLWindowTests()
    {
        this.mockAppService = Substitute.For<IAppService>();
        this.mockGLContext = Substitute.For<IGLContext>();
        this.mockSilkWindow = Substitute.For<SilkIWindow>();
        this.mockSilkWindow.GLContext.Returns(this.mockGLContext);

        // Mock the input context
        this.mockSilkInputContext = Substitute.For<IInputContext>();
        this.mockNativeInputFactory = Substitute.For<INativeInputFactory>();
        this.mockNativeInputFactory.CreateInput().Returns(this.mockSilkInputContext);

        // Mock the keyboard
        this.mockSilkKeyboard = Substitute.For<IKeyboard>();
        var keyboards = new List<IKeyboard> { this.mockSilkKeyboard };
        this.mockSilkInputContext.Keyboards.Returns(keyboards.AsReadOnly());

        // Mock the mouse
        this.mockSilkMouse = Substitute.For<IMouse>();
        var mice = new List<IMouse> { this.mockSilkMouse };
        this.mockSilkInputContext.Mice.Returns(mice.AsReadOnly());

        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGlfw = Substitute.For<IGlfwInvoker>();
        this.mockDisplayService = Substitute.For<ISystemDisplayService>();
        this.mockPlatform = Substitute.For<IPlatform>();
        this.mockSceneManager = Substitute.For<ISceneManager>();
        this.mockTaskService = Substitute.For<ITaskService>();
        this.mockStatsWindowService = Substitute.For<IStatsWindowService>();
        this.mockImGuiFacade = Substitute.For<IImGuiFacade>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockMouseReactable = Substitute.For<IPushReactable<MouseStateData>>();
        this.mockKeyboardReactable = Substitute.For<IPushReactable<KeyboardKeyStateData>>();
        this.mockGLReactable = Substitute.For<IPushReactable<GL>>();
        this.mockGLObjectsReactable = Substitute.For<GLObjectsReactable>();

        var mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockPushWinSizeReactable = Substitute.For<IPushReactable<WindowSizeData>>();
        this.mockPullWinSizeReactable = Substitute.For<IPullReactable<WindowSizeData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
        this.mockReactableFactory.CreateMouseReactable().Returns(this.mockMouseReactable);
        this.mockReactableFactory.CreateKeyboardReactable().Returns(this.mockKeyboardReactable);
        this.mockReactableFactory.CreateGLReactable().Returns(this.mockGLReactable);
        this.mockReactableFactory.CreateGLObjectsReactable().Returns(this.mockGLObjectsReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
        this.mockReactableFactory.CreatePushWindowSizeReactable().Returns(this.mockPushWinSizeReactable);
        this.mockReactableFactory.CreatePullWindowSizeReactable().Returns(this.mockPullWinSizeReactable);

        this.mockTimerService = Substitute.For<ITimerService>();
    }

    #region Contructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullAppServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                null,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'appService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSilkWindowParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                null,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'silkWindow')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullNativeInputFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                null,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'nativeInputFactory')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                null,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'glInvoker')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullGLFWInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                null,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                null,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'systemDisplayService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                null,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'platform')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullTaskServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                null,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'taskService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullStatsWindowServiceServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                null,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'statsWindowServiceService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullImGuiFacadeParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
            100,
            200,
            this.mockAppService,
            this.mockSilkWindow,
            this.mockNativeInputFactory,
            this.mockGL,
            this.mockGlfw,
            this.mockDisplayService,
            this.mockPlatform,
            this.mockTaskService,
            this.mockStatsWindowService,
            null,
            this.mockSceneManager,
            this.mockReactableFactory,
            this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imGuiFacade')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSceneManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                null,
                this.mockReactableFactory,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'sceneManager')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                null,
                this.mockTimerService);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullTimerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GLWindow(
                100,
                200,
                this.mockAppService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGL,
                this.mockGlfw,
                this.mockDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockStatsWindowService,
                this.mockImGuiFacade,
                this.mockSceneManager,
                this.mockReactableFactory,
                null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'timerService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WhenInvoked_SubscribesToPullWinSizeRequests()
    {
        // Arrange & Act
        var mockUnsubscriber = Substitute.For<IDisposable>();
        this.mockSilkWindow.Size.Returns(new Vector2D<int>(100, 200));
        IRespondSubscription<WindowSizeData>? subscription = null;
        this.mockPullWinSizeReactable.Subscribe(Arg.Any<IRespondSubscription<WindowSizeData>>())
            .Returns(mockUnsubscriber);
        this.mockPullWinSizeReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<WindowSizeData>>()))
            .Do(callInfo => subscription = callInfo.Arg<IRespondSubscription<WindowSizeData>>());

        var sut = CreateSystemUnderTest(100, 200);
        var pulledWinSize = subscription.OnRespond();
        subscription.OnUnsubscribe();

        // Assert
        subscription.Should().NotBeNull();
        subscription.Id.Should().Be(PullNotifications.GetWindowSizeId);
        subscription.Name.Should().Be($"{nameof(GLWindow)}.ctor() - {PullNotifications.GetWindowSizeId}");
        this.mockPullWinSizeReactable.Received(1).Subscribe(subscription);
        pulledWinSize.Should().Be(new WindowSizeData { Width = 100, Height = 200 });
        mockUnsubscriber.Received(1).Dispose();

        sut.Width.Should().Be(100);
        sut.Height.Should().Be(200);
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WhenInvoked_HasCorrectDefaultPropValues()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.AutoSceneLoading.Should().BeTrue();
        sut.AutoSceneUnloading.Should().BeTrue();
        sut.AutoSceneUpdating.Should().BeTrue();
        sut.AutoSceneRendering.Should().BeTrue();
    }
    #endregion

    #region Prop Tests
    [Fact]
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void Width_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void Height_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void Title_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
    public void Position_WhenCachingValueOnOSXPlatform_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPlatform.CurrentPlatform.Returns(OSPlatform.OSX);

        this.mockDisplayService.MainDisplay.Returns(new SystemDisplay(this.mockPlatform)
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
    [Trait("Category", Prop)]
    public void Position_WhenCachingValueOnWindowsPlatform_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPlatform.CurrentPlatform.Returns(OSPlatform.Windows);

        this.mockDisplayService.MainDisplay.Returns(new SystemDisplay(this.mockPlatform)
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
    [Trait("Category", Prop)]
    public void Position_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void UpdateFrequency_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void MouseCursorVisible_WhenSettingValueAndNotCaching_ReturnsCorrectResult()
    {
        // Arrange
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
    [Trait("Category", Prop)]
    public void WindowState_WhenGettingInvalidValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'this.silkWindow.{nameof(WindowState)}' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(WindowState)}'. (Parameter 'this.silkWindow.{nameof(WindowState)}')";

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();
        this.mockSilkWindow.WindowState = (WindowState)invalidValue;

        // Act
        var act = () => _ = sut.WindowState;

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    [Trait("Category", Prop)]
    public void WindowState_WhenSettingInvalidValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(StateOfWindow)}'. (Parameter 'value')";

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        var act = () => sut.WindowState = (StateOfWindow)invalidValue;

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WindowState_WhenSettingValueAndNotCaching_ReturnsCorrectResult(int sutStateValue)
    {
        // Arrange
        var sutState = (StateOfWindow)sutStateValue;
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void Initialized_WhenWindowIsInitialized_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        sut.Initialized.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", Prop)]
    public void TypeOfBorder_WhenGettingInvalidValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'this.silkWindow.{nameof(WindowBorder)}' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(WindowBorder)}'. (Parameter 'this.silkWindow.{nameof(WindowBorder)}')";

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();
        this.mockSilkWindow.WindowBorder = (SilkWindowBorder)invalidValue;

        // Act
        var act = () => _ = sut.TypeOfBorder;

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    [Trait("Category", Prop)]
    public void TypeOfBorder_WhenSettingInvalidValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(WindowBorder)}'. (Parameter 'value')";

        this.mockSilkWindow.WindowBorder.Returns((SilkWindowBorder)invalidValue);
        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        var act = () => sut.TypeOfBorder = (VelaptorWindowBorder)invalidValue;

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TypeOfBorder_WhenSettingValueAndNotCaching_ReturnsCorrectResult(int sutBorderValue)
    {
        // Arrange
        var sutBorder = (VelaptorWindowBorder)sutBorderValue;

        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.CachedTypeOfBorder.IsCaching = false;

        // Act
        sut.TypeOfBorder = sutBorder;
        var actual = sut.TypeOfBorder;

        // Assert
        actual.Should().Be(sutBorder);
    }

    [Fact]
    public void SceneManager_WhenGettingValue_IsExpectedObject()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.SceneManager.Should().BeSameAs(this.mockSceneManager);
    }

    [Fact]
    public void AutoSceneLoading_WhenSettingValue_ReturnsCorrectValue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var expected = !sut.AutoSceneLoading;

        // Act
        sut.AutoSceneLoading = !sut.AutoSceneLoading;

        // Assert
        sut.AutoSceneLoading.Should().Be(expected);
    }

    [Fact]
    public void AutoSceneUnloading_WhenSettingValue_ReturnsCorrectValue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var expected = !sut.AutoSceneUnloading;

        // Act
        sut.AutoSceneUnloading = !sut.AutoSceneUnloading;

        // Assert
        sut.AutoSceneUnloading.Should().Be(expected);
    }

    [Fact]
    public void AutoSceneUpdating_WhenSettingValue_ReturnsCorrectValue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var expected = !sut.AutoSceneUpdating;

        // Act
        sut.AutoSceneUpdating = !sut.AutoSceneUpdating;

        // Assert
        sut.AutoSceneUpdating.Should().Be(expected);
    }

    [Fact]
    public void AutoSceneRendering_WhenSettingValue_ReturnsCorrectValue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var expected = !sut.AutoSceneRendering;

        // Act
        sut.AutoSceneRendering = !sut.AutoSceneRendering;

        // Assert
        sut.AutoSceneRendering.Should().Be(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Show_WithNoSystemKeyboards_ThrowsException()
    {
        // Arrange
        this.mockSilkInputContext.Keyboards.Returns(Array.Empty<IKeyboard>().AsReadOnly());
        var sut = CreateSystemUnderTest();

        // Act
        var act = () =>
        {
            sut.Show();
            this.mockSilkWindow.Load += Raise.Event<Action>();
        };

        // Assert
        act.Should().Throw<NoKeyboardException>()
            .WithMessage("Input Exception: No connected keyboards available.");
    }

    [Fact]
    public void Show_WithNoSystemMice_ThrowsException()
    {
        // Arrange
        this.mockSilkInputContext.Mice.Returns(Array.Empty<IMouse>().AsReadOnly());
        var sut = CreateSystemUnderTest();

        // Act
        var act = () =>
        {
            sut.Show();
            this.mockSilkWindow.Load += Raise.Event<Action>();
        };

        // Assert
        act.Should().Throw<NoMouseException>()
            .WithMessage("Input Exception: No connected mice available.");
    }

    [Fact]
    public void Show_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.Show();

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'GLWindow'.");
    }

    [Fact]
    public void Show_WhenInvoked_SubscribesToEvents()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        this.mockSilkWindow.Received().Load += Arg.Any<Action>();
        this.mockSilkWindow.Received().Closing += Arg.Any<Action>();
        this.mockSilkWindow.Received().Resize += Arg.Any<Action<Vector2D<int>>>();
        this.mockSilkWindow.Received().Update += Arg.Any<Action<double>>();
        this.mockSilkWindow.Received().Render += Arg.Any<Action<double>>();
    }

    [Fact]
    public void Show_WhenInvoked_SetsUpOpenGLErrorCallback()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        this.mockGL.Received().GLError += Arg.Any<EventHandler<GLErrorEventArgs>>();
    }

    [Fact]
    public async Task ShowAsync_WhileDisposed_ThrowsException()
    {
        // Arrange
        this.mockTaskService.SetAction(Arg.Do<Action>(action => action()));

        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = async () =>
        {
            await sut.ShowAsync();
            this.mockTaskService.Start();
        };

        // Assert
        await act.Should().ThrowAsync<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'GLWindow'.");
    }

    [Fact]
    public async Task ShowAsync_WhenInvoked_StartsInternalShowTask()
    {
        // Arrange
        this.mockTaskService.SetAction(Arg.Do<Action>(action => action()));
        var sut = CreateSystemUnderTest();

        // Act
        await sut.ShowAsync();

        // Assert
        this.mockTaskService.Received(1).Start();
    }

    [Fact]
    public async Task ShowAsync_WhenAfterStartParamIsNotNull_ExecutesAtCorrectTime()
    {
        // Arrange
        var taskServiceSetActionInvoked = false;
        var taskServiceStartInvoked = false;
        this.mockTaskService.When(x => x.SetAction(Arg.Any<Action>()))
            .Do(_ => taskServiceSetActionInvoked = true);
        this.mockTaskService.When(x => x.Start())
            .Do(_ => taskServiceStartInvoked = true);

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
        this.mockSilkWindow.When(x => x.Close())
            .Do(_ => this.mockSilkWindow.Closing += Raise.Event<Action>());

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
        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockPushReactable.Received(1).UnsubscribeAll();
        this.mockGL.Received().GLError -= Arg.Any<EventHandler<GLErrorEventArgs>>();

        // Assert unsubscriptions from keyboard and mouse
        this.mockSilkKeyboard.Received().KeyDown -= Arg.Any<Action<IKeyboard, Key, int>>();
        this.mockSilkKeyboard.Received().KeyUp -= Arg.Any<Action<IKeyboard, Key, int>>();
        this.mockSilkMouse.Received().MouseDown -= Arg.Any<Action<IMouse, SilkMouseButton>>();
        this.mockSilkMouse.Received().MouseUp -= Arg.Any<Action<IMouse, SilkMouseButton>>();
        this.mockSilkMouse.Received().MouseMove -= Arg.Any<Action<IMouse, SysVector2>>();
        this.mockSilkMouse.Received().Scroll -= Arg.Any<Action<IMouse, ScrollWheel>>();

        // Assert unsubscriptions from window events
        this.mockSilkWindow.Received().Load -= Arg.Any<Action>();
        this.mockSilkWindow.Received().Update -= Arg.Any<Action<double>>();
        this.mockSilkWindow.Received().Render -= Arg.Any<Action<double>>();
        this.mockSilkWindow.Received().Resize -= Arg.Any<Action<Vector2D<int>>>();
        this.mockSilkWindow.Received().Closing -= Arg.Any<Action>();

        // Assert dispose invokes
        this.mockStatsWindowService.Received(1).Dispose();
        this.mockTaskService.Received(1).Dispose();
        this.mockImGuiFacade.Received(1).Dispose();
        this.mockGlfw.Received(1).Dispose();
        this.mockGL.Received(1).Dispose();
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
        this.mockSilkWindow.Received(1).Close();
    }
    #endregion

    #region Internal Tests
    [Fact]
    public void GLWindow_WhenInternalLoadEventIsInvoked_RunsInitialization()
    {
        // Arrange
        var sut = CreateSystemUnderTest(123, 456);
        GLObjectsData glObjectsData = default;

        this.mockGLObjectsReactable
            .When(x => x.Push(PushNotifications.GLObjectsCreatedId, in Arg.Any<GLObjectsData>()))
            .Do(callInfo =>
            {
                glObjectsData = callInfo.Arg<GLObjectsData>();
            });

        // Act
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        this.mockSilkWindow.Size.Should().Be(new Vector2D<int>(123, 456));
        this.mockNativeInputFactory.Received(1).CreateInput();

        glObjectsData.GL.Context.Should().BeSameAs(this.mockGLContext);
        glObjectsData.Window.Should().BeSameAs(this.mockSilkWindow);
        glObjectsData.InputContext.Should().BeSameAs(this.mockSilkInputContext);

        this.mockGLObjectsReactable.Received(1).Push(PushNotifications.GLObjectsCreatedId, glObjectsData);
        this.mockGLObjectsReactable.Received(1).Unsubscribe(PushNotifications.GLObjectsCreatedId);

        this.mockSilkKeyboard.Received().KeyDown += Arg.Any<Action<IKeyboard, Key, int>>();
        this.mockSilkKeyboard.Received().KeyUp += Arg.Any<Action<IKeyboard, Key, int>>();

        this.mockSilkMouse.Received().MouseDown += Arg.Any<Action<IMouse, SilkMouseButton>>();
        this.mockSilkMouse.Received().MouseUp += Arg.Any<Action<IMouse, SilkMouseButton>>();
        this.mockSilkMouse.Received().MouseMove += Arg.Any<Action<IMouse, SysVector2>>();
        this.mockSilkMouse.Received().Scroll += Arg.Any<Action<IMouse, ScrollWheel>>();

        this.mockAppService.Received(1).Init();
    }

    [Fact]
    public void GLWindow_WhenInternalLoadEventIsInvoked_LoadsWindow()
    {
        // Arrange
        var initInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Initialize = () => initInvoked = true;

        // Act
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        this.mockGL.Received(1).SetupErrorCallback();
        this.mockGL.Received(1).Enable(GLEnableCap.DebugOutput);
        this.mockGL.Received(1).Enable(GLEnableCap.DebugOutputSynchronous);
        this.mockGL.Received().GLError += Arg.Any<EventHandler<GLErrorEventArgs>>();

        // Assert that all prop caching has been disabled
        sut.CachedStringProps.Values.Should().AllSatisfy(prop => prop.IsCaching.Should().BeFalse());
        sut.CachedBoolProps.Values.Should().AllSatisfy(prop => prop.IsCaching.Should().BeFalse());
        sut.CachedIntProps.Values.Should().AllSatisfy(prop => prop.IsCaching.Should().BeFalse());
        sut.CachedUIntProps.Values.Should().AllSatisfy(prop => prop.IsCaching.Should().BeFalse());
        sut.CachedPosition.IsCaching.Should().BeFalse();
        sut.CachedWindowState.IsCaching.Should().BeFalse();
        sut.CachedTypeOfBorder.IsCaching.Should().BeFalse();

        initInvoked.Should().BeTrue();
        this.mockPushReactable.Received(1).Push(PushNotifications.GLInitializedId);
        this.mockPushReactable.Received(1).Unsubscribe(PushNotifications.GLInitializedId);
        sut.Initialized.Should().BeTrue();

        this.mockGLReactable.Received(1).Push(PushNotifications.GLContextCreatedId, in Arg.Any<GL>());
        this.mockGLReactable.Received(1).Unsubscribe(PushNotifications.GLContextCreatedId);
        this.mockNativeInputFactory.Received(1).CreateInput();
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
        this.mockSilkWindow.Resize += Raise.Event<Action<Vector2D<int>>>(new Vector2D<int>(11, 22));

        // Assert
        this.mockGL.Viewport(0, 0, 11, 22);
        this.mockPushWinSizeReactable.Received(1)
            .Push(PushNotifications.WindowSizeChangedId, new WindowSizeData { Width = 11u, Height = 22u });
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
        this.mockSilkWindow.Closing += Raise.Event<Action>();
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        sutUpdateInvoked.Should().BeFalse($"{nameof(GLWindow.Update)} should not of been invoked during sut shutdown.");
        this.mockMouseReactable.DidNotReceive().Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());
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
        this.mockMouseReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<MouseStateData>()))
            .Do(callInfo => actual = callInfo.Arg<MouseStateData>());

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
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockTimerService.Received(1).Start();
        sutUpdateInvoked.Should().BeTrue($"{nameof(GLWindow.Update)} was not invoked.");
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhileUpdatingWhenNotShuttingDown_UpdatesStatsWindow()
    {
        // Arrange
        var expectedFrameTime = new FrameTime { ElapsedTime = TimeSpan.FromMilliseconds(16), };
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockStatsWindowService.Received(1).Update(expectedFrameTime);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameWithAutoClearEnabled_ClearsGLBuffer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockGL.Received(1).Clear(GLClearBufferMask.ColorBufferBit);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameWithAutoClearDisabled_ClearsGLBuffer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoClearBuffer = false;
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockGL.DidNotReceive().Clear(Arg.Any<GLClearBufferMask>());
    }

    [Fact]
    public void GLWindow_WhenRenderingFrame_RendersStatsWindow()
    {
        // Arrange
        this.mockTimerService.MillisecondsPassed.Returns(4);
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.0);

        // Assert
        this.mockStatsWindowService.Received(1).UpdateFpsStat(Arg.Any<float>());
        this.mockStatsWindowService.Received(1).Render();
    }

    [Fact]
    public void GLWindow_WhenRenderingFrameWhileInitializingStatsWindow_SetsStatsWindowPosition()
    {
        // Arrange
        var eventInitializedInvoked = false;
        this.mockTimerService.MillisecondsPassed.Returns(4);
        this.mockStatsWindowService.Size.Returns(new Size(40, 20));
        var sut = CreateSystemUnderTest(100, 200);
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        this.mockStatsWindowService.Initialized += (_, _) => eventInitializedInvoked = true;

        // Act
        this.mockStatsWindowService.Initialized += Raise.Event();

        // Assert
        eventInitializedInvoked.Should().BeTrue();
        this.mockStatsWindowService.Received(1).Position = new Point(10, 170);
    }

    [Fact]
    public void GLWindow_WhenRenderingFrame_InvokesDrawAndSwapsBuffer()
    {
        // Arrange
        this.mockTimerService.MillisecondsPassed.Returns(4);
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
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        drawInvoked.Should().BeTrue($"the '{nameof(GLWindow.Draw)}()' method should of been invoked.");
        this.mockGLContext.Received(1).SwapBuffers();
        this.mockTimerService.Received(1).Stop();
        sut.Fps.Should().Be(250);
        this.mockTimerService.Received(1).Reset();
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
        this.mockSilkWindow.Closing += Raise.Event<Action>();
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockGL.DidNotReceive().Clear(Arg.Any<GLClearBufferMask>());
        drawInvoked.Should().BeFalse($"the '{nameof(GLWindow.Draw)}()' method should not of been invoked.");
        this.mockGLContext.DidNotReceive().SwapBuffers();
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
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        uninitializeInvoked.Should().BeTrue();
        this.mockPushReactable.Received(1).Push(PushNotifications.SystemShuttingDownId);
    }

    [Fact]
    public void GLWindow_WhenKeyboardKeyIsPressedDown_UpdatesKeyboardState()
    {
        // Arrange
        var expected = new KeyboardKeyStateData { Key = KeyCode.Space, IsDown = true };

        KeyboardKeyStateData? actual = null;
        this.mockKeyboardReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<KeyboardKeyStateData>()))
            .Do(callInfo => actual = callInfo.Arg<KeyboardKeyStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkKeyboard.KeyDown += Raise.Event<Action<IKeyboard, Key, int>>(null, Key.Space, 0);

        // Assert
        this.mockKeyboardReactable.Received(1)
            .Push(PushNotifications.KeyboardStateChangedId, Arg.Any<KeyboardKeyStateData>());
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLWindow_WhenKeyboardKeyIsReleased_UpdatesKeyboardState()
    {
        // Arrange
        var expected = new KeyboardKeyStateData { Key = KeyCode.K, IsDown = false };

        KeyboardKeyStateData? actual = null;
        this.mockKeyboardReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<KeyboardKeyStateData>()))
            .Do(callInfo => actual = callInfo.Arg<KeyboardKeyStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkKeyboard.KeyUp += Raise.Event<Action<IKeyboard, Key, int>>(null, Key.K, 0);

        // Assert
        this.mockKeyboardReactable.Push(PushNotifications.KeyboardStateChangedId, Arg.Any<KeyboardKeyStateData>());
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

        MouseStateData? actual = null;

        this.mockMouseReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<MouseStateData>()))
            .Do(callInfo => actual = callInfo.Arg<MouseStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkMouse.MouseDown += Raise.Event<Action<IMouse, SilkMouseButton>>(null, SilkMouseButton.Left);

        // Assert
        this.mockMouseReactable.Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

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

        MouseStateData? actual = null;
        this.mockMouseReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<MouseStateData>()))
            .Do(callInfo => actual = callInfo.Arg<MouseStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkMouse.MouseUp += Raise.Event<Action<IMouse, SilkMouseButton>>(null, SilkMouseButton.Right);

        // Assert
        this.mockMouseReactable.Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

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

        MouseStateData? actual = null;
        this.mockMouseReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<MouseStateData>()))
            .Do(callInfo => actual = callInfo.Arg<MouseStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkMouse.Scroll += Raise.Event<Action<IMouse, ScrollWheel>>(null, new ScrollWheel(0, wheelValue));

        // Assert
        this.mockMouseReactable.Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expectedStateData);
    }

    [Fact]
    public void GLWindow_WhenMouseMoves_UpdatesMouseInputState()
    {
        // Arrange
        var expected = new MouseStateData { X = 11, Y = 22 };

        MouseStateData? actual = null;

        this.mockMouseReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<MouseStateData>()))
            .Do(callInfo => actual = callInfo.Arg<MouseStateData>());

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        this.mockSilkMouse.MouseMove += Raise.Event<Action<IMouse, SysVector2>>(null, new SysVector2(11f, 22f));

        // Assert
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GLError_WhenErrorOccurs_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Make sure that the load process is invoked
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        var act = () =>
        {
            this.mockGL.GLError += Raise.EventWith(sut, new GLErrorEventArgs("test-error"));
        };

        // Assert
        act.Should().Throw<GLException>().WithMessage("test-error");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GLWindow"/> for the purpose of testing.
    /// </summary>
    /// <param name="width">The width of the sut.</param>
    /// <param name="height">The height of the sut.</param>
    /// <returns>The instance to test.</returns>
    private GLWindow CreateSystemUnderTest(uint width = 10, uint height = 20)
        => new (width, height,
            this.mockAppService,
            this.mockSilkWindow,
            this.mockNativeInputFactory,
            this.mockGL,
            this.mockGlfw,
            this.mockDisplayService,
            this.mockPlatform,
            this.mockTaskService,
            this.mockStatsWindowService,
            this.mockImGuiFacade,
            this.mockSceneManager,
            this.mockReactableFactory,
            this.mockTimerService);
}
