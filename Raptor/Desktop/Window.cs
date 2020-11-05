// <copyright file="Window.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Desktop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using Raptor.Content;

    /// <summary>
    /// A system window that graphics can be rendered to.
    /// </summary>
    public abstract class Window : IWindowProps, IDisposable
    {
        private readonly IWindow window;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The window implementation that contains the window functionality.</param>
        public Window(IWindow window)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window), "Window must not be null.");

            this.window = window;
            this.window.Init = OnLoad;
            this.window.Update = OnUpdate;
            this.window.Draw = OnDraw;
            this.window.WinResize = OnResize;
            this.window.UpdateFreq = 60;
        }

        /// <inheritdoc/>
        public string Title
        {
            get => this.window.Title;
            set => this.window.Title = value;
        }

        /// <inheritdoc/>
        public Vector2 Position
        {
            get => this.window.Position;
            set => this.window.Position = value;
        }

        /// <inheritdoc/>
        public int Width
        {
            get => this.window.Width;
            set => this.window.Width = value;
        }

        /// <inheritdoc/>
        public int Height
        {
            get => this.window.Height;
            set => this.window.Height = value;
        }

        /// <inheritdoc/>
        public int UpdateFrequency
        {
            get => this.window.UpdateFreq;
            set => this.window.UpdateFreq = value;
        }

        /// <inheritdoc/>
        public bool AutoClearBuffer
        {
            get => this.window.AutoClearBuffer;
            set => this.window.AutoClearBuffer = value;
        }

        /// <inheritdoc/>
        public bool MouseCursorVisible
        {
            get => this.window.MouseCursorVisible;
            set => this.window.MouseCursorVisible = value;
        }

        /// <inheritdoc/>
        public StateOfWindow WindowState
        {
            get => this.window.WindowState;
            set => this.window.WindowState = value;
        }

        /// <inheritdoc/>
        public BorderType TypeOfBorder
        {
            get => this.window.TypeOfBorder;
            set => this.window.TypeOfBorder = value;
        }

        /// <summary>
        /// Gets the content loader for loading content.
        /// </summary>
        public IContentLoader? ContentLoader { get; protected set; }

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
