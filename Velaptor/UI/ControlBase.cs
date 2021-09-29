// <copyright file="ControlBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
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

#pragma warning disable SA1623
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlBase"/> class.
        /// </summary>
        protected ControlBase() => this.mouse = new Mouse();

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public event EventHandler<EventArgs>? Click;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public event EventHandler<EventArgs>? MouseDown;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public event EventHandler<EventArgs>? MouseUp;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public event EventHandler<MousePositionEventArgs>? MouseMove;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual Point Position { get; set; }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Left
        {
            get => Position.X;
            set => Position = new Point(value, Position.Y);
        }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Right
        {
            get => Position.X + Width;
            set => Position = new Point(value - Width, Position.Y);
        }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Top
        {
            get => Position.Y;
            set => Position = new Point(Position.X, value);
        }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Bottom
        {
            get => Position.Y + Height;
            set => Position = new Point(Position.X, value - Height);
        }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the mouse is hovering over the button.
        /// </summary>
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the color to tint the control surface.
        /// </summary>
        protected Color TintColor { get; set; } = Color.White;

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual void LoadContent()
        {
            if (IsLoaded)
            {
                return;
            }

            IsLoaded = true;
        }

        /// <inheritdoc cref="IControl"/>
        public virtual void Update(FrameTime frameTime)
        {
            if (IsLoaded is false || Enabled is false)
            {
                return;
            }

            this.currentMouseState = this.mouse.GetState();
            this.currentMousePos = this.currentMouseState.GetPosition().ToPoint();
            var controlRect = new Rectangle(Position.X, Position.Y, Width, Height);

            IsMouseOver = controlRect.Contains(this.currentMouseState.GetX(), this.currentMouseState.GetY());

            // If the mouse button is in the down position
            if (this.currentMouseState.IsLeftButtonDown() && IsMouseOver)
            {
                TintColor = Color.FromArgb(255, 190, 190, 190);
            }
            else
            {
                TintColor = IsMouseOver ? Color.FromArgb(255, 230, 230, 230) : Color.White;
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

        /// <inheritdoc cref="IControl"/>
        [ExcludeFromCodeCoverage]
        public virtual void Render(ISpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// <inheritdoc cref="IControl"/>
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual void Dispose() => GC.SuppressFinalize(this);
#pragma warning restore SA1623
    }
}
