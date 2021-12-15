// <copyright file="Label.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Velaptor.Content;
    using Velaptor.Graphics;

    /// <summary>
    /// A simple label that renders text on the screen.
    /// </summary>
    public sealed class Label : ControlBase
    {
        private readonly IContentLoader? contentLoader;
        private IFont? font;
        private string labelText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for rendering the label.</param>
        public Label(IContentLoader contentLoader) => this.contentLoader = contentLoader;

        /// <summary>
        /// Gets or sets the labelText of the label.
        /// </summary>
        public string Text
        {
            get => this.labelText;
            set
            {
                this.labelText = value;

                UpdateLabelSize();
            }
        }

        public override int Left
        {
            get => (int)(Position.X - (Width / 2f));
            set => Position = new Point((int)(value + (Width / 2f)), Position.Y);
        }

        public override int Right
        {
            get => (int)(Position.X - (Width / 2f));
            set => Position = new Point((int)(value - (Width / 2f)), Position.Y);
        }

        public override int Top
        {
            get => (int)(Position.Y - (Height / 2f));
            set => Position = new Point(Position.X, (int)(value + (Height / 2f)));
        }

        public override int Bottom
        {
            get => (int)(Position.Y - (Height / 2f));
            set => Position = new Point(Position.X, (int)(value - (Height / 2f)));
        }

        /// <summary>
        /// Gets or sets the font style of the text.
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        /// <inheritdoc cref="ControlBase.UnloadContent"/>
        /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
        public override void LoadContent()
        {
            ThrowExceptionIfLoadingWhenDisposed();

            if (IsLoaded)
            {
                return;
            }

            this.font = this.contentLoader?.Load<IFont>("TimesNewRoman");

            UpdateLabelSize();

            base.LoadContent();
        }

        /// <inheritdoc cref="ControlBase.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            this.font?.Dispose();

            base.UnloadContent();
        }

        /// <inheritdoc cref="IDrawable.Render"/>
        /// <exception cref="ArgumentNullException">Invoked if the <paramref name="spriteBatch"/> is null.</exception>
        public override void Render(ISpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
            {
                throw new ArgumentNullException(nameof(spriteBatch), "The parameter must not be null.");
            }

            if (IsLoaded is false || Visible is false)
            {
                return;
            }

            if (this.font is not null)
            {
                spriteBatch.Render(this.font, Text, Position.X, Position.Y, Color);
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

            if (disposing)
            {
                if (this.font?.IsPooled is false)
                {
                    this.font?.Dispose();
                    IsLoaded = false;
                }
            }

            base.Dispose(true);
        }

        /// <summary>
        /// Updates the width and height of the label by measuring the size of the text.
        /// </summary>
        private void UpdateLabelSize()
        {
            var textSize = this.font?.Measure(this.labelText);

            Width = (uint)(textSize?.Width ?? 0u);
            Height = (uint)(textSize?.Height ?? 0u);
        }
    }
}
