// <copyright file="Label.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Factories;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// A simple label that renders text on the screen.
    /// </summary>
    public sealed class Label : ControlBase
    {
        private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
        private readonly IContentLoader contentLoader;
        private IFont? font;
        private string labelText = string.Empty;
        private FontStyle cachedStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Label() => this.contentLoader = ContentLoaderFactory.CreateContentLoader();

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads various kinds of content.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the any of the parameters below are null:
        ///     <list type="bullet">
        ///         <item><paramref name="contentLoader"/></item>
        ///     </list>
        /// </exception>
        internal Label(IContentLoader contentLoader) =>
            this.contentLoader =
                contentLoader ??
                throw new ArgumentNullException(nameof(contentLoader), "The parameter must not be null.");

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

        /// <inheritdoc cref="ControlBase.Left"/>
        public override int Left
        {
            get => (int)(Position.X - (Width / 2f));
            set => Position = new Point((int)(value + (Width / 2f)), Position.Y);
        }

        /// <inheritdoc cref="ControlBase.Right"/>
        public override int Right
        {
            get => (int)(Position.X + (Width / 2f));
            set => Position = new Point((int)(value - (Width / 2f)), Position.Y);
        }

        /// <inheritdoc cref="ControlBase.Top"/>
        public override int Top
        {
            get => (int)(Position.Y - (Height / 2f));
            set => Position = new Point(Position.X, (int)(value + (Height / 2f)));
        }

        /// <inheritdoc cref="ControlBase.Bottom"/>
        public override int Bottom
        {
            get => (int)(Position.Y + (Height / 2f));
            set => Position = new Point(Position.X, (int)(value - (Height / 2f)));
        }

        /// <summary>
        /// Gets or sets the font style of the text.
        /// </summary>
        public FontStyle Style
        {
            get => this.font?.Style ?? this.cachedStyle;
            set
            {
                if (this.font is null)
                {
                    this.cachedStyle = value;
                }
                else
                {
                    this.font.Style = this.cachedStyle;
                    this.cachedStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the size of the text of the label.
        /// </summary>
        public float Size { get; set; } = 1f;

        /// <inheritdoc cref="ControlBase.UnloadContent"/>
        /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
        public override void LoadContent()
        {
            if (IsLoaded)
            {
                return;
            }

            this.font = this.contentLoader.LoadFont(DefaultRegularFont, 12);

            UpdateLabelSize();

            base.LoadContent();
        }

        /// <inheritdoc cref="ControlBase.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded)
            {
                return;
            }

            if (this.font is not null)
            {
                this.contentLoader.UnloadFont(this.font);
            }

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
                spriteBatch.Render(this.font, Text, Position.X, Position.Y, Size, 0f, Color);
            }

            base.Render(spriteBatch);
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
