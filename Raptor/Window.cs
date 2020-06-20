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
        private readonly IWindow window;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The internal window implementation that manages a window.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside of constructor.")]
        public Window(IWindow window)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "IWindow must not be null");

            this.window = window;
            UpdateFrequency = 60;

            ContentLoader = new ContentLoader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
            };

            this.window = new GLWindow(gameWindowSettings, nativeWindowSettings)
            {
                Update = OnUpdate,
                Draw = OnDraw,
                Init = OnLoad,
                WinResize = OnResize,
            };
            UpdateFrequency = 60;

            ContentLoader = new ContentLoader();
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get => this.window.Title;
            set => this.window.Title = value;
        }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int Width
        {
            get => this.window.Width;
            set => this.window.Width = value;
        }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int Height
        {
            get => this.window.Height;
            set => this.window.Height = value;
        }

        /// <summary>
        /// Gets or sets the frequency of how often the window updates and draws
        /// in hertz.
        /// </summary>
        public int UpdateFrequency
        {
            get => this.window.UpdateFreq;
            set => this.window.UpdateFreq = value;
        }

        /// <summary>
        /// Gets the content loader for loading content.
        /// </summary>
        public IContentLoader? ContentLoader { get; private set; }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public void Show() => this.window.Show();

        /// <summary>
        /// Invoked when the window is loaded.
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Invoked when the window is updated.
        /// </summary>
        /// <param name="frameTime">The amount of time since the last frame.</param>
        public virtual void OnUpdate(FrameTime frameTime)
        {
        }

        /// <summary>
        /// Invoked when the window renders its content.
        /// </summary>
        /// <param name="frameTime">The amount of time since the last frame.</param>
        public virtual void OnDraw(FrameTime frameTime)
        {
        }

        /// <summary>
        /// Invoked when the window size changes.
        /// </summary>
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
                    this.window.Dispose();

                this.isDisposed = true;
            }
        }
    }
}
