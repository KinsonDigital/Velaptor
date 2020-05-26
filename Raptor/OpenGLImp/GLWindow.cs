using OpenToolkit.Graphics.ES11;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;
using Raptor.Plugins;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Raptor.OpenGLImp
{
    internal class GLWindow : GameWindow, IWindow
    {
        #region Private Fields
        private int _fps;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="GLWindow"/>.
        /// </summary>
        /// <param name="gameWinSettings">The game window settings.</param>
        /// <param name="nativeWinSettings">The native window settings.</param>
        public GLWindow(GameWindowSettings gameWinSettings, NativeWindowSettings nativeWinSettings)
            : base(gameWinSettings, nativeWinSettings)
        {
            Title = "Raptor Application";
            UpdateFrequency = 60;
            RenderFrequency = 60;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        public int Width
        {
            get => Size.X;
            set => Size = new Vector2i(value, Size.Y);
        }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        public int Height
        {
            get => Size.Y;
            set => Size = new Vector2i(Size.X, value);
        }

        /// <summary>
        /// Invoked at a particular frame rate to run update logic.
        /// </summary>
        public Action<FrameTime>? Update { get; set; }

        /// <summary>
        /// Invoked at a particular frame rate to render graphics.
        /// </summary>
        public Action<FrameTime>? Draw { get; set; }

        /// <summary>
        /// Invoked when the window is resized.
        /// </summary>
        public Action? Resize { get; set; }

        /// <summary>
        /// Invoked once to initialize.
        /// </summary>
        public Action? Init { get; set; }

        /// <summary>
        /// Gets or sets the value of how often the <see cref="Update"/>
        /// and <see cref="Draw"/> actions are invoked in hertz.
        /// </summary>
        public int UpdateFreq
        {
            get
            {
                if (UpdateFrequency != RenderFrequency)
                    throw new Exception($"The update and render frequencies must match for this '{nameof(GLWindow)}' implementation.");


                return (int)UpdateFrequency;
            }
            set
            {
                UpdateFrequency = value;
                RenderFrequency = value;
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Shows the window.
        /// </summary>
        public void Show()
        {
            Run();
        }


        /// <summary>
        /// Run immediately after Run() is called.
        /// </summary>
        protected override void OnLoad()
        {
            Init?.Invoke();

            base.OnLoad();
        }


        /// <summary>
        /// Run when the window is ready to update.
        /// </summary>
        /// <param name="args">The event arguments for this frame.</param>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(args.Time * 1000.0))
            };

            Update?.Invoke(frameTime);
            
            base.OnUpdateFrame(args);
        }


        /// <summary>
        /// Run when the window is ready to update.
        /// </summary>
        /// <param name="args">The event arguments for this frame.</param>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(args.Time * 1000.0))
            };

            Draw?.Invoke(frameTime);

            SwapBuffers();

            base.OnRenderFrame(args);
        }


        /// <summary>
        /// Raises the <see cref="Resize"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ResizeEventArgs"/> that contains the event data.</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            Resize?.Invoke();

            base.OnResize(e);
        }
        #endregion
    }
}
