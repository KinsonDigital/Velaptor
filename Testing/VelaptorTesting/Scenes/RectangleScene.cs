// <copyright file="RectangleScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor.Scene;
using Velaptor;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.UI;

/// <summary>
/// Tests out rectangle rendering.
/// </summary>
public class RectangleScene : SceneBase
{
    private const int Speed = 200;
    private const int LeftMargin = 30;
    private const int RightMargin = 10;
    private const int BottomMargin = 10;
    private const int VertButtonSpacing = 10;
    private const int HoriButtonSpacing = 10;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private readonly IAppInput<KeyboardState> keyboard;
    private IFontRenderer? fontRenderer;
    private IRectangleRenderer? rectRenderer;
    private IFont? font;
    private KeyboardState currentKeyState;
    private RectShape rectangle;
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
    private Vector2 instructionsPos;
    private string instructions = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleScene"/> class.
    /// </summary>
    public RectangleScene() => this.keyboard = AppInputFactory.CreateKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        var renderFactory = new RendererFactory();

        this.fontRenderer = renderFactory.CreateFontRenderer();
        this.rectRenderer = renderFactory.CreateRectangleRenderer();

        this.rectangle = new RectShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Width = 100,
            Height = 100,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.IndianRed,
            IsFilled = true,
        };

        this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);

        var lines = new[]
        {
            "                    Instructions",
            "----------------------------------------------",
            "Rectangle Movement: Arrow Keys",
            "Size:",
            "   Increase Width: Shift + Right Arrow",
            "   Decrease Width: Shift + Left Arrow",
            "   Increase Height: Shift + Down Arrow",
            "   Decrease Height: Shift + Up Arrow",
        };
        this.instructions = string.Join(Environment.NewLine, lines);
        var size = this.font.Measure(this.instructions);
        this.instructionsPos = new Vector2(WindowCenter.X, 25 + (size.Height / 2f));

        CreateButtons();

        base.LoadContent();

        LayoutButtonsLeftSide();
        LayoutButtonsRightSide();
        LayoutButtonsBottom();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        MoveRect(frameTime);
        ChangeRectSize(frameTime);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.rectRenderer.Render(this.rectangle);

        this.fontRenderer.Render(this.font, this.instructions, this.instructionsPos, Color.White);

        base.Render();
    }

    /// <summary>
    /// Creates the buttons for the scene.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <see cref="ColorGradient"/> is invalid.
    /// </exception>
    private void CreateButtons()
    {
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
            LayoutButtonsBottom();
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
            LayoutButtonsBottom();
        };

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

    /// <summary>
    /// Lays out the buttons on the left side of the scene.
    /// </summary>
    private void LayoutButtonsLeftSide()
    {
        var excludeList = new[]
        {
            nameof(this.btnIncreaseWidth),
            nameof(this.btnDecreaseWidth),
            nameof(this.btnIncreaseHeight),
            nameof(this.btnDecreaseHeight),
            nameof(this.btnIsFilled),
            nameof(this.btnSolidFillClr),
            nameof(this.btnIncreaseBorderThickness),
            nameof(this.btnDecreaseBorderThickness),
        };

        var buttons = Controls.Where(c => excludeList.Contains(c.Name)).ToImmutableArray();

        var totalHeight = (from b in buttons
            select (int)b.Height).ToArray().Sum();
        totalHeight += (buttons.Length - 1) * VertButtonSpacing;
        var totalHalfHeight = totalHeight / 2;

        IControl? prevButton = null;

        foreach (var button in buttons)
        {
            button.Left = LeftMargin;
            button.Top = prevButton?.Bottom + VertButtonSpacing ?? WindowCenter.Y - totalHalfHeight;
            prevButton = button;
        }
    }

    /// <summary>
    /// Lays out the buttons on the right side of the scene.
    /// </summary>
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

        var buttons = Controls.Where(c => includeList.Contains(c.Name) && c is Button).ToImmutableArray();

        var totalHeight = (from b in buttons
            select (int)b.Height).ToArray().Sum();
        totalHeight += (buttons.Length - 1) * VertButtonSpacing;
        var totalHalfHeight = totalHeight / 2;

        IControl? prevButton = null;

        foreach (var button in buttons)
        {
            button.Right = (int)(WindowSize.Width - RightMargin);

            button.Top = prevButton?.Bottom + VertButtonSpacing ?? WindowCenter.Y - totalHalfHeight;

            prevButton = button;
        }
    }

    /// <summary>
    /// Lays out the buttons on the bottom of the scene.
    /// </summary>
    private void LayoutButtonsBottom()
    {
        var includeList = new[]
        {
            nameof(this.btnGradientType),
            nameof(this.btnGradClrStart),
            nameof(this.btnGradClrStop),
        };

        var buttons = Controls.Where(c => includeList.Contains(c.Name) && c is Button).ToImmutableArray();

        var totalWidth = (from b in buttons
            select (int)b.Width).ToArray().Sum();
        totalWidth += (buttons.Length - 1) * HoriButtonSpacing;
        var totalHalfWidth = totalWidth / 2;
        var buttonRowStart = WindowCenter.X - totalHalfWidth;

        IControl? prevButton = null;

        foreach (var button in buttons)
        {
            button.Bottom = (int)(WindowSize.Height - BottomMargin);
            button.Left = prevButton?.Right + HoriButtonSpacing ?? buttonRowStart;
            prevButton = button;
        }
    }

    /// <summary>
    /// Moves the rectangle using the keyboard arrow keys.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    private void MoveRect(FrameTime frameTime)
    {
        if (this.currentKeyState.AnyShiftKeysDown())
        {
            return;
        }

        var value = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            this.rectangle.Position -= new Vector2(value, 0f);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.rectangle.Position += new Vector2(value, 0f);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.rectangle.Position -= new Vector2(0f, value);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.rectangle.Position += new Vector2(0f, value);
        }

        // Left edge containment1
        if (this.rectangle.Position.X < this.rectangle.HalfWidth)
        {
            this.rectangle.Position = new Vector2(this.rectangle.HalfWidth, this.rectangle.Position.Y);
        }

        // Right edge containment
        if (this.rectangle.Position.X > WindowSize.Width - this.rectangle.HalfWidth)
        {
            this.rectangle.Position = new Vector2(WindowSize.Width - this.rectangle.HalfWidth, this.rectangle.Position.Y);
        }

        // Top edge containment
        if (this.rectangle.Position.Y < this.rectangle.HalfHeight)
        {
            this.rectangle.Position = new Vector2(this.rectangle.Position.X, this.rectangle.HalfHeight);
        }

        // Bottom edge containment
        if (this.rectangle.Position.Y > WindowSize.Height - this.rectangle.HalfHeight)
        {
            this.rectangle.Position = new Vector2(this.rectangle.Position.X, WindowSize.Height - this.rectangle.HalfHeight);
        }
    }

    /// <summary>
    /// Changes the size of the rectangle using the keyboard keys.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    private void ChangeRectSize(FrameTime frameTime)
    {
        if (this.currentKeyState.IsLeftShiftKeyDown() is false && this.currentKeyState.IsRightShiftKeyDown() is false)
        {
            return;
        }

        var value = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            this.rectangle.Width -= value;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.rectangle.Width += value;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.rectangle.Height -= value;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.rectangle.Height += value;
        }
    }
}
