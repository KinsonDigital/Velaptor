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
        private readonly Dictionary<char, int> glyphHeights = new ();
        private readonly Dictionary<char, int> glyphWidths = new ();
        private IFont? font;
        private string? labelText = string.Empty;
        private bool isDisposed;

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
            get => this.labelText ?? string.Empty;
            set
            {
                this.labelText = value;

                LoadGlyphSizes();
                Width = CalculateWidth(this.labelText);
                Height = CalculateHeight(this.labelText);
            }
        }

        /// <summary>
        /// Gets or sets the font style of the text.
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        /// <inheritdoc cref="ControlBase"/>
        public override void LoadContent()
        {
            this.font = this.contentLoader?.Load<IFont>("TimesNewRoman");

            LoadGlyphSizes();

            Width = CalculateWidth(this.labelText);
            Height = CalculateHeight(this.labelText);

            base.LoadContent();
        }

        /// <inheritdoc cref="IControl"/>
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
                var posY = Position.Y + Height;
                spriteBatch.Render(this.font, Text, Position.X, posY, Color);
            }

            base.Render(spriteBatch);
        }

        /// <inheritdoc cref="IControl"/>
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
                this.font?.Dispose();
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Loads the glyph sizes based on the current glyphs in the label text.
        /// </summary>
        private void LoadGlyphSizes()
        {
            this.glyphWidths.Clear();
            this.glyphHeights.Clear();

            if (this.font is null)
            {
                return;
            }

            var widths = this.font.Metrics.Select(m => new KeyValuePair<char, int>(m.Glyph, m.GlyphWidth));

            foreach (var (glyph, width) in widths)
            {
                if (this.glyphWidths.ContainsKey(glyph) is false)
                {
                    this.glyphWidths.Add(glyph, width);
                }
            }

            var heights = this.font.Metrics.Select(m => new KeyValuePair<char, int>(m.Glyph, m.GlyphHeight));

            foreach (var (glyph, height) in heights)
            {
                if (this.glyphHeights.ContainsKey(glyph) is false)
                {
                    this.glyphHeights.Add(glyph, height);
                }
            }
        }

        /// <summary>
        /// Calculates the width of the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to measure the width from.</param>
        /// <returns>The width of the text.</returns>
        private int CalculateWidth(string? text)
            => string.IsNullOrEmpty(text) || this.glyphWidths.Count <= 0
                ? 0
                : text.Select(character => this.glyphWidths[character]).Sum();

        /// <summary>
        /// Calculates the height of the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to measure the height from.</param>
        /// <returns>The height of the text.</returns>
        private int CalculateHeight(string? text)
            => string.IsNullOrEmpty(text) || this.glyphHeights.Count <= 0
                ? 0
                : text.Select(character => this.glyphHeights[character]).Prepend(int.MinValue).Max();
    }
}
