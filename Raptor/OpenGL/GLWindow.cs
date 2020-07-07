// <copyright file="GLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;
    using OpenToolkit.Windowing.Common;
    using OpenToolkit.Windowing.Desktop;
    using OpenToolkit.Windowing.GraphicsLibraryFramework;

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
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used in constructor.")]
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
            this.gameWindow.UpdateFrequency = 60;

            this.debugProc = DebugCallback;

            /*NOTE:
             * This is here to help prevent an issue with an obscure System.ExecutionException from occuring.
             * The garbage collector performas a collect on the delegate passed into GL.DebugMesageCallback()
             * without the native system knowing about it which causes this acception. The GC.KeepAlive()
             * method tells the garbage collector to not collect the delgate to prevent this from happening.
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
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside of property.")]
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

        private void GameWindow_KeyDown(KeyboardKeyEventArgs e)
        {
        }

        private void GameWindow_KeyUp(KeyboardKeyEventArgs e)
        {
        }

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
        private unsafe void GameWindow_UpdateFrame(FrameEventArgs deltaTime)
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
        /// <param name="deltaTime">The frame event arges.</param>
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
