// <copyright file="Window.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OpenToolkit.Mathematics;
    using OpenToolkit.Windowing.Desktop;
    using Raptor.Content;
    using Raptor.OpenGL;

    /// <summary>
    /// A system window that graphics can be rendered to.
    /// </summary>
    public abstract class Window : IDisposable
    {
        private IWindow? window;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The window implementation that contains the window functionality.</param>
        /// <param name="contentLoader">Loads content.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception messages only used inside constructor.")]
        public Window(IWindow? window, IContentLoader? contentLoader)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "Window must not be null.");

            if (contentLoader is null)
                throw new ArgumentNullException(nameof(contentLoader), "Content loader must not be null.");

            this.window = window;
            ContentLoader = contentLoader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception messages only used inside constructor.")]
        public Window(IContentLoader contentLoader, int width = 800, int height = 600)
        {
            if (contentLoader is null)
                throw new ArgumentNullException(nameof(contentLoader), "Content loader must not be null.");

            ContentLoader = contentLoader;
            InitWindow(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The internal window implementation that manages a window.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside of constructor.")]
        [ExcludeFromCodeCoverage]
        public Window(IWindow window)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "Window must not be null.");

            this.window = window;
            ContentLoader = new ContentLoader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        [ExcludeFromCodeCoverage]
        public Window(int width, int height)
        {
            ContentLoader = new ContentLoader();
            InitWindow(width, height);
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get => this.window is null ? string.Empty : this.window.Title;
            set
            {
                if (this.window is null)
                    return;

                this.window.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int Width
        {
            get => this.window is null ? 0 : this.window.Width;
            set
            {
                if (this.window is null)
                    return;

                this.window.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int Height
        {
            get => this.window is null ? 0 : this.window.Height;
            set
            {
                if (this.window is null)
                    return;

                this.window.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the frequency of how often the window updates and draws
        /// in hertz.
        /// </summary>
        public int UpdateFrequency
        {
            get => this.window is null ? 0 : this.window.UpdateFreq;
            set
            {
                if (this.window is null)
                    return;

                this.window.UpdateFreq = value;
            }
        }

        /// <summary>
        /// Gets the content loader for loading content.
        /// </summary>
        public IContentLoader? ContentLoader { get; private set; }

        /// <summary>
        /// Shows the window.
        /// </summary>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside of method.")]
        public void Show()
        {
            if (this.window is null)
                throw new Exception("Internal window implementation not set.");

            this.window.Show();
        }

        /// <summary>
        /// Invoked when the window is loaded.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Invoked when the window is updated.
        /// </summary>
        /// <param name="frameTime">The amount of time since the last frame.</param>
        [ExcludeFromCodeCoverage]
        public virtual void OnUpdate(FrameTime frameTime)
        {
        }

        /// <summary>
        /// Invoked when the window renders its content.
        /// </summary>
        /// <param name="frameTime">The amount of time since the last frame.</param>
        [ExcludeFromCodeCoverage]
        public virtual void OnDraw(FrameTime frameTime)
        {
        }

        /// <summary>
        /// Invoked when the window size changes.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual void OnResize()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                    this.window?.Dispose();

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        [ExcludeFromCodeCoverage]
        private void InitWindow(int width, int height)
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
            };

            this.window = new GLWindow(gameWindowSettings, nativeWindowSettings)
            {
                Update = OnUpdate,
                Draw = OnDraw,
                Init = OnLoad,
                WinResize = OnResize,
                UpdateFrequency = 60,
            };
        }
    }
}
