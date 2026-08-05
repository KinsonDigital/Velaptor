// <copyright file="WgpuWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Carbonate;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Factories;
using Input;
using Input.Exceptions;
using NativeInterop.GLFW;
using ReactableData;
using Scene;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Telemetry;
using Services;
using SilkIWindow = Silk.NET.Windowing.IWindow;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using VelaptorIWindow = UI.IWindow;
using VelaptorMouseButton = Input.MouseButton;
using VelaptorWindowBorder = WindowBorder;

/// <summary>
/// A WebGPU-backed window implementation used inside the <see cref="Velaptor.UI.Window"/> class.
/// </summary>
internal sealed class WgpuWindow : VelaptorIWindow
{
    private const int WindowPadding = 10;
    private readonly SilkIWindow silkWindow;
    private readonly INativeInputFactory nativeInputFactory;
    private readonly IGlfwInvoker glfw;
    private readonly ISystemDisplayService systemDisplayService;
    private readonly IPlatform platform;
    private readonly ITaskService taskService;
    private readonly IStatsWindowService statsWindowServiceService;
    private readonly IPushReactable pushReactable;
    private readonly IPushReactable<MouseStateData> mouseReactable;
    private readonly IPushReactable<KeyboardKeyStateData> keyboardReactable;
    private readonly IPushReactable<ViewPortSizeData> viewPortReactable;
    private readonly IPushReactable<WindowSizeData> pushWinSizeReactable;
    private readonly Dictionary<string, CachedValue<string>> cachedStringProps = new ();
    private readonly Dictionary<string, CachedValue<int>> cachedIntProps = new ();
    private readonly Dictionary<string, CachedValue<uint>> cachedUIntProps = new ();
    private readonly Dictionary<string, CachedValue<bool>> cachedBoolProps = new ();
    private readonly ITimerService timerService;
    private readonly IDisposable pullWinSizeUnsubscriber;
    private CachedValue<StateOfWindow>? cachedWindowState;
    private CachedValue<VelaptorWindowBorder>? cachedTypeOfBorder;
    private CachedValue<Vector2>? cachedPosition;
    private MouseStateData mouseStateData;
    private IInputContext? glInputContext;
    private bool isShuttingDown;
    private bool firstRenderInvoked;
    private bool isDisposed;
    private Action? afterUnloadAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="WgpuWindow"/> class.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <param name="telemetryService">Provides telemetry services.</param>
    /// <param name="silkWindow">The Silk.NET window object.</param>
    /// <param name="nativeInputFactory">Creates native input objects.</param>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="systemDisplayService">Provides information about system displays.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <param name="taskService">Runs asynchronous tasks.</param>
    /// <param name="statsWindowServiceService">Manages the ImGui stats window.</param>
    /// <param name="sceneManager">Manages scenes.</param>
    /// <param name="reactableFactory">Creates reactables for push/pull notifications.</param>
    /// <param name="timerService">Measures game-loop frame time.</param>
    public WgpuWindow(
        uint width,
        uint height,
        ITelemetryService telemetryService,
        SilkIWindow silkWindow,
        INativeInputFactory nativeInputFactory,
        IGlfwInvoker glfwInvoker,
        ISystemDisplayService systemDisplayService,
        IPlatform platform,
        ITaskService taskService,
        IStatsWindowService statsWindowServiceService,
        ISceneManager sceneManager,
        IReactableFactory reactableFactory,
        ITimerService timerService)
    {
        ArgumentNullException.ThrowIfNull(telemetryService);
        ArgumentNullException.ThrowIfNull(silkWindow);
        ArgumentNullException.ThrowIfNull(nativeInputFactory);
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(systemDisplayService);
        ArgumentNullException.ThrowIfNull(platform);
        ArgumentNullException.ThrowIfNull(taskService);
        ArgumentNullException.ThrowIfNull(statsWindowServiceService);
        ArgumentNullException.ThrowIfNull(sceneManager);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(timerService);

        this.silkWindow = silkWindow;
        this.nativeInputFactory = nativeInputFactory;
        this.glfw = glfwInvoker;
        this.systemDisplayService = systemDisplayService;
        this.platform = platform;
        this.taskService = taskService;
        this.statsWindowServiceService = statsWindowServiceService;
        SceneManager = sceneManager;

        this.pushReactable = reactableFactory.CreateNoDataPushReactable();
        this.mouseReactable = reactableFactory.CreateMouseReactable();
        this.keyboardReactable = reactableFactory.CreateKeyboardReactable();
        this.viewPortReactable = reactableFactory.CreateViewPortReactable();
        this.pushWinSizeReactable = reactableFactory.CreatePushWindowSizeReactable();
        var pullWinSizeReactable = reactableFactory.CreatePullWindowSizeReactable();
        this.timerService = timerService;

        this.mouseStateData = default;

        SetupWidthHeightPropCaches(width <= 0u ? 1u : width, height <= 0u ? 1u : height);
        SetupOtherPropCaches();

        this.statsWindowServiceService.Initialized += (_, _) =>
        {
            this.statsWindowServiceService.Position = new Point(
                WindowPadding,
                (int)Height - (this.statsWindowServiceService.Size.Height + WindowPadding));
        };

        this.pullWinSizeUnsubscriber = pullWinSizeReactable.CreateOneWayRespond(
            PullNotifications.GetWindowSizeId,
            () => new WindowSizeData { Width = Width, Height = Height },
            () => this.pullWinSizeUnsubscriber?.Dispose());

        telemetryService.TrackAppStart();
        telemetryService.TrackHardware();
    }

