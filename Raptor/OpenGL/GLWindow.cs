// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using Raptor.Content;
    using Raptor.Input;
    using Raptor.NativeInterop;
    using Raptor.Observables;
    using Raptor.Services;
    using Raptor.UI;
    using GLMouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
    using GLWindowState = OpenTK.Windowing.Common.WindowState;
    using RaptorMouseButton = Raptor.Input.MouseButton;
    using SysVector2 = System.Numerics.Vector2;
#pragma warning restore IDE0001 // Name can be simplified

    /// <summary>
    /// An OpenGL window implementation to be used inside of the <see cref="Window"/> class.
    /// </summary>
    internal sealed class GLWindow : IWindow
    {
        private const string NullParamExceptionMessage = "The parameter must not be null.";
        private readonly IGLInvoker gl;
        private readonly ISystemMonitorService systemMonitorService;
        private readonly IGameWindowFacade windowFacade;
        private readonly IPlatform platform;
        private readonly ITaskService taskService;
        private readonly IKeyboardInput<KeyCode, KeyboardState> keyboard;
        private readonly IMouseInput<RaptorMouseButton, MouseState> mouse;
        private readonly OpenGLObservable glObservable;
        private DebugProc? debugProc;
        private bool isShuttingDown;
        private bool isDiposed;
        private bool firstRenderInvoked;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindow"/> class.
        /// </summary>
        /// <param name="glInvoker">Invokes OpenGL functions.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="systemMonitorService">Manages the systems monitors/screens.</param>
        /// <param name="windowFacade">The internal OpenGL window facade.</param>
        /// <param name="platform">Information about the platform that is running the application.</param>
        /// <param name="taskService">Runs asynchronous tasks.</param>
        /// <param name="keyboard">Provides keyboard input.</param>
        /// <param name="mouse">Provides mouse input.</param>
        /// <param name="contentLoader">Loads various kinds of content.</param>
        /// <param name="glObservable">Provides push notifications to OpenGL related events.</param>
        public GLWindow(
            int width,
            int height,
            IGLInvoker glInvoker,
            ISystemMonitorService systemMonitorService,
            IGameWindowFacade windowFacade,
            IPlatform platform,
            ITaskService taskService,
            IKeyboardInput<KeyCode, KeyboardState> keyboard,
            IMouseInput<RaptorMouseButton, MouseState> mouse,
            IContentLoader contentLoader,
            OpenGLObservable glObservable)
        {
            if (glInvoker is null)
            {
                throw new ArgumentNullException(nameof(glInvoker), NullParamExceptionMessage);
            }

            if (systemMonitorService is null)
            {
                throw new ArgumentNullException(nameof(systemMonitorService), NullParamExceptionMessage);
            }

            if (windowFacade is null)
            {
                throw new ArgumentNullException(nameof(windowFacade), NullParamExceptionMessage);
            }

            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform), NullParamExceptionMessage);
            }

            if (taskService is null)
            {
                throw new ArgumentNullException(nameof(taskService), NullParamExceptionMessage);
            }

            if (keyboard is null)
            {
                throw new ArgumentNullException(nameof(keyboard), NullParamExceptionMessage);
            }

            if (mouse is null)
            {
                throw new ArgumentNullException(nameof(mouse), NullParamExceptionMessage);
            }

            if (contentLoader is null)
            {
                throw new ArgumentNullException(nameof(contentLoader), NullParamExceptionMessage);
            }

            this.gl = glInvoker;
            this.systemMonitorService = systemMonitorService;
            this.windowFacade = windowFacade;
            this.platform = platform;
            this.taskService = taskService;
            this.keyboard = keyboard;
            this.mouse = mouse;
            this.glObservable = glObservable;

            ContentLoader = contentLoader;

            SetupWidthHeightPropCaches(width <= 0 ? 1 : width, height <= 0 ? 1 : height);
            SetupOtherPropCaches();
        }

        /// <inheritdoc/>
        public string Title
        {
            get => CachedStringProps[nameof(Title)].GetValue();
            set => CachedStringProps[nameof(Title)].SetValue(value);
        }

        /// <inheritdoc/>
        public SysVector2 Position
        {
            get
            {
                if (CachedPosition is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                }

                return CachedPosition.GetValue();
            }
            set
            {
                if (CachedPosition is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                }

                CachedPosition.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public int Width
        {
            get => CachedIntProps[nameof(Width)].GetValue();
            set => CachedIntProps[nameof(Width)].SetValue(value);
        }

        /// <inheritdoc/>
        public int Height
        {
            get => CachedIntProps[nameof(Height)].GetValue();
            set => CachedIntProps[nameof(Height)].SetValue(value);
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
            get
            {
                if (CachedWindowState is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                }

                return CachedWindowState.GetValue();
            }
            set
            {
                if (CachedWindowState is null)
                {
                    throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                }

                CachedWindowState.SetValue(value);
            }
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
        public Action? WinResize { get; set; }

        /// <inheritdoc/>
        public BorderType TypeOfBorder
        {
            get
            {
                if (CachedTypeOfBorder is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                }

                return CachedTypeOfBorder.GetValue();
            }
            set
            {
                if (CachedTypeOfBorder is null)
                {
                    throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                }

                CachedTypeOfBorder.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public IContentLoader ContentLoader { get; set; }

        /// <inheritdoc/>
        public int UpdateFrequency
        {
            get => CachedIntProps[nameof(UpdateFrequency)].GetValue();
            set => CachedIntProps[nameof(UpdateFrequency)].SetValue(value);
        }

        /// <inheritdoc/>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Gets the list of caches for <see langword=""="string"/> properties.
        /// </summary>
        public Dictionary<string, CachedValue<string>> CachedStringProps { get; } = new Dictionary<string, CachedValue<string>>();

        /// <summary>
        /// Gets the list of caches for <see langword=""="int"/> properties.
        /// </summary>
        public Dictionary<string, CachedValue<int>> CachedIntProps { get; } = new Dictionary<string, CachedValue<int>>();

        /// <summary>
        /// Gets the list of caches for <see langword=""="bool"/> properties.
        /// </summary>
        public Dictionary<string, CachedValue<bool>> CachedBoolProps { get; } = new Dictionary<string, CachedValue<bool>>();

        /// <summary>
        /// Gets the cache for the <see cref="WindowState"/> property.
        /// </summary>
        public CachedValue<StateOfWindow>? CachedWindowState { get; private set; }

        /// <summary>
        /// Gets the cache for the <see cref="TypeOfBorder"/> property.
        /// </summary>
        public CachedValue<BorderType>? CachedTypeOfBorder { get; private set; }

        /// <summary>
        /// Gets the cache for the <see cref="Position"/> property.
        /// </summary>
        public CachedValue<SysVector2>? CachedPosition { get; private set; }

        /// <inheritdoc/>
        public void Show()
        {
            SetupWindow();

            this.windowFacade?.Show();
        }

        /// <inheritdoc/>
        public async Task ShowAsync(Action dispose)
        {
            this.taskService.SetAction(
                () =>
                {
                    SetupWindow();
                    this.windowFacade?.Show();
                });

            this.taskService.Start();

            await this.taskService.ContinueWith(
                (t) =>
                {
                    dispose();
                },
                TaskContinuationOptions.ExecuteSynchronously, // Execute the continuation on the same thread as the show task
                TaskScheduler.Default);
        }

        /// <inheritdoc/>
        public void Close() => this.windowFacade.Close();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Maps the given OpenGL mouse button to a <see cref="Raptor.Input.MouseButton"/>.
        /// </summary>
        /// <param name="from">The OpenGL mouse button to map.</param>
        /// <returns>The mouse button.</returns>
        private static RaptorMouseButton MapMouseButton(GLMouseButton from)
        {
            var result = (RaptorMouseButton)123456789; // Invalid raptor mouse button to start

            switch (from)
            {
                case GLMouseButton.Left: // Same as Button1
                    result = RaptorMouseButton.LeftButton;
                    break;
                case GLMouseButton.Middle: // Same as Button3
                case GLMouseButton.Last:
                    result = RaptorMouseButton.MiddleButton;
                    break;
                case GLMouseButton.Right: // Same as Button2
                    result = RaptorMouseButton.RightButton;
                    break;
            }

            switch (from)
            {
                case GLMouseButton.Button1: // Same as LeftButton
                    result = RaptorMouseButton.LeftButton;
                    break;
                case GLMouseButton.Button2: // Same as RightButton
                    result = RaptorMouseButton.RightButton;
                    break;
                case GLMouseButton.Button3: // Same as MiddleButton
                    result = RaptorMouseButton.MiddleButton;
                    break;
                case GLMouseButton.Button4:
                case GLMouseButton.Button5:
                case GLMouseButton.Button6:
                case GLMouseButton.Button7:
                case GLMouseButton.Button8:
                    throw new Exception("Unrecognized OpenGL mouse button.");
            }

            return result;
        }

        /// <summary>
        /// Occurs when a keyboard key is pressed into the down position.
        /// </summary>
        /// <param name="e">The keyboard info of the event.</param>
        private void GameWindow_KeyDown(KeyboardKeyEventArgs e)
        {
            var mappedKey = (KeyCode)e.Key;

            this.keyboard.SetState(mappedKey, true);
        }

        /// <summary>
        /// Occurs when a keyboard key is released to the up position.
        /// </summary>
        /// <param name="e">The keyboard info of the event.</param>
        private void GameWindow_KeyUp(KeyboardKeyEventArgs e)
        {
            var mappedKey = (KeyCode)e.Key;

            this.keyboard.SetState(mappedKey, false);
        }

        /// <summary>
        /// Occurs every time any mouse is pressed into the down position.
        /// </summary>
        /// <param name="e">Information about the mouse event.</param>
        private void GameWindow_MouseDown(MouseButtonEventArgs e)
        {
            var mappedButton = MapMouseButton(e.Button);
            this.mouse.SetState(mappedButton, true);
        }

        /// <summary>
        /// Occurs every time any mouse is released into the up position.
        /// </summary>
        /// <param name="e">Information about the mouse event.</param>
        private void GameWindow_MouseUp(MouseButtonEventArgs e)
        {
            var mappedButton = MapMouseButton(e.Button);
            this.mouse.SetState(mappedButton, false);
        }

        /// <summary>
        /// Occurs every time the mouse is moved over the window.
        /// </summary>
        /// <param name="e">Information about the mouse move event.</param>
        private void GameWindow_MouseMove(MouseMoveEventArgs e)
        {
            this.mouse.SetXPos((int)e.X);
            this.mouse.SetYPos((int)e.Y);
        }

        /// <summary>
        /// Invokes the <see cref="Initialize"/> action property.
        /// </summary>
        private void GameWindow_Load() => Initialize?.Invoke();

        /// <summary>
        /// Starts the unload process.
        /// </summary>
        private void GameWindow_Closed() => GameWindow_Unload();

        /// <summary>
        /// Invokes the <see cref="Update"/> action property.
        /// </summary>
        /// <param name="deltaTime">The frame event args.</param>
        private void GameWindow_UpdateFrame(FrameEventArgs deltaTime)
        {
            if (this.isShuttingDown)
            {
                return;
            }

            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(deltaTime.Time * 1000.0)),
            };

            Update?.Invoke(frameTime);
        }

        /// <summary>
        /// Invokes the <see cref="Draw"/> action property.
        /// </summary>
        /// <param name="deltaTime">The frame event args.</param>
        private void GameWindow_RenderFrame(FrameEventArgs deltaTime)
        {
            if (this.firstRenderInvoked is false)
            {
                GameWindow_UpdateFrame(deltaTime);
                this.firstRenderInvoked = true;
            }

            if (this.isShuttingDown)
            {
                return;
            }

            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(deltaTime.Time * 1000.0)),
            };

            if (AutoClearBuffer)
            {
                this.gl.Clear(ClearBufferMask.ColorBufferBit);
            }

            Draw?.Invoke(frameTime);

            this.windowFacade.SwapBuffers();
        }

        /// <summary>
        /// Invokes the <see cref="WinResize"/> action property..
        /// </summary>
        /// <param name="currentSize">Resize event args.</param>
        private void GameWindow_Resize(ResizeEventArgs currentSize)
        {
            // Update the view port so it is the same size as the window
            this.gl.Viewport(0, 0, currentSize.Width, currentSize.Height);
            WinResize?.Invoke();
        }

        /// <summary>
        /// Sets the state of the window as shutting down and starts the uninitialize process.
        /// </summary>
        private void GameWindow_Unload()
        {
            this.isShuttingDown = true;
            Uninitialize?.Invoke();
        }

        /// <summary>
        /// Sets up the OpenGL window.
        /// </summary>
        private void SetupWindow()
        {
            this.windowFacade.Init(Width, Height);

            this.windowFacade.Load += GameWindow_Load;
            this.windowFacade.Unload += GameWindow_Unload;
            this.windowFacade.UpdateFrame += GameWindow_UpdateFrame;
            this.windowFacade.RenderFrame += GameWindow_RenderFrame;
            this.windowFacade.Resize += GameWindow_Resize;
            this.windowFacade.KeyDown += GameWindow_KeyDown;
            this.windowFacade.KeyUp += GameWindow_KeyUp;
            this.windowFacade.MouseDown += GameWindow_MouseDown;
            this.windowFacade.MouseUp += GameWindow_MouseUp;
            this.windowFacade.MouseMove += GameWindow_MouseMove;
            this.windowFacade.Closed += GameWindow_Closed;

            this.debugProc = DebugCallback;

            /*NOTE:
             * This is here to help prevent an issue with an obscure System.ExecutionException from occurring.
             * The garbage collector performs a collect on the delegate passed into GL.DebugMesageCallback()
             * without the native system knowing about it which causes this exception. The GC.KeepAlive()
             * method tells the garbage collector to not collect the delegate to prevent this from happening.
             */
            GC.KeepAlive(this.debugProc);

            this.gl.Enable(EnableCap.DebugOutput);
            this.gl.Enable(EnableCap.DebugOutputSynchronous);
            this.gl.DebugMessageCallback(this.debugProc, Marshal.StringToHGlobalAnsi(string.Empty));

            CachedStringProps.Values.ToList().ForEach(i => i.IsCaching = false);
            CachedBoolProps.Values.ToList().ForEach(i => i.IsCaching = false);
            CachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

            if (!(CachedPosition is null))
            {
                CachedPosition.IsCaching = false;
            }

            if (!(CachedWindowState is null))
            {
                CachedWindowState.IsCaching = false;
            }

            if (!(CachedTypeOfBorder is null))
            {
                CachedTypeOfBorder.IsCaching = false;
            }

            Initialized = true;

            // Send a push notification to all subscribers that OpenGL is initialized.
            // The context of initialized here is that the OpenGL context is set
            // and the related GLFW window has been created and is ready to go.
            this.glObservable.OnOpenGLInitialized();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to release managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDiposed)
            {
                if (disposing)
                {
                    this.taskService?.Dispose();
                    this.glObservable.Dispose();

                    CachedStringProps.Clear();
                    CachedIntProps.Clear();
                    CachedBoolProps.Clear();

                    this.windowFacade.Load -= GameWindow_Load;
                    this.windowFacade.Unload -= GameWindow_Unload;
                    this.windowFacade.UpdateFrame -= GameWindow_UpdateFrame;
                    this.windowFacade.RenderFrame -= GameWindow_RenderFrame;
                    this.windowFacade.Resize -= GameWindow_Resize;
                    this.windowFacade.KeyDown -= GameWindow_KeyDown;
                    this.windowFacade.KeyUp -= GameWindow_KeyUp;
                    this.windowFacade.MouseDown -= GameWindow_MouseDown;
                    this.windowFacade.MouseUp -= GameWindow_MouseUp;
                    this.windowFacade.MouseMove -= GameWindow_MouseMove;
                    this.windowFacade.Closed -= GameWindow_Closed;
                    this.windowFacade.Dispose();
                }

                this.isDiposed = true;
            }
        }

        /// <summary>
        /// Sets up caching for the <see cref="Width"/> and <see cref="Height"/> properties.
        /// </summary>
        /// <param name="width">The window width.</param>
        /// <param name="height">The window height.</param>
        private void SetupWidthHeightPropCaches(int width, int height)
        {
            CachedIntProps.Add(
                nameof(Width), // key
                new CachedValue<int>( // value
                    defaultValue: width,
                    getterWhenNotCaching: () =>
                    {
                        return this.windowFacade.Size.X;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        this.windowFacade.Size = new Vector2i(value, this.windowFacade.Size.Y);
                    }));

            CachedIntProps.Add(
                nameof(Height), // key
                new CachedValue<int>( // value
                    defaultValue: height,
                    getterWhenNotCaching: () =>
                    {
                        return this.windowFacade.Size.Y;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        this.windowFacade.Size = new Vector2i(this.windowFacade.Size.X, value);
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
                    defaultValue: "Raptor Application",
                    getterWhenNotCaching: () =>
                    {
                        return this.windowFacade.Title;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        this.windowFacade.Title = value;
                    }));

            SysVector2 defaultPosition = SysVector2.Zero;

            var mainMonitor = this.systemMonitorService.MainMonitor;

            float ToMonitorScale(float value)
            {
                return value * (mainMonitor is null ? 0 : mainMonitor.HorizontalDPI) /
                    (this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f);
            }

            var halfWidth = ToMonitorScale(Width / 2f);
            var halfHeight = ToMonitorScale(Height / 2f);

            if (!(mainMonitor is null))
            {
                defaultPosition = new SysVector2(mainMonitor.Center.X - halfWidth, mainMonitor.Center.Y - halfHeight);
            }

            CachedPosition = new CachedValue<SysVector2>(
                defaultValue: defaultPosition,
                getterWhenNotCaching: () =>
                {
                    return new SysVector2(this.windowFacade.Location.X, this.windowFacade.Location.Y);
                },
                setterWhenNotCaching: (value) =>
                {
                    this.windowFacade.Location = new Vector2i((int)value.X, (int)value.Y);
                });

            CachedIntProps.Add(
                nameof(UpdateFrequency), // key
                new CachedValue<int>( // value
                    defaultValue: 60,
                    getterWhenNotCaching: () =>
                    {
                        return (int)this.windowFacade.UpdateFrequency;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        this.windowFacade.UpdateFrequency = value;
                    }));

            CachedBoolProps.Add(
                nameof(MouseCursorVisible), // key
                new CachedValue<bool>( // value
                    defaultValue: true,
                    getterWhenNotCaching: () =>
                    {
                        return this.windowFacade.CursorVisible;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        this.windowFacade.CursorVisible = value;
                    }));

            CachedWindowState = new CachedValue<StateOfWindow>(
                defaultValue: StateOfWindow.Normal,
                getterWhenNotCaching: () =>
                {
                    return (StateOfWindow)this.windowFacade.WindowState;
                },
                setterWhenNotCaching: (value) =>
                {
                    this.windowFacade.WindowState = (GLWindowState)value;
                });

            CachedTypeOfBorder = new CachedValue<BorderType>(
                defaultValue: BorderType.Resizable,
                getterWhenNotCaching: () =>
                {
                    return (BorderType)this.windowFacade.WindowBorder;
                },
                setterWhenNotCaching: (value) =>
                {
                    this.windowFacade.WindowBorder = (WindowBorder)value;
                });
        }

        /// <summary>
        /// Invoked when an OpenGL error occurs.
        /// </summary>
        /// <param name="src">The source of the debug message.</param>
        /// <param name="type">The type of debug message.</param>
        /// <param name="id">The id of the debug message.</param>
        /// <param name="severity">The severity of debug message.</param>
        /// <param name="length">The length of this debug message.</param>
        /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
        /// <param name="userParam">A pointer to a user-specified parameter.</param>
        private void DebugCallback(DebugSource src, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            var errorMessage = Marshal.PtrToStringAnsi(message);

            errorMessage += $"\n\tSrc: {src}";
            errorMessage += $"\n\tType: {type}";
            errorMessage += $"\n\tID: {id}";
            errorMessage += $"\n\tSeverity: {severity}";
            errorMessage += $"\n\tLength: {length}";
            errorMessage += $"\n\tUser Param: {Marshal.PtrToStringAnsi(userParam)}";

            if (severity != DebugSeverity.DebugSeverityNotification)
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
