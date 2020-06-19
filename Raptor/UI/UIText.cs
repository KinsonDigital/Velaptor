using Raptor.Graphics;
using System;
using System.Drawing;
using System.Numerics;

namespace Raptor.UI
{
    /// <summary>
    /// Represents a single piece of text rendered to a graphics surface.
    /// </summary>
    public class UIText
    {

        #region Private Fields
        private int _elapsedTime;//The amount of time that has elapsed since the last frame in miliseconds.
        private bool _updateText;//Indicates if the text can be updated.  Only updated if the UpdateFrequency value is >= to the elapsed time
        private RenderText? _labelText;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="UIText"/>.
        /// </summary>
        public UIText() => Position = Vector2.Zero;


        /// <summary>
        /// Creates a new instance of <see cref="UIText"/>.
        /// </summary>
        /// <param name="position">The position to to render the text item.</param>
        public UIText(Vector2 position) => Position = position;


        /// <summary>
        /// Creates a new instance of <see cref="UIText"/>.
        /// </summary>
        /// <param name="x">The X location of the text item.</param>
        /// <param name="y">The Y location of the text item.</param>
        public UIText(int x = 0, int y = 0) => Position = new Vector2(x, y);
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets a value indicating if the update frequency should be ignored.
        /// </summary>
        public bool IgnoreUpdateFrequency { get; set; } = true;

        /// <summary>
        /// Gets or sets the selected color of the text item.
        /// </summary>
        public Color SelectedColor { get; set; } = Color.FromArgb(255, 255, 255, 0);

        /// <summary>
        /// Gets or sets the name of the text item.
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the label section of the text item.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public RenderText? LabelText
        {
            get => _labelText;
            set
            {
                if (value is null)
                    throw new Exception($"The property '{nameof(LabelText)}' value must not be set to null.");

                value.Text += ": ";
                _labelText = value;
            }
        }

        /// <summary>
        /// Gets or sets the value section of the text.
        /// </summary>
        public RenderText? ValueText { get; set; }

        /// <summary>
        /// Gets or sets the position of the text on the graphics surface.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the text will render as selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the frequency in milliseconds that the text will get updated.
        /// </summary>
        public int UpdateFrequency { get; set; } = 62;

        /// <summary>
        /// Gets or sets the size of the text. <see cref="Vector2.X"/> is for the width and <see cref="Vector2.Y"/> is for the height.
        /// </summary>
        public Vector2 TextItemSize => new Vector2(Width, Height);

        /// <summary>
        /// Gets the width of the entire text.
        /// </summary>
        public int Width
        {
            get
            {
                var labelTextWidth = LabelText is null ? 0 : LabelText.Width;
                var valueTextWidth = ValueText is null ? 0 : ValueText.Width;


                return labelTextWidth + SectionSpacing + valueTextWidth;
            }
        }

        /// <summary>
        /// Gets the height of the entire text item.
        /// </summary>
        public int Height
        {
            get
            {
                var labelTextHeight = LabelText is null ? 0 : LabelText.Height;
                var valueTextHeight = ValueText is null ? 0 : ValueText.Height;


                return labelTextHeight > valueTextHeight ? labelTextHeight : valueTextHeight;
            }
        }

        /// <summary>
        /// Gets the location of the right side of the <see cref="UIText"/>.
        /// </summary>
        public int Right => (int)Position.X + Width;

        /// <summary>
        /// Gets the location of the bottom of the <see cref="UIText"/>.
        /// </summary>
        public int Bottom => (int)Position.Y + Height;

        /// <summary>
        /// Adds an additional amount of space to the vertical position of the label section of the text.
        /// </summary>
        public int VerticalLabelOffset { get; set; } = 0;

        /// <summary>
        /// Adds an additional amount of space to the vertical position of the value section of the text.
        /// </summary>
        public int VerticalValueOffset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the spacing between the label and value sections.
        /// </summary>
        public int SectionSpacing { get; set; } = 5;

        /// <summary>
        /// Gets or sets the color of the label section of the text.
        /// </summary>
        public Color LabelColor { get; set; } = Color.FromArgb(255, 0, 0, 0);

        /// <summary>
        /// Gets or sets the color of the value section of the text.
        /// </summary>
        public Color ValueColor { get; set; } = Color.FromArgb(255, 0, 0, 0);

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="UIText"/> item will render in the
        /// regular color or disabled color.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the forecolor of the <see cref="UIText"/> item when disabled.
        /// </summary>
        public Color DisabledForecolor { get; set; } = Color.FromArgb(255, 100, 100, 100);
        #endregion


        #region Public Methods
        /// <summary>
        /// Sets the text of the label section.
        /// </summary>
        /// <param name="text">The text to set the label section to.</param>
        public void SetLabelText(string text)
        {
            if (!(LabelText is null) && (_updateText || UpdateFrequency == 0 || IgnoreUpdateFrequency))
            {
                LabelText.Text = text;
                _updateText = false;
            }
        }


        /// <summary>
        /// Sets the text of the value section.
        /// </summary>
        /// <param name="text">The text to set the value section to.</param>
        public void SetValueText(string text)
        {
            if (!(ValueText is null) && (_updateText || UpdateFrequency == 0 || IgnoreUpdateFrequency))
            {
                ValueText.Text = text;
                _updateText = false;
            }
        }



        /// <summary>
        /// Updates the text item. This helps keep the update frequency up to date.
        /// </summary>
        /// <param name="frameTime">The game time of the last frame.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Update(FrameTime frameTime)
        {
            _elapsedTime += frameTime.ElapsedTime.Milliseconds;

            if (_elapsedTime >= UpdateFrequency)
            {
                _elapsedTime = 0;
                _updateText = true;
            }
        }


        /// <summary>
        /// Render the text item to the screen.
        /// </summary>
        /// <param name="renderer">The renderer to use to render the <see cref="UIText"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(RendererREFONLY renderer)
        {
            if (renderer is null)
                throw new Exception($"The renderer cannot be null.");

            if (!(LabelText is null))
                renderer.Render(LabelText, Position.X, Position.Y + VerticalLabelOffset);

            if (!(ValueText is null))
                renderer.Render(ValueText, Position.X + (LabelText is null ? 0 : LabelText.Width) + SectionSpacing, Position.Y + VerticalValueOffset);
        }
        #endregion
    }
}
