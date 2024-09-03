// <copyright file="ShapeScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests out rectangle rendering.
/// </summary>
public class ShapeScene : SceneBase
{
    private const int WindowPadding = 10;
    private const int Speed = 200;
    private const float DefaultCircleDiameter = 250;
    private const float DefaultBorderThickness = 2;
    private const float DefaultRectWidth = 250;
    private const float DefaultRectHeight = 250;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ControlFactory ctrlFactory;

    private readonly Color[] clrList =
    [
        Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue
    ];

    private IShapeRenderer? shapeRenderer;
    private ILoader<IFont>? fontLoader;
    private IFont? font;
    private KeyboardState currentKeyState;
    private RectShape rectangle;
    private CircleShape circle;
    private IControlGroup? grpRectInstructions;
    private IControlGroup? grpCircleInstructions;
    private IControlGroup? grpShapeType;
    private IControlGroup? grpCircleCtrls;
    private IControlGroup? grpCircleClrGradCtrls;
    private IControlGroup? grpRectCtrls;
    private IControlGroup? grpRectClrGradCtrls;
    private IControlGroup? grpRectCornerRadiusCtrls;
    private BackgroundManager? backgroundManager;
    private ShapeType shapeType;
    private string? sldCircleDiameterName;
    private string? cmbCircleSolidColorName;
    private string? cmbRectSolidColorName;
    private string? sldRectWidthName;
    private string? sldRectHeightName;
    private string? sldTopLeftRadiusName;
    private string? sldTopRightRadiusName;
    private string? sldBottomRightRadiusName;
    private string? sldBottomLeftRadiusName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeScene"/> class.
    /// </summary>
    public ShapeScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.ctrlFactory = new ControlFactory();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.shapeType = ShapeType.Circle;
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();