    /// <inheritdoc/>
    public string Title
    {
        get => this.cachedStringProps[nameof(Title)].GetValue();
        set => this.cachedStringProps[nameof(Title)].SetValue(value);
    }

    /// <inheritdoc/>
    public Vector2 Position
    {
        get => this.cachedPosition?.GetValue() ?? Vector2.Zero;
        set => this.cachedPosition?.SetValue(value);
    }

    /// <inheritdoc/>
    public uint Width
    {
        get => this.cachedUIntProps[nameof(Width)].GetValue();
        set => this.cachedUIntProps[nameof(Width)].SetValue(value);
    }

    /// <inheritdoc/>
    public uint Height
    {
        get => this.cachedUIntProps[nameof(Height)].GetValue();
        set => this.cachedUIntProps[nameof(Height)].SetValue(value);
    }

    /// <inheritdoc/>
    public bool AutoClearBuffer { get; set; } = true;

    /// <inheritdoc/>
    public bool MouseCursorVisible
    {
        get => this.cachedBoolProps[nameof(MouseCursorVisible)].GetValue();
        set => this.cachedBoolProps[nameof(MouseCursorVisible)].SetValue(value);
    }

    /// <inheritdoc/>
    public StateOfWindow WindowState
    {
        get => this.cachedWindowState?.GetValue() ?? StateOfWindow.Normal;
        set => this.cachedWindowState?.SetValue(value);
    }

    /// <inheritdoc/>
    public Action? Initialize { get; set; }

    /// <inheritdoc/>
    public Action<FrameTime>? Update { get; set; }

    /// <inheritdoc/>
    public Action<FrameTime>? Draw { get; set; }

    /// <inheritdoc/>
    public Action? Uninitialize { get; set; }

    /// <inheritdoc/>
    public Action<SizeU>? WinResize { get; set; }

    /// <inheritdoc/>
    public VelaptorWindowBorder TypeOfBorder
    {
        get => this.cachedTypeOfBorder?.GetValue() ?? VelaptorWindowBorder.Resizable;
        set => this.cachedTypeOfBorder?.SetValue(value);
    }

    /// <inheritdoc/>
    public ISceneManager SceneManager { get; }

    /// <inheritdoc/>
    public bool AutoSceneLoading { get; set; } = true;

    /// <inheritdoc/>
    public bool AutoSceneUnloading { get; set; } = true;

    /// <inheritdoc/>
    public bool AutoSceneUpdating { get; set; } = true;

    /// <inheritdoc/>
    public bool AutoSceneRendering { get; set; } = true;

