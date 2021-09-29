// <copyright file="Button.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Graphics;

    /// <summary>
    /// A button that can be clicked to execute functionality.
    /// </summary>
    public sealed class Button : ControlBase
    {
        private readonly IContentLoader contentLoader;
        private readonly IControl label;
        private ITexture? texture;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads the button content for rendering.</param>
        /// <param name="label">The label on the face of the button.</param>
        public Button(IContentLoader contentLoader, IControl label)
        {
            this.contentLoader = contentLoader;
            this.label = label;
        }

        /// <inheritdoc cref="IControl"/>
        public override Point Position
        {
            get => base.Position;
            set
            {
                base.Position = value;

                var posX = (int)(Position.X + ((Width / 2f) - (this.label.Width / 2f)));
                var posY = (int)(Position.Y + ((Height / 2f) - (this.label.Height / 2f)));

                this.label.Position = new Point(posX, posY);
            }
        }

        /// <summary>
        /// Gets the width of the <see cref="Button"/>.
        /// </summary>
        public override int Width => this.texture?.Width ?? 0;

        /// <summary>
        /// Gets the height of the <see cref="Button"/>.
        /// </summary>
        public override int Height => this.texture?.Height ?? 0;

        /// <summary>
        /// Loads the content for the <see cref="Button"/>.
        /// </summary>
        public override void LoadContent()
        {
            // TODO: Check if already loaded and return if it has been
            // TODO: Add label with simple text.  This means adding a text property
            this.texture = this.contentLoader.Load<ITexture>("button-face");
            this.label.LoadContent();

            base.LoadContent();
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

            if (this.texture is not null)
            {
                var posX = Position.X + (Width / 2);
                var posY = Position.Y + (Height / 2);

                spriteBatch.Render(this.texture, posX, posY, TintColor);

                this.label.Render(spriteBatch);
            }

            base.Render(spriteBatch);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable" />
        /// </summary>
        public override void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable"/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.texture?.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
