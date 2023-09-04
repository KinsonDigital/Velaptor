// <copyright file="ShapeScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using Velaptor.UI;

/// <summary>
/// Tests out rectangle rendering.
/// </summary>
public class ShapeScene : SceneBase
{
    private const int Speed = 200;
    private const int LeftMargin = 30;
    private const int RightMargin = 10;
    private const int BottomMargin = 10;
    private const int VertButtonSpacing = 10;
    private const int HoriButtonSpacing = 10;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly List<string> rectButtons = new ();
    private readonly List<string> circleButtons = new ();
    private IFontRenderer? fontRenderer;
    private IShapeRenderer? shapeRenderer;
    private IFont? font;
    private KeyboardState currentKeyState;
    private RectShape rectangle;
    private CircleShape circle;
    private ShapeType shapeType = ShapeType.Circle;
    private Button? btnShapeType;
    private Button? btnIncCircleDiam;
    private Button? btnDecCircleDiam;
    private Button? btnIncreaseWidth;
    private Button? btnDecreaseWidth;
    private Button? btnIncreaseHeight;
    private Button? btnDecreaseHeight;
    private Button? btnIsSolid;
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
    private string rectInstructions = string.Empty;
    private string circleInstructions = string.Empty;
    private Vector2 rectInstructionsPos;
    private Vector2 circleInstructionsPos;
    private BackgroundManager? backgroundManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeScene"/> class.
    /// </summary>
    public ShapeScene() => this.keyboard = HardwareFactory.GetKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var renderFactory = new RendererFactory();

        this.fontRenderer = renderFactory.CreateFontRenderer();
        this.shapeRenderer = renderFactory.CreateShapeRenderer();

