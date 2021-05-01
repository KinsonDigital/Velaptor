// <copyright file="UIText.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.UI
{
    using System;
    using System.Drawing;
    using System.Numerics;
    using Raptor.Graphics;

    /// <summary>
    /// Represents a single piece of text rendered to a graphics surface.
    /// </summary>
    public class UIText
    {
        private int elapsedTime; // The amount of time that has elapsed since the last frame in milliseconds.
        private bool updateText; // Indicates if the text can be updated.  Only updated if the UpdateFrequency value is >= to the elapsed time
        private RenderText? labelText;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIText"/> class.
        /// </summary>
        public UIText() => Position = Vector2.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIText"/> class.
        /// </summary>
        /// <param name="position">The position to to render the text item.</param>
        public UIText(Vector2 position) => Position = position;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIText"/> class.
        /// </summary>
        /// <param name="x">The X location of the text item.</param>
        /// <param name="y">The Y location of the text item.</param>
        public UIText(int x = 0, int y = 0) => Position = new Vector2(x, y);

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value indicating if the update frequency should be ignored.
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
        public RenderText? LabelText
        {
            get => this.labelText;
            set
            {
                if (value is null)
                {
                    throw new Exception($"The property '{nameof(LabelText)}' value must not be set to null.");
                }

                value.Text += ": ";
                this.labelText = value;
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
        /// Gets or sets a value indicating whether gets or sets a value indicating if the text will render as selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the frequency in milliseconds that the text will get updated.
        /// </summary>
        public int UpdateFrequency { get; set; } = 62;

        /// <summary>
        /// Gets the size of the text. <see cref="Vector2.X"/> is for the width and <see cref="Vector2.Y"/> is for the height.
        /// </summary>
        public Vector2 TextItemSize => new (Width, Height);

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
        /// Gets or sets an additional amount of space to the vertical position of the label section of the text.
        /// </summary>
        public int VerticalLabelOffset { get; set; }

        /// <summary>
        /// Gets or sets an additional amount of space to the vertical position of the value section of the text.
        /// </summary>
        public int VerticalValueOffset { get; set; }

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
        /// Gets or sets a value indicating whether gets or sets a value indicating if the <see cref="UIText"/> item will render in the
        /// regular color or disabled color.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the fore color of the <see cref="UIText"/> item when disabled.
        /// </summary>
        public Color DisabledForecolor { get; set; } = Color.FromArgb(255, 100, 100, 100);

        /// <summary>
        /// Sets the text of the label section.
        /// </summary>
        /// <param name="text">The text to set the label section to.</param>
        public void SetLabelText(string text)
        {
            if (LabelText is not null && (this.updateText || UpdateFrequency == 0 || IgnoreUpdateFrequency))
            {
                LabelText.Text = text;
                this.updateText = false;
            }
        }

        /// <summary>
        /// Sets the text of the value section.
        /// </summary>
        /// <param name="text">The text to set the value section to.</param>
        public void SetValueText(string text)
        {
            if (ValueText is not null && (this.updateText || UpdateFrequency == 0 || IgnoreUpdateFrequency))
            {
                ValueText.Text = text;
                this.updateText = false;
            }
        }

        /// <summary>
        /// Updates the text item. This helps keep the update frequency up to date.
        /// </summary>
        /// <param name="frameTime">The update iteration time of the last frame.</param>
        public void Update(FrameTime frameTime)
        {
            this.elapsedTime += frameTime.ElapsedTime.Milliseconds;

            if (this.elapsedTime >= UpdateFrequency)
            {
                this.elapsedTime = 0;
                this.updateText = true;
            }
        }

        /// <summary>
        /// Render the text item to the screen.
        /// </summary>
        /// <param name="renderer">The renderer to use to render the <see cref="UIText"/>.</param>
        public void Render(object renderer)
        {
            // if (renderer is null)
            //    throw new Exception($"The renderer cannot be null.");

            // if (LabelText is not null)
            //    renderer.Render(LabelText, Position.X, Position.Y + VerticalLabelOffset);

            // if (ValueText is not null)
            //    renderer.Render(ValueText, Position.X + (LabelText is null ? 0 : LabelText.Width) + SectionSpacing, Position.Y + VerticalValueOffset);
        }
    }
}
