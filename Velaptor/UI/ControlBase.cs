// <copyright file="ControlBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.Input;

    /// <summary>
    /// Represents a basic control with behavior that is shared among all controls.
    /// </summary>
    public abstract class ControlBase : IControl
    {
        private readonly Mouse mouse;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private Point currentMousePos;
        private Point previousMousePos;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlBase"/> class.
        /// </summary>
        protected ControlBase() => this.mouse = new Mouse();

        /// <inheritdoc cref="IControl.Click"/>
        public event EventHandler<EventArgs>? Click;

        /// <inheritdoc cref="IControl.MouseDown"/>
        public event EventHandler<EventArgs>? MouseDown;

        /// <inheritdoc cref="IControl.MouseUp"/>
        public event EventHandler<EventArgs>? MouseUp;

        /// <inheritdoc cref="IControl.MouseMove"/>
        public event EventHandler<MousePositionEventArgs>? MouseMove;

        /// <inheritdoc cref="IControl.Name"/>
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc cref="IControl.Position"/>
        public virtual Point Position { get; set; }

        /// <inheritdoc cref="IControl.Left"/>
        public virtual int Left
        {
            get => Position.X;
            set => Position = new Point(value, Position.Y);
        }

        /// <inheritdoc cref="IControl.Right"/>
        public virtual int Right
        {
            get => Position.X + (int)Width;
            set => Position = new Point(value - (int)Width, Position.Y);
        }

        /// <inheritdoc cref="IControl.Top"/>
        public virtual int Top
        {
            get => Position.Y;
            set => Position = new Point(Position.X, value);
        }

        /// <inheritdoc cref="IControl.Bottom"/>
        public virtual int Bottom
        {
            get => Position.Y + (int)Height;
            set => Position = new Point(Position.X, value - (int)Height);
        }

        /// <inheritdoc cref="ISizable.Width"/>
        public virtual uint Width { get; set; }

        /// <inheritdoc cref="ISizable.Height"/>
        public virtual uint Height { get; set; }

        /// <inheritdoc cref="IControl.Visible"/>
        public virtual bool Visible { get; set; } = true;

        /// <inheritdoc cref="IControl.Enabled"/>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the mouse is hovering over the button.
        /// </summary>
        public bool IsMouseOver { get; private set; }

        /// <inheritdoc cref="IContentLoadable.IsLoaded"/>
        public bool IsLoaded { get; protected set; }

        /// <summary>
        /// Gets or sets the color to apply to the control when the
        /// mouse button is in the down position over the control.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used by library users.")]
        public Color MouseDownColor { get; set; } = Color.FromArgb(255, 190, 190, 190);

        /// <summary>
        /// Gets or sets the color to apply to the control when the
        /// mouse button is hovering over the control.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used by library users.")]
        public Color MouseHoverColor { get; set; } = Color.FromArgb(255, 230, 230, 230);

        /// <summary>
        /// Gets the tint color to apply the control surface when the mouse hovers over the control.
        /// </summary>
        /// <remarks>
        ///     This is used to signify to the user that the mouse is hovering over the control or
        ///     the mouse button is in the down position over the control.
        /// </remarks>
        protected Color TintColor { get; private set; } = Color.White;

        /// <summary>
        /// Gets a value indicating whether the control has been disposed.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected bool IsDisposed { get; private set; }

        /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
        public virtual void LoadContent() => IsLoaded = true;

        /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
        public virtual void UnloadContent() => IsLoaded = false;

        /// <inheritdoc cref="IControl"/>
        public virtual void Update(FrameTime frameTime)
        {
            if (IsLoaded is false || Enabled is false)
            {
                return;
            }

            this.currentMouseState = this.mouse.GetState();
            this.currentMousePos = this.currentMouseState.GetPosition().ToPoint();
            var controlRect = new Rectangle(Position.X, Position.Y, (int)Width, (int)Height);

            IsMouseOver = controlRect.Contains(this.currentMouseState.GetX(), this.currentMouseState.GetY());

            // If the mouse button is in the down position
            if (this.currentMouseState.IsLeftButtonDown() && IsMouseOver)
            {
                TintColor = MouseDownColor;
            }
            else
            {
                TintColor = IsMouseOver ? MouseHoverColor : Color.White;
            }

            if (IsMouseOver)
            {
                // Invoked the mouse move event if the mouse has been moved
                if (this.currentMousePos != this.previousMousePos)
                {
                    var relativePos = new Point(Position.X - this.currentMousePos.X, Position.Y - this.currentMousePos.Y);
                    this.MouseMove?.Invoke(this, new MousePositionEventArgs(relativePos));
                }

                if (this.currentMouseState.IsLeftButtonDown())
                {
                    this.MouseDown?.Invoke(this, EventArgs.Empty);
                }
            }

            if (this.currentMouseState.IsLeftButtonUp() &&
                this.previousMouseState.IsLeftButtonDown() &&
                IsMouseOver)
            {
                this.MouseUp?.Invoke(this, EventArgs.Empty);
                this.Click?.Invoke(this, EventArgs.Empty);
            }

            this.previousMouseState = this.currentMouseState;
            this.previousMousePos = this.currentMousePos;
        }

        /// <summary>
        /// Renders the control to the screen.
        /// </summary>
        /// <param name="spriteBatch">Renders the control.</param>
        [ExcludeFromCodeCoverage]
        public virtual void Render(ISpriteBatch spriteBatch)
        {
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        /// <summary>
        /// Throws an exception if the control is being loaded when it has already been disposed.
        /// </summary>
        /// <exception cref="Exception">Thrown when the control has been disposed.</exception>
        protected void ThrowExceptionIfLoadingWhenDisposed()
        {
            if (IsDisposed)
            {
                throw new Exception("Cannot load a control that has been disposed.");
            }
        }
    }
}