    /// <inheritdoc/>
    public float Fps { get; private set; }

    /// <inheritdoc/>
    public int UpdateFrequency
    {
        get => this.cachedIntProps[nameof(UpdateFrequency)].GetValue();
        set => this.cachedIntProps[nameof(UpdateFrequency)].SetValue(value);
    }

    /// <inheritdoc/>
    public bool Initialized { get; private set; }

    /// <inheritdoc/>
    public void Show()
    {
        PreInit();
        RunGLWindow();
    }

    /// <inheritdoc/>
    public async Task ShowAsync(Action? afterStart = null, Action? afterUnload = null)
    {
        this.afterUnloadAction = afterUnload;

        this.taskService.SetAction(
            () =>
            {
                PreInit();
                RunGLWindow();
            });

        this.taskService.Start();

        if (afterStart is not null)
        {
            afterStart();
            return;
        }

        await this.taskService.ContinueWith(
            _ => { },
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    /// <inheritdoc/>
    public void Close() => this.silkWindow.Close();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Runs the window.
    /// </summary>
    private void RunGLWindow()
    {
        this.silkWindow.Run();

        /*NOTE:
         * Only dispose of the window here and not in the Dispose() method!!
         *
         * This is because the line of code below will not be executed until the Window.Run() method
         * has finished executing.  This happens once the window is closed.
         *
         * If you dispose of the window in the Dispose() method before the Run() method is finished
         * then the application will crash.
         */
        this.silkWindow.Dispose();
    }

    /// <summary>
    /// Initializes window-related setup before the Silk.NET window Load event fires.
    /// </summary>
    private void PreInit()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(WgpuWindow));
        }

