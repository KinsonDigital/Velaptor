// <copyright file="Button.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Graphics;

    /// <summary>
    /// A button that can be clicked to execute functionality.
    /// </summary>
    public sealed class Button : ControlBase
    {
        private readonly IContentLoader contentLoader;
        private Label? label;
        private ITexture? texture;
        private string cachedText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads the button content for rendering.</param>
        public Button(IContentLoader contentLoader)
            => this.contentLoader = contentLoader ?? throw new ArgumentNullException(nameof(contentLoader));

        /// <inheritdoc cref="IControl"/>
        public override Point Position
        {
            get => base.Position;
            set
            {
                base.Position = value;

                CalculateLabelPosition();
            }
        }

        /// <inheritdoc cref="ISizable.Width"/>
        public override int Width => this.texture?.Width ?? 0;

        /// <inheritdoc cref="ISizable.Height"/>
        public override int Height => this.texture?.Height ?? 0;

        /// <summary>
        /// Gets or sets the text of the button.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public string Text
        {
            get => IsLoaded
                        ? this.label?.Text ?? string.Empty
                        : this.cachedText;
            set
            {
                if (this.label is null || IsLoaded is false)
                {
                    this.cachedText = value;
                    return;
                }

                this.label.Text = value;
            }
        }

        /// <inheritdoc cref="ControlBase.UnloadContent"/>
        /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
        public override void LoadContent()
        {
            ThrowExceptionIfLoadingWhenDisposed();

            if (IsLoaded)
            {
                return;
            }

            // TODO: Add label with simple text.  This means adding a text property
            this.texture = this.contentLoader.Load<ITexture>("button-face");
            this.label = new Label(this.contentLoader);
            this.label.LoadContent();
            this.label.Text = this.cachedText;

            CalculateLabelPosition();

            base.LoadContent();
        }

        /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            base.UnloadContent();
        }

        /// <inheritdoc cref="IDrawable.Render"/>
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

                this.label?.Render(spriteBatch);
            }

            base.Render(spriteBatch);
        }

        /// <inheritdoc cref="ControlBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !IsLoaded)
            {
                return;
            }

            if (disposing && this.texture is not null)
            {
                if (this.texture.IsPooled is false)
                {
                    this.texture.Dispose();
                }

                this.label?.Dispose();
                IsLoaded = false;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Calculates the position of the label on the button.
        /// </summary>
        private void CalculateLabelPosition()
        {
            if (this.label is null)
            {
                return;
            }

            /* Calculate the position of the label to be centered
             * horizontally and vertically over the face of the button.
            */
            var posX = (int)(Position.X + ((Width / 2f) - (this.label.Width / 2f)));
            var posY = (int)(Position.Y + ((Height / 2f) - (this.label.Height / 2f)));

            this.label.Position = new Point(posX, posY);
        }
    }
}
