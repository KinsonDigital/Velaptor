using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Desktop;
using Raptor.OpenGLImp;
using Raptor.Plugins;
using System;

namespace Raptor
{
    /// <summary>
    /// A system window that graphics can be rendered to.
    /// </summary>
    public abstract class Window : IDisposable
    {
        #region Private Fields
        private readonly IWindow _window;
        private bool _isDisposed;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Window"/>.
        /// </summary>
        /// <param name="window">The internal window implementation that manages a window.</param>
        public Window(IWindow window)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "IWindow must not be null");

            _window = window;
            UpdateFrequency = 60;
        }


        /// <summary>
        /// Creates a new instance of <see cref="Window"/>.
        /// </summary>
        public Window()
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600)
            };

            _window = new GLWindow(gameWindowSettings, nativeWindowSettings)
            {
                Update = OnUpdate,
                Draw = OnDraw,
                Init = OnLoad,
                Resize = OnResize
            };
            UpdateFrequency = 60;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get => _window.Title;
            set => _window.Title = value;
        }


        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int Width
        {
            get => _window.Width;
            set => _window.Width = value;
        }


        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int Height
        {
            get => _window.Height;
            set => _window.Height = value;
        }


        /// <summary>
        /// Gets or sets the frequency of how often the window updates and draws
        /// in hertz.
        /// </summary>
        public int UpdateFrequency
        {
            get => _window.UpdateFreq;
            set => _window.UpdateFreq = value;
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Shows the window.
        /// </summary>
        public void Show() => _window.Show();


        /// <summary>
        /// Invoked when the window is loaded.
        /// </summary>
        public virtual void OnLoad() { }


        /// <summary>
        /// Invoked when the window is updated.
        /// </summary>
        public virtual void OnUpdate(FrameTime frameTime) { }


        /// <summary>
        /// Invoked when the window renders its content.
        /// </summary>
        public virtual void OnDraw(FrameTime frameTime) { }


        /// <summary>
        /// Invoked when the window size changes.
        /// </summary>
        public virtual void OnResize() { }
        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Protected Methods
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _window.Dispose();

                _isDisposed = true;
            }
        }
        #endregion
    }
}
