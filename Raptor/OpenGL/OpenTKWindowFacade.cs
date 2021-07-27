// <copyright file="OpenTKWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using Velaptor.Input;
    using SysVector2 = System.Numerics.Vector2;

    /// <summary>
    /// The internal OpenTK OpenGL window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class OpenTKWindowFacade : IGameWindowFacade
    {
        private readonly IKeyboardInput<KeyCode, KeyboardState> keyboard;
        private readonly IMouseInput<MouseButton, MouseState> mouse;
        private readonly string nullWindowExceptionMsg;
        private readonly object objectLock = new ();
        private GameWindow? glWindow;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTKWindowFacade"/> class.
        /// </summary>
        public OpenTKWindowFacade(IKeyboardInput<KeyCode, KeyboardState> keyboard, IMouseInput<MouseButton, MouseState> mouse)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
            this.nullWindowExceptionMsg = $"The OpenGL context has not been created yet.  Invoke the '{nameof(IGameWindowFacade.PreInit)}()' method first.";
        }

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? Load;

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? Unload;

        /// <inheritdoc/>
        public event EventHandler<FrameTimeEventArgs>? UpdateFrame;

        /// <inheritdoc/>
        public event EventHandler<FrameTimeEventArgs>? RenderFrame;

        /// <inheritdoc/>
        public event EventHandler<WindowSizeEventArgs>? Resize;

        /// <inheritdoc/>
        public SysVector2 Size
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return new SysVector2(this.glWindow.Size.X, this.glWindow.Size.Y);
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.Size = new Vector2i((int)value.X, (int)value.Y);
            }
        }

        /// <inheritdoc/>
        public SysVector2 Location
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return new SysVector2(this.glWindow.Location.X, this.glWindow.Location.Y);
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.Location = new Vector2i((int)value.X, (int)value.Y);
            }
        }

        /// <inheritdoc/>
        public double UpdateFrequency
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return this.glWindow.UpdateFrequency;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.UpdateFrequency = value;
            }
        }

        /// <inheritdoc/>
        public bool CursorVisible
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return this.glWindow.CursorVisible;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.CursorVisible = value;
            }
        }

        /// <inheritdoc/>
        public StateOfWindow WindowState
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return (StateOfWindow)this.glWindow.WindowState;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.WindowState = (WindowState)value;
            }
        }

        /// <inheritdoc/>
        public Velaptor.WindowBorder WindowBorder
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return (Velaptor.WindowBorder)this.glWindow.WindowBorder;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.WindowBorder = (WindowBorder)value;
            }
        }

        /// <inheritdoc/>
        public string Title
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return this.glWindow.Title;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.Title = value;
            }
        }

        /// <inheritdoc/>
        public void PreInit()
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(700, 600), // Default size.  Size will be set in Init() method
                StartVisible = false,
            };

            // The creation of the OpenTK.GameWindow is when the current context is created.
            // All OpenGL calls can be made after this has occurred.
            this.glWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);
            this.glWindow.Load += GLWindow_Load;
            this.glWindow.Unload += GLWindow_Unload;
            this.glWindow.Resize += GLWindow_Resize;
            this.glWindow.UpdateFrame += GLWindow_UpdateFrame;
            this.glWindow.RenderFrame += GLWindow_RenderFrame;
            this.glWindow.KeyDown += GLWindow_KeyDown;
            this.glWindow.KeyUp += GLWindow_KeyUp;
            this.glWindow.MouseDown += GLWindow_MouseDown;
            this.glWindow.MouseUp += GLWindow_MouseUp;
            this.glWindow.MouseMove += GLWindow_MouseMove;
        }

        private void GLWindow_MouseMove(MouseMoveEventArgs obj)
        {
            this.mouse.SetXPos((int)obj.X);
            this.mouse.SetYPos((int)obj.Y);
        }

        /// <inheritdoc/>
        public void Init(int width, int height)
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glWindow.Size = new Vector2i(width, height);
        }

        /// <inheritdoc/>
        public void Show()
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glWindow.Run();

            /*NOTE:
             * Only dispose of the window here and not in the Dispose() method!!
             *
             * This is because this line of code will not be executed until the Window.Run() method
             * has finished executing.  This happens once the window is closed.
             *
             * If you dispose of the window in the Dispose() method before the Run() method is finished
             * then the application will crash.
             */
            this.glWindow.Dispose();
        }

        /// <inheritdoc/>
        public void SwapBuffers()
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glWindow.SwapBuffers();
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glWindow.Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.glWindow is not null)
                    {
                        this.glWindow.Load -= GLWindow_Load;
                        this.glWindow.Unload -= GLWindow_Unload;
                        this.glWindow.Resize -= GLWindow_Resize;
                        this.glWindow.UpdateFrame -= GLWindow_UpdateFrame;
                        this.glWindow.RenderFrame -= GLWindow_RenderFrame;
                        this.glWindow.KeyDown -= GLWindow_KeyDown;
                        this.glWindow.KeyUp -= GLWindow_KeyUp;
                        this.glWindow.MouseDown -= GLWindow_MouseDown;
                        this.glWindow.MouseUp -= GLWindow_MouseUp;
                        this.glWindow.MouseMove -= GLWindow_MouseMove;
                    }
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Invokes the window resize event.
        /// </summary>
        private void GLWindow_Resize(ResizeEventArgs e) => Resize?.Invoke(this, new WindowSizeEventArgs(e.Width, e.Height));

        private List<double> frameTimes = new List<double>();

        /// <summary>
        /// Invokes the update frame event.
        /// </summary>
        private void GLWindow_UpdateFrame(FrameEventArgs e)
        {
            this.frameTimes.Add(e.Time);

            if (this.frameTimes.Count >= 10000)
            {
                this.frameTimes.RemoveRange(0, 5);
                //0.016650452276138043
                var average = this.frameTimes.Average();

                Debugger.Break();
            }

            UpdateFrame?.Invoke(this, new FrameTimeEventArgs(e.Time));
        }

        /// <summary>
        /// Invokes the <see cref="Load"/> event.
        /// </summary>
        private void GLWindow_Load() => Load?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invokes the <see cref="Unload"/> event.
        /// </summary>
        private void GLWindow_Unload() => Unload?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invokes the render frame event.
        /// </summary>
        private void GLWindow_RenderFrame(FrameEventArgs e) => RenderFrame?.Invoke(this, new FrameTimeEventArgs(e.Time));

        /// <summary>
        /// Updates the keyboard state when a keyboard key has been pressed down.
        /// </summary>
        private void GLWindow_KeyDown(KeyboardKeyEventArgs e) => this.keyboard.SetState((KeyCode)e.Key, true);

        /// <summary>
        /// Updates the keyboard state when a keyboard key has been released.
        /// </summary>
        private void GLWindow_KeyUp(KeyboardKeyEventArgs e) => this.keyboard.SetState((KeyCode)e.Key, false);

        /// <summary>
        /// Updates the mouse state when a mouse button has been pressed down.
        /// </summary>
        private void GLWindow_MouseUp(MouseButtonEventArgs e) => this.mouse.SetState((MouseButton)e.Button, false);

        /// <summary>
        /// Updates the mouse state when a mouse button has been released.
        /// </summary>
        private void GLWindow_MouseDown(MouseButtonEventArgs e) => this.mouse.SetState((MouseButton)e.Button, true);
    }
}
