// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
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
        private readonly InternalGLWindow appWindow;
        private readonly DebugProc debugProc;
        private readonly IGLInvoker gl;
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
            var gameWinSettings = new GameWindowSettings();
            var nativeWinSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
            };

            this.appWindow = new InternalGLWindow(gameWinSettings, nativeWinSettings);

            /*NOTE:
             * The IoC container get instance must be called after the
             * game window has been called.  This is because an OpenGL context
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
            this.appWindow.UpdateFrequency = 60;

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

            Title = "Raptor Application";
        }

        /// <inheritdoc/>
        public string Title
        {
            get => this.appWindow.Title;
            set => this.appWindow.Title = value;
        }

        /// <inheritdoc/>
        public SysVector2 Position
        {
            get => new SysVector2(this.appWindow.Location.X, this.appWindow.Location.Y);
            set => this.appWindow.Location = new Vector2i((int)value.X, (int)value.Y);
        }

        /// <inheritdoc/>
        public int Width
        {
            get => this.appWindow.Size.X;
            set => this.appWindow.Size = new Vector2i(value, this.appWindow.Size.Y);
        }

        /// <inheritdoc/>
        public int Height
        {
            get => this.appWindow.Size.Y;
            set => this.appWindow.Size = new Vector2i(this.appWindow.Size.X, value);
        }

        /// <inheritdoc/>
        public bool AutoClearBuffers { get; set; } = true;

        /// <inheritdoc/>
        public bool MouseCursorVisible
        {
            get => this.appWindow.CursorVisible;
            set => this.appWindow.CursorVisible = value;
        }

        /// <inheritdoc/>3
        public StateOfWindow WindowState
        {
            get => (StateOfWindow)this.appWindow.WindowState;
            set => this.appWindow.WindowState = (GLWindowState)value;
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
            get => this.appWindow.WindowBorder switch
            {
                WindowBorder.Resizable => BorderType.Resizable,
                WindowBorder.Fixed => BorderType.Fixed,
                WindowBorder.Hidden => BorderType.Hidden,
                _ => throw new Exception($"The '{nameof(WindowBorder)}' is invalid."),
            };
            set
            {
                switch (value)
                {
                    case BorderType.Resizable:
                        this.appWindow.WindowBorder = WindowBorder.Resizable;
                        break;
                    case BorderType.Fixed:
                        this.appWindow.WindowBorder = WindowBorder.Fixed;
                        break;
                    case BorderType.Hidden:
                        this.appWindow.WindowBorder = WindowBorder.Hidden;
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public int UpdateFreq
        {
            get
            {
                if (this.appWindow.UpdateFrequency != this.appWindow.RenderFrequency)
                    throw new Exception($"The update and render frequencies must match for this '{nameof(GLWindow)}' implementation.");

                return (int)this.appWindow.UpdateFrequency;
            }
            set
            {
                this.appWindow.UpdateFrequency = value;
                this.appWindow.RenderFrequency = value;
            }
        }

        /// <inheritdoc/>
        public void Show() => this.appWindow.Run();

        /// <inheritdoc/>
        public void Close() => this.appWindow.Close();

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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to release managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDiposed)
                return;

            if (disposing)
            {
                this.appWindow.Load -= GameWindow_Load;
                this.appWindow.UpdateFrame -= GameWindow_UpdateFrame;
                this.appWindow.RenderFrame -= GameWindow_RenderFrame;
                this.appWindow.Resize -= GameWindow_Resize;
                this.appWindow.Unload -= GameWindow_Unload;
                this.appWindow.Dispose();
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
                return;

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
                return;

            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(deltaTime.Time * 1000.0)),
            };

            if (AutoClearBuffers)
            {
                this.gl.Clear(ClearBufferMask.ColorBufferBit);
            }

            Draw?.Invoke(frameTime);

            this.appWindow.SwapBuffers();
        }

        /// <summary>
        /// Invokes the <see cref="WinResize"/> action property..
        /// </summary>
        /// <param name="currentSize">Resize event args.</param>
        private void GameWindow_Resize(ResizeEventArgs currentSize)
        {
            GL.Viewport(0, 0, currentSize.Width, currentSize.Height);
            WinResize?.Invoke();
        }

        /// <summary>
        /// Sets the state of the window as shutting down.
        /// </summary>
        private void GameWindow_Unload() => this.isShuttingDown = true;

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
