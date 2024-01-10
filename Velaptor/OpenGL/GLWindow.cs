// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Exceptions;
using Factories;
using Input;
using Input.Exceptions;
using NativeInterop.GLFW;
using NativeInterop.OpenGL;
using ReactableData;
using Scene;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Velaptor.Services;
using SilkIWindow = Silk.NET.Windowing.IWindow;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkWindowBorder = Silk.NET.Windowing.WindowBorder;
using VelaptorIWindow = UI.IWindow;
using VelaptorMouseButton = Input.MouseButton;
using VelaptorWindowBorder = WindowBorder;

/// <summary>
/// An OpenGL window implementation to be used inside of the <see cref="Velaptor.UI.Window"/> class.
/// </summary>
internal sealed class GLWindow : VelaptorIWindow
{
    private readonly IAppService appService;
    private readonly INativeInputFactory nativeInputFactory;
    private readonly IGLInvoker gl;
    private readonly IGlfwInvoker glfw;
    private readonly ISystemDisplayService systemDisplayService;
    private readonly IPlatform platform;
    private readonly ITaskService taskService;
    private readonly IPushReactable pushReactable;
    private readonly IPushReactable<MouseStateData> mouseReactable;
    private readonly IPushReactable<KeyboardKeyStateData> keyboardReactable;
    private readonly IPushReactable<GL> glReactable;
    private readonly IPushReactable<ViewPortSizeData> viewPortReactable;
    private readonly IPushReactable<WindowSizeData> winSizeReactable;
    private readonly ITimerService timerService;
    private MouseStateData mouseStateData;
    private SilkIWindow glWindow = null!;
    private IInputContext glInputContext = null!;
    private bool isShuttingDown;
    private bool firstRenderInvoked;
    private bool isDisposed;
    private Action? afterUnloadAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLWindow"/> class.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <param name="appService">Provides application relates services.</param>
    /// <param name="silkWindow">The <see cref="Silk"/> specific <see cref="Silk.NET.Windowing.IWindow"/> object.</param>
    /// <param name="nativeInputFactory">Creates a native input object.</param>
    /// <param name="glInvoker">Invokes OpenGL functions.</param>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="systemDisplayService">Provides information about the system's displays.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <param name="taskService">Runs asynchronous tasks.</param>
    /// <param name="sceneManager">Manages scenes.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="timerService">Measures the time it takes to process the game loop.</param>
    public GLWindow(
        uint width,
        uint height,
        IAppService appService,
        SilkIWindow silkWindow,
        INativeInputFactory nativeInputFactory,
        IGLInvoker glInvoker,
        IGlfwInvoker glfwInvoker,
        ISystemDisplayService systemDisplayService,
        IPlatform platform,
        ITaskService taskService,
        ISceneManager sceneManager,
        IReactableFactory reactableFactory,
        ITimerService timerService)
    {
        ArgumentNullException.ThrowIfNull(appService);
        ArgumentNullException.ThrowIfNull(silkWindow);
        ArgumentNullException.ThrowIfNull(nativeInputFactory);
        ArgumentNullException.ThrowIfNull(glInvoker);
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(systemDisplayService);
        ArgumentNullException.ThrowIfNull(platform);
        ArgumentNullException.ThrowIfNull(taskService);
        ArgumentNullException.ThrowIfNull(sceneManager);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(timerService);

        this.appService = appService;
        this.glWindow = silkWindow;
        this.nativeInputFactory = nativeInputFactory;
        this.gl = glInvoker;
        this.glfw = glfwInvoker;
        this.systemDisplayService = systemDisplayService;
        this.platform = platform;
        this.taskService = taskService;
        SceneManager = sceneManager;

        this.pushReactable = reactableFactory.CreateNoDataPushReactable();
        this.mouseReactable = reactableFactory.CreateMouseReactable();
        this.keyboardReactable = reactableFactory.CreateKeyboardReactable();
        this.glReactable = reactableFactory.CreateGLReactable();
        this.viewPortReactable = reactableFactory.CreateViewPortReactable();
        this.winSizeReactable = reactableFactory.CreateWindowSizeReactable();
        this.timerService = timerService;

        this.mouseStateData = default;

        SetupWidthHeightPropCaches(width <= 0u ? 1u : width, height <= 0u ? 1u : height);
        SetupOtherPropCaches();
    }

