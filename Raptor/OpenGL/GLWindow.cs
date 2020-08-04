// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;
    using OpenToolkit.Windowing.Common;
    using OpenToolkit.Windowing.Desktop;
    using Raptor.Input;
    using GLMouseButton = OpenToolkit.Windowing.Common.Input.MouseButton;
    using RaptorMouseButton = Raptor.Input.MouseButton;

    /// <summary>
    /// An OpenGL window implementation to be used inside of the <see cref="Window"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class GLWindow : IWindow
    {
        private readonly InternalGLWindow gameWindow;
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

            this.gameWindow = new InternalGLWindow(gameWinSettings, nativeWinSettings);

            /*NOTE:
             * The IoC container get instance must be called after the
             * game window has been called.  This is because an OpenGL context
             * must be created first before any GL calls can be made.
             */
            this.gl = IoC.Container.GetInstance<IGLInvoker>();

            this.gameWindow.Load += GameWindow_Load;
            this.gameWindow.UpdateFrame += GameWindow_UpdateFrame;
            this.gameWindow.RenderFrame += GameWindow_RenderFrame;
            this.gameWindow.Resize += GameWindow_Resize;
            this.gameWindow.Unload += GameWindow_Unload;
            this.gameWindow.KeyDown += GameWindow_KeyDown;
            this.gameWindow.KeyUp += GameWindow_KeyUp;
            this.gameWindow.MouseDown += GameWindow_MouseDown;
            this.gameWindow.MouseUp += GameWindow_MouseUp;
            this.gameWindow.MouseMove += GameWindow_MouseMove;
            this.gameWindow.UpdateFrequency = 60;

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
            get => this.gameWindow.Title;
            set => this.gameWindow.Title = value;
        }

        /// <inheritdoc/>
        public int Width
        {
            get => this.gameWindow.Size.X;
            set => this.gameWindow.Size = new Vector2i(value, this.gameWindow.Size.Y);
        }

        /// <inheritdoc/>
        public int Height
        {
            get => this.gameWindow.Size.Y;
            set => this.gameWindow.Size = new Vector2i(this.gameWindow.Size.X, value);
        }

        /// <inheritdoc/>
        public Action<FrameTime>? Update { get; set; }

        /// <inheritdoc/>
        public Action<FrameTime>? Draw { get; set; }

        /// <inheritdoc/>
        public Action? WinResize { get; set; }

        /// <inheritdoc/>
        public Action? Init { get; set; }

        /// <inheritdoc/>
        public int UpdateFreq
        {
            get
            {
                if (this.gameWindow.UpdateFrequency != this.gameWindow.RenderFrequency)
                    throw new Exception($"The update and render frequencies must match for this '{nameof(GLWindow)}' implementation.");

                return (int)this.gameWindow.UpdateFrequency;
            }
            set
            {
                this.gameWindow.UpdateFrequency = value;
                this.gameWindow.RenderFrequency = value;
            }
        }

        /// <inheritdoc/>
        public void Show() => this.gameWindow.Run();

        /// <inheritdoc/>
        public void Close() => this.gameWindow.Close();

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
        /// <returns>The raptor mouse button.</returns>
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
                case GLMouseButton.LastButton:
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
                this.gameWindow.Load -= GameWindow_Load;
                this.gameWindow.UpdateFrame -= GameWindow_UpdateFrame;
                this.gameWindow.RenderFrame -= GameWindow_RenderFrame;
                this.gameWindow.Resize -= GameWindow_Resize;
                this.gameWindow.Unload -= GameWindow_Unload;
                this.gameWindow.Dispose();
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

            Draw?.Invoke(frameTime);

            this.gameWindow.SwapBuffers();
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
