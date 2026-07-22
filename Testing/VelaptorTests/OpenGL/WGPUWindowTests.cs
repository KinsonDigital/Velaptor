// <copyright file="WGPUWindowTests.cs" company="KinsonDigital">
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
using Shouldly;
using Helpers;
using NSubstitute;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Hardware;
using Velaptor.Input;
using Velaptor.Input.Exceptions;
using Velaptor.NativeInterop.GLFW;
using Velaptor.NativeInterop.ImGui;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Velaptor.Services;
using Velaptor.Telemetry;
using Xunit;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkIWindow = Silk.NET.Windowing.IWindow;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using SysVector2 = System.Numerics.Vector2;
using VelaptorMouseButton = Velaptor.Input.MouseButton;
using VelaptorWindowBorder = Velaptor.WindowBorder;
using WindowBorder = Velaptor.WindowBorder;
using Velaptor.WebGPU;

/// <summary>
/// Tests the <see cref="WGPUWindow"/> class.
/// </summary>
public class WGPUWindowTests : TestsBase
{
    private readonly ITelemetryService mockTelemetryService;
    private readonly IGlfwInvoker mockGlfw;
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
    private readonly SilkIWindow mockSilkWindow;
    private readonly ITimerService mockTimerService;
    private readonly INativeInputFactory? mockNativeInputFactory;
    private readonly IInputContext? mockSilkInputContext;
    private readonly IKeyboard? mockSilkKeyboard;
    private readonly IMouse? mockSilkMouse;