        this.rectangle = new RectShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Width = 100,
            Height = 100,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.IndianRed,
            IsSolid = true,
        };

        this.circle = new CircleShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Diameter = 100,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.IndianRed,
            BorderThickness = 10f,
            IsSolid = false,
        };

        this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);

        var rectLines = new[]
        {
            "                    Rectangle Instructions",
            "----------------------------------------------",
            "Movement: Arrow Keys",
            "Size:",
            "   Increase Width: Shift + Right Arrow",
            "   Decrease Width: Shift + Left Arrow",
            "   Increase Height: Shift + Up Arrow",
            "   Decrease Height: Shift + Down Arrow",
        };
        this.rectInstructions = string.Join(Environment.NewLine, rectLines);
        var rectInstructionsSize = this.font.Measure(this.rectInstructions);
        this.rectInstructionsPos = new Vector2(WindowCenter.X, 25 + (rectInstructionsSize.Height / 2f));

        var circleLines = new[]
        {
            "                    Circle Instructions",
            "----------------------------------------------",
            "Movement: Arrow Keys",
            "Size:",
            "   Increase Diameter: Shift + Up Or Right Arrow",
            "   Decrease Diameter: Shift + Down Or Left Arrow",
        };
        this.circleInstructions = string.Join(Environment.NewLine, circleLines);
        var circleInstructionsSize = this.font.Measure(this.circleInstructions);
        this.circleInstructionsPos = new Vector2(WindowCenter.X, 25 + (circleInstructionsSize.Height / 2f));

        CreateButtons();

        base.LoadContent();

        LayoutButtonsLeftSide();
        LayoutButtonsRightSide();
        LayoutButtonsBottom();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        this.backgroundManager?.Unload();
        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        MoveShape(frameTime);
        ChangeRectSize(frameTime);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.shapeRenderer.Render(this.rectangle, 1);
                break;
            case ShapeType.Circle:
                this.shapeRenderer.Render(this.circle, 1);
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        var instructionText = this.shapeType switch
        {
            ShapeType.Rectangle => this.rectInstructions,
            ShapeType.Circle => this.circleInstructions,
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        var instructionPos = this.shapeType switch
        {
            ShapeType.Rectangle => this.rectInstructionsPos,
            ShapeType.Circle => this.circleInstructionsPos,
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        this.backgroundManager?.Render();
        this.fontRenderer.Render(this.font, instructionText, instructionPos, Color.White, 2);

        base.Render();
    }

    /// <summary>
    /// Creates the buttons for the scene.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <see cref="ColorGradient"/> is invalid.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "SwitchStatementHandlesSomeKnownEnumValuesWithDefault",
        Justification = "Not required.")]
    private void CreateButtons()
    {
        this.btnShapeType = new Button
        {
            Text = this.shapeType switch
            {
                ShapeType.Rectangle => "Rectangle",
                ShapeType.Circle => "Circle",
                _ => throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType)),
            },
            Name = nameof(this.btnShapeType),
        };
        this.btnShapeType.Click += btnShapeType_Click;

        this.btnIncCircleDiam = new Button { Text = "Diameter +", Name = nameof(this.btnIncCircleDiam), };
        this.btnIncCircleDiam.MouseDown += (_, _) =>
        {
            this.circle.IncreaseDiameter(5);
        };

        this.btnDecCircleDiam = new Button { Text = "Diameter -", Name = nameof(this.btnDecCircleDiam), };
        this.btnDecCircleDiam.MouseDown += (_, _) =>
        {
            this.circle.DecreaseDiameter(5);
        };

        this.btnIncreaseWidth = new Button
        {
            Text = "Width +",
            Name = nameof(this.btnIncreaseWidth),
        };
        this.btnIncreaseWidth.MouseDown += (_, _) => this.rectangle.IncreaseWidth(5);

        this.btnDecreaseWidth = new Button
        {
            Text = "Width -",
            Name = nameof(this.btnDecreaseWidth),
        };
        this.btnDecreaseWidth.MouseDown += (_, _) => this.rectangle.DecreaseWidth(5);

        this.btnIncreaseHeight = new Button
        {
            Text = "Height +",
            Name = nameof(this.btnIncreaseHeight),
        };
        this.btnIncreaseHeight.MouseDown += (_, _) => this.rectangle.IncreaseHeight(5);

        this.btnDecreaseHeight = new Button
        {
            Text = "Height -",
            Name = nameof(this.btnDecreaseHeight),
        };
        this.btnDecreaseHeight.MouseDown += (_, _) => this.rectangle.DecreaseHeight(5);

        this.btnIsSolid = new Button
        {
            Text = $"Is Solid: {(this.shapeType == ShapeType.Rectangle ? this.rectangle.IsSolid : this.circle.IsSolid)}",
            Name = nameof(this.btnIsSolid),
        };
        this.btnIsSolid.Click += btnIsSolid_Click;

        this.btnSolidFillClr = new Button
        {
            Text = "Solid Fill Clr: Blue",
            Name = nameof(this.btnSolidFillClr),
        };
        this.btnSolidFillClr.Click += btnSolidFillClr_Click;

        this.btnIncreaseBorderThickness = new Button
        {
            Text = "Border Thickness +",
            Name = nameof(this.btnIncreaseBorderThickness),
            Enabled = this.shapeType == ShapeType.Rectangle ? !this.rectangle.IsSolid : !this.circle.IsSolid,
        };
        this.btnIncreaseBorderThickness.MouseDown += btnIncreaseBorderThickness_MouseDown;

        this.btnDecreaseBorderThickness = new Button
        {
            Text = "Border Thickness -",
            Name = nameof(this.btnDecreaseBorderThickness),
            Enabled = this.shapeType == ShapeType.Rectangle ? !this.rectangle.IsSolid : !this.circle.IsSolid,
        };
        this.btnDecreaseBorderThickness.MouseDown += btnDecreaseBorderThickness_MouseDown;

        this.btnIncreaseTopLeftRadius = new Button
        {
            Text = "Top Left Radius +",
            Name = nameof(this.btnIncreaseTopLeftRadius),
        };
        this.btnIncreaseTopLeftRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;

            var newValue = this.rectangle.CornerRadius.TopLeft > maxValue ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopLeft(newValue);
        };

        this.btnDecreaseTopLeftRadius = new Button
        {
            Text = "Top Left Radius -",
            Name = nameof(this.btnDecreaseTopLeftRadius),
        };
        this.btnDecreaseTopLeftRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.TopLeft <= 0 ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopLeft(newValue);
        };

        this.btnIncreaseBottomLeftRadius = new Button
        {
            Text = "Bottom Left Radius +",
            Name = nameof(this.btnIncreaseBottomLeftRadius),
        };
        this.btnIncreaseBottomLeftRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.BottomLeft > maxValue ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomLeft(newValue);
        };

        this.btnDecreaseBottomLeftRadius = new Button
        {
            Text = "Bottom Left Radius -",
            Name = nameof(this.btnDecreaseBottomLeftRadius),
        };
        this.btnDecreaseBottomLeftRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.BottomLeft <= 0 ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomLeft(newValue);
        };

        this.btnIncreaseBottomRightRadius = new Button
        {
            Text = "Bottom Right Radius +",
            Name = nameof(this.btnIncreaseBottomRightRadius),
        };
        this.btnIncreaseBottomRightRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.BottomRight > maxValue ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseBottomRight(newValue);
        };

        this.btnDecreaseBottomRightRadius = new Button
        {
            Text = "Bottom Right Radius -",
            Name = nameof(this.btnDecreaseBottomRightRadius),
        };
        this.btnDecreaseBottomRightRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.BottomRight <= 0 ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseBottomRight(newValue);
        };

        this.btnIncreaseTopRightRadius = new Button
        {
            Text = "Top Right Radius +",
            Name = nameof(this.btnIncreaseTopRightRadius),
        };
        this.btnIncreaseTopRightRadius.MouseDown += (_, _) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.TopRight > maxValue ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.IncreaseTopRight(newValue);
        };

        this.btnDecreaseTopRightRadius = new Button
        {
            Text = "Top Right Radius -",
            Name = nameof(this.btnDecreaseTopRightRadius),
        };
        this.btnDecreaseTopRightRadius.MouseDown += (_, _) =>
        {
            var newValue = this.rectangle.CornerRadius.TopRight <= 0 ? 0 : 1;
            this.rectangle.CornerRadius = this.rectangle.CornerRadius.DecreaseTopRight(newValue);
        };

        this.btnGradientType = new Button
        {
            Text = "Gradient Type: None",
            Name = nameof(this.btnGradientType),
        };
        this.btnGradientType.Click += btnGradientType_Click;

        this.btnGradClrStart = new Button
        {
            Text = "Grad Clr Start: Red",
            Name = nameof(this.btnGradClrStart),
        };
        this.btnGradClrStart.Click += btnGradClrStart_Click;

        this.btnGradClrStop = new Button
        {
            Text = "Grad Clr Stop: Red",
            Name = nameof(this.btnGradClrStop),
        };
        this.btnGradClrStop.Click += btnGradClrStop_Click;

        AddControl(this.btnShapeType);
        AddControl(this.btnIncCircleDiam);
        AddControl(this.btnDecCircleDiam);
        AddControl(this.btnIncreaseWidth);
        AddControl(this.btnDecreaseWidth);
        AddControl(this.btnIncreaseHeight);
        AddControl(this.btnDecreaseHeight);
        AddControl(this.btnIsSolid);
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

        // Circle buttons
        this.circleButtons.Add(nameof(this.btnIncCircleDiam));
        this.circleButtons.Add(nameof(this.btnDecCircleDiam));
        this.circleButtons.Add(nameof(this.btnShapeType));
        this.circleButtons.Add(nameof(this.btnIsSolid));
        this.circleButtons.Add(nameof(this.btnSolidFillClr));
        this.circleButtons.Add(nameof(this.btnIncreaseBorderThickness));
        this.circleButtons.Add(nameof(this.btnDecreaseBorderThickness));
        this.circleButtons.Add(nameof(this.btnGradientType));
        this.circleButtons.Add(nameof(this.btnGradClrStart));
        this.circleButtons.Add(nameof(this.btnGradClrStop));

        // Rectangle buttons
        this.rectButtons.Add(nameof(this.btnShapeType));
        this.rectButtons.Add(nameof(this.btnIncreaseWidth));
        this.rectButtons.Add(nameof(this.btnDecreaseWidth));
        this.rectButtons.Add(nameof(this.btnIncreaseHeight));
        this.rectButtons.Add(nameof(this.btnDecreaseHeight));
        this.rectButtons.Add(nameof(this.btnIsSolid));
        this.rectButtons.Add(nameof(this.btnSolidFillClr));
        this.rectButtons.Add(nameof(this.btnIncreaseBorderThickness));
        this.rectButtons.Add(nameof(this.btnDecreaseBorderThickness));
        this.rectButtons.Add(nameof(this.btnIncreaseTopLeftRadius));
        this.rectButtons.Add(nameof(this.btnDecreaseTopLeftRadius));
        this.rectButtons.Add(nameof(this.btnIncreaseBottomLeftRadius));
        this.rectButtons.Add(nameof(this.btnDecreaseBottomLeftRadius));
        this.rectButtons.Add(nameof(this.btnIncreaseBottomRightRadius));
        this.rectButtons.Add(nameof(this.btnDecreaseBottomRightRadius));
        this.rectButtons.Add(nameof(this.btnIncreaseTopRightRadius));
        this.rectButtons.Add(nameof(this.btnDecreaseTopRightRadius));
        this.rectButtons.Add(nameof(this.btnGradientType));
        this.rectButtons.Add(nameof(this.btnGradClrStart));
        this.rectButtons.Add(nameof(this.btnGradClrStop));

        ShowButtonsForShape();
        LayoutButtonsLeftSide();
    }

    // Element should begin with upper-case letter
    #pragma warning disable SA1300
    #region Control Events
    private void btnIsSolid_Click(object? sender, EventArgs e)
    {
        bool isSolid;

        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.IsSolid = !this.rectangle.IsSolid;
                isSolid = this.rectangle.IsSolid;
                break;
            case ShapeType.Circle:
                this.circle.IsSolid = !this.circle.IsSolid;
                isSolid = this.circle.IsSolid;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        this.btnIsSolid.Text = isSolid ? "Is Solid: true" : "Is Solid: false";
        this.btnIncreaseBorderThickness.Enabled = !isSolid;
        this.btnDecreaseBorderThickness.Enabled = !isSolid;
        LayoutButtonsLeftSide();
    }

    private void btnSolidFillClr_Click(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.SwapFillColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnSolidFillClr.Text = $"Solid Fill Clr: {color}");
                break;
            case ShapeType.Circle:
                this.circle.SwapFillColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnSolidFillClr.Text = $"Solid Fill Clr: {color}");
                break;
        }

        LayoutButtonsLeftSide();
    }

    private void btnIncreaseBorderThickness_MouseDown(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.BorderThickness = this.rectangle.BorderThickness += 1;
                break;
            case ShapeType.Circle:
                this.circle.BorderThickness = this.circle.BorderThickness += 1;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }
    }

    private void btnDecreaseBorderThickness_MouseDown(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.BorderThickness = this.rectangle.BorderThickness -= 1;
                break;
            case ShapeType.Circle:
                this.circle.BorderThickness = this.circle.BorderThickness -= 1;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }
    }

    private void btnShapeType_Click(object? sender, EventArgs e)
    {
        this.shapeType = this.shapeType switch
        {
            ShapeType.Rectangle => ShapeType.Circle,
            ShapeType.Circle => ShapeType.Rectangle,
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        this.btnShapeType.Text = this.shapeType switch
        {
            ShapeType.Rectangle => "Rectangle",
            ShapeType.Circle => "Circle",
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        this.btnGradientType.Text = this.shapeType switch
        {
            ShapeType.Rectangle => $"Gradient Type: {this.rectangle.GradientType}",
            ShapeType.Circle => $"Gradient Type: {this.circle.GradientType}",
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        this.btnSolidFillClr.Enabled = this.shapeType switch
        {
            ShapeType.Rectangle => this.rectangle.GradientType == ColorGradient.None,
            ShapeType.Circle => this.circle.GradientType == ColorGradient.None,
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        ShowButtonsForShape();
        LayoutButtonsLeftSide();
        LayoutButtonsBottom();
    }

    private void btnGradientType_Click(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.GradientType = this.rectangle.GradientType.SwapGradient();
                break;
            case ShapeType.Circle:
                this.circle.GradientType = this.circle.GradientType.SwapGradient();
                break;
        }

        this.btnGradientType.Text = this.shapeType switch
        {
            ShapeType.Rectangle => $"Gradient Type: {this.rectangle.GradientType}",
            ShapeType.Circle => $"Gradient Type: {this.circle.GradientType}",
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        this.btnSolidFillClr.Enabled = this.shapeType switch
        {
            ShapeType.Rectangle => this.rectangle.GradientType == ColorGradient.None,
            ShapeType.Circle => this.circle.GradientType == ColorGradient.None,
            _ => throw new InvalidEnumArgumentException(
                $"this.{nameof(this.shapeType)}",
                (int)this.shapeType,
                typeof(ShapeType)),
        };

        LayoutButtonsBottom();
    }

    private void btnGradClrStop_Click(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.SwapGradStopColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnGradClrStop.Text = $"Solid Fill Clr: {color}");
                break;
            case ShapeType.Circle:
                this.circle.SwapGradStopColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnGradClrStop.Text = $"Solid Fill Clr: {color}");
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        LayoutButtonsBottom();
    }

    private void btnGradClrStart_Click(object? sender, EventArgs e)
    {
        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.SwapGradStartColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnGradClrStart.Text = $"Solid Fill Clr: {color}");
                break;
            case ShapeType.Circle:
                this.circle.SwapGradStartColor(new[] { Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue },
                    color => this.btnGradClrStart.Text = $"Solid Fill Clr: {color}");
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        LayoutButtonsBottom();
    }
    #endregion
    #pragma warning restore SA1300

    /// <summary>
    /// Lays out the buttons on the left side of the scene.
    /// </summary>
    private void LayoutButtonsLeftSide()
    {
        var excludeList = new[]
        {
            nameof(this.btnShapeType),
            nameof(this.btnIncCircleDiam),
            nameof(this.btnDecCircleDiam),
            nameof(this.btnIncreaseWidth),
            nameof(this.btnDecreaseWidth),
            nameof(this.btnIncreaseHeight),
            nameof(this.btnDecreaseHeight),
            nameof(this.btnIsSolid),
            nameof(this.btnSolidFillClr),
            nameof(this.btnIncreaseBorderThickness),
            nameof(this.btnDecreaseBorderThickness),
        };

        var buttons = Controls.Where(c => excludeList.Contains(c.Name) && c.Visible).ToImmutableArray();

        var totalHeight = (from b in buttons
            select (int)b.Height).Sum();
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
            select (int)b.Height).Sum();
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
            select (int)b.Width).Sum();
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

    private void ShowButtonsForShape()
    {
        var rectControls = Controls.Where(c => this.rectButtons.Contains(c.Name) && c is Button).ToImmutableArray();
        var circleControls = Controls.Where(c => this.circleButtons.Contains(c.Name) && c is Button).ToImmutableArray();

        // Set all buttons to be false to start off
        foreach (var ctrl in Controls)
        {
            ctrl.Visible = false;
        }

        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                foreach (var button in rectControls)
                {
                    button.Visible = this.shapeType == ShapeType.Rectangle;
                }

                break;
            case ShapeType.Circle:
                foreach (var button in circleControls)
                {
                    button.Visible = this.shapeType == ShapeType.Circle;
                }

                break;
        }
    }

    /// <summary>
    /// Moves the shape using the keyboard arrow keys.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    private void MoveShape(FrameTime frameTime)
    {
        if (this.currentKeyState.AnyShiftKeysDown())
        {
            return;
        }

        var velocity = Vector2.Zero;
        var displacement = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            velocity.X -= displacement;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            velocity.X += displacement;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            velocity.Y -= displacement;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            velocity.Y += displacement;
        }

        var shapePos = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.Position
            : this.circle.Position;

        shapePos += velocity;

        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.Position = shapePos;
                break;
            case ShapeType.Circle:
                this.circle.Position = shapePos;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        ContainShape();
    }

    /// <summary>
    /// Contains the current shape within the window.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown with an invalid <see cref="ShapeType"/>.</exception>
    private void ContainShape()
    {
        var shapePos = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.Position
            : this.circle.Position;

        var overlapX = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.HalfWidth
            : this.circle.Radius;

        var overlapY = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.HalfHeight
            : this.circle.Radius;

        // Left edge containment
        if (shapePos.X < overlapX)
        {
            shapePos = shapePos with { X = overlapX };
        }

        // Right edge containment
        if (shapePos.X > WindowSize.Width - overlapX)
        {
            shapePos = shapePos with { X = WindowSize.Width - overlapX };
        }

        // Top edge containment
        if (shapePos.Y < overlapY)
        {
            shapePos = shapePos with { Y = overlapY };
        }

        // Bottom edge containment
        if (shapePos.Y > WindowSize.Height - overlapY)
        {
            shapePos = shapePos with { Y = WindowSize.Height - overlapY };
        }

        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.Position = shapePos;
                break;
            case ShapeType.Circle:
                this.circle.Position = shapePos;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }
    }

    /// <summary>
    /// Changes the size of the shape using the keyboard keys.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    private void ChangeRectSize(FrameTime frameTime)
    {
        if (this.currentKeyState.IsLeftShiftKeyDown() is false && this.currentKeyState.IsRightShiftKeyDown() is false)
        {
            return;
        }

        var width = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.Width
            : this.circle.Diameter;
        var height = this.shapeType == ShapeType.Rectangle
            ? this.rectangle.Height
            : this.circle.Diameter;

        var size = new Vector2(width, height);
        var diameterChange = 0f;
        var change = Vector2.Zero;

        var newValue = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            change.X -= newValue;
            diameterChange -= newValue;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            change.X += newValue;
            diameterChange += newValue;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            change.Y += newValue;
            diameterChange += newValue;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            change.Y -= newValue;
            diameterChange -= newValue;
        }

        size += change;

        switch (this.shapeType)
        {
            case ShapeType.Rectangle:
                this.rectangle.Width = size.X;
                this.rectangle.Height = size.Y;
                break;
            case ShapeType.Circle:
                this.circle.Diameter += diameterChange;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.shapeType)}",
                    (int)this.shapeType,
                    typeof(ShapeType));
        }

        this.rectangle.Width = this.rectangle.Width > WindowSize.Width ? WindowSize.Width : this.rectangle.Width;
        this.rectangle.Height = this.rectangle.Height > WindowSize.Height ? WindowSize.Height : this.rectangle.Height;

        var smallestWindowSize = WindowSize.Width < WindowSize.Height ? WindowSize.Width : WindowSize.Height;
        this.circle.Diameter = this.circle.Diameter > smallestWindowSize ? smallestWindowSize : this.circle.Diameter;

        ContainShape();
    }
}