        this.rectangle = new RectShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Width = 250,
            Height = 250,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.SeaGreen,
            IsSolid = false,
        };

        this.circle = new CircleShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Diameter = DefaultCircleDiameter,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.SeaGreen,
            BorderThickness = DefaultBorderThickness,
            IsSolid = false,
        };

        this.fontLoader = ContentLoaderFactory.CreateFontLoader();
        this.font = this.fontLoader.Load(DefaultRegularFont, 12);

        CreateCircleInstructions();
        CreateRectInstructions();
        CreateShapeTypeCtrls();
        CreateCircleCtrls();
        CreateCircleGradCtrls();
        CreateRectGradCtrls();
        CreateRectCtrls();
        CreateRadiusCtrls();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        this.backgroundManager?.Unload();
        this.fontLoader.Unload(this.font);
        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        MoveShape(frameTime);
        ChangeShapeSize(frameTime);

        this.grpCircleInstructions.Position = new Point(WindowCenter.X - this.grpCircleInstructions.HalfWidth, WindowPadding);

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

        this.backgroundManager?.Render();

        this.grpCircleInstructions.Render();
        this.grpRectInstructions.Render();
        this.grpShapeType.Render();
        this.grpCircleCtrls.Render();
        this.grpCircleClrGradCtrls.Render();
        this.grpRectClrGradCtrls.Render();
        this.grpRectCtrls.Render();
        this.grpRectCornerRadiusCtrls.Render();

        base.Render();
    }

    private void CreateCircleInstructions()
    {
        var textLines = new[]
        {
            "----Circle Instructions----", "1. Movement: Arrow Keys", "2. Size:", "   - Increase Diameter: Shift + Up Or Right Arrow",
            "   - Decrease Diameter: Shift + Down Or Left Arrow",
        };
        var circleInstructions = string.Join(Environment.NewLine, textLines);
        var lblCircleInstructions = this.ctrlFactory.CreateLabel();
        lblCircleInstructions.Name = nameof(lblCircleInstructions);
        lblCircleInstructions.Text = circleInstructions;

        this.grpCircleInstructions = this.ctrlFactory.CreateControlGroup();
        this.grpCircleInstructions.Title = "Circle Instructions";
        this.grpCircleInstructions.AutoSizeToFitContent = true;
        this.grpCircleInstructions.TitleBarVisible = false;
        this.grpCircleInstructions.Add(lblCircleInstructions);
    }

    private void CreateRectInstructions()
    {
        var textLines = new[]
        {
            "----Rectangle Instructions----", "1. Movement: Arrow Keys", "2. Size:", "   - Increase Width: Shift + Right Arrow",
            "   - Decrease Width: Shift + Left Arrow", "   - Increase Height: Shift + Up Arrow", "   - Decrease Height: Shift + Down Arrow",
        };

        var rectInstructions = string.Join(Environment.NewLine, textLines);
        var lblRectInstructions = this.ctrlFactory.CreateLabel();
        lblRectInstructions.Name = nameof(lblRectInstructions);
        lblRectInstructions.Text = rectInstructions;

        this.grpRectInstructions = this.ctrlFactory.CreateControlGroup();
        this.grpRectInstructions.Title = "Rect Instructions";
        this.grpRectInstructions.AutoSizeToFitContent = true;
        this.grpRectInstructions.TitleBarVisible = false;
        this.grpRectInstructions.Visible = false;
        this.grpRectInstructions.SizeChanged += (_, size) =>
        {
            this.grpRectInstructions.Position = new Point(WindowCenter.X - (size.Width / 2), WindowPadding);
        };
        this.grpRectInstructions.Add(lblRectInstructions);
    }

    private void CreateShapeTypeCtrls()
    {
        var cmbShapeType = this.ctrlFactory.CreateComboBox();
        cmbShapeType.Name = nameof(cmbShapeType);
        cmbShapeType.Label = "Shape Type:";
        cmbShapeType.Width = 125;
        cmbShapeType.Items =
        [
            ShapeType.Rectangle.ToString(),
            ShapeType.Circle.ToString(),
        ];
        cmbShapeType.SelectedItemIndex = 1;
        cmbShapeType.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.shapeType = (ShapeType)selectedIndex;

            this.grpCircleInstructions.Visible = this.shapeType == ShapeType.Circle;
            this.grpCircleCtrls.Visible = this.shapeType == ShapeType.Circle;
            this.grpCircleClrGradCtrls.Visible = this.shapeType == ShapeType.Circle;

            this.grpRectInstructions.Visible = this.shapeType == ShapeType.Rectangle;
            this.grpRectCtrls.Visible = this.shapeType == ShapeType.Rectangle;
            this.grpRectClrGradCtrls.Visible = this.shapeType == ShapeType.Rectangle;
            this.grpRectCornerRadiusCtrls.Visible = this.shapeType == ShapeType.Rectangle;
        };

        this.grpShapeType = this.ctrlFactory.CreateControlGroup();
        this.grpShapeType.Title = "Shape Type:";
        this.grpShapeType.TitleBarVisible = false;
        this.grpShapeType.AutoSizeToFitContent = true;
        this.grpShapeType.Initialized += (_, _) =>
        {
            this.grpShapeType.Position = new Point(WindowPadding, WindowPadding);
        };
        this.grpShapeType.Add(cmbShapeType);
    }

    private void CreateCircleCtrls()
    {
        var sldCircleBorderThickness = this.ctrlFactory.CreateSlider();
        sldCircleBorderThickness.Name = nameof(sldCircleBorderThickness);
        sldCircleBorderThickness.Text = "Border Thickness:";
        sldCircleBorderThickness.Value = DefaultBorderThickness;
        sldCircleBorderThickness.Min = 1;
        sldCircleBorderThickness.Max = DefaultCircleDiameter / 2;
        sldCircleBorderThickness.ValueChanged += (_, newValue) =>
        {
            this.circle.BorderThickness = newValue;
        };

        var sldCircleDiameter = this.ctrlFactory.CreateSlider();
        this.sldCircleDiameterName = nameof(sldCircleDiameter);
        sldCircleDiameter.Name = nameof(sldCircleDiameter);
        sldCircleDiameter.Text = "Diameter:";
        sldCircleDiameter.Value = DefaultCircleDiameter;
        sldCircleDiameter.Min = 10;
        sldCircleDiameter.Max = 500;
        sldCircleDiameter.ValueChanged += (_, newValue) =>
        {
            var sldBorderThickness = this.grpCircleCtrls.GetControl<ISlider>(nameof(sldCircleBorderThickness));
            sldBorderThickness.Max = newValue / 2;
            this.circle.Diameter = newValue;
        };

        var cmbCircleSolidColor = this.ctrlFactory.CreateComboBox();
        cmbCircleSolidColor.Name = nameof(cmbCircleSolidColor);
        this.cmbCircleSolidColorName = nameof(cmbCircleSolidColor);
        cmbCircleSolidColor.Label = "Solid Color:";
        cmbCircleSolidColor.Width = 100;
        cmbCircleSolidColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbCircleSolidColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.circle.Color = this.clrList[selectedIndex];
        };
        cmbCircleSolidColor.SelectedItemIndex = 2;

        var chkCircleIsSolid = this.ctrlFactory.CreateCheckbox();
        chkCircleIsSolid.Name = nameof(chkCircleIsSolid);
        chkCircleIsSolid.LabelWhenChecked = "Solid";
        chkCircleIsSolid.LabelWhenUnchecked = "Not Solid";
        chkCircleIsSolid.CheckedChanged += (_, isChecked) =>
        {
            this.circle.IsSolid = isChecked;
        };

        this.grpCircleCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpCircleCtrls.Title = "Circle Props";
        this.grpCircleCtrls.AutoSizeToFitContent = true;
        this.grpCircleCtrls.Initialized += (_, _) =>
        {
            this.grpCircleCtrls.Position = new Point(WindowPadding, this.grpShapeType.Bottom + WindowPadding);
        };
        this.grpCircleCtrls.Add(chkCircleIsSolid);
        this.grpCircleCtrls.Add(cmbCircleSolidColor);
        this.grpCircleCtrls.Add(sldCircleBorderThickness);
        this.grpCircleCtrls.Add(sldCircleDiameter);
    }

    private void CreateCircleGradCtrls()
    {
        var cmbCircleGradType = this.ctrlFactory.CreateComboBox();
        cmbCircleGradType.Name = nameof(cmbCircleGradType);
        cmbCircleGradType.Label = "Gradient Type:";
        cmbCircleGradType.Width = 125;
        cmbCircleGradType.Items =
        [
            ColorGradient.None.ToString(),
            ColorGradient.Horizontal.ToString(),
            ColorGradient.Vertical.ToString(),
        ];
        cmbCircleGradType.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            var selectedGradType = (ColorGradient)selectedIndex;
            var cmbSolidClr = this.grpCircleCtrls.GetControl<IComboBox>(this.cmbCircleSolidColorName);

            if (cmbSolidClr is not null)
            {
                cmbSolidClr.Enabled = selectedGradType == ColorGradient.None;
            }

            this.circle.GradientType = selectedGradType;
        };

        var cmbCircleGradStartColor = this.ctrlFactory.CreateComboBox();
        cmbCircleGradStartColor.Name = nameof(cmbCircleGradStartColor);
        cmbCircleGradStartColor.Label = "Grad Start Color:";
        cmbCircleGradStartColor.Width = 100;
        cmbCircleGradStartColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbCircleGradStartColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.circle.GradientStart = this.clrList[selectedIndex];
        };

        var cmbCircleGradStopColor = this.ctrlFactory.CreateComboBox();
        cmbCircleGradStopColor.Name = nameof(cmbCircleGradStopColor);
        cmbCircleGradStopColor.Label = "Grad Stop Color:";
        cmbCircleGradStopColor.Width = 100;
        cmbCircleGradStopColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbCircleGradStopColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.circle.GradientStop = this.clrList[selectedIndex];
        };
        cmbCircleGradStopColor.SelectedItemIndex = 1;

        this.grpCircleClrGradCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpCircleClrGradCtrls.Title = "Circle Color Gradient Props";
        this.grpCircleClrGradCtrls.AutoSizeToFitContent = true;
        this.grpCircleClrGradCtrls.Initialized += (_, _) =>
        {
            this.grpCircleClrGradCtrls.Position = new Point(WindowPadding, WindowCenter.Y - this.grpCircleClrGradCtrls.HalfHeight);
        };
        this.grpCircleClrGradCtrls.Add(cmbCircleGradType);
        this.grpCircleClrGradCtrls.Add(cmbCircleGradStartColor);
        this.grpCircleClrGradCtrls.Add(cmbCircleGradStopColor);
    }

    private void CreateRectGradCtrls()
    {
        var cmbRectGradType = this.ctrlFactory.CreateComboBox();
        cmbRectGradType.Name = nameof(cmbRectGradType);
        cmbRectGradType.Label = "Gradient Type:";
        cmbRectGradType.Width = 125;
        cmbRectGradType.Items =
        [
            ColorGradient.None.ToString(),
            ColorGradient.Horizontal.ToString(),
            ColorGradient.Vertical.ToString(),
        ];
        cmbRectGradType.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            var selectedGradType = (ColorGradient)selectedIndex;
            var cmbSolidClr = this.grpRectCtrls.GetControl<IComboBox>(this.cmbRectSolidColorName);

            if (cmbSolidClr is not null)
            {
                cmbSolidClr.Enabled = selectedGradType == ColorGradient.None;
            }

            this.rectangle.GradientType = selectedGradType;
        };

        var cmbRectGradStartColor = this.ctrlFactory.CreateComboBox();
        cmbRectGradStartColor.Name = nameof(cmbRectGradStartColor);
        cmbRectGradStartColor.Label = "Grad Start Color:";
        cmbRectGradStartColor.Width = 100;
        cmbRectGradStartColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbRectGradStartColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.rectangle.GradientStart = this.clrList[selectedIndex];
        };

        var cmbRectGradStopColor = this.ctrlFactory.CreateComboBox();
        cmbRectGradStopColor.Name = nameof(cmbRectGradStopColor);
        cmbRectGradStopColor.Label = "Grad Stop Color:";
        cmbRectGradStopColor.Width = 100;
        cmbRectGradStopColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbRectGradStopColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.rectangle.GradientStop = this.clrList[selectedIndex];
        };
        cmbRectGradStopColor.SelectedItemIndex = 1;

        this.grpRectClrGradCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpRectClrGradCtrls.Title = "Rect Color Gradient Props";
        this.grpRectClrGradCtrls.AutoSizeToFitContent = true;
        this.grpRectClrGradCtrls.Visible = false;
        this.grpRectClrGradCtrls.Initialized += (_, _) =>
        {
            this.grpRectClrGradCtrls.Position = new Point(WindowPadding, WindowCenter.Y - this.grpRectClrGradCtrls.HalfHeight);
        };
        this.grpRectClrGradCtrls.Add(cmbRectGradType);
        this.grpRectClrGradCtrls.Add(cmbRectGradStopColor);
        this.grpRectClrGradCtrls.Add(cmbRectGradStartColor);
    }

    private void CreateRectCtrls()
    {
        var sldRectBorderThickness = this.ctrlFactory.CreateSlider();
        sldRectBorderThickness.Name = nameof(sldRectBorderThickness);
        sldRectBorderThickness.Text = "Border Thickness:";
        sldRectBorderThickness.Value = DefaultBorderThickness;
        sldRectBorderThickness.Min = 1;
        sldRectBorderThickness.Max = 125;
        sldRectBorderThickness.ValueChanged += (_, newValue) =>
        {
            this.rectangle.BorderThickness = newValue;
        };

        var sldRectWidth = this.ctrlFactory.CreateSlider();
        sldRectWidth.Name = nameof(sldRectWidth);
        this.sldRectWidthName = nameof(sldRectWidth);
        sldRectWidth.Text = "Width:";
        sldRectWidth.Value = DefaultRectWidth;
        sldRectWidth.Min = 50;
        sldRectWidth.Max = 500;
        sldRectWidth.ValueChanged += (_, newWidth) =>
        {
            this.rectangle.Width = newWidth;
            var newValue = (this.rectangle.Width < this.rectangle.Height
                ? this.rectangle.Width
                : this.rectangle.Height) / 2;

            var sldBorderThicknessCtrl = this.grpRectCtrls.GetControl<ISlider>(nameof(sldRectBorderThickness));
            var sldTopLeftRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldTopLeftRadiusName);
            var sldTopRightRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldTopRightRadiusName);
            var sldBottomRightRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldBottomRightRadiusName);
            var sldBottomLeftRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldBottomLeftRadiusName);

            sldBorderThicknessCtrl.Max = newValue;
            sldTopLeftRadiusCtrl.Max = newValue;
            sldTopRightRadiusCtrl.Max = newValue;
            sldBottomRightRadiusCtrl.Max = newValue;
            sldBottomLeftRadiusCtrl.Max = newValue;
        };

        var sldRectHeight = this.ctrlFactory.CreateSlider();
        sldRectHeight.Name = nameof(sldRectHeight);
        this.sldRectHeightName = nameof(sldRectHeight);
        sldRectHeight.Text = "Height:";
        sldRectHeight.Value = DefaultRectHeight;
        sldRectHeight.Min = 50;
        sldRectHeight.Max = 500;
        sldRectHeight.ValueChanged += (_, newHeight) =>
        {
            this.rectangle.Height = newHeight;
            var newValue = (this.rectangle.Height < this.rectangle.Width
                ? this.rectangle.Height
                : this.rectangle.Width) / 2;

            var sldBorderThicknessCtrl = this.grpRectCtrls.GetControl<ISlider>(nameof(sldRectBorderThickness));
            var sldTopLeftRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldTopLeftRadiusName);
            var sldTopRightRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldTopRightRadiusName);
            var sldBottomRightRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldBottomRightRadiusName);
            var sldBottomLeftRadiusCtrl = this.grpRectCornerRadiusCtrls.GetControl<ISlider>(this.sldBottomLeftRadiusName);

            sldBorderThicknessCtrl.Max = newValue;
            sldTopLeftRadiusCtrl.Max = newValue;
            sldTopRightRadiusCtrl.Max = newValue;
            sldBottomRightRadiusCtrl.Max = newValue;
            sldBottomLeftRadiusCtrl.Max = newValue;
        };

        var cmbRectSolidColor = this.ctrlFactory.CreateComboBox();
        cmbRectSolidColor.Name = nameof(cmbRectSolidColor);
        this.cmbRectSolidColorName = nameof(cmbRectSolidColor);
        cmbRectSolidColor.Label = "Solid Color:";
        cmbRectSolidColor.Width = 100;
        cmbRectSolidColor.Items =
        [
            "Red",
            "Green",
            "Blue",
        ];
        cmbRectSolidColor.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.rectangle.Color = this.clrList[selectedIndex];
        };
        cmbRectSolidColor.SelectedItemIndex = 2;

        var chkRectIsSolid = this.ctrlFactory.CreateCheckbox();
        chkRectIsSolid.Name = nameof(chkRectIsSolid);
        chkRectIsSolid.LabelWhenChecked = "Solid";
        chkRectIsSolid.LabelWhenUnchecked = "Not Solid";
        chkRectIsSolid.CheckedChanged += (_, isChecked) =>
        {
            this.rectangle.IsSolid = isChecked;
        };

        this.grpRectCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpRectCtrls.Title = "Rectangle Props";
        this.grpRectCtrls.AutoSizeToFitContent = true;
        this.grpRectCtrls.Visible = false;
        this.grpRectCtrls.Initialized += (_, _) =>
        {
            this.grpRectCtrls.Position = new Point(WindowPadding, this.grpShapeType.Bottom + WindowPadding);
        };
        this.grpRectCtrls.Add(chkRectIsSolid);
        this.grpRectCtrls.Add(cmbRectSolidColor);
        this.grpRectCtrls.Add(sldRectBorderThickness);
        this.grpRectCtrls.Add(sldRectHeight);
        this.grpRectCtrls.Add(sldRectWidth);
    }

    [SuppressMessage("csharpsquid", "S2583", Justification = "Need to leave as is in case of constant value change.")]
    [SuppressMessage("csharpsquid", "S3776", Justification = "Do not care about cognitive complexity.")]
    private void CreateRadiusCtrls()
    {
        // ReSharper disable once HeuristicUnreachableCode
        const float defaultRadius = (DefaultRectWidth < DefaultRectHeight ? DefaultRectWidth : DefaultRectHeight) / 2;

        var sldTopLeftRadius = this.ctrlFactory.CreateSlider();
        sldTopLeftRadius.Name = nameof(sldTopLeftRadius);
        this.sldTopLeftRadiusName = nameof(sldTopLeftRadius);
        sldTopLeftRadius.Text = "Top Left Radius:";
        sldTopLeftRadius.Min = 0;
        sldTopLeftRadius.Max = defaultRadius;
        sldTopLeftRadius.ValueChanged += (_, newRadius) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.TopLeft > maxValue ? maxValue : newRadius;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius with { TopLeft = newValue, };
        };

        var sldTopRightRadius = this.ctrlFactory.CreateSlider();
        sldTopRightRadius.Name = nameof(sldTopRightRadius);
        this.sldTopRightRadiusName = nameof(sldTopRightRadius);
        sldTopRightRadius.Text = "Top Right:";
        sldTopRightRadius.Min = 0;
        sldTopRightRadius.Max = defaultRadius;
        sldTopRightRadius.ValueChanged += (_, newRadius) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.TopRight > maxValue ? maxValue : newRadius;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius with { TopRight = newValue, };
        };

        var sldBottomRightRadius = this.ctrlFactory.CreateSlider();
        sldBottomRightRadius.Name = nameof(sldBottomRightRadius);
        this.sldBottomRightRadiusName = nameof(sldBottomRightRadius);
        sldBottomRightRadius.Text = "Bottom Right:";
        sldBottomRightRadius.Min = 0;
        sldBottomRightRadius.Max = defaultRadius;
        sldBottomRightRadius.ValueChanged += (_, newRadius) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.BottomRight > maxValue ? maxValue : newRadius;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius with { BottomRight = newValue, };
        };

        var sldBottomLeftRadius = this.ctrlFactory.CreateSlider();
        sldBottomLeftRadius.Name = nameof(sldBottomLeftRadius);
        this.sldBottomLeftRadiusName = nameof(sldBottomLeftRadius);
        sldBottomLeftRadius.Text = "Bottom Left:";
        sldBottomLeftRadius.Min = 0;
        sldBottomLeftRadius.Max = defaultRadius;
        sldBottomLeftRadius.ValueChanged += (_, newRadius) =>
        {
            var maxValue = (this.rectangle.Width > this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height) / 2f;
            var newValue = this.rectangle.CornerRadius.BottomLeft > maxValue ? maxValue : newRadius;

            this.rectangle.CornerRadius = this.rectangle.CornerRadius with { BottomLeft = newValue, };
        };

        this.grpRectCornerRadiusCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpRectCornerRadiusCtrls.Title = "Rect Radius Props";
        this.grpRectCornerRadiusCtrls.AutoSizeToFitContent = true;
        this.grpRectCornerRadiusCtrls.Visible = false;
        this.grpRectCornerRadiusCtrls.SizeChanged += (_, size) =>
        {
            this.grpRectCornerRadiusCtrls.Position = new Point(
                (int)WindowSize.Width - (size.Width + WindowPadding),
                WindowCenter.Y - (size.Height / 2));
        };
        this.grpRectCornerRadiusCtrls.Add(sldBottomLeftRadius);
        this.grpRectCornerRadiusCtrls.Add(sldBottomRightRadius);
        this.grpRectCornerRadiusCtrls.Add(sldTopRightRadius);
        this.grpRectCornerRadiusCtrls.Add(sldTopLeftRadius);
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
    /// <exception cref="InvalidEnumArgumentException">
    ///     Thrown if the <see cref="ShapeType"/> is invalid.
    /// </exception>
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
    [SuppressMessage("csharpsquid", "S3776", Justification = "Do not care about cognitive complexity.")]
    private void ChangeShapeSize(FrameTime frameTime)
    {
        if (!this.currentKeyState.IsLeftShiftKeyDown() && !this.currentKeyState.IsRightShiftKeyDown())
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

                var sldRectWidthCtrl = this.grpRectCtrls.GetControl<ISlider>(this.sldRectWidthName);
                var sldRectHeightCtrl = this.grpRectCtrls.GetControl<ISlider>(this.sldRectHeightName);

                this.rectangle.Width = this.rectangle.Width > sldRectWidthCtrl.Max ? sldRectWidthCtrl.Max : this.rectangle.Width;
                this.rectangle.Width = this.rectangle.Width < sldRectWidthCtrl.Min ? sldRectWidthCtrl.Min : this.rectangle.Width;

                this.rectangle.Height = this.rectangle.Height > sldRectHeightCtrl.Max ? sldRectHeightCtrl.Max : this.rectangle.Height;
                this.rectangle.Height = this.rectangle.Height < sldRectHeightCtrl.Min ? sldRectHeightCtrl.Min : this.rectangle.Height;

                sldRectWidthCtrl.Value = this.rectangle.Width;
                sldRectHeightCtrl.Value = this.rectangle.Height;

                break;
            case ShapeType.Circle:
                this.circle.Diameter += diameterChange;

                var sldCircleDiameterCtrl = this.grpCircleCtrls.GetControl<ISlider>(this.sldCircleDiameterName);

                this.circle.Diameter = this.circle.Diameter > sldCircleDiameterCtrl.Max ? sldCircleDiameterCtrl.Max : this.circle.Diameter;
                this.circle.Diameter = this.circle.Diameter < sldCircleDiameterCtrl.Min ? sldCircleDiameterCtrl.Min : this.circle.Diameter;

                sldCircleDiameterCtrl.Value = this.circle.Diameter;
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
