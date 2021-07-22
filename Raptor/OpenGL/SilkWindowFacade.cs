// <copyright file="SilkWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Numerics;
    using Raptor.Input;
    using Raptor.Observables;
    using Silk.NET.Input;
    using Silk.NET.Maths;
    using Silk.NET.Windowing;
    using RaptorMouseButton = Input.MouseButton;
    using SilkMouseButton = Silk.NET.Input.MouseButton;

    /// <summary>
    /// The internal SILK OpenGL window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class SilkWindowFacade : IGameWindowFacade
    {
        private readonly string nullWindowExceptionMsg;
        private readonly object objectLock = new ();
        private readonly OpenGLContextObservable glContextObservable;
        private readonly IKeyboardInput<KeyCode, KeyboardState> keyboard;
        private readonly IMouseInput<RaptorMouseButton, MouseState> mouse;
        private IWindow? glWindow;
        private IInputContext? glInputContext;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SilkWindowFacade"/> class.
        /// </summary>
        /// <param name="glObservable">
        ///     Receives push notifications when the OpenGL context has been created.
        /// </param>
        public SilkWindowFacade(OpenGLContextObservable glObservable, IKeyboardInput<KeyCode, KeyboardState> keyboard, IMouseInput<RaptorMouseButton, MouseState> mouse)
        {
            this.nullWindowExceptionMsg = $"The OpenGL context has not been created yet.  Invoke the '{nameof(IGameWindowFacade.PreInit)}()' method first.";
            this.glContextObservable = glObservable;
            this.keyboard = keyboard;
            this.mouse = mouse;
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
        public Vector2 Size
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return new Vector2(this.glWindow.Size.X, this.glWindow.Size.Y);
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.Size = new Vector2D<int>((int)value.X, (int)value.Y);
            }
        }

        /// <inheritdoc/>
        public Vector2 Location
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return new Vector2(this.glWindow.Position.X, this.glWindow.Position.Y);
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.Position = new Vector2D<int>((int)value.X, (int)value.Y);
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

                return this.glWindow.UpdatesPerSecond;
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.UpdatesPerSecond = value;
            }
        }

        /// <inheritdoc/>
        public bool CursorVisible
        {
            get
            {
                if (this.glInputContext is null)
                {
                    throw new InvalidOperationException($"The OpenGL context has not been created yet.  Invoke the '{nameof(IGameWindowFacade.PreInit)}()' method first.");
                }

                if (this.glInputContext.Mice[0] is null)
                {
                    return false;
                }
                else
                {
                    return this.glInputContext.Mice[0].Cursor.CursorMode == CursorMode.Normal;
                }
            }
            set
            {
                var cursorMode = value ? CursorMode.Normal : CursorMode.Hidden;

                this.glInputContext.Mice[0].Cursor.CursorMode = cursorMode;
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

                return this.glWindow.WindowState switch
                {
                    Silk.NET.Windowing.WindowState.Normal => StateOfWindow.Normal,
                    Silk.NET.Windowing.WindowState.Minimized => StateOfWindow.Minimized,
                    Silk.NET.Windowing.WindowState.Maximized => StateOfWindow.Maximized,
                    Silk.NET.Windowing.WindowState.Fullscreen => StateOfWindow.FullScreen,
                    _ => throw new Exception("Invalid Window State"),
                };
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.WindowState = value switch
                {
                    StateOfWindow.Normal => Silk.NET.Windowing.WindowState.Normal,
                    StateOfWindow.Minimized => Silk.NET.Windowing.WindowState.Minimized,
                    StateOfWindow.Maximized => Silk.NET.Windowing.WindowState.Maximized,
                    StateOfWindow.FullScreen => Silk.NET.Windowing.WindowState.Fullscreen,
                    _ => throw new Exception("Invalid Window State"),
                };
            }
        }

        /// <inheritdoc/>
        public Raptor.WindowBorder WindowBorder
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return this.glWindow.WindowBorder switch
                {
                    Silk.NET.Windowing.WindowBorder.Fixed => Raptor.WindowBorder.Fixed,
                    Silk.NET.Windowing.WindowBorder.Hidden => Raptor.WindowBorder.Hidden,
                    Silk.NET.Windowing.WindowBorder.Resizable => Raptor.WindowBorder.Resizable,
                    _ => throw new Exception("Invalid border"),
                };
            }
            set
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                this.glWindow.WindowBorder = value switch
                {
                    Raptor.WindowBorder.Fixed => Silk.NET.Windowing.WindowBorder.Fixed,
                    Raptor.WindowBorder.Hidden => Silk.NET.Windowing.WindowBorder.Hidden,
                    Raptor.WindowBorder.Resizable => Silk.NET.Windowing.WindowBorder.Resizable,
                    _ => throw new Exception("Invalid border"),
                };
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
        public void PreInit()
        {
            var windowOptions = WindowOptions.Default;
            windowOptions.ShouldSwapAutomatically = false;

            this.glWindow = Window.Create(windowOptions);
            this.glWindow.UpdatesPerSecond = 120;
            this.glWindow.Load += GLWindow_Load;
            this.glWindow.Closing += GLWindow_Closing;
            this.glWindow.Resize += GLWindow_Resize;
            this.glWindow.Update += GLWindow_Update;
            this.glWindow.Render += GLWindow_Render;
        }

        /// <inheritdoc/>
        public void Init(int width, int height)
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glContextObservable.OnGLContextCreated(this.glWindow);

            this.glWindow.Size = new Vector2D<int>(width, height);
            this.glInputContext = this.glWindow.CreateInput();

            if (this.glInputContext.Keyboards.Count <= 0)
            {
                throw new Exception("SILK Input Exception: No connected keyboards available.");
            }

            // TODO: unregister on dispose
            this.glInputContext.Keyboards[0].KeyDown += GLKeyboardInput_KeyDown;
            this.glInputContext.Keyboards[0].KeyUp += GLKeyboardInput_KeyUp;

            if (this.glInputContext.Mice.Count <= 0)
            {
                throw new Exception("SILK Input Exception: No connected mice available.");
            }

            this.glInputContext.Mice[0].MouseDown += GLMouseInput_MouseDown;
            this.glInputContext.Mice[0].MouseUp += GLMouseInput_MouseUp;
            this.glInputContext.Mice[0].MouseMove += GLMouseMove_MouseMove;
        }

        private void GLMouseMove_MouseMove(IMouse arg1, Vector2 arg2)
        {
            this.mouse.SetXPos((int)arg2.X);
            this.mouse.SetYPos((int)arg2.Y);
        }

        private void GLMouseInput_MouseDown(IMouse arg1, SilkMouseButton arg2) => this.mouse.SetState((RaptorMouseButton)arg2, true);

        private void GLMouseInput_MouseUp(IMouse arg1, SilkMouseButton arg2) => this.mouse.SetState((RaptorMouseButton)arg2, false);

        private void GLKeyboardInput_KeyDown(IKeyboard arg1, Key arg2, int arg3) => this.keyboard.SetState((KeyCode)arg2, true);

        private void GLKeyboardInput_KeyUp(IKeyboard arg1, Key arg2, int arg3) => this.keyboard.SetState((KeyCode)arg2, false);

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
                        this.glWindow.Closing -= GLWindow_Closing;
                        this.glWindow.Resize -= GLWindow_Resize;
                        this.glWindow.Update -= GLWindow_Update;
                        this.glWindow.Render -= GLWindow_Render;
                    }
                }

                this.isDisposed = true;
            }
        }

        private void GLWindow_Load() => Load?.Invoke(this, EventArgs.Empty);

        private void GLWindow_Closing() => Unload?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invoked every time the size of the window changs and invokes the
        /// <see cref="IGameWindowFacade.Resize"/> event.
        /// </summary>
        private void GLWindow_Resize(Vector2D<int> obj) => Resize?.Invoke(this, new WindowSizeEventArgs(obj.X, obj.Y));

        private void GLWindow_Render(double obj) => RenderFrame?.Invoke(this, new FrameTimeEventArgs(obj));

        private List<double> frameTimes = new List<double>();

        private void GLWindow_Update(double obj)
        {
            this.frameTimes.Add(obj);

            if (this.frameTimes.Count >= 10000)
            {
                this.frameTimes.RemoveRange(0, 5);
                //0.033422924932466382
                var average = this.frameTimes.Average();

                Debugger.Break();
            }

            UpdateFrame?.Invoke(this, new FrameTimeEventArgs(obj));
        }
    }
}
