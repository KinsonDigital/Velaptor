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

                if (this.label is not null)
                {
                    this.label.Position = Position;
                }
            }
        }

        /// <inheritdoc cref="ISizable.Width"/>
        public override uint Width => this.texture?.Width ?? 0;

        /// <inheritdoc cref="ISizable.Height"/>
        public override uint Height => this.texture?.Height ?? 0;

        /// <summary>
        /// Gets or sets the name of the texture to be displayed on the face of the button.
        /// </summary>
        /// <summary>This is the name of the texture that is rendered.</summary>
        public string FaceTextureName { get; set; } = "button-face-small";

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

        /// <summary>
        /// Gets or sets the size of the text on the face of the button.
        /// </summary>
        public float Size
        {
            get => this.label?.Size ?? 0f;
            set
            {
                if (this.label != null)
                {
                    this.label.Size = value;
                }
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

            // TODO: Once the label can be injected into the constructor, setup caching for the
            // size property so that way it can be set before the LoadContent() method is called

            this.texture = this.contentLoader.Load<ITexture>(FaceTextureName);
            this.label = new Label(this.contentLoader);
            this.label.LoadContent();
            this.label.Text = this.cachedText;

            this.label.Position = Position;

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
                spriteBatch.Render(this.texture, Position.X, Position.Y, TintColor);

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
    }
}
