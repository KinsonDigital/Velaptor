// <copyright file="Button.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Input;

namespace Velaptor.UI
{
    using System;
    using System.Drawing;
    using System.Numerics;
    using Content;
    using Graphics;

    /// <summary>
    /// A button that can be clicked to execute functionality.
    /// </summary>
    public class Button : ControlBase
    {
        private readonly ILoader<ITexture> textureLoader;
        private readonly string textureName;
        private readonly Mouse mouse;
        private ITexture? texture;
        private Color tintColor = Color.White;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private Point currentMousePos;
        private Point previousMousePos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="textureName">The textureName to be displayed when the mouse is over the button.</param>
        public Button(string textureName)
        {
            this.mouse = new Mouse();
            this.textureName = textureName;
            this.textureLoader = Factories.ContentLoaderFactory.CreateTextureLoader();
        }

        /// <summary>
        /// Occurs when the button has been clicked.
        /// </summary>
        public event EventHandler<EventArgs>? Click;

        /// <summary>
        /// Occurs when the left mouse button is in the down position over the button.
        /// </summary>
        public event EventHandler<EventArgs>? MouseDown;

        /// <summary>
        /// Occurs when the left mouse button is in the up position over the button
        /// after the mouse has been in the down position.
        /// </summary>
        public event EventHandler<EventArgs>? MouseUp;

        /// <summary>
        /// Occurs when the mouse moves over the button.
        /// </summary>
        // TODO: This event should be managed in the ControlBase
        public event EventHandler<MousePositionEventArgs>? MouseMove;

        /// <summary>
        /// Gets the width of the <see cref="Button"/>.
        /// </summary>
        public override int Width => this.texture?.Width ?? 0;

        /// <summary>
        /// Gets the height of the <see cref="Button"/>.
        /// </summary>
        public override int Height => this.texture?.Height ?? 0;

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the mouse is hovering over the button.
        /// </summary>
        // TODO: Can be put into the ControlBase and made protected as well as the value set in ControlBase
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// Initializes the <see cref="Button"/>.
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// Loads the content for the <see cref="Button"/>.
        /// </summary>
        public override void LoadContent()
        {
            this.texture = this.textureLoader.Load(this.textureName);
            base.LoadContent();
        }

        /// <summary>
        /// Updates the <see cref="Button"/>.
        /// </summary>
        /// <param name="frameTime">The update iteration time.</param>
        public override void Update(FrameTime frameTime)
        {
            if (IsLoaded is false || Enabled is false)
            {
                return;
            }

            this.currentMouseState = this.mouse.GetState();
            this.currentMousePos = this.currentMouseState.GetPosition().ToPoint();
            var controlRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            IsMouseOver = controlRect.Contains(this.currentMouseState.GetX(), this.currentMouseState.GetY());

            // If the mouse button is in the down position
            if (this.currentMouseState.IsLeftButtonDown() && IsMouseOver)
            {
                this.tintColor = Color.FromArgb(255, 190, 190, 190);
            }
            else
            {
                this.tintColor = IsMouseOver ? Color.FromArgb(255, 230, 230, 230) : Color.White;
            }

            if (IsMouseOver)
            {
                // Invoked teh mouse move event if the mouse has been moved
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
        /// Renders the <see cref="Button"/> to the screen.
        /// </summary>
        /// <param name="spriteBatch">Renders the <see cref="Button"/>.</param>
        public override void Render(ISpriteBatch spriteBatch)
        {
            if (IsLoaded is false || Visible is false)
            {
                return;
            }

            var posX = (int)Position.X + (int)(Width / 2f);
            var posY = (int)Position.Y + (int)(Height / 2f);

            spriteBatch.Render(this.texture, posX, posY, this.tintColor);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.texture.Dispose();
        }
    }
}
