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
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using Raptor.Content;
    using Raptor.Input;
    using Raptor.NativeInterop;
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
        private readonly Dictionary<string, CachedValue<string>> cachedStringProps = new Dictionary<string, CachedValue<string>>();
        private readonly Dictionary<string, CachedValue<int>> cachedIntProps = new Dictionary<string, CachedValue<int>>();
        private readonly Dictionary<string, CachedValue<bool>> cachedBoolProps = new Dictionary<string, CachedValue<bool>>();
        private readonly CancellationTokenSource tokenSrc = new CancellationTokenSource();
        private readonly IGLInvoker gl;
        private readonly ISystemMonitorService systemMonitorService;
        private readonly IGameWindowFacade windowFacade;
        private readonly IPlatform platform;
        private CachedValue<StateOfWindow>? cachedWindowState;
        private CachedValue<BorderType>? cachedTypeOfBorder;
        private CachedValue<SysVector2>? cachedPosition;
        private DebugProc? debugProc;
        private bool isShuttingDown;
        private bool isDiposed;
        private bool firstRenderInvoked;
        private Task? showTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindow"/> class.
        /// </summary>
        /// <param name="glInvoker">Invokes OpenGL functions.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="systemMonitorService">Manages the systems monitors/screens.</param>
        /// <param name="windowFacade">The internal OpenGL window facade.</param>
        /// <param name="platform">Information about the platform that is running the application.</param>
        /// <param name="contentLoader">Loads various kinds of content.</param>
        public GLWindow(
            int width,
            int height,
            IGLInvoker glInvoker,
            ISystemMonitorService systemMonitorService,
            IGameWindowFacade windowFacade,
            IPlatform platform,
            IContentLoader contentLoader)
        {
            if (glInvoker is null)
            {
                throw new ArgumentNullException(nameof(glInvoker), "The parameter must not be null.");
            }

            if (systemMonitorService is null)
            {
                throw new ArgumentNullException(nameof(systemMonitorService), "The parameter must not be null.");
            }

            if (windowFacade is null)
            {
                throw new ArgumentNullException(nameof(windowFacade), "The parameter must not be null.");
            }

            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform), "The parameter must not be null.");
            }

            if (contentLoader is null)
            {
                throw new ArgumentNullException(nameof(contentLoader), "The parameter must not be null.");
            }

            this.gl = glInvoker;
            this.systemMonitorService = systemMonitorService;
            this.windowFacade = windowFacade;
            this.platform = platform;
            ContentLoader = contentLoader;

            SetupWidthHeightPropCaches(width <= 0 ? 1 : width, height <= 0 ? 1 : height);
            SetupOtherPropCaches();

            IGLInvoker.OpenGLInitialized += IGLInvoker_OpenGLInitialized;
        }

        /// <inheritdoc/>
        public string Title
        {
            get => this.cachedStringProps[nameof(Title)].GetValue();
            set => this.cachedStringProps[nameof(Title)].SetValue(value);
        }

        /// <inheritdoc/>
        public SysVector2 Position
        {
            get
            {
                if (this.cachedPosition is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                }

                return this.cachedPosition.GetValue();
            }
            set
            {
                if (this.cachedPosition is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                }

                this.cachedPosition.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public int Width
        {
            get => this.cachedIntProps[nameof(Width)].GetValue();
            set => this.cachedIntProps[nameof(Width)].SetValue(value);
        }

        /// <inheritdoc/>
        public int Height
        {
            get => this.cachedIntProps[nameof(Height)].GetValue();
            set => this.cachedIntProps[nameof(Height)].SetValue(value);
        }

        /// <inheritdoc/>
        public bool AutoClearBuffer { get; set; } = true;

        /// <inheritdoc/>
        public bool MouseCursorVisible
        {
            get => this.cachedBoolProps[nameof(MouseCursorVisible)].GetValue();
            set => this.cachedBoolProps[nameof(MouseCursorVisible)].SetValue(value);
        }

        /// <inheritdoc/>3
        public StateOfWindow WindowState
        {
            get
            {
                if (this.cachedWindowState is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                }

                return this.cachedWindowState.GetValue();
            }
            set
            {
                if (this.cachedWindowState is null)
                {
                    throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                }

                this.cachedWindowState.SetValue(value);
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
                if (this.cachedTypeOfBorder is null)
                {
                    throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                }

                return this.cachedTypeOfBorder.GetValue();
            }
            set
            {
                if (this.cachedTypeOfBorder is null)
                {
                    throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                }

                this.cachedTypeOfBorder.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public IContentLoader ContentLoader { get; set; }

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
            // If the window is showing asynchronously, exit
            if (this.showTask is null)
            {
                return;
            }

            SetupWindow();

            this.windowFacade?.Run();
        }

        /// <inheritdoc/>
        public async Task ShowAsync(Action dispose)
        {
            this.showTask = new Task(
                () =>
                {
                    SetupWindow();
                    this.windowFacade?.Run();
                }, this.tokenSrc.Token);

            this.showTask.Start();

            await this.showTask.ConfigureAwait(true);
            await this.showTask.ContinueWith(
                (t) =>
                {
                    dispose();
                },
                this.tokenSrc.Token,
                TaskContinuationOptions.ExecuteSynchronously, // Execute the continuation on the same thread as the show task
                TaskScheduler.Default).ConfigureAwait(true);
        }

        /// <inheritdoc/>
        public void Close()
        {
            this.windowFacade.Close();
        }

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
            switch (from)
            {
                case GLMouseButton.Button1: // Same as LeftButton
                    return RaptorMouseButton.LeftButton;
                case GLMouseButton.Button2: // Same as RightButton
                    return RaptorMouseButton.RightButton;
                case GLMouseButton.Button3: // Same as MiddleButton
                    return RaptorMouseButton.MiddleButton;
                case GLMouseButton.Last:
                    return RaptorMouseButton.None;
            }

            // By default, Button 1, 2 and 3 are fired for left, middle and right
            // This is here just in case the OpenTK implementation changes.
            switch (from)
            {
                case GLMouseButton.Left: // Same as Button1
                    return RaptorMouseButton.LeftButton;
                case GLMouseButton.Middle: // Same as Button3
                    return RaptorMouseButton.MiddleButton;
                case GLMouseButton.Right: // Same as Button2
                    return RaptorMouseButton.RightButton;
                case GLMouseButton.Button4:
                case GLMouseButton.Button5:
                case GLMouseButton.Button6:
                case GLMouseButton.Button7:
                case GLMouseButton.Button8:
                    return RaptorMouseButton.None;
            }

            return RaptorMouseButton.None;
        }

        /// <summary>
        /// Occurs when OpenGL has been initialized.
        /// </summary>
        private void IGLInvoker_OpenGLInitialized(object? sender, EventArgs e)
        {
            this.cachedStringProps.Values.ToList().ForEach(i => i.IsCaching = false);
            this.cachedBoolProps.Values.ToList().ForEach(i => i.IsCaching = false);
            this.cachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

            if (!(this.cachedPosition is null))
            {
                this.cachedPosition.IsCaching = false;
            }

            if (!(this.cachedWindowState is null))
            {
                this.cachedWindowState.IsCaching = false;
            }

            if (!(this.cachedTypeOfBorder is null))
            {
                this.cachedTypeOfBorder.IsCaching = false;
            }

            Initialized = true;
        }

        /// <summary>
        /// Occurs when a keyboard key is pressed into the down position.
        /// </summary>
        /// <param name="e">The keyboard info of the event.</param>
        private void GameWindow_KeyDown(KeyboardKeyEventArgs e)
        {
            var mappedKey = (KeyCode)e.Key;

            Keyboard.SetKeyState(mappedKey, true);
        }

        /// <summary>
        /// Occurs when a keyboard key is released to the up position.
        /// </summary>
        /// <param name="e">The keyboard info of the event.</param>
        private void GameWindow_KeyUp(KeyboardKeyEventArgs e)
        {
            var mappedKey = (KeyCode)e.Key;

            Keyboard.SetKeyState(mappedKey, false);
        }

        /// <summary>
        /// Occurs every time any mouse is pressed into the down position.
        /// </summary>
        /// <param name="e">Information about the mouse event.</param>
        private void GameWindow_MouseDown(MouseButtonEventArgs e)
        {
            var mappedButton = MapMouseButton(e.Button);
            Mouse.SetButtonState(mappedButton, true);
        }

        /// <summary>
        /// Occurs every time any mouse is released into the up position.
        /// </summary>
        /// <param name="e">Information about the mouse event.</param>
        private void GameWindow_MouseUp(MouseButtonEventArgs e)
        {
            var mappedButton = MapMouseButton(e.Button);
            Mouse.SetButtonState(mappedButton, false);
        }

        /// <summary>
        /// Occurs every time the mouse is moved over the window.
        /// </summary>
        /// <param name="e">Information about the mouse move event.</param>
        private void GameWindow_MouseMove(MouseMoveEventArgs e) => Mouse.SetPosition((int)e.X, (int)e.Y);

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

            // Set OpenGL as initialized.  Once the InternalGLWindow has been created,
            // that means OpenGL has been initialized by OpenTK itself.
            IGLInvoker.SetOpenGLAsInitialized();
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
                    this.tokenSrc.Dispose();
                    this.showTask?.Dispose();
                    this.cachedStringProps.Clear();
                    this.cachedIntProps.Clear();
                    this.cachedBoolProps.Clear();

                    IGLInvoker.OpenGLInitialized -= IGLInvoker_OpenGLInitialized;

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
        /// Sets up caching for the <see cref="Width"/> and <see cref="Height"/> properties.
        /// </summary>
        /// <param name="width">The window width.</param>
        /// <param name="height">The window height.</param>
        private void SetupWidthHeightPropCaches(int width, int height)
        {
            this.cachedIntProps.Add(
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

            this.cachedIntProps.Add(
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
            this.cachedStringProps.Add(
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

            this.cachedPosition = new CachedValue<SysVector2>(
                defaultValue: defaultPosition,
                getterWhenNotCaching: () =>
                {
                    return new SysVector2(this.windowFacade.Location.X, this.windowFacade.Location.Y);
                },
                setterWhenNotCaching: (value) =>
                {
                    this.windowFacade.Location = new Vector2i((int)value.X, (int)value.Y);
                });

            this.cachedIntProps.Add(
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

            this.cachedBoolProps.Add(
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

            this.cachedWindowState = new CachedValue<StateOfWindow>(
                defaultValue: StateOfWindow.Normal,
                getterWhenNotCaching: () =>
                {
                    return (StateOfWindow)this.windowFacade.WindowState;
                },
                setterWhenNotCaching: (value) =>
                {
                    this.windowFacade.WindowState = (GLWindowState)value;
                });

            this.cachedTypeOfBorder = new CachedValue<BorderType>(
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

            errorMessage += errorMessage;
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
