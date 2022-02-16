// <copyright file="TestRectangleScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using VelaptorTesting.Core;

    public class TestRectangleScene : SceneBase
    {
        private const int LeftMargin = 10;
        private const int RightMargin = 10;
        private const int BottomMargin = 10;
        private const int VertButtonSpacing = 10;
        private const int HoriButtonSpacing = 10;
        private readonly Point windowCenter;
        private RectShape rectangle;
        private Button? btnLeft;
        private Button? btnRight;
        private Button? btnUp;
        private Button? btnDown;
        private Button? btnIncreaseWidth;
        private Button? btnDecreaseWidth;
        private Button? btnIncreaseHeight;
        private Button? btnDecreaseHeight;
        private Button? btnIsFilled;
        private Button? btnIncreaseBorderThickness;
        private Button? btnDecreaseBorderThickness;
        private Button? btnIncreaseTopLeftRadius;
        private Button? btnDecreaseTopLeftRadius;
        private Button? btnIncreaseBottomLeftRadius;
        private Button? btnDecreaseBottomLeftRadius;
        private Button? btnIncreaseBottomRightRadius;
        private Button? btnDecreaseBottomRightRadius;
        private Button? btnIncreaseTopRightRadius;
        private Button? btnDecreaseTopRightRadius;
        private Button? btnGradientType;
        private Button? btnGradClrStart;
        private Button? btnGradClrStop;

        public TestRectangleScene(IContentLoader contentLoader)
            : base(contentLoader) =>
                this.windowCenter = new Point((int)(MainWindow.WindowWidth / 2f), (int)(MainWindow.WindowHeight / 2f));

        public override void LoadContent()
        {
            this.rectangle = new RectShape
            {
                Position = new Vector2(this.windowCenter.X, this.windowCenter.Y),
                Width = 100,
                Height = 100,
                Color = Color.CornflowerBlue,
                GradientType = ColorGradient.None,
                GradientStart = Color.Red,
                GradientStop = Color.Red,
                IsFilled = true,
            };

            CreateButtons();

            base.LoadContent();

            LayoutButtonsLeftSide();
            LayoutButtonsRightSide();
            LayoutButtonsBottom();
        }

        public override void Render(ISpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);
            spriteBatch.Render(this.rectangle);
        }

        private void CreateButtons()
        {
            this.btnLeft = new Button
            {
                Text = "Left",
                Name = nameof(this.btnLeft),
            };
            this.btnLeft.MouseDown += (_, _) =>
                this.rectangle.Position = new Vector2(this.rectangle.Position.X - 5, this.rectangle.Position.Y);

            this.btnRight = new Button
            {
                Text = "Right",
                Name = nameof(this.btnRight),
            };
            this.btnRight.MouseDown += (_, _) =>
                this.rectangle.Position = new Vector2(this.rectangle.Position.X + 5, this.rectangle.Position.Y);

            this.btnUp = new Button
            {
                Text = "Up",
                Name = nameof(this.btnUp),
            };
            this.btnUp.MouseDown += (_, _) =>
                this.rectangle.Position = new Vector2(this.rectangle.Position.X, this.rectangle.Position.Y - 5);

            this.btnDown = new Button
            {
                Text = "Down",
                Name = nameof(this.btnDown),
            };
            this.btnDown.MouseDown += (_, _) =>
                this.rectangle.Position = new Vector2(this.rectangle.Position.X, this.rectangle.Position.Y + 5);

            this.btnIncreaseWidth = new Button
            {
                Text = "Width +",
                Name = nameof(this.btnIncreaseWidth),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseWidth.MouseDown += (_, _) => this.rectangle.Width += 5;

            this.btnDecreaseWidth = new Button
            {
                Text = "Width -",
                Name = nameof(this.btnDecreaseWidth),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseWidth.MouseDown += (_, _) => this.rectangle.Width -= 5;

            this.btnIncreaseHeight = new Button
            {
                Text = "Height +",
                Name = nameof(this.btnIncreaseHeight),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseHeight.MouseDown += (_, _) => this.rectangle.Height += 5;

            this.btnDecreaseHeight = new Button
            {
                Text = "Height -",
                Name = nameof(this.btnDecreaseHeight),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseHeight.MouseDown += (_, _) => this.rectangle.Height -= 5;

            this.btnIsFilled = new Button
            {
                Text = "Is Filled: true",
                Name = nameof(this.btnIsFilled),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIsFilled.Click += (_, _) =>
            {
                this.rectangle.IsFilled = !this.rectangle.IsFilled;
                this.btnIsFilled.Text = this.rectangle.IsFilled ? "Is Filled: true" : "Is Filled: false";
                this.btnIncreaseBorderThickness.Enabled = !this.rectangle.IsFilled;
                this.btnDecreaseBorderThickness.Enabled = !this.rectangle.IsFilled;
            };

            this.btnIncreaseBorderThickness = new Button
            {
                Text = "Border Thickness +",
                Name = nameof(this.btnIncreaseBorderThickness),
                FaceTextureName = "button-face-extra-large",
                Enabled = false,
            };
            this.btnIncreaseBorderThickness.MouseDown += (_, _) =>
                this.rectangle.BorderThickness = this.rectangle.BorderThickness += 1f;

            this.btnDecreaseBorderThickness = new Button
            {
                Text = "Border Thickness -",
                Name = nameof(this.btnDecreaseBorderThickness),
                FaceTextureName = "button-face-extra-large",
                Enabled = false,
            };
            this.btnDecreaseBorderThickness.MouseDown += (_, _) =>
                this.rectangle.BorderThickness = this.rectangle.BorderThickness -= 1f;

            this.btnIncreaseTopLeftRadius = new Button
            {
                Text = "Top Left Radius +",
                Name = nameof(this.btnIncreaseTopLeftRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseTopLeftRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopLeft(1);

            this.btnDecreaseTopLeftRadius = new Button
            {
                Text = "Top Left Radius -",
                Name = nameof(this.btnDecreaseTopLeftRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseTopLeftRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopLeft(1);

            this.btnIncreaseBottomLeftRadius = new Button
            {
                Text = "Bottom Left Radius +",
                Name = nameof(this.btnIncreaseBottomLeftRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseBottomLeftRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomLeft(1);

            this.btnDecreaseBottomLeftRadius = new Button
            {
                Text = "Bottom Left Radius -",
                Name = nameof(this.btnDecreaseBottomLeftRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseBottomLeftRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomLeft(1);

            this.btnIncreaseBottomRightRadius = new Button
            {
                Text = "Bottom Right Radius +",
                Name = nameof(this.btnIncreaseBottomRightRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseBottomRightRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomRight(1);

            this.btnDecreaseBottomRightRadius = new Button
            {
                Text = "Bottom Right Radius -",
                Name = nameof(this.btnDecreaseBottomRightRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseBottomRightRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomRight(1);

            this.btnIncreaseTopRightRadius = new Button
            {
                Text = "Top Right Radius +",
                Name = nameof(this.btnIncreaseTopRightRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnIncreaseTopRightRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopRight(1);

            this.btnDecreaseTopRightRadius = new Button
            {
                Text = "Top Right Radius -",
                Name = nameof(this.btnDecreaseTopRightRadius),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnDecreaseTopRightRadius.MouseDown += (_, _) =>
                this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopRight(1);

            this.btnGradientType = new Button
            {
                Text = "Gradient Type: None",
                Name = nameof(this.btnGradientType),
                FaceTextureName = "button-face-extra-extra-large",
            };
            this.btnGradientType.Click += (_, _) =>
            {
                this.rectangle.GradientType = this.rectangle.GradientType switch
                {
                    ColorGradient.None => ColorGradient.Horizontal,
                    ColorGradient.Horizontal => ColorGradient.Vertical,
                    ColorGradient.Vertical => ColorGradient.None,
                    _ => throw new ArgumentOutOfRangeException()
                };

                this.btnGradientType.Text = $"Gradient Type: {this.rectangle.GradientType}";
            };

            this.btnGradClrStart = new Button
            {
                Text = "Grad Clr Start: Red",
                Name = nameof(this.btnGradClrStart),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnGradClrStart.Click += (_, _) =>
            {
                var clrStr = "ERROR";
                if (this.rectangle.GradientStart == Color.Red)
                {
                    this.rectangle.GradientStart = Color.Green;
                    clrStr = "Green";
                }
                else if (this.rectangle.GradientStart == Color.Green)
                {
                    this.rectangle.GradientStart = Color.Blue;
                    clrStr = "Blue";
                }
                else if (this.rectangle.GradientStart == Color.Blue)
                {
                    this.rectangle.GradientStart = Color.Red;
                    clrStr = "Red";
                }

                this.btnGradClrStart.Text = $"Grad Clr Start: {clrStr}";
            };

            this.btnGradClrStop = new Button
            {
                Text = "Grad Clr Stop: Red",
                Name = nameof(this.btnGradClrStop),
                FaceTextureName = "button-face-extra-large",
            };
            this.btnGradClrStop.Click += (_, _) =>
            {
                var clrStr = "ERROR";

                if (this.rectangle.GradientStop == Color.Red)
                {
                    this.rectangle.GradientStop = Color.Green;
                    clrStr = "Green";
                }
                else if (this.rectangle.GradientStop == Color.Green)
                {
                    this.rectangle.GradientStop = Color.Blue;
                    clrStr = "Blue";
                }
                else if (this.rectangle.GradientStop == Color.Blue)
                {
                    this.rectangle.GradientStop = Color.Red;
                    clrStr = "Red";
                }

                this.btnGradClrStop.Text = $"Grad Clr Stop: {clrStr}";
            };

            AddControl(this.btnLeft);
            AddControl(this.btnRight);
            AddControl(this.btnUp);
            AddControl(this.btnDown);
            AddControl(this.btnIncreaseWidth);
            AddControl(this.btnDecreaseWidth);
            AddControl(this.btnIncreaseHeight);
            AddControl(this.btnDecreaseHeight);
            AddControl(this.btnIsFilled);
            AddControl(this.btnIncreaseBorderThickness);
            AddControl(this.btnDecreaseBorderThickness);
            AddControl(this.btnIncreaseTopLeftRadius);
            AddControl(this.btnDecreaseTopLeftRadius);
            AddControl(this.btnIncreaseBottomLeftRadius);
            AddControl(this.btnDecreaseBottomLeftRadius);
            AddControl(this.btnIncreaseBottomRightRadius);
            AddControl(this.btnDecreaseBottomRightRadius);
            AddControl(this.btnIncreaseTopRightRadius);
            AddControl(this.btnDecreaseTopRightRadius);
            AddControl(this.btnGradientType);
            AddControl(this.btnGradClrStart);
            AddControl(this.btnGradClrStop);
        }

        private void LayoutButtonsLeftSide()
        {
            var excludeList = new[]
            {
                nameof(this.btnLeft),
                nameof(this.btnRight),
                nameof(this.btnUp),
                nameof(this.btnDown),
                nameof(this.btnIncreaseWidth),
                nameof(this.btnDecreaseWidth),
                nameof(this.btnIncreaseHeight),
                nameof(this.btnDecreaseHeight),
                nameof(this.btnIsFilled),
                nameof(this.btnIncreaseBorderThickness),
                nameof(this.btnDecreaseBorderThickness),
            };
            var buttons = (from c in GetControls<Button>()
                where excludeList.Contains(c.Name)
                select c).ToArray();

            var totalHeight = (from b in buttons
                select (int)b.Height).ToArray().Sum();
            totalHeight += (buttons.Length - 1) * VertButtonSpacing;
            var totalHalfHeight = totalHeight / 2;

            IControl? prevButton = null;

            foreach (var button in buttons)
            {
                button.Left = LeftMargin;

                button.Top = prevButton is null
                    ? button.Top = this.windowCenter.Y - totalHalfHeight
                    : button.Top = prevButton.Bottom + VertButtonSpacing;

                prevButton = button;
            }

            // Center all of the buttons horizontally relative to each other
            var largestBtnWidth = buttons.Max(b => b.Width);
            var desiredPosition = (from b in buttons
                where b.Width == largestBtnWidth
                select b.Position).FirstOrDefault();

            foreach (var button in buttons)
            {
                button.Position = new Point(desiredPosition.X, button.Position.Y);
            }
        }

        private void LayoutButtonsRightSide()
        {
            var includeList = new[]
            {
                nameof(this.btnIncreaseTopLeftRadius),
                nameof(this.btnDecreaseTopLeftRadius),
                nameof(this.btnIncreaseBottomLeftRadius),
                nameof(this.btnDecreaseBottomLeftRadius),
                nameof(this.btnIncreaseBottomRightRadius),
                nameof(this.btnDecreaseBottomRightRadius),
                nameof(this.btnIncreaseTopRightRadius),
                nameof(this.btnDecreaseTopRightRadius),
            };
            var buttons = (from c in GetControls<Button>()
                where includeList.Contains(c.Name)
                select c).ToArray();

            var totalHeight = (from b in buttons
                select (int)b.Height).ToArray().Sum();
            totalHeight += (buttons.Length - 1) * VertButtonSpacing;
            var totalHalfHeight = totalHeight / 2;

            IControl? prevButton = null;

            foreach (var button in buttons)
            {
                button.Right = (int)(MainWindow.WindowWidth - RightMargin);

                button.Top = prevButton is null
                    ? button.Top = this.windowCenter.Y - totalHalfHeight
                    : button.Top = prevButton.Bottom + VertButtonSpacing;

                prevButton = button;
            }
        }

        private void LayoutButtonsBottom()
        {
            var includeList = new[]
            {
                nameof(this.btnGradientType),
                nameof(this.btnGradClrStart),
                nameof(this.btnGradClrStop),
            };
            var buttons = (from c in GetControls<Button>()
                where includeList.Contains(c.Name)
                select c).ToArray();

            var totalWidth = (from b in buttons
                select (int)b.Width).ToArray().Sum();
            totalWidth += (buttons.Length - 1) * VertButtonSpacing;
            var totalHalfWidth = totalWidth / 2;

            IControl? prevButton = null;

            foreach (var button in buttons)
            {
                button.Bottom = (int)(MainWindow.WindowHeight - BottomMargin);

                button.Left = prevButton is null
                    ? button.Left = this.windowCenter.X - totalHalfWidth
                    : button.Left = prevButton.Right + HoriButtonSpacing;

                prevButton = button;
            }
        }
    }
}
