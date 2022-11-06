// <copyright file="TestRectangleScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.UI;
using VelaptorTesting.Core;

namespace VelaptorTesting.Scenes;

public class TestRectangleScene : SceneBase
{
    private const int LeftMargin = 30;
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
    private Button? btnSolidFillClr;
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
            GradientStart = Color.IndianRed,
            GradientStop = Color.IndianRed,
            IsFilled = true,
        };

        CreateButtons();

        base.LoadContent();

        LayoutButtonsLeftSide();
        LayoutButtonsRightSide();
        LayoutButtonsBottom();
    }

    public override void Render(IRenderer renderer)
    {
        base.Render(renderer);
        renderer.Render(this.rectangle);
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
        };
        this.btnIncreaseWidth.MouseDown += (_, _) => this.rectangle.Width += 5;

        this.btnDecreaseWidth = new Button
        {
            Text = "Width -",
            Name = nameof(this.btnDecreaseWidth),
        };
        this.btnDecreaseWidth.MouseDown += (_, _) => this.rectangle.Width -= 5;

        this.btnIncreaseHeight = new Button
        {
            Text = "Height +",
            Name = nameof(this.btnIncreaseHeight),
        };
        this.btnIncreaseHeight.MouseDown += (_, _) => this.rectangle.Height += 5;

        this.btnDecreaseHeight = new Button
        {
            Text = "Height -",
            Name = nameof(this.btnDecreaseHeight),
        };
        this.btnDecreaseHeight.MouseDown += (_, _) => this.rectangle.Height -= 5;

        this.btnIsFilled = new Button
        {
            Text = "Is Filled: true",
            Name = nameof(this.btnIsFilled),
        };
        this.btnIsFilled.Click += (_, _) =>
        {
            this.rectangle.IsFilled = !this.rectangle.IsFilled;
            this.btnIsFilled.Text = this.rectangle.IsFilled ? "Is Filled: true" : "Is Filled: false";
            this.btnIncreaseBorderThickness.Enabled = !this.rectangle.IsFilled;
            this.btnDecreaseBorderThickness.Enabled = !this.rectangle.IsFilled;
        };

        this.btnSolidFillClr = new Button
        {
            Text = "Solid Fill Clr: Blue",
            Name = nameof(this.btnSolidFillClr),
        };
        this.btnSolidFillClr.Click += (_, _) =>
        {
            var clrStr = "ERROR";
            if (this.rectangle.Color == Color.IndianRed)
            {
                this.rectangle.Color = Color.SeaGreen;
                clrStr = "Green";
            }
            else if (this.rectangle.Color == Color.SeaGreen)
            {
                this.rectangle.Color = Color.CornflowerBlue;
                clrStr = "Blue";
            }
            else if (this.rectangle.Color == Color.CornflowerBlue)
            {
                this.rectangle.Color = Color.IndianRed;
                clrStr = "Red";
            }

            this.btnSolidFillClr.Text = $"Solid Fill Clr: {clrStr}";
        };

        this.btnIncreaseBorderThickness = new Button
        {
            Text = "Border Thickness +",
            Name = nameof(this.btnIncreaseBorderThickness),
            Enabled = false,
        };
        this.btnIncreaseBorderThickness.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.BorderThickness >= maxValue
                ? 0
                : 1;

            this.rectangle.BorderThickness = this.rectangle.BorderThickness += newValue;
        };

        this.btnDecreaseBorderThickness = new Button
        {
            Text = "Border Thickness -",
            Name = nameof(this.btnDecreaseBorderThickness),
            Enabled = false,
        };
        this.btnDecreaseBorderThickness.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.BorderThickness <= 0
                ? 0
                : 1;

            this.rectangle.BorderThickness = this.rectangle.BorderThickness -= newValue;
        };

        this.btnIncreaseTopLeftRadius = new Button
        {
            Text = "Top Left Radius +",
            Name = nameof(this.btnIncreaseTopLeftRadius),
        };
        this.btnIncreaseTopLeftRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.CornerRadius.TopLeft > maxValue
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopLeft(newValue);
        };

        this.btnDecreaseTopLeftRadius = new Button
        {
            Text = "Top Left Radius -",
            Name = nameof(this.btnDecreaseTopLeftRadius),
        };
        this.btnDecreaseTopLeftRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.TopLeft <= 0
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopLeft(newValue);
        };

        this.btnIncreaseBottomLeftRadius = new Button
        {
            Text = "Bottom Left Radius +",
            Name = nameof(this.btnIncreaseBottomLeftRadius),
        };
        this.btnIncreaseBottomLeftRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.CornerRadius.BottomLeft > maxValue
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomLeft(newValue);
        };

        this.btnDecreaseBottomLeftRadius = new Button
        {
            Text = "Bottom Left Radius -",
            Name = nameof(this.btnDecreaseBottomLeftRadius),
        };
        this.btnDecreaseBottomLeftRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.BottomLeft <= 0
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomLeft(newValue);
        };

        this.btnIncreaseBottomRightRadius = new Button
        {
            Text = "Bottom Right Radius +",
            Name = nameof(this.btnIncreaseBottomRightRadius),
        };
        this.btnIncreaseBottomRightRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.CornerRadius.BottomRight > maxValue
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomRight(newValue);
        };

        this.btnDecreaseBottomRightRadius = new Button
        {
            Text = "Bottom Right Radius -",
            Name = nameof(this.btnDecreaseBottomRightRadius),
        };
        this.btnDecreaseBottomRightRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.BottomRight <= 0
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomRight(newValue);
        };

        this.btnIncreaseTopRightRadius = new Button
        {
            Text = "Top Right Radius +",
            Name = nameof(this.btnIncreaseTopRightRadius),
        };
        this.btnIncreaseTopRightRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.CornerRadius.TopRight > maxValue
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopRight(newValue);
        };

        this.btnDecreaseTopRightRadius = new Button
        {
            Text = "Top Right Radius -",
            Name = nameof(this.btnDecreaseTopRightRadius),
        };
        this.btnDecreaseTopRightRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.TopRight <= 0
                ? 0
                : 1;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopRight(newValue);
        };

        this.btnGradientType = new Button
        {
            Text = "Gradient Type: None",
            Name = nameof(this.btnGradientType),
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
            LayoutButtonsBottom();
        };

        this.btnGradClrStart = new Button
        {
            Text = "Grad Clr Start: Red",
            Name = nameof(this.btnGradClrStart),
        };
        this.btnGradClrStart.Click += (_, _) =>
        {
            var clrStr = "ERROR";
            if (this.rectangle.GradientStart == Color.IndianRed)
            {
                this.rectangle.GradientStart = Color.SeaGreen;
                clrStr = "Green";
            }
            else if (this.rectangle.GradientStart == Color.SeaGreen)
            {
                this.rectangle.GradientStart = Color.CornflowerBlue;
                clrStr = "Blue";
            }
            else if (this.rectangle.GradientStart == Color.CornflowerBlue)
            {
                this.rectangle.GradientStart = Color.IndianRed;
                clrStr = "Red";
            }

            this.btnGradClrStart.Text = $"Grad Clr Start: {clrStr}";
        };

        this.btnGradClrStop = new Button
        {
            Text = "Grad Clr Stop: Red",
            Name = nameof(this.btnGradClrStop),
        };
        this.btnGradClrStop.Click += (_, _) =>
        {
            var clrStr = "ERROR";

            if (this.rectangle.GradientStop == Color.IndianRed)
            {
                this.rectangle.GradientStop = Color.SeaGreen;
                clrStr = "Green";
            }
            else if (this.rectangle.GradientStop == Color.SeaGreen)
            {
                this.rectangle.GradientStop = Color.CornflowerBlue;
                clrStr = "Blue";
            }
            else if (this.rectangle.GradientStop == Color.CornflowerBlue)
            {
                this.rectangle.GradientStop = Color.IndianRed;
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
        AddControl(this.btnSolidFillClr);
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
            nameof(this.btnSolidFillClr),
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
        totalWidth += (buttons.Length - 1) * HoriButtonSpacing;
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
