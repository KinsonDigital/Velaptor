// <copyright file="GLWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using Silk.NET.Input;
    using Silk.NET.Maths;
    using Silk.NET.Windowing;
    using Velaptor.Guards;
    using Velaptor.Input;
    using Velaptor.Input.Exceptions;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using SilkMouseButton = Silk.NET.Input.MouseButton;
    using VelaptorMouseButton = Velaptor.Input.MouseButton;
    using VelaptorWindowBorder = Velaptor.WindowBorder;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// The internal SILK OpenGL window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Cannot be abstract due to dependency injection.")]
    internal class GLWindowFacade : IGameWindowFacade
    {
        private readonly string nullWindowExceptionMsg;
        private readonly IReactable<GLContextData> glContextReactable;
        private readonly IKeyboardInput<KeyCode, KeyboardState> keyboardInput;
        private readonly IMouseInput<VelaptorMouseButton, MouseState> mouseInput;
        private IWindow? glWindow;
        private IInputContext? glInputContext;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLWindowFacade"/> class.
        /// </summary>
        /// <param name="glReactable">
        ///     Receives push notifications when the OpenGL context has been created.
        /// </param>
        /// <param name="keyboardInput">The system keyboardInput for handling keyboardInput events.</param>
        /// <param name="mouseInput">The system mouseInput for handling mouseInput events.</param>
        public GLWindowFacade(
            IReactable<GLContextData> glReactable,
            IKeyboardInput<KeyCode, KeyboardState> keyboardInput,
            IMouseInput<VelaptorMouseButton,
            MouseState> mouseInput)
        {
            EnsureThat.ParamIsNotNull(glReactable);
            EnsureThat.ParamIsNotNull(keyboardInput);
            EnsureThat.ParamIsNotNull(mouseInput);

            this.glContextReactable = glReactable;
            this.keyboardInput = keyboardInput;
            this.mouseInput = mouseInput;

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
                    throw new InvalidOperationException($"The OpenGL context has not been created yet.  Invoke the '{nameof(IGameWindowFacade.Init)}()' method first.");
                }

                if (this.glInputContext.Mice.Count <= 0)
                {
                    return false;
                }

                return this.glInputContext.Mice[0].Cursor.CursorMode == CursorMode.Normal;
            }
            set
            {
                var cursorMode = value ? CursorMode.Normal : CursorMode.Hidden;

                if (this.glInputContext is null)
                {
                    throw new InvalidOperationException($"The OpenGL context has not been created yet.  Invoke the '{nameof(IGameWindowFacade.Init)}()' method first.");
                }

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
        public VelaptorWindowBorder WindowBorder
        {
            get
            {
                if (this.glWindow is null)
                {
                    throw new InvalidOperationException(this.nullWindowExceptionMsg);
                }

                return this.glWindow.WindowBorder switch
                {
                    Silk.NET.Windowing.WindowBorder.Fixed => VelaptorWindowBorder.Fixed,
                    Silk.NET.Windowing.WindowBorder.Hidden => VelaptorWindowBorder.Hidden,
                    Silk.NET.Windowing.WindowBorder.Resizable => VelaptorWindowBorder.Resizable,
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
                    VelaptorWindowBorder.Fixed => Silk.NET.Windowing.WindowBorder.Fixed,
                    VelaptorWindowBorder.Hidden => Silk.NET.Windowing.WindowBorder.Hidden,
                    VelaptorWindowBorder.Resizable => Silk.NET.Windowing.WindowBorder.Resizable,
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
        public void Init(uint width, uint height)
        {
            if (this.glWindow is null)
            {
                throw new InvalidOperationException(this.nullWindowExceptionMsg);
            }

            this.glContextReactable.PushNotification(new GLContextData(this.glWindow));

            this.glWindow.Size = new Vector2D<int>((int)width, (int)height);
            this.glInputContext = this.glWindow.CreateInput();

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
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.glInputContext is not null)
                {
                    this.glInputContext.Keyboards[0].KeyDown -= GLKeyboardInput_KeyDown;
                    this.glInputContext.Keyboards[0].KeyUp -= GLKeyboardInput_KeyUp;
                    this.glInputContext.Mice[0].MouseDown -= GLMouseInput_MouseDown;
                    this.glInputContext.Mice[0].MouseUp -= GLMouseInput_MouseUp;
                    this.glInputContext.Mice[0].MouseMove -= GLMouseMove_MouseMove;
                    this.glInputContext.Mice[0].Scroll -= GLMouseInput_MouseScroll;
                }

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

        /// <summary>
        /// Invoked when there is mouse scroll wheel input.
        /// </summary>
        /// <param name="mouse">The system mouse object.</param>
        /// <param name="wheelData">Positional data about the mouse scroll wheel.</param>
        private void GLMouseInput_MouseScroll(IMouse mouse, ScrollWheel wheelData)
        {
            var wheelValue = Math.Abs(wheelData.Y);

            this.mouseInput.SetScrollWheelSpeed((int)wheelValue);

            var wheelDirection = wheelData.Y switch
            {
                > 0 => MouseScrollDirection.ScrollUp,
                < 0 => MouseScrollDirection.ScrollDown,
                _ => MouseScrollDirection.None
            };

            this.mouseInput.SetScrollWheelDirection(wheelDirection);
        }

        /// <summary>
        /// Invoked when the mouse moves over the window.
        /// </summary>
        /// <param name="mouse">The system mouse object.</param>
        /// <param name="position">The position of the mouseInput.</param>
        private void GLMouseMove_MouseMove(IMouse mouse, Vector2 position)
        {
            this.mouseInput.SetXPos((int)position.X);
            this.mouseInput.SetYPos((int)position.Y);
        }

        /// <summary>
        /// Invoked when any of the mouse buttons are pressed into the down position over the window.
        /// </summary>
        /// <param name="mouse">The system mouse object.</param>
        /// <param name="button">The button that was pushed down.</param>
        private void GLMouseInput_MouseDown(IMouse mouse, SilkMouseButton button) => this.mouseInput.SetState((VelaptorMouseButton)button, true);

        /// <summary>
        /// Invoked when any of the mouse buttons are released from the down position into the up position over the window.
        /// </summary>
        /// <param name="mouse">The system mouse object.</param>
        /// <param name="button">The button that was pushed down.</param>
        private void GLMouseInput_MouseUp(IMouse mouse, SilkMouseButton button) => this.mouseInput.SetState((VelaptorMouseButton)button, false);

        /// <summary>
        /// Invoked when any keyboardInput key transitions from the up position to the down position.
        /// </summary>
        /// <param name="keyboard">The system keyboardInput.</param>
        /// <param name="key">The key that was pushed down.</param>
        /// <param name="arg3">Additional argument from OpenGL.</param>
        private void GLKeyboardInput_KeyDown(IKeyboard keyboard, Key key, int arg3) => this.keyboardInput.SetState((KeyCode)key, true);

        /// <summary>
        /// Invoked when any keyboardInput key transitions from the down position to the up position.
        /// </summary>
        /// <param name="keyboard">The system keyboardInput.</param>
        /// <param name="key">The key that was released.</param>
        /// <param name="arg3">Additional argument from OpenGL.</param>
        private void GLKeyboardInput_KeyUp(IKeyboard keyboard, Key key, int arg3) => this.keyboardInput.SetState((KeyCode)key, false);

        /// <summary>
        /// Invoked when the window is loaded and invokes the <see cref="Load"/> event.
        /// </summary>
        private void GLWindow_Load() => this.Load?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invoked when the window is in the process of closing and invokes the <see cref="Unload"/> event.
        /// </summary>
        private void GLWindow_Closing() => this.Unload?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invoked every time the size of the window changes and invokes the
        /// <see cref="IGameWindowFacade.Resize"/> event.
        /// </summary>
        private void GLWindow_Resize(Vector2D<int> obj) => this.Resize?.Invoke(this, new WindowSizeEventArgs(obj.X, obj.Y));

        /// <summary>
        /// Invoked once per frame and invokes the <see cref="RenderFrame"/> event.
        /// </summary>
        /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
        private void GLWindow_Render(double frameTime) => this.RenderFrame?.Invoke(this, new FrameTimeEventArgs(frameTime));

        /// <summary>
        /// Invoked once per frame and invokes the <see cref="UpdateFrame"/> event.
        /// </summary>
        /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
        private void GLWindow_Update(double frameTime)
        {
            this.UpdateFrame?.Invoke(this, new FrameTimeEventArgs(frameTime));
            this.mouseInput.SetScrollWheelSpeed(0);
            this.mouseInput.SetScrollWheelDirection(MouseScrollDirection.None);
        }
    }
}
