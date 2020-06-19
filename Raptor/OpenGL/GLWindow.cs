using System;
using System.Runtime.InteropServices;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Graphics.OpenGL4;
using NETColor = System.Drawing.Color;

namespace Raptor.OpenGL
{
    internal class GLWindow : GameWindow, IWindow
    {
        #region Private Fields
        private int _fps;
        private bool isShuttingDown;
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
            if (nativeWinSettings is null)
                throw new ArgumentNullException(nameof(nativeWinSettings), "The argument must not be null");

            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            GL.DebugMessageCallback(DebugCallback, Marshal.StringToHGlobalAnsi(""));

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
            if (this.isShuttingDown)
                return;

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
            if (this.isShuttingDown)
                return;

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


        protected override void OnUnload()
        {
            this.isShuttingDown = true;
            base.OnUnload();
        }
        #endregion


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