        this.silkWindow.UpdatesPerSecond = 60;
        this.silkWindow.Load += GLWindow_Load;
        this.silkWindow.Closing += GLWindow_Closing;
        this.silkWindow.Resize += GLWindow_Resize;
        this.silkWindow.Update += GLWindow_Update;
        this.silkWindow.Render += GLWindow_Render;
    }

    /// <summary>
    /// Sets up input and fires the initial viewport resize after the Silk.NET window has loaded.
    /// </summary>
    private void Init(uint width, uint height)
    {
        this.silkWindow.Size = new Vector2D<int>((int)width, (int)height);
        this.glInputContext = this.nativeInputFactory.CreateInput();

        if (this.glInputContext.Keyboards.Count <= 0)
        {
            throw new NoKeyboardException("Input Exception: No connected keyboards are available.");
        }

        this.glInputContext.Keyboards[0].KeyDown += GLKeyboardInput_KeyDown;
        this.glInputContext.Keyboards[0].KeyUp += GLKeyboardInput_KeyUp;

        if (this.glInputContext.Mice.Count <= 0)
        {
            throw new NoMouseException("Input Exception: No connected mice are available.");
        }

        this.glInputContext.Mice[0].MouseDown += GLMouseInput_MouseDown;
        this.glInputContext.Mice[0].MouseUp += GLMouseInput_MouseUp;
        this.glInputContext.Mice[0].MouseMove += GLMouseMove_MouseMove;
        this.glInputContext.Mice[0].Scroll += GLMouseInput_MouseScroll;

        // Manually invoke the resize to update the rest of the system, such as the viewport.
        GLWindow_Resize(new Vector2D<int>((int)width, (int)height));
    }

    /// <summary>
    /// Invokes the <see cref="Initialize"/> action property.
    /// </summary>
    private void GLWindow_Load()
    {
        Init(Width, Height);

        this.cachedStringProps.Values.ToList().ForEach(i => i.IsCaching = false);
        this.cachedBoolProps.Values.ToList().ForEach(i => i.IsCaching = false);
        this.cachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);
        this.cachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

        if (this.cachedPosition is not null)
        {
            this.cachedPosition.IsCaching = false;
        }

        if (this.cachedWindowState is not null)
        {
            this.cachedWindowState.IsCaching = false;
        }

        if (this.cachedTypeOfBorder is not null)
        {
            this.cachedTypeOfBorder.IsCaching = false;
        }

        // Notify all subscribers that the window is ready. For WebGPU this is the signal
        // for WgpuBatcher to initialize the WebGPU surface, adapter, device and pipelines.
        // This MUST happen BEFORE Initialize?.Invoke() because content loading may trigger
        // texture creation which needs the WebGPU device to be initialized first.
        this.pushReactable.Push(PushNotifications.GLInitializedId);
        this.pushReactable.Unsubscribe(PushNotifications.GLInitializedId);

        Initialize?.Invoke();

        // Re-push the viewport size so any GPU buffers created during Initialize?.Invoke()
        // receive the correct window dimensions (the earlier push in Init() fires before
        // content is loaded so those renderers miss it).
        this.viewPortReactable.Push(
            PushNotifications.ViewPortSizeChangedId,
            new ViewPortSizeData { Width = Width, Height = Height });

        Initialized = true;
    }

    /// <summary>
    /// Invoked when the window is closing.
    /// </summary>
    private void GLWindow_Closing()
    {
        this.isShuttingDown = true;

        Uninitialize?.Invoke();

        // Triggers cache clean-up in texture/audio loaders and GPU resource release
        // before the WebGPU device is torn down.
        this.pushReactable.Push(PushNotifications.SystemShuttingDownId);

        this.afterUnloadAction?.Invoke();
    }

    /// <summary>
    /// Invoked every time the native window size changes.
    /// </summary>
    private void GLWindow_Resize(Vector2D<int> obj)
    {
        var width = (uint)obj.X;
        var height = (uint)obj.Y;

        // Signal the WebGPU batcher to reconfigure the swap chain so its textures
        // match the new framebuffer dimensions before the next render pass opens.
        this.pushReactable.Push(PushNotifications.SurfaceReconfigureId);

        var size = new SizeU { Width = width, Height = height };
        WinResize?.Invoke(size);

        this.viewPortReactable.Push(PushNotifications.ViewPortSizeChangedId, new ViewPortSizeData { Width = width, Height = height });
        this.pushWinSizeReactable.Push(PushNotifications.WindowSizeChangedId, new WindowSizeData { Width = width, Height = height });
    }

    /// <summary>
    /// Invoked once per frame for the update step.
    /// </summary>
    private void GLWindow_Update(double time)
    {
        this.timerService.Start();

        if (this.isShuttingDown)
        {
            return;
        }

        var frameTime = new FrameTime
        {
            ElapsedTime = TimeSpan.FromMilliseconds(time * 1000.0),
        };

        Update?.Invoke(frameTime);

        this.statsWindowServiceService.Update(frameTime);

        this.mouseStateData = this.mouseStateData with
        {
            ScrollDirection = MouseScrollDirection.None,
            ScrollWheelValue = 0,
        };

        this.mouseReactable.Push(PushNotifications.MouseStateChangedId, this.mouseStateData);
    }

    /// <summary>
    /// Invoked once per frame for the render step.
    /// </summary>
    private void GLWindow_Render(double time)
    {
        if (!this.firstRenderInvoked)
        {
            Update?.Invoke(new FrameTime
            {
                ElapsedTime = TimeSpan.FromMilliseconds(time * 1000.0),
            });
            this.firstRenderInvoked = true;
        }

        if (this.isShuttingDown)
        {
            return;
        }

        var frameTime = new FrameTime
        {
            ElapsedTime = TimeSpan.FromMilliseconds(time * 1000.0),
        };

        Draw?.Invoke(frameTime);

        this.statsWindowServiceService.UpdateFpsStat(Fps);

        this.pushReactable.Push(PushNotifications.SubmitRenderPassId);

        this.timerService.Stop();
        Fps = 1000f / this.timerService.MillisecondsPassed;
    }

    /// <summary>
    /// Invoked when a keyboard key transitions to the down position.
    /// </summary>
    private void GLKeyboardInput_KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        var keyStateData = new KeyboardKeyStateData { Key = (KeyCode)key, IsDown = true };

        this.keyboardReactable.Push(PushNotifications.KeyboardStateChangedId, keyStateData);
    }

    /// <summary>
    /// Invoked when a keyboard key transitions to the up position.
    /// </summary>
    private void GLKeyboardInput_KeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        var keyStateData = new KeyboardKeyStateData { Key = (KeyCode)key, IsDown = false };

        this.keyboardReactable.Push(PushNotifications.KeyboardStateChangedId, keyStateData);
    }

    /// <summary>
    /// Invoked when a mouse button is pressed.
    /// </summary>
    private void GLMouseInput_MouseDown(IMouse mouse, SilkMouseButton button)
    {
        this.mouseStateData = this.mouseStateData with
        {
            Button = (VelaptorMouseButton)button,
            ButtonIsDown = true,
        };

        this.mouseReactable.Push(PushNotifications.MouseStateChangedId, this.mouseStateData);
    }

    /// <summary>
    /// Invoked when a mouse button is released.
    /// </summary>
    private void GLMouseInput_MouseUp(IMouse mouse, SilkMouseButton button)
    {
        this.mouseStateData = this.mouseStateData with
        {
            Button = (VelaptorMouseButton)button,
            ButtonIsDown = false,
        };

        this.mouseReactable.Push(PushNotifications.MouseStateChangedId, this.mouseStateData);
    }

    /// <summary>
    /// Invoked when the mouse scroll wheel is used.
    /// </summary>
    private void GLMouseInput_MouseScroll(IMouse mouse, ScrollWheel wheelData)
    {
        this.mouseStateData = this.mouseStateData with
        {
            ScrollWheelValue = (int)wheelData.Y,
            ScrollDirection = wheelData.Y switch
            {
                > 0 => MouseScrollDirection.ScrollUp,
                < 0 => MouseScrollDirection.ScrollDown,
                _ => MouseScrollDirection.None
            },
        };

        this.mouseReactable.Push(PushNotifications.MouseStateChangedId, this.mouseStateData);
    }

    /// <summary>
    /// Invoked when the mouse moves over the window.
    /// </summary>
    private void GLMouseMove_MouseMove(IMouse mouse, Vector2 position)
    {
        this.mouseStateData = this.mouseStateData with
        {
            X = (int)position.X,
            Y = (int)position.Y,
        };

        this.mouseReactable.Push(PushNotifications.MouseStateChangedId, this.mouseStateData);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.pushReactable.UnsubscribeAll();

            this.cachedStringProps.Clear();
            this.cachedIntProps.Clear();
            this.cachedBoolProps.Clear();

            if (this.glInputContext is not null)
            {
                this.glInputContext.Keyboards[0].KeyDown -= GLKeyboardInput_KeyDown;
                this.glInputContext.Keyboards[0].KeyUp -= GLKeyboardInput_KeyUp;
                this.glInputContext.Mice[0].MouseDown -= GLMouseInput_MouseDown;
                this.glInputContext.Mice[0].MouseUp -= GLMouseInput_MouseUp;
                this.glInputContext.Mice[0].MouseMove -= GLMouseMove_MouseMove;
                this.glInputContext.Mice[0].Scroll -= GLMouseInput_MouseScroll;
            }

            this.silkWindow.Load -= GLWindow_Load;
            this.silkWindow.Update -= GLWindow_Update;
            this.silkWindow.Render -= GLWindow_Render;
            this.silkWindow.Resize -= GLWindow_Resize;
            this.silkWindow.Closing -= GLWindow_Closing;

            this.statsWindowServiceService.Dispose();
            this.taskService.Dispose();

            this.glfw.Dispose();
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Sets up caching for the <see cref="Width"/> and <see cref="Height"/> properties.
    /// </summary>
    private void SetupWidthHeightPropCaches(uint width, uint height)
    {
        this.cachedUIntProps.Add(
            nameof(Width),
            new CachedValue<uint>(
                defaultValue: width,
                getterWhenNotCaching: () => (uint)this.silkWindow.Size.X,
                setterWhenNotCaching: value => this.silkWindow.Size = new Vector2D<int>((int)value, this.silkWindow.Size.Y)));

        this.cachedUIntProps.Add(
            nameof(Height),
            new CachedValue<uint>(
                defaultValue: height,
                getterWhenNotCaching: () => (uint)this.silkWindow.Size.Y,
                setterWhenNotCaching: value => this.silkWindow.Size = new Vector2D<int>(this.silkWindow.Size.X, (int)value)));
    }

    /// <summary>
    /// Sets up caching for all remaining window properties.
    /// </summary>
    private void SetupOtherPropCaches()
    {
        this.cachedStringProps.Add(
            nameof(Title),
            new CachedValue<string>(
                defaultValue: "Velaptor Application",
                getterWhenNotCaching: () => this.silkWindow.Title,
                setterWhenNotCaching: value => this.silkWindow.Title = value));

        var mainDisplay = this.systemDisplayService.MainDisplay;

        float ToDisplayScale(float value) => value * mainDisplay.HorizontalDPI /
                   (this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f);

        var halfWidth = ToDisplayScale(Width / 2f);
        var halfHeight = ToDisplayScale(Height / 2f);

        var defaultPosition = new Vector2(mainDisplay.Center.X - halfWidth, mainDisplay.Center.Y - halfHeight);

        this.cachedPosition = new CachedValue<Vector2>(
            defaultValue: defaultPosition,
            getterWhenNotCaching: () => new Vector2(this.silkWindow.Position.X, this.silkWindow.Position.Y),
            setterWhenNotCaching: value => this.silkWindow.Position = new Vector2D<int>((int)value.X, (int)value.Y));

        this.cachedIntProps.Add(
            nameof(UpdateFrequency),
            new CachedValue<int>(
                defaultValue: 60,
                getterWhenNotCaching: () => (int)this.silkWindow.UpdatesPerSecond,
                setterWhenNotCaching: value => this.silkWindow.UpdatesPerSecond = value));

        this.cachedBoolProps.Add(
            nameof(MouseCursorVisible),
            new CachedValue<bool>(
                defaultValue: true,
                getterWhenNotCaching: () => this.glInputContext?.Mice.Count > 0 &&
                                            this.glInputContext.Mice[0].Cursor.CursorMode == CursorMode.Normal,
                setterWhenNotCaching: value =>
                {
                    if (this.glInputContext is null)
                    {
                        return;
                    }

                    foreach (var mouse in this.glInputContext.Mice)
                    {
                        mouse.Cursor.CursorMode = value ? CursorMode.Normal : CursorMode.Hidden;
                    }
                }));

        this.cachedWindowState = new CachedValue<StateOfWindow>(
            defaultValue: StateOfWindow.Normal,
            getterWhenNotCaching: () =>
            {
                var silkState = this.silkWindow.WindowState;
                if (!Enum.IsDefined(typeof(WindowState), silkState))
                {
                    throw new InvalidEnumArgumentException(
                        $"this.silkWindow.{nameof(WindowState)}",
                        (int)silkState,
                        typeof(WindowState));
                }

                return (StateOfWindow)silkState;
            },
            setterWhenNotCaching: value =>
            {
                if (!Enum.IsDefined(typeof(StateOfWindow), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StateOfWindow));
                }

                this.silkWindow.WindowState = (WindowState)value;
            });

        this.cachedTypeOfBorder = new CachedValue<VelaptorWindowBorder>(
            defaultValue: VelaptorWindowBorder.Resizable,
            getterWhenNotCaching: () =>
            {
                var silkBorder = this.silkWindow.WindowBorder;
                if (!Enum.IsDefined(typeof(SilkWindowBorder), silkBorder))
                {
                    throw new InvalidEnumArgumentException(
                        $"this.silkWindow.{nameof(WindowBorder)}",
                        (int)silkBorder,
                        typeof(SilkWindowBorder));
                }

                return (VelaptorWindowBorder)silkBorder;
            },
            setterWhenNotCaching: value =>
            {
                if (!Enum.IsDefined(typeof(VelaptorWindowBorder), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(VelaptorWindowBorder));
                }

                this.silkWindow.WindowBorder = (SilkWindowBorder)value;
            });
    }
}
