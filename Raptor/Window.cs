// <copyright file="Window.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Content;

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
        /// <param name="window">The window implementation that contains the window functionality.</param>
        /// <param name="contentLoader">Loads content.</param>
        public Window(IWindow window, IContentLoader? contentLoader)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "Window must not be null.");

            if (contentLoader is null)
                throw new ArgumentNullException(nameof(contentLoader), "Content loader must not be null.");

            this.window = window;
            this.window.Init = OnLoad;
            this.window.Update = OnUpdate;
            this.window.Draw = OnDraw;
            this.window.WinResize = OnResize;
            this.window.UpdateFreq = 60;

            ContentLoader = contentLoader;
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
                    this.window.Dispose();

                this.isDisposed = true;
            }
        }
    }
}