    /// <summary>
    /// Initializes a new instance of the <see cref="WGPUWindowTests"/> class.
    /// </summary>
    public WGPUWindowTests()
    {
        this.mockTelemetryService = Substitute.For<ITelemetryService>();
        this.mockSilkWindow = Substitute.For<SilkIWindow>();

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

        var mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockPushWinSizeReactable = Substitute.For<IPushReactable<WindowSizeData>>();
        this.mockPullWinSizeReactable = Substitute.For<IPullReactable<WindowSizeData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
        this.mockReactableFactory.CreateMouseReactable().Returns(this.mockMouseReactable);
        this.mockReactableFactory.CreateKeyboardReactable().Returns(this.mockKeyboardReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
        this.mockReactableFactory.CreatePushWindowSizeReactable().Returns(this.mockPushWinSizeReactable);
        this.mockReactableFactory.CreatePullWindowSizeReactable().Returns(this.mockPullWinSizeReactable);

        this.mockTimerService = Substitute.For<ITimerService>();
    }

    #region Contructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullTelemetryServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                null,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'telemetryService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSilkWindowParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                null,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'silkWindow')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullNativeInputFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'nativeInputFactory')");
    }


    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullGLFWInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'systemDisplayService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'platform')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullTaskServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'taskService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullStatsWindowServiceServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'statsWindowServiceService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullImGuiFacadeParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
            100,
            200,
            this.mockTelemetryService,
            this.mockSilkWindow,
            this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imGuiFacade')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullSceneManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'sceneManager')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullTimerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new WGPUWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'timerService')");
    }


    [Fact]
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
        subscription.ShouldNotBeNull();
        subscription.Id.ShouldBe(PullNotifications.GetWindowSizeId);
        this.mockPullWinSizeReactable.Received(1).Subscribe(subscription);
        pulledWinSize.ShouldBe(new WindowSizeData { Width = 100, Height = 200 });
        mockUnsubscriber.Received(1).Dispose();

        sut.Width.ShouldBe(100u);
        sut.Height.ShouldBe(200u);
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WhenInvoked_HasCorrectDefaultPropValues()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.AutoSceneLoading.ShouldBeTrue();
        sut.AutoSceneUnloading.ShouldBeTrue();
        sut.AutoSceneUpdating.ShouldBeTrue();
        sut.AutoSceneRendering.ShouldBeTrue();
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
        actual.ShouldBe(100u);
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
        actual.ShouldBe(111u);
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
        actual.ShouldBe(200u);
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
        actual.ShouldBe(111u);
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
        actual.ShouldBe("Velaptor Application");
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
        actual.ShouldBe("test-title");
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
        actual.ShouldBe(new SysVector2(950, 400));
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
        actual.ShouldBe(new SysVector2(950, 400));
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
        actual.ShouldBe(new SysVector2(123, 456));
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
        actual.ShouldBe(60);
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
        actual.ShouldBe(30);
    }

    [Fact]
    public void MouseCursorVisible_WhenCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.MouseCursorVisible;

        // Assert
        actual.ShouldBeTrue();
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
        actual.ShouldBeFalse();
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

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => sut.WindowState);
        exception.Message.ShouldBe(expected);
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

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => sut.WindowState = (StateOfWindow)invalidValue);
        exception.Message.ShouldBe(expected);
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
        actual.ShouldBe(StateOfWindow.Normal);
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
        actual.ShouldBe(sutState);
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
        sut.Initialize.ShouldBeSameAs(initAction);
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
        sut.Uninitialize.ShouldBeSameAs(unInitAction);
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
        sut.Initialized.ShouldBeTrue();
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

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => sut.TypeOfBorder);
        exception.Message.ShouldBe(expected);
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

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => sut.TypeOfBorder = (VelaptorWindowBorder)invalidValue);
        exception.Message.ShouldBe(expected);
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
        actual.ShouldBe(VelaptorWindowBorder.Resizable);
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
        actual.ShouldBe(sutBorder);
    }

    [Fact]
    public void SceneManager_WhenGettingValue_IsExpectedObject()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.SceneManager.ShouldBeSameAs(this.mockSceneManager);
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
        sut.AutoSceneLoading.ShouldBe(expected);
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
        sut.AutoSceneUnloading.ShouldBe(expected);
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
        sut.AutoSceneUpdating.ShouldBe(expected);
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
        sut.AutoSceneRendering.ShouldBe(expected);
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
        var exception = act.ShouldThrow<NoKeyboardException>();
        exception.Message.ShouldBe("Input Exception: No connected keyboards are available.");
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
        var exception = act.ShouldThrow<NoMouseException>();
        exception.Message.ShouldBe("Input Exception: No connected mice are available.");
    }

    [Fact]
    public void Show_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = sut.Show;

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'WGPUWindow'.");
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
        var exception = await act.ShouldThrowAsync<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'WGPUWindow'.");
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
        taskServiceSetActionInvoked.ShouldBeTrue(
            $"the {nameof(ITaskService)}.{nameof(ITaskService.SetAction)}() method must be executed before the 'afterStart` parameter");
        taskServiceStartInvoked.ShouldBeTrue(
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
        afterUnloadExecuted.ShouldBeTrue("the 'afterUnload' parameter must be executed after the sut unloads.");
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
    public void WGPUWindow_WhenUpdatingWhileShuttingDown_DoesNotUpdateAnything()
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
        sutUpdateInvoked.ShouldBeFalse($"{nameof(WGPUWindow.Update)} should not of been invoked during sut shutdown.");
        this.mockMouseReactable.DidNotReceive().Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());
    }

    [Fact]
    public void WGPUWindow_WhileUpdatingWhenNotShuttingDown_PerformsUpdate()
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

            time.ElapsedTime.Days.ShouldBe(0);
            time.ElapsedTime.Hours.ShouldBe(0);
            time.ElapsedTime.Minutes.ShouldBe(0);
            time.ElapsedTime.Seconds.ShouldBe(0);
            time.ElapsedTime.Milliseconds.ShouldBe(16);
        };

        // Act
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockTimerService.Received(1).Start();
        sutUpdateInvoked.ShouldBeTrue($"{nameof(WGPUWindow.Update)} was not invoked.");
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());

        actual.ShouldNotBeNull();
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void WGPUWindow_WhileUpdatingWhenNotShuttingDown_UpdatesStatsWindow()
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
    public void WGPUWindow_WhenRenderingFrameWhileInitializingStatsWindow_SetsStatsWindowPosition()
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
        eventInitializedInvoked.ShouldBeTrue();
        this.mockStatsWindowService.Received(1).Position = new Point(10, 170);
    }

    [Fact]
    public void WGPUWindow_WhenClosingWindow_ShutsDownWindow()
    {
        // Arrange
        var uninitializeInvoked = false;
        var sut = CreateSystemUnderTest();
        sut.Uninitialize += () => uninitializeInvoked = true;
        sut.Show();

        // Act
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        uninitializeInvoked.ShouldBeTrue();
        this.mockPushReactable.Received(1).Push(PushNotifications.SystemShuttingDownId);
    }

    [Fact]
    public void WGPUWindow_WhenKeyboardKeyIsPressedDown_UpdatesKeyboardState()
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
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void WGPUWindow_WhenKeyboardKeyIsReleased_UpdatesKeyboardState()
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
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void WGPUWindow_WhenMouseButtonIsPressedDown_UpdatesMouseInputState()
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

        actual.ShouldNotBeNull();
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void WGPUWindow_WhenMouseButtonIsReleased_UpdatesMouseInputState()
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

        actual.ShouldNotBeNull();
        actual.ShouldBeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(123, MouseScrollDirection.ScrollUp)]
    [InlineData(-123, MouseScrollDirection.ScrollDown)]
    [InlineData(0, MouseScrollDirection.None)]
    public void WGPUWindow_WhenMouseIsScrolled_UpdatesMouseInputState(int wheelValue, MouseScrollDirection expected)
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

        actual.ShouldNotBeNull();
        actual.ShouldBe(expectedStateData);
    }

    [Fact]
    public void WGPUWindow_WhenMouseMoves_UpdatesMouseInputState()
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

        actual.ShouldNotBeNull();
        actual.ShouldBe(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="WGPUWindow"/> for the purpose of testing.
    /// </summary>
    /// <param name="width">The width of the sut.</param>
    /// <param name="height">The height of the sut.</param>
    /// <returns>The instance to test.</returns>
    [Fact]
    [Trait("Category", Method)]
    public void WGPUWindow_WhenRenderingFrameInWebGpuMode_InvokesImGuiFacadeUpdateAndRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert — ImGui facade must still be driven in WebGPU mode so KdGui can use ImGui
        this.mockImGuiFacade.Received().Update(Arg.Any<double>());
        this.mockImGuiFacade.Received().Render();
    }

    [Fact]
    [Trait("Category", Method)]
    public void WGPUWindow_WhenRenderingFrameInWebGpuMode_DoesNotRenderStatsWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockStatsWindowService.DidNotReceive().Render();
    }

    [Fact]
    [Trait("Category", Method)]
    public void WGPUWindow_WhenRenderingFrameInWebGpuMode_PushesSubmitRenderPassNotification()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        this.mockPushReactable.Received(1).Push(PushNotifications.SubmitRenderPassId);
    }

    private WGPUWindow CreateSystemUnderTest(uint width = 10, uint height = 20)
        => new (width, height,
            this.mockTelemetryService,
            this.mockSilkWindow,
            this.mockNativeInputFactory,
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



