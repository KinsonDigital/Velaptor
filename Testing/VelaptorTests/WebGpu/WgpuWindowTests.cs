// <copyright file="WgpuWindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable ConvertToLocalFunction
namespace VelaptorTests.WebGpu;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using NSubstitute;
using Shouldly;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Input.Exceptions;
using Velaptor.NativeInterop.GLFW;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Velaptor.Services;
using Velaptor.Telemetry;
using Velaptor.WebGpu;
using Xunit;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using VelaptorButton = Velaptor.Input.MouseButton;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using VelaptorWindowBorder = Velaptor.WindowBorder;

/// <summary>
/// Tests the <see cref="WgpuWindow"/> class.
/// </summary>
public class WgpuWindowTests
{
    private readonly ITelemetryService mockTelemetryService;
    private readonly IWindow mockSilkWindow;
    private readonly INativeInputFactory mockNativeInputFactory;
    private readonly IGlfwInvoker mockGlfwInvoker;
    private readonly ISystemDisplayService mockSystemDisplayService;
    private readonly IPlatform mockPlatform;
    private readonly ITaskService mockTaskService;
    private readonly ISceneManager mockSceneManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly ILoggingService mockLoggingService;
    private readonly IFrameMetricsTracker mockMetricsTracker;
    private readonly IPushReactable mockPushReactable;
    private readonly IPushReactable<MouseStateData> mockMouseReactable;
    private readonly IPushReactable<KeyboardKeyStateData> mockKeyboardReactable;
    private readonly IPushReactable<ViewPortSizeData> mockViewPortReactable;
    private readonly IPushReactable<WindowSizeData> mockPushWinSizeReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="WgpuWindowTests"/> class.
    /// </summary>
    public WgpuWindowTests()
    {
        this.mockTelemetryService = Substitute.For<ITelemetryService>();
        this.mockSilkWindow = Substitute.For<IWindow>();
        this.mockNativeInputFactory = Substitute.For<INativeInputFactory>();
        this.mockGlfwInvoker = Substitute.For<IGlfwInvoker>();
        this.mockSystemDisplayService = Substitute.For<ISystemDisplayService>();
        this.mockPlatform = Substitute.For<IPlatform>();
        this.mockTaskService = Substitute.For<ITaskService>();
        this.mockSceneManager = Substitute.For<ISceneManager>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockMouseReactable = Substitute.For<IPushReactable<MouseStateData>>();
        this.mockKeyboardReactable = Substitute.For<IPushReactable<KeyboardKeyStateData>>();
        this.mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockPushWinSizeReactable = Substitute.For<IPushReactable<WindowSizeData>>();
        var mockPullWinSizeReactable = Substitute.For<IPullReactable<WindowSizeData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
        this.mockReactableFactory.CreateMouseReactable().Returns(this.mockMouseReactable);
        this.mockReactableFactory.CreateKeyboardReactable().Returns(this.mockKeyboardReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(this.mockViewPortReactable);
        this.mockReactableFactory.CreatePushWindowSizeReactable().Returns(this.mockPushWinSizeReactable);
        this.mockReactableFactory.CreatePullWindowSizeReactable().Returns(mockPullWinSizeReactable);

        this.mockLoggingService = Substitute.For<ILoggingService>();

        this.mockMetricsTracker = Substitute.For<IFrameMetricsTracker>();
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullTelemetryServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                null,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'telemetryService')");
    }