    /// <inheritdoc/>
    public string Title
    {
        get => CachedStringProps[nameof(Title)].GetValue();
        set => CachedStringProps[nameof(Title)].SetValue(value);
    }

    /// <inheritdoc/>
    public Vector2 Position
    {
        get => CachedPosition.GetValue();
        set => CachedPosition.SetValue(value);
    }

    /// <inheritdoc/>
    public uint Width
    {
        get => CachedUIntProps[nameof(Width)].GetValue();
        set => CachedUIntProps[nameof(Width)].SetValue(value);
    }

    /// <inheritdoc/>
    public uint Height
    {
        get => CachedUIntProps[nameof(Height)].GetValue();
        set => CachedUIntProps[nameof(Height)].SetValue(value);
    }

    /// <inheritdoc/>
    public bool AutoClearBuffer { get; set; } = true;

    /// <inheritdoc/>
    public bool MouseCursorVisible
    {
        get => CachedBoolProps[nameof(MouseCursorVisible)].GetValue();
        set => CachedBoolProps[nameof(MouseCursorVisible)].SetValue(value);
    }

    /// <inheritdoc/>
    public StateOfWindow WindowState
    {
        get => CachedWindowState.GetValue();
        set => CachedWindowState.SetValue(value);
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
        get => CachedTypeOfBorder.GetValue();
        set => CachedTypeOfBorder.SetValue(value);
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
        get => CachedIntProps[nameof(UpdateFrequency)].GetValue();
        set => CachedIntProps[nameof(UpdateFrequency)].SetValue(value);
    }

    /// <inheritdoc/>
    public bool Initialized { get; private set; }

    /// <summary>
    /// Gets the list of caches for <see langword="string"/> properties.
    /// </summary>
    public Dictionary<string, CachedValue<string>> CachedStringProps { get; } = new ();

    /// <summary>
    /// Gets the list of caches for <see langword="int"/> properties.
    /// </summary>
    public Dictionary<string, CachedValue<int>> CachedIntProps { get; } = new ();

    /// <summary>
    /// Gets the list of caches for <see langword="uint"/> properties.
    /// </summary>
    public Dictionary<string, CachedValue<uint>> CachedUIntProps { get; } = new ();

    /// <summary>
    /// Gets the list of caches for <see langword="bool"/> properties.
    /// </summary>
    public Dictionary<string, CachedValue<bool>> CachedBoolProps { get; } = new ();

    /// <summary>
    /// Gets the cache for the <see cref="WindowState"/> property.
    /// </summary>
    public CachedValue<StateOfWindow> CachedWindowState { get; private set; } = null!;

    /// <summary>
    /// Gets the cache for the <see cref="TypeOfBorder"/> property.
    /// </summary>
    public CachedValue<VelaptorWindowBorder> CachedTypeOfBorder { get; private set; } = null!;

