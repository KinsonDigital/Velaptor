// <copyright file="Button.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// A button that can be clicked to execute functionality.
    /// </summary>
    public sealed class Button : ControlBase
    {
        private readonly IContentLoader contentLoader;
        private readonly Color disabledColor = Color.FromArgb(255, 100, 100, 100);
        private Label? label;
        private ITexture? texture;
        private string cachedText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button() => this.contentLoader = ContentLoaderFactory.CreateContentLoader();

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads various kinds of content.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the any of the parameters below are null:
        ///     <list type="bullet">
        ///         <item><paramref name="contentLoader"/></item>
        ///     </list>
        /// </exception>
        internal Button(IContentLoader contentLoader) =>
            this.contentLoader =
                contentLoader ??
                throw new ArgumentNullException(nameof(contentLoader), "The parameter must not be null.");

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
        /// Gets the name of the texture to be displayed on the face of the button.
        /// </summary>
        /// <summary>This is the name of the texture that is rendered.</summary>
        public string FaceTextureName { get; init; } = "button-face-small";

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
            if (IsLoaded)
            {
                return;
            }

            /* TODO: Once the label can be injected into the constructor, setup caching for the
             * size property so that way it can be set before the LoadContent() method is called
             */

            this.texture = this.contentLoader.LoadTexture(FaceTextureName);
            this.label = new Label(this.contentLoader);
            this.label.LoadContent();
            this.label.Text = this.cachedText;

            this.label.Position = Position;

            base.LoadContent();
        }

        /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded)
            {
                return;
            }

            if (this.texture is not null)
            {
                this.contentLoader.UnloadTexture(this.texture);
            }

            this.label?.UnloadContent();

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
                spriteBatch.Render(this.texture, Position.X, Position.Y, Enabled ? TintColor : this.disabledColor);
                this.label?.Render(spriteBatch);
            }

            base.Render(spriteBatch);
        }
    }
}
