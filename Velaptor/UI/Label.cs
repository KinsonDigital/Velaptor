
namespace Velaptor.UI
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Velaptor.Graphics;

    // TODO: Create a ControlBase abstract type to inherit IControl instead
    public class Label : ControlBase, ISizable
    {
        private readonly ILoader<IFont> contentLoader;
        private Dictionary<char, int> glyphHeights = new ();
        private Dictionary<char, int> glyphWidths = new ();
        private IFont font;
        private string text = string.Empty;

        public Label()
        {
            this.contentLoader = ContentLoaderFactory.CreateFontLoader();
        }

        /// <summary>
        /// Gets or sets the text of the label.
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;

                Width = CalculateWidth(this.text);
                Height = CalculateHeight(this.text);
            }
        }

        public FontStyle Style { get; set; }

        public Color Color { get; set; } = Color.Black;

        public void Initialize() => throw new System.NotImplementedException();

        public override void LoadContent()
        {
            this.font = this.contentLoader.Load("TimesNewRoman");
            this.glyphWidths = new Dictionary<char, int>(this.font.Metrics.Select(m => new KeyValuePair<char, int>(m.Glyph, m.GlyphWidth)));
            this.glyphHeights = new Dictionary<char, int>(this.font.Metrics.Select(m => new KeyValuePair<char, int>(m.Glyph, m.GlyphHeight)));

            Width = CalculateWidth(this.text);
            Height = CalculateHeight(this.text);

            base.LoadContent();
        }

        public override void Update(FrameTime frameTime)
        {
            if (IsLoaded is false || Enabled is false)
            {
                return;
            }

            base.Update(frameTime);
        }

        public override void Render(ISpriteBatch spriteBatch)
        {
            if (IsLoaded is false || Visible is false)
            {
                return;
            }

            var posY = (int)(Position.Y + (Height / 2f));
            spriteBatch.Render(this.font, Text, (int)Position.X, posY, Color);

            base.Render(spriteBatch);
        }

        public void Dispose() => this.font.Dispose();

        private int CalculateWidth(string text)
            => text.Length == 0 || this.glyphWidths.Count <= 0
                ? 0
                : text.Select(character => this.glyphWidths[character]).Sum();

        private int CalculateHeight(string text)
            => text.Length == 0 || this.glyphHeights.Count <= 0
                ? 0
                : text.Select(character => this.glyphHeights[character]).Prepend(int.MinValue).Max();
    }
}
