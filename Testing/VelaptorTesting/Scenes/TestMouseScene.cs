// <copyright file="TestMouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.Input;
    using Velaptor.UI;
    using VelaptorTesting.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Used to test that the mouse works correctly.
    /// </summary>
    public class TestMouseScene : SceneBase
    {
        private const int LeftMargin = 5;
        private const int TopMargin = 25;
        private const int LineSpacing = 20;
        private readonly Mouse mouse;
        private readonly Label mousePosLabel;
        private readonly Label mouseLeftButtonLabel;
        private readonly Label mouseRightButtonLabel;
        private readonly Label mouseMiddleButtonLabel;
        private readonly Label mouseWheelValueLabel;
        private MouseState currentMouseState;
        private Dictionary<char, int>? glyphHeights;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMouseScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestMouseScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
            this.mouse = new Mouse();
            this.mousePosLabel = new Label(ContentLoader) { Color = Color.White };
            this.mouseLeftButtonLabel = new Label(ContentLoader) { Color = Color.White };
            this.mouseRightButtonLabel = new Label(ContentLoader) { Color = Color.White };
            this.mouseMiddleButtonLabel = new Label(ContentLoader) { Color = Color.White };
            this.mouseWheelValueLabel = new Label(ContentLoader) { Color = Color.White };
        }

        /// <summary>
        /// Loads the content for the scene.
        /// </summary>
        public override void Load()
        {
            var font = ContentLoader.Load<IFont>("TimesNewRoman");
            this.glyphHeights = new Dictionary<char, int>(font.Metrics.Select(m => new KeyValuePair<char, int>(m.Glyph, m.GlyphHeight)));

            this.mousePosLabel.LoadContent();
            this.mouseLeftButtonLabel.LoadContent();
            this.mouseRightButtonLabel.LoadContent();
            this.mouseMiddleButtonLabel.LoadContent();
            this.mouseWheelValueLabel.LoadContent();

            AddControl(this.mousePosLabel);
            AddControl(this.mouseLeftButtonLabel);
            AddControl(this.mouseRightButtonLabel);
            AddControl(this.mouseMiddleButtonLabel);
            AddControl(this.mouseWheelValueLabel);

            base.Load();
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="frameTime">The amount of time passed this current frame.</param>
        public override void Update(FrameTime frameTime)
        {
            this.currentMouseState = this.mouse.GetState();

            // Update the mouse position label
            this.mousePosLabel.Text = $"Mouse Position: {this.currentMouseState.GetX()}, {this.currentMouseState.GetY()}";
            this.mousePosLabel.Position = new Point(LeftMargin, TopMargin);

            // Update the mouse left button label
            this.mouseLeftButtonLabel.Text = $"Mouse Left Button: {(this.currentMouseState.IsLeftButtonDown() ? "Down" : "Up")}";
            this.mouseLeftButtonLabel.Position =
                new Point(LeftMargin,
                    (int)(TopMargin + this.mousePosLabel.Position.Y + LineSpacing + (CalculateHeight(this.mouseLeftButtonLabel.Text) / 2f)));

            // Update the mouse right button label
            this.mouseRightButtonLabel.Text = $"Mouse Right Button: {(this.currentMouseState.IsRightButtonDown() ? "Down" : "Up")}";
            this.mouseRightButtonLabel.Position =
                new Point(LeftMargin,
                    (int)(TopMargin + this.mouseLeftButtonLabel.Position.Y + LineSpacing + (CalculateHeight(this.mouseRightButtonLabel.Text) / 2f)));

            // Update the mouse middle button label
            this.mouseMiddleButtonLabel.Text = $"Mouse Middle Button: {(this.currentMouseState.IsMiddleButtonDown() ? "Down" : "Up")}";
            this.mouseMiddleButtonLabel.Position =
                new Point(LeftMargin,
                    (int)(TopMargin + this.mouseRightButtonLabel.Position.Y + LineSpacing + (CalculateHeight(this.mouseMiddleButtonLabel.Text) / 2f)));

            // TODO: Fix the scroll wheel value.  It is not working
            this.mouseWheelValueLabel.Text = $"Mouse Middle Button: {this.currentMouseState.GetScrollWheelValue()} - Currently Broken";
            this.mouseWheelValueLabel.Position =
                new Point(LeftMargin,
                    (int)(TopMargin + this.mouseMiddleButtonLabel.Position.Y + LineSpacing + (CalculateHeight(this.mouseWheelValueLabel.Text) / 2f)));

            base.Update(frameTime);
        }

        /// <summary>
        /// Unloads the content from the scene.
        /// </summary>
        public override void Unload()
        {
            this.glyphHeights.Clear();
            base.Unload();
        }

        /// <summary>
        /// Calculates the width of the text based on the glyphs in the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to calculate the width from.</param>
        /// <returns>The width of all the text.</returns>
        private int CalculateHeight(string text)
            => text.Select(character => this.glyphHeights[character]).Prepend(int.MinValue).Max();
    }
}