    /// <summary>
    /// Gets the cache for the <see cref="Position"/> property.
    /// </summary>
    public CachedValue<Vector2> CachedPosition { get; private set; } = null!;

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
            TaskContinuationOptions.ExecuteSynchronously, // Execute the continuation on the same thread as the show task
            TaskScheduler.Default);
    }

    /// <inheritdoc/>
    public void Close() => this.glWindow.Close();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Invoked when an OpenGL error occurs.
    /// </summary>
    private static void GL_GLError(object? sender, GLErrorEventArgs e) => throw new GLException(e.ErrorMessage);

    /// <summary>
    /// Runs the OpenGL window.
    /// </summary>
    private void RunGLWindow()
    {
        this.glWindow.Run();

        /*NOTE:
         * Only dispose of the window here and not in the Dispose() method!!
         *
         * This is because the line of code below will not be executed until the Window.Run() method
         * has finished executing.  This happens once the window is closed.
         *
         * If you dispose of the window in the Dispose() method before the Run() method is finished
         * then the application will crash.
         */
        this.glWindow.Dispose();
    }

    /// <summary>
    /// Initializes window related setup before the <see cref="Silk.NET.Windowing.IWindow"/>.<see cref="IView.Load"/>
    /// event is fired.
    /// </summary>
    private void PreInit()
    {
        this.glWindow.UpdatesPerSecond = 60;
        this.glWindow.Load += GLWindow_Load;
        this.glWindow.Closing += GLWindow_Closing;
        this.glWindow.Resize += GLWindow_Resize;
        this.glWindow.Update += GLWindow_Update;
        this.glWindow.Render += GLWindow_Render;
    }

    /// <summary>
    /// Initializes window related setup after the <see cref="Silk.NET.Windowing.IWindow"/>.<see cref="IView.Load"/>
    /// event is fired.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <exception cref="NoKeyboardException">Thrown if no keyboard could be created.</exception>
    /// <exception cref="NoMouseException">Thrown if no mouse could be created.</exception>
    private void Init(uint width, uint height)
    {
        this.glReactable.Push(this.glWindow.CreateOpenGL(), PushNotifications.GLContextCreatedId);
        this.glReactable.Unsubscribe(PushNotifications.GLContextCreatedId);

        this.glWindow.Size = new Vector2D<int>((int)width, (int)height);
        this.glInputContext = this.nativeInputFactory.CreateInput();

        if (this.glInputContext.Keyboards.Count <= 0)
        {
            throw new NoKeyboardException("Input Exception: No connected keyboards available.");
        }

        this.glInputContext.Keyboards[0].KeyDown += GLKeyboardInput_KeyDown;
        this.glInputContext.Keyboards[0].KeyUp += GLKeyboardInput_KeyUp;

        if (this.glInputContext.Mice.Count <= 0)
        {
            throw new NoMouseException("Input Exception: No connected mice available.");
        }

        this.glInputContext.Mice[0].MouseDown += GLMouseInput_MouseDown;
        this.glInputContext.Mice[0].MouseUp += GLMouseInput_MouseUp;
        this.glInputContext.Mice[0].MouseMove += GLMouseMove_MouseMove;
        this.glInputContext.Mice[0].Scroll += GLMouseInput_MouseScroll;

        this.appService.Init();
    }

    /// <summary>
    /// Invokes the <see cref="Initialize"/> action property.
    /// </summary>
    private void GLWindow_Load()
    {
        // OpenGL is ready to take function calls after this Init() call has executed
        Init(Width, Height);

        this.gl.SetupErrorCallback();
        this.gl.Enable(GLEnableCap.DebugOutput);
        this.gl.Enable(GLEnableCap.DebugOutputSynchronous);

        this.gl.GLError += GL_GLError;

        CachedStringProps.Values.ToList().ForEach(i => i.IsCaching = false);
        CachedBoolProps.Values.ToList().ForEach(i => i.IsCaching = false);
        CachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);
        CachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

        CachedPosition.IsCaching = false;
        CachedWindowState.IsCaching = false;
        CachedTypeOfBorder.IsCaching = false;

        Initialize?.Invoke();

        /* Send a push notification to all subscribers that OpenGL is initialized.
         * The context of initialized here is that the OpenGL context is set
         * and the related GLFW window has been created and is ready to go.
         */

        this.pushReactable.Push(PushNotifications.GLInitializedId);
        this.pushReactable.Unsubscribe(PushNotifications.GLInitializedId);

        Initialized = true;
    }

    /// <summary>
    /// Invoked when the window is in the process of closing and invokes the <see cref="Uninitialize"/> action.
    /// </summary>
    private void GLWindow_Closing()
    {
        this.isShuttingDown = true;

        SceneManager.UnloadContent();
        Uninitialize?.Invoke();

        /* NOTE:
         * Pushing this notification is very very important.  The reason is because
         * currently in this method, the GL context still exists.  After leaving this method,
         * the GL context will be destroyed.  Any further disposal attempts to textures
         * will fail due to the GL context being destroyed.  Sending this push notification
         * will trigger subscriptions in the texture cache which in turn will send
         * disposal notifications to all textures.
         *
         * Other types that depend on this shutdown process occurring before the GL context
         * is destroyed are the shaders and gpu buffers.
         */
        this.pushReactable.Push(PushNotifications.SystemShuttingDownId);

        this.afterUnloadAction?.Invoke();
    }

    /// <summary>
    /// Invoked every time the native window size changes and invokes the
    /// <see cref="VelaptorIWindow.WinResize"/> event.
    /// </summary>
    private void GLWindow_Resize(Vector2D<int> obj)
    {
        var width = (uint)obj.X;
        var height = (uint)obj.Y;

        // Updates the viewport so it is the same size as the window
        this.gl.Viewport(0, 0, width, height);
        var size = new SizeU { Width = width, Height = height };
        WinResize?.Invoke(size);

        this.viewPortReactable.Push(new ViewPortSizeData { Width = width, Height = height }, PushNotifications.ViewPortSizeChangedId);
        this.winSizeReactable.Push(new WindowSizeData { Width = width, Height = height }, PushNotifications.WindowSizeChangedId);
    }

    /// <summary>
    /// Invoked once per frame and invokes the <see cref="Update"/> action.
    /// </summary>
    /// <param name="time">The amount of time that has passed for the current frame.</param>
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

        this.mouseStateData = this.mouseStateData with
        {
            ScrollDirection = MouseScrollDirection.None,
            ScrollWheelValue = 0,
        };

        this.mouseReactable.Push(this.mouseStateData, PushNotifications.MouseStateChangedId);
    }

    /// <summary>
    /// Invoked once per frame and invokes the <see cref="Draw"/> action.
    /// </summary>
    /// <param name="time">The amount of time that has passed for the current frame.</param>
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

        if (AutoClearBuffer)
        {
            this.gl.Clear(GLClearBufferMask.ColorBufferBit);
        }

        Draw?.Invoke(frameTime);

        this.glWindow.SwapBuffers();

        this.timerService.Stop();
        Fps = 1000f / this.timerService.MillisecondsPassed;
        this.timerService.Reset();
    }

    /// <summary>
    /// Invoked when any keyboard input key transitions from the up position to the down position.
    /// </summary>
    /// <param name="keyboard">Manages keyboard input.</param>
    /// <param name="key">The key that was pushed down.</param>
    /// <param name="arg3">Additional argument from OpenGL.</param>
    private void GLKeyboardInput_KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        var keyStateData = new KeyboardKeyStateData { Key = (KeyCode)key, IsDown = true };

        this.keyboardReactable.Push(keyStateData, PushNotifications.KeyboardStateChangedId);
    }

    /// <summary>
    /// Invoked when any keyboard input key transitions from the down position to the up position.
    /// </summary>
    /// <param name="keyboard">The system keyboard input.</param>
    /// <param name="key">The key that was released.</param>
    /// <param name="arg3">Additional argument from OpenGL.</param>
    private void GLKeyboardInput_KeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        var keyStateData = new KeyboardKeyStateData { Key = (KeyCode)key, IsDown = false };

        this.keyboardReactable.Push(keyStateData, PushNotifications.KeyboardStateChangedId);
    }

    /// <summary>
    /// Invoked when any of the mouse buttons are in the down position over the window.
    /// </summary>
    /// <param name="mouse">The system mouse object.</param>
    /// <param name="button">The button that was pushed down.</param>
    private void GLMouseInput_MouseDown(IMouse mouse, SilkMouseButton button)
    {
        this.mouseStateData = this.mouseStateData with
        {
            Button = (VelaptorMouseButton)button,
            ButtonIsDown = true,
        };

        this.mouseReactable.Push(this.mouseStateData, PushNotifications.MouseStateChangedId);
    }

    /// <summary>
    /// Invoked when any of the mouse buttons are released from the down position into the up position over the window.
    /// </summary>
    /// <param name="mouse">The system mouse object.</param>
    /// <param name="button">The button that was pushed down.</param>
    private void GLMouseInput_MouseUp(IMouse mouse, SilkMouseButton button)
    {
        this.mouseStateData = this.mouseStateData with
        {
            Button = (VelaptorMouseButton)button,
            ButtonIsDown = false,
        };

        this.mouseReactable.Push(this.mouseStateData, PushNotifications.MouseStateChangedId);
    }

    /// <summary>
    /// Invoked when there is mouse scroll wheel input.
    /// </summary>
    /// <param name="mouse">The system mouse object.</param>
    /// <param name="wheelData">Positional data about the mouse scroll wheel.</param>
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

        this.mouseReactable.Push(this.mouseStateData, PushNotifications.MouseStateChangedId);
    }

    /// <summary>
    /// Invoked when the mouse moves over the window.
    /// </summary>
    /// <param name="mouse">The system mouse object.</param>
    /// <param name="position">The position of the mouse input.</param>
    private void GLMouseMove_MouseMove(IMouse mouse, Vector2 position)
    {
        this.mouseStateData = this.mouseStateData with
        {
            X = (int)position.X,
            Y = (int)position.Y,
        };

        this.mouseReactable.Push(this.mouseStateData, PushNotifications.MouseStateChangedId);
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.pushReactable.UnsubscribeAll();

            CachedStringProps.Clear();
            CachedIntProps.Clear();
            CachedBoolProps.Clear();

            this.gl.GLError -= GL_GLError;

            this.glInputContext.Keyboards[0].KeyDown -= GLKeyboardInput_KeyDown;
            this.glInputContext.Keyboards[0].KeyUp -= GLKeyboardInput_KeyUp;
            this.glInputContext.Mice[0].MouseDown -= GLMouseInput_MouseDown;
            this.glInputContext.Mice[0].MouseUp -= GLMouseInput_MouseUp;
            this.glInputContext.Mice[0].MouseMove -= GLMouseMove_MouseMove;
            this.glInputContext.Mice[0].Scroll -= GLMouseInput_MouseScroll;

            this.glWindow.Load -= GLWindow_Load;
            this.glWindow.Update -= GLWindow_Update;
            this.glWindow.Render -= GLWindow_Render;
            this.glWindow.Resize -= GLWindow_Resize;
            this.glWindow.Closing -= GLWindow_Closing;

            this.taskService.Dispose();

            this.gl.Dispose();
            this.glfw.Dispose();
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Sets up caching for the <see cref="Width"/> and <see cref="Height"/> properties.
    /// </summary>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    private void SetupWidthHeightPropCaches(uint width, uint height)
    {
        CachedUIntProps.Add(
            nameof(Width), // key
            new CachedValue<uint>( // value
                defaultValue: width,
                getterWhenNotCaching: () => (uint)this.glWindow.Size.X,
                setterWhenNotCaching: value =>
                {
                    this.glWindow.Size = new Vector2D<int>((int)value, this.glWindow.Size.Y);
                }));

        CachedUIntProps.Add(
            nameof(Height), // key
            new CachedValue<uint>( // value
                defaultValue: height,
                getterWhenNotCaching: () => (uint)this.glWindow.Size.Y,
                setterWhenNotCaching: value =>
                {
                    this.glWindow.Size = new Vector2D<int>(this.glWindow.Size.X, (int)value);
                }));
    }

    /// <summary>
    /// Setup all of the caching for the properties that need caching.
    /// </summary>
    private void SetupOtherPropCaches()
    {
        CachedStringProps.Add(
            nameof(Title), // key
            new CachedValue<string>( // value
                defaultValue: "Velaptor Application",
                getterWhenNotCaching: () => this.glWindow.Title,
                setterWhenNotCaching: value =>
                {
                    this.glWindow.Title = value;
                }));

        var mainDisplay = this.systemDisplayService.MainDisplay;

        float ToDisplayScale(float value)
        {
            return value * mainDisplay.HorizontalDPI /
                   (this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f);
        }

        var halfWidth = ToDisplayScale(Width / 2f);
        var halfHeight = ToDisplayScale(Height / 2f);

        // Set the default position to be in the center of the display
        var defaultPosition = new Vector2(mainDisplay.Center.X - halfWidth, mainDisplay.Center.Y - halfHeight);

        CachedPosition = new CachedValue<Vector2>(
            defaultValue: defaultPosition,
            getterWhenNotCaching: () => new Vector2(this.glWindow.Position.X, this.glWindow.Position.Y),
            setterWhenNotCaching: value =>
            {
                this.glWindow.Position = new Vector2D<int>((int)value.X, (int)value.Y);
            });

        CachedIntProps.Add(
            nameof(UpdateFrequency), // key
            new CachedValue<int>( // value
                defaultValue: 60,
                getterWhenNotCaching: () => (int)this.glWindow.UpdatesPerSecond,
                setterWhenNotCaching: value =>
                {
                    this.glWindow.UpdatesPerSecond = value;
                }));

        CachedBoolProps.Add(
            nameof(MouseCursorVisible), // key
            new CachedValue<bool>( // value
                defaultValue: true,
                getterWhenNotCaching: () => this.glInputContext.Mice.Count > 0 &&
                                            this.glInputContext.Mice[0].Cursor.CursorMode == CursorMode.Normal,
                setterWhenNotCaching: value =>
                {
                    var cursorMode = value ? CursorMode.Normal : CursorMode.Hidden;
                    this.glInputContext.Mice[0].Cursor.CursorMode = cursorMode;
                }));

        CachedWindowState = new CachedValue<StateOfWindow>(
            defaultValue: StateOfWindow.Normal,
            getterWhenNotCaching: () =>
            {
                const string argName = $"this.{nameof(this.glWindow)}.{nameof(this.glWindow.WindowState)}";

                return this.glWindow.WindowState switch
                {
                    Silk.NET.Windowing.WindowState.Normal => StateOfWindow.Normal,
                    Silk.NET.Windowing.WindowState.Minimized => StateOfWindow.Minimized,
                    Silk.NET.Windowing.WindowState.Maximized => StateOfWindow.Maximized,
                    Silk.NET.Windowing.WindowState.Fullscreen => StateOfWindow.FullScreen,
                    _ => throw new InvalidEnumArgumentException(
                        argName,
                        (int)this.glWindow.WindowState,
                        typeof(WindowState)),
                };
            },
            setterWhenNotCaching: value =>
            {
                this.glWindow.WindowState = value switch
                {
                    StateOfWindow.Normal => Silk.NET.Windowing.WindowState.Normal,
                    StateOfWindow.Minimized => Silk.NET.Windowing.WindowState.Minimized,
                    StateOfWindow.Maximized => Silk.NET.Windowing.WindowState.Maximized,
                    StateOfWindow.FullScreen => Silk.NET.Windowing.WindowState.Fullscreen,
                    _ => throw new InvalidEnumArgumentException(
                        nameof(value),
                        (int)value,
                        typeof(StateOfWindow)),
                };
            });

        CachedTypeOfBorder = new CachedValue<VelaptorWindowBorder>(
            defaultValue: VelaptorWindowBorder.Resizable,
            getterWhenNotCaching: () =>
            {
                const string argName = $"this.{nameof(this.glWindow)}.{nameof(this.glWindow.WindowBorder)}";

                return this.glWindow.WindowBorder switch
                {
                    SilkWindowBorder.Fixed => VelaptorWindowBorder.Fixed,
                    SilkWindowBorder.Hidden => VelaptorWindowBorder.Hidden,
                    SilkWindowBorder.Resizable => VelaptorWindowBorder.Resizable,
                    _ => throw new InvalidEnumArgumentException(
                        argName,
                        (int)this.glWindow.WindowBorder,
                        typeof(WindowBorder)),
                };
            },
            setterWhenNotCaching: value =>
            {
                this.glWindow.WindowBorder = value switch
                {
                    VelaptorWindowBorder.Fixed => SilkWindowBorder.Fixed,
                    VelaptorWindowBorder.Hidden => SilkWindowBorder.Hidden,
                    VelaptorWindowBorder.Resizable => SilkWindowBorder.Resizable,
                    _ => throw new InvalidEnumArgumentException(
                        nameof(value),
                        (int)value,
                        typeof(WindowBorder)),
                };
            });
    }
}