    [Fact]
    public void Ctor_WithNullSilkWindowParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                null,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'silkWindow')");
    }

    [Fact]
    public void Ctor_WithNullNativeInputFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                null,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'nativeInputFactory')");
    }

    [Fact]
    public void Ctor_WithNullGlfwInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                null,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                null,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'systemDisplayService')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                null,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'platform')");
    }

    [Fact]
    public void Ctor_WithNullTaskServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                null,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'taskService')");
    }

    [Fact]
    public void Ctor_WithNullSceneManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                null,
                this.mockReactableFactory,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'sceneManager')");
    }

    [Fact]
    public void Ctor_WithNullLoggingServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                null,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'loggingService')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                null,
                this.mockLoggingService,
                this.mockMetricsTracker);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullFrameMetricsTrackerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WgpuWindow(
                100,
                200,
                this.mockTelemetryService,
                this.mockSilkWindow,
                this.mockNativeInputFactory,
                this.mockGlfwInvoker,
                this.mockSystemDisplayService,
                this.mockPlatform,
                this.mockTaskService,
                this.mockSceneManager,
                this.mockReactableFactory,
                this.mockLoggingService,
                null);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'frameMetricsTracker')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Title_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Title = "test-title";
        var actual = sut.Title;

        // Assert
        actual.ShouldBe("test-title");
        this.mockSilkWindow.DidNotReceive().Title = Arg.Any<string>();
    }

    [Fact]
    public void Title_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);

        // Act
        sut.Title = "test-title";
        var actual = sut.Title;

        // Assert
        actual.ShouldBe("test-title");
        this.mockSilkWindow.Received(1).Title = "test-title";
    }

    [Fact]
    public void Width_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Width = 123u;
        var actual = sut.Width;

        // Assert
        actual.ShouldBe(123u);
        this.mockSilkWindow.DidNotReceive().Size = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void Width_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);
        this.mockSilkWindow.ClearReceivedCalls();

        // Act
        sut.Width = 123u;
        var actual = sut.Width;

        // Assert
        actual.ShouldBe(123u);
        this.mockSilkWindow.Received(1).Size = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void Height_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Height = 456u;
        var actual = sut.Height;

        // Assert
        actual.ShouldBe(456u);
        this.mockSilkWindow.DidNotReceive().Size = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void Height_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);
        this.mockSilkWindow.ClearReceivedCalls();

        // Act
        sut.Height = 456u;
        var actual = sut.Height;

        // Assert
        actual.ShouldBe(456u);
        this.mockSilkWindow.Received(1).Size = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void UpdateFrequency_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UpdateFrequency = 123;
        var actual = sut.UpdateFrequency;

        // Assert
        actual.ShouldBe(123);
        this.mockSilkWindow.DidNotReceive().UpdatesPerSecond = Arg.Any<double>();
    }

    [Fact]
    public void UpdateFrequency_WhenSettingValueWhileCachingIsOff_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);

        // Act
        sut.UpdateFrequency = 123;
        var actual = sut.UpdateFrequency;

        // Assert
        actual.ShouldBe(123);
        this.mockSilkWindow.Received(1).UpdatesPerSecond = 123;
    }

    [Fact]
    public void MouseCursorVisible_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MouseCursorVisible = false;
        var actual = sut.MouseCursorVisible;

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void MouseCursorVisible_WhenSettingValueWhileCachingIsOff_DelegatesToInputContext()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);

        // Act
        sut.MouseCursorVisible = false;
        var actual = sut.MouseCursorVisible;

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void WindowState_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.WindowState = StateOfWindow.FullScreen;
        var actual = sut.WindowState;

        // Assert
        actual.ShouldBe(StateOfWindow.FullScreen);
        this.mockSilkWindow.DidNotReceive().WindowState = Arg.Any<WindowState>();
    }

    [Fact]
    public void WindowState_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);
        this.mockSilkWindow.ClearReceivedCalls();

        // Act
        sut.WindowState = StateOfWindow.FullScreen;
        var actual = sut.WindowState;

        // Assert
        actual.ShouldBe(StateOfWindow.FullScreen);
        this.mockSilkWindow.Received(1).WindowState = Arg.Any<WindowState>();
    }

    [Fact]
    public void TypeOfBorder_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.TypeOfBorder = VelaptorWindowBorder.Fixed;
        var actual = sut.TypeOfBorder;

        // Assert
        actual.ShouldBe(VelaptorWindowBorder.Fixed);
        this.mockSilkWindow.DidNotReceive().WindowBorder = Arg.Any<SilkWindowBorder>();
    }

    [Fact]
    public void TypeOfBorder_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);
        this.mockSilkWindow.ClearReceivedCalls();

        // Act
        sut.TypeOfBorder = VelaptorWindowBorder.Fixed;
        var actual = sut.TypeOfBorder;

        // Assert
        actual.ShouldBe(VelaptorWindowBorder.Fixed);
        this.mockSilkWindow.Received(1).WindowBorder = Arg.Any<SilkWindowBorder>();
    }

    [Fact]
    public void Position_WhenSettingValueWhileCachingIsOn_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Vector2(11, 22);
        var actual = sut.Position;

        // Assert
        actual.ShouldBe(new Vector2(11, 22));
        this.mockSilkWindow.DidNotReceive().Position = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void Position_WhenSettingValueWhileCachingIsOff_DelegatesToSilkWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.SetPropertyCaching(false);
        this.mockSilkWindow.ClearReceivedCalls();

        // Act
        sut.Position = new Vector2(11, 22);
        var actual = sut.Position;

        // Assert
        actual.ShouldBe(new Vector2(11, 22));
        this.mockSilkWindow.Received(1).Position = Arg.Any<Vector2D<int>>();
    }

    [Fact]
    public void AutoClearBuffer_WhenInvoked_DefaultsToTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoClearBuffer;

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void AutoSceneLoading_WhenInvoked_DefaultsToTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSceneLoading;

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void AutoSceneUnloading_WhenInvoked_DefaultsToTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSceneUnloading;

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void AutoSceneUpdating_WhenInvoked_DefaultsToTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSceneUpdating;

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void AutoSceneRendering_WhenInvoked_DefaultsToTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSceneRendering;

        // Assert
        actual.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Show_WhenInvoked_PreInitializesWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        this.mockSilkWindow.Received(1).UpdatesPerSecond = 60;
        this.mockSilkWindow.Received(1).Load += Arg.Any<Action>();
        this.mockSilkWindow.Received(1).Closing += Arg.Any<Action>();
        this.mockSilkWindow.Received(1).Resize += Arg.Any<Action<Vector2D<int>>>();
        this.mockSilkWindow.Received(1).Update += Arg.Any<Action<double>>();
        this.mockSilkWindow.Received(1).Render += Arg.Any<Action<double>>();
        this.mockSilkWindow.Received(1).Run(Arg.Any<Action>());
        this.mockSilkWindow.Received(1).Dispose();
    }

    [Fact]
    public async Task ShowAsync_WithNonNullParams_ShowsWindowAndDoesNotContinueTask()
    {
        // Arrange
        var afterStartInvoked = false;
        var afterUnloadInvoked = false;

        this.mockTaskService.When(x => x.SetAction(Arg.Any<Action>()))
            .Do(callInfo =>
            {
                // Invoked the internal action to run the PreInit() and RunWindow() methods.
                // This is necessary to wire up the closing event.
                var action = callInfo.Arg<Action>();
                action();
            });

        var sut = CreateSystemUnderTest();

        var afterStart = () =>
        {
            afterStartInvoked = true;
        };

        var afterUnload = () =>
        {
            afterUnloadInvoked = true;
        };

        // Act
        await sut.ShowAsync(afterStart, afterUnload);
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        afterStartInvoked.ShouldBeTrue();
        afterUnloadInvoked.ShouldBeTrue();
        await this.mockTaskService.DidNotReceive()
            .ContinueWith(Arg.Any<Action<Task>>(), Arg.Any<TaskContinuationOptions>(), Arg.Any<TaskScheduler>());
    }

    [Fact]
    public async Task ShowAsync_WithNullAfterStart_ShowsWindowAndContinuesTask()
    {
        // Arrange
        var afterUnloadInvoked = false;

        this.mockTaskService.When(x => x.SetAction(Arg.Any<Action>()))
            .Do(callInfo =>
            {
                // Invoked the internal action to run the PreInit() and RunWindow() methods.
                // This is necessary to wire up the closing event.
                var action = callInfo.Arg<Action>();
                action();
            });

        var sut = CreateSystemUnderTest();

        var afterUnload = () =>
        {
            afterUnloadInvoked = true;
        };

        // Act
        await sut.ShowAsync(null, afterUnload);
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        afterUnloadInvoked.ShouldBeTrue();
        await this.mockTaskService.Received(1)
            .ContinueWith(Arg.Any<Action<Task>>(), TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    [Fact]
    public void Close_WhenInvoked_ClosesWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Close();

        // Assert
        this.mockSilkWindow.Received(1).Close();
    }
    #endregion

    #region Internal Tests
    [Fact]
    public void InternalLoad_WhenNoKeyboardsAvailable_ThrowsException()
    {
        // Arrange
        var mockInputContext = Substitute.For<IInputContext>();
        var keyboards = new List<IKeyboard>([]);

        mockInputContext.Keyboards.Returns(keyboards);
        this.mockNativeInputFactory.CreateInput().Returns(mockInputContext);

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        act.ShouldThrow<NoKeyboardException>()
            .Message.ShouldBe("Input Exception: No connected keyboards are available.");
    }

    [Fact]
    public void InternalLoad_WhenNoMiceAreAvailable_ThrowsException()
    {
        // Arrange
        var mockKeyboard = Substitute.For<IKeyboard>();
        var mockInputContext = Substitute.For<IInputContext>();
        var keyboards = new List<IKeyboard>([mockKeyboard]);
        var mice = new List<IMouse>([]);

        mockInputContext.Keyboards.Returns(keyboards);
        mockInputContext.Mice.Returns(mice);
        this.mockNativeInputFactory.CreateInput().Returns(mockInputContext);

        var sut = CreateSystemUnderTest();
        sut.Show();

        // Act
        var act = () => this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        act.ShouldThrow<NoMouseException>()
            .Message.ShouldBe("Input Exception: No connected mice are available.");
    }

    [Fact]
    public void InternalInit_WhenInvoked_InitializesWindow()
    {
        // Arrange
        (IInputContext mockInputContext, IKeyboard mockKeyboard, IMouse mockMouse) = MockInputContext();

        var winResizeInvoked = false;
        var expectedViewPortSize = new ViewPortSizeData { Width = 111, Height = 222 };
        var expectedWinSizeData = new WindowSizeData { Width = 111, Height = 222 };

        var sut = CreateSystemUnderTest();
        sut.Width = 111;
        sut.Height = 222;

        sut.WinResize += size =>
        {
            winResizeInvoked = true;

            size.Width.ShouldBe(111u);
            size.Height.ShouldBe(222u);
        };

        sut.Show();

        // Act
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Assert
        mockInputContext.Keyboards.ShouldHaveSingleItem();
        mockKeyboard.Received(1).KeyDown += Arg.Any<Action<IKeyboard, Key, int>>();
        mockKeyboard.Received(1).KeyUp += Arg.Any<Action<IKeyboard, Key, int>>();
        mockMouse.Received(1).MouseDown += Arg.Any<Action<IMouse, SilkMouseButton>>();
        mockMouse.Received(1).MouseUp += Arg.Any<Action<IMouse, SilkMouseButton>>();
        mockMouse.Received(1).MouseMove += Arg.Any<Action<IMouse, Vector2>>();
        mockMouse.Received(1).Scroll += Arg.Any<Action<IMouse, ScrollWheel>>();

        this.mockPushReactable.Received(1).Push(PushNotifications.SurfaceReconfigureId);
        winResizeInvoked.ShouldBeTrue($"The {nameof(WgpuWindow.WinResize)} is not being invoked.");
        this.mockViewPortReactable.Received(2).Push(PushNotifications.ViewPortSizeChangedId, expectedViewPortSize);
        this.mockPushWinSizeReactable.Received(1).Push(PushNotifications.WindowSizeChangedId, expectedWinSizeData);
    }

    [Fact]
    public async Task InternalClosing_WhenInvoked_StartsClosingProcess()
    {
        // Arrange
        var uninitializeInvoked = false;
        var afterUnloadInvoked = false;

        Action? afterStart = null;

        this.mockTaskService
            .When(x => x.SetAction(Arg.Any<Action>()))
            .Do(call => afterStart = call.Arg<Action>());

        var sut = CreateSystemUnderTest();
        sut.Uninitialize += () => uninitializeInvoked = true;
        await sut.ShowAsync(null, () => afterUnloadInvoked = true);

        afterStart?.Invoke();

        // Act
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        uninitializeInvoked.ShouldBeTrue($"The {nameof(WgpuWindow.Uninitialize)} property was not invoked.");
        this.mockPushReactable.Received(1).Push(PushNotifications.SystemShuttingDownId);
        afterUnloadInvoked.ShouldBeTrue("The 'afterUnload' action parameter was not invoked.");
    }

    [Fact]
    public async Task InternalClosing_WhenUninitializingThrowsAnException_LogsError()
    {
        // Arrange
        var exception = new Exception("test-exception");
        var sut = CreateSystemUnderTest();
        sut.Uninitialize += () => throw exception;

        sut.Show();

        // Act
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Assert
        this.mockLoggingService.Received(1).Error(exception);
    }

    [Fact]
    public void InternalUpdate_WhenInvoked_ProcessesUpdate()
    {
        // Arrange
        var expectedMouseData = new MouseStateData { ScrollDirection = MouseScrollDirection.None, ScrollWheelValue = 0, };
        var updateInvoked = false;

        var sut = CreateSystemUnderTest();
        sut.Show();
        sut.Update += (frameTime) =>
        {
            updateInvoked = true;
            frameTime.ElapsedTime.Milliseconds.ShouldBe(16);
        };

        // Act
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        updateInvoked.ShouldBeTrue($"The '${nameof(WgpuWindow.Update)}' property was not invoked.");
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, expectedMouseData);
    }

    [Fact]
    public void InternalUpdate_WhenShuttingDown_DoesNotProcessesUpdate()
    {
        // Arrange
        var updateInvoked = false;

        var sut = CreateSystemUnderTest();
        sut.Update += _ => updateInvoked = true;
        sut.Show();
        this.mockSilkWindow.Closing += Raise.Event<Action>();

        // Act
        this.mockSilkWindow.Update += Raise.Event<Action<double>>(0.016);

        // Assert
        updateInvoked.ShouldBeFalse($"The '${nameof(WgpuWindow.Update)}' property was invoked.");
        this.mockMouseReactable.DidNotReceive().Push(PushNotifications.MouseStateChangedId, Arg.Any<MouseStateData>());
    }

    [Fact]
    public void InternalRender_WithSingleRender_ProcessesRender()
    {
        // Arrange
        var updateInvoked = false;
        var drawInvoked = false;
        var expectedMetrics = new FrameMetrics
        {
            AverageFps = 123,
        };

        this.mockMetricsTracker.CurrentMetrics.Returns(expectedMetrics);

        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.Update += (frameTime) =>
        {
            frameTime.ElapsedTime.Milliseconds.ShouldBe(16);
            updateInvoked = true;
        };

        sut.Draw += (frameTime) =>
        {
            frameTime.ElapsedTime.Milliseconds.ShouldBe(16);
            drawInvoked = true;
        };

        // Act
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        updateInvoked.ShouldBeTrue();
        drawInvoked.ShouldBeTrue();
        this.mockPushReactable.Received(1).Push(PushNotifications.SubmitRenderPassId);
        this.mockMetricsTracker.Received(1).RecordFrame(0.016);
        sut.Fps.ShouldBe(123);
    }

    [Fact]
    public void InternalRender_WhenShuttingDown_DoesNotProcessRender()
    {
        // Arrange
        var updateInvoked = false;
        var drawInvoked = false;
        var expectedMetrics = new FrameMetrics
        {
            AverageFps = 123,
        };

        this.mockMetricsTracker.CurrentMetrics.Returns(expectedMetrics);

        var sut = CreateSystemUnderTest();
        sut.Show();

        sut.Update += (frameTime) =>
        {
            frameTime.ElapsedTime.Milliseconds.ShouldBe(16);
            updateInvoked = true;
        };

        sut.Draw += (frameTime) =>
        {
            frameTime.ElapsedTime.Milliseconds.ShouldBe(16);
            drawInvoked = true;
        };

        // Act
        this.mockSilkWindow.Closing += Raise.Event<Action>();
        this.mockSilkWindow.Render += Raise.Event<Action<double>>(0.016);

        // Assert
        updateInvoked.ShouldBeTrue();
        drawInvoked.ShouldBeFalse();

        this.mockPushReactable.DidNotReceive().Push(PushNotifications.SubmitRenderPassId);
        this.mockMetricsTracker.DidNotReceive().RecordFrame(Arg.Any<double>());
        sut.Fps.ShouldBe(0f);
    }

    [Fact]
    public void InternalKeyDown_WhenInvoked_ProcessesKeyboardKeyDown()
    {
        // Arrange
        var expectedKeyState = new KeyboardKeyStateData { Key = KeyCode.A, IsDown = true };
        (IInputContext _, IKeyboard mockKeyboard, IMouse _) = MockInputContext();

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockKeyboard.KeyDown += Raise.Event<Action<IKeyboard, Key, int>>(mockKeyboard, Key.A, 0);

        // Assert
        this.mockKeyboardReactable.Received(1).Push(PushNotifications.KeyboardStateChangedId, expectedKeyState);
    }

    [Fact]
    public void InternalKeyUp_WhenInvoked_ProcessesKeyboardKeyUp()
    {
        // Arrange
        var expectedKeyState = new KeyboardKeyStateData { Key = KeyCode.B, IsDown = false };
        (IInputContext _, IKeyboard mockKeyboard, IMouse _) = MockInputContext();

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockKeyboard.KeyUp += Raise.Event<Action<IKeyboard, Key, int>>(mockKeyboard, Key.B, 0);

        // Assert
        this.mockKeyboardReactable.Received(1).Push(PushNotifications.KeyboardStateChangedId, expectedKeyState);
    }

    [Fact]
    public void InternalMouseDown_WhenInvoked_ProcessesKeyboardMouseDown()
    {
        // Arrange
        var expectedMouseState = new MouseStateData { Button = VelaptorButton.LeftButton, ButtonIsDown = true };
        (IInputContext _, IKeyboard _, IMouse mockMouse) = MockInputContext();

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockMouse.MouseDown += Raise.Event<Action<IMouse, SilkMouseButton>>(mockMouse, SilkMouseButton.Left);

        // Assert
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, expectedMouseState);
    }

    [Fact]
    public void InternalMouseUp_WhenInvoked_ProcessesKeyboardMouseUp()
    {
        // Arrange
        var expectedMouseState = new MouseStateData { Button = VelaptorButton.LeftButton, ButtonIsDown = false };
        (IInputContext _, IKeyboard _, IMouse mockMouse) = MockInputContext();

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockMouse.MouseUp += Raise.Event<Action<IMouse, SilkMouseButton>>(mockMouse, SilkMouseButton.Left);

        // Assert
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, expectedMouseState);
    }

    [Theory]
    [InlineData(10, MouseScrollDirection.ScrollUp)]
    [InlineData(-10, MouseScrollDirection.ScrollDown)]
    [InlineData(0, MouseScrollDirection.None)]
    public void InternalMouseScroll_WhenInvoked_ProcessesKeyboardMouseScroll(int wheelPos, MouseScrollDirection direction)
    {
        // Arrange
        var expectedMouseState = new MouseStateData
        {
            ScrollWheelValue = wheelPos,
            ScrollDirection = direction,
        };
        (IInputContext _, IKeyboard _, IMouse mockMouse) = MockInputContext();

        var wheelData = new ScrollWheel(44, wheelPos);
        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockMouse.Scroll += Raise.Event<Action<IMouse, ScrollWheel>>(mockMouse, wheelData);

        // Assert
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, expectedMouseState);
    }

    [Fact]
    public void InternalMouseMove_WhenInvoked_ProcessesKeyboardMouseMove()
    {
        // Arrange
        var expectedMouseState = new MouseStateData { X = 10, Y = 20, };
        (IInputContext _, IKeyboard _, IMouse mockMouse) = MockInputContext();

        var sut = CreateSystemUnderTest();
        sut.Show();
        this.mockSilkWindow.Load += Raise.Event<Action>();

        // Act
        mockMouse.MouseMove += Raise.Event<Action<IMouse, Vector2>>(mockMouse, new Vector2(10, 20));

        // Assert
        this.mockMouseReactable.Received(1).Push(PushNotifications.MouseStateChangedId, expectedMouseState);
    }
    #endregion

    /// <summary>
    /// Mocks the input context with a single keyboard and mouse.
    /// </summary>
    /// <returns>The mocked input, keyboard, and mouse objects.</returns>
    private (IInputContext inputContext, IKeyboard keyboard, IMouse mouse) MockInputContext()
    {
        var mockInputContext = Substitute.For<IInputContext>();

        var mockKeyboard = Substitute.For<IKeyboard>();
        var mockMouse = Substitute.For<IMouse>();

        var keyboards = new List<IKeyboard>([mockKeyboard]);
        var mice = new List<IMouse>([mockMouse]);

        mockInputContext.Keyboards.Returns(keyboards);
        mockInputContext.Mice.Returns(mice);

        this.mockNativeInputFactory.CreateInput().Returns(mockInputContext);

        return (mockInputContext, mockKeyboard, mockMouse);
    }

    /// <summary>
    /// Creates a new instance of <see cref="WgpuWindow"/> for testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private WgpuWindow CreateSystemUnderTest()
    {
        return new WgpuWindow(
            100,
            200,
            this.mockTelemetryService,
            this.mockSilkWindow,
            this.mockNativeInputFactory,
            this.mockGlfwInvoker,
            this.mockSystemDisplayService,
            this.mockPlatform,
            this.mockTaskService,
            this.mockSceneManager,
            this.mockReactableFactory,
            this.mockLoggingService,
            this.mockMetricsTracker);
    }
}
