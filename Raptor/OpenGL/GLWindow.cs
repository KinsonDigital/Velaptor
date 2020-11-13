// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using Raptor.Content;
    using Raptor.Desktop;
    using Raptor.Factories;
    using Raptor.Input;
    using GLMouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
    using GLWindowState = OpenTK.Windowing.Common.WindowState;
    using RaptorMouseButton = Raptor.Input.MouseButton;
    using SysVector2 = System.Numerics.Vector2;

    /// <summary>
    /// An OpenGL window implementation to be used inside of the <see cref="Window"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class GLWindow : IWindow
    {
        private readonly Dictionary<string, CachedValue<string>> cachedStringProps = new Dictionary<string, CachedValue<string>>();
        private readonly Dictionary<string, CachedValue<int>> cachedIntProps = new Dictionary<string, CachedValue<int>>();
        private readonly Dictionary<string, CachedValue<bool>> cachedBoolProps = new Dictionary<string, CachedValue<bool>>();
        private readonly int cachedWindowWidth;
        private readonly int cachedWindowHeight;
        private GameWindowSettings? gameWinSettings;
        private NativeWindowSettings? nativeWinSettings;
        private CachedValue<StateOfWindow>? cachedWindowState;
        private CachedValue<BorderType>? cachedTypeOfBorder;
        private CachedValue<SysVector2>? cachedPosition;
        private InternalGLWindow? appWindow;
        private DebugProc? debugProc;
        private IGLInvoker? gl;
        private bool isShuttingDown;
        private bool isDiposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindow"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public GLWindow(int width, int height)
        {
            this.cachedWindowWidth = width;
            this.cachedWindowHeight = height;
            SetupPropertyCaches();
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
        public Action? Init { get; set; }

        /// <inheritdoc/>
        public Action<FrameTime>? Update { get; set; }

        /// <inheritdoc/>
        public Action<FrameTime>? Draw { get; set; }

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
        public IContentLoader? ContentLoader { get; set; }

        /// <inheritdoc/>
        public int UpdateFrequency
        {
            get => this.cachedIntProps[nameof(UpdateFrequency)].GetValue();
            set => this.cachedIntProps[nameof(UpdateFrequency)].SetValue(value);
        }

        /// <inheritdoc/>
        public void Show()
        {
            this.gameWinSettings = new GameWindowSettings();
            this.nativeWinSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(this.cachedWindowWidth, this.cachedWindowHeight),
                StartVisible = false,
            };

            this.appWindow = new InternalGLWindow(this.gameWinSettings, this.nativeWinSettings);

            TurnOffCaching();

            /*NOTE:
             * The IoC container get instance must be called after the
             * window has been called.  This is because an OpenGL context
             * must be created first before any GL calls can be made.
             */
            this.gl = IoC.Container.GetInstance<IGLInvoker>();

            this.appWindow.Load += GameWindow_Load;
            this.appWindow.UpdateFrame += GameWindow_UpdateFrame;
            this.appWindow.RenderFrame += GameWindow_RenderFrame;
            this.appWindow.Resize += GameWindow_Resize;
            this.appWindow.Unload += GameWindow_Unload;
            this.appWindow.KeyDown += GameWindow_KeyDown;
            this.appWindow.KeyUp += GameWindow_KeyUp;
            this.appWindow.MouseDown += GameWindow_MouseDown;
            this.appWindow.MouseUp += GameWindow_MouseUp;
            this.appWindow.MouseMove += GameWindow_MouseMove;

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

            ContentLoader = ContentLoaderFactory.CreateContentLoader();

            this.appWindow.Run();
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (this.appWindow is null)
            {
                throw new Exception($"There was an issue trying to close the '{nameof(IWindow)}'.");
            }

            this.appWindow.Close();
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
        /// Turns off all of the caching for any props that are having their values cached.
        /// </summary>
        private void TurnOffCaching()
        {
            this.cachedStringProps.Values.ToList().ForEach(i => i.IsCaching = false);
            this.cachedBoolProps.Values.ToList().ForEach(i => i.IsCaching = false);
            this.cachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

            if (!(this.cachedWindowState is null))
            {
                this.cachedWindowState.IsCaching = false;
            }

            if (!(this.cachedTypeOfBorder is null))
            {
                this.cachedTypeOfBorder.IsCaching = false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to release managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDiposed)
            {
                return;
            }

            if (disposing)
            {
                if (!(this.appWindow is null))
                {
                    this.appWindow.Load -= GameWindow_Load;
                    this.appWindow.UpdateFrame -= GameWindow_UpdateFrame;
                    this.appWindow.RenderFrame -= GameWindow_RenderFrame;
                    this.appWindow.Resize -= GameWindow_Resize;
                    this.appWindow.Unload -= GameWindow_Unload;
                    this.appWindow.Dispose();
                }
            }

            this.isDiposed = true;
        }

        /// <summary>
        /// Invokes the <see cref="Init"/> action property.
        /// </summary>
        private void GameWindow_Load() => Init?.Invoke();

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
                if (this.gl is null)
                {
                    throw new Exception($"There was an issue rendering the '{nameof(IWindow)}'.");
                }

                this.gl.Clear(ClearBufferMask.ColorBufferBit);
            }

            Draw?.Invoke(frameTime);

            if (this.appWindow is null)
            {
                throw new Exception($"There was an issue rendering the '{nameof(IWindow)}'.");
            }

            this.appWindow.SwapBuffers();
        }

        /// <summary>
        /// Invokes the <see cref="WinResize"/> action property..
        /// </summary>
        /// <param name="currentSize">Resize event args.</param>
        private void GameWindow_Resize(ResizeEventArgs currentSize)
        {
            if (this.gl is null)
            {
                throw new Exception($"There was an issue resizing the '{nameof(IWindow)}'.");
            }

            // Update the view port so it is the same size as the window
            this.gl.Viewport(0, 0, currentSize.Width, currentSize.Height);
            WinResize?.Invoke();
        }

        /// <summary>
        /// Sets the state of the window as shutting down.
        /// </summary>
        private void GameWindow_Unload() => this.isShuttingDown = true;

        /// <summary>
        /// Setup all of the caching for the properties that need caching.
        /// </summary>
        private void SetupPropertyCaches()
        {
            this.cachedStringProps.Add(
                nameof(Title), // key
                new CachedValue<string>( // value
                    defaultValue: "Raptor Application",
                    getterWhenNotCaching: () =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Title)}' property value.");
                        }

                        return this.appWindow.Title;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(Title)}' property value.");
                        }

                        this.appWindow.Title = value;
                    }));

            this.cachedPosition = new CachedValue<SysVector2>(
                defaultValue: SysVector2.Zero,
                getterWhenNotCaching: () =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                    }

                    return new SysVector2(this.appWindow.Location.X, this.appWindow.Location.Y);
                },
                setterWhenNotCaching: (value) =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(Position)}' property value.");
                    }

                    this.appWindow.Location = new Vector2i((int)value.X, (int)value.Y);
                });

            this.cachedIntProps.Add(
                nameof(UpdateFrequency), // key
                new CachedValue<int>( // value
                    defaultValue: 60,
                    getterWhenNotCaching: () =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(UpdateFrequency)}' property value.");
                        }

                        return (int)this.appWindow.UpdateFrequency;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(UpdateFrequency)}' property value.");
                        }

                        this.appWindow.UpdateFrequency = value;
                    }));

            this.cachedIntProps.Add(
                nameof(Width), // key
                new CachedValue<int>( // value
                    defaultValue: this.cachedWindowWidth,
                    getterWhenNotCaching: () =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Width)}' property value.");
                        }

                        return this.appWindow.Size.X;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(Width)}' property value.");
                        }

                        this.appWindow.Size = new Vector2i(value, this.appWindow.Size.Y);
                    }));

            this.cachedIntProps.Add(
                nameof(Height), // key
                new CachedValue<int>( // value
                    defaultValue: this.cachedWindowHeight,
                    getterWhenNotCaching: () =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(Height)}' property value.");
                        }

                        return this.appWindow.Size.Y;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(Height)}' property value.");
                        }

                        this.appWindow.Size = new Vector2i(this.appWindow.Size.X, value);
                    }));

            this.cachedBoolProps.Add(
                nameof(MouseCursorVisible), // key
                new CachedValue<bool>( // value
                    defaultValue: true,
                    getterWhenNotCaching: () =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(MouseCursorVisible)}' property value.");
                        }

                        return this.appWindow.CursorVisible;
                    },
                    setterWhenNotCaching: (value) =>
                    {
                        if (this.appWindow is null)
                        {
                            throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(MouseCursorVisible)}' property value.");
                        }

                        this.appWindow.CursorVisible = value;
                    }));

            this.cachedWindowState = new CachedValue<StateOfWindow>(
                defaultValue: StateOfWindow.Normal,
                getterWhenNotCaching: () =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                    }

                    return (StateOfWindow)this.appWindow.WindowState;
                },
                setterWhenNotCaching: (value) =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(WindowState)}' property value.");
                    }

                    this.appWindow.WindowState = (GLWindowState)value;
                });

            this.cachedTypeOfBorder = new CachedValue<BorderType>(
                defaultValue: BorderType.Resizable,
                getterWhenNotCaching: () =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue getting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                    }

                    return (BorderType)this.appWindow.WindowBorder;
                },
                setterWhenNotCaching: (value) =>
                {
                    if (this.appWindow is null)
                    {
                        throw new Exception($"There was an issue setting the '{nameof(IWindow)}.{nameof(TypeOfBorder)}' property value.");
                    }

                    this.appWindow.WindowBorder = (WindowBorder)value;
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
