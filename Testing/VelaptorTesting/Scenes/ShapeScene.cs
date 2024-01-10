// <copyright file="ShapeScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using UI;
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
    private readonly Color[] clrList =
    [
        Color.IndianRed, Color.SeaGreen, Color.CornflowerBlue
    ];
    private IFontRenderer? fontRenderer;
    private IShapeRenderer? shapeRenderer;
    private ILoader<IFont>? fontLoader;
    private IFont? font;
    private KeyboardState currentKeyState;
    private RectShape rectangle;
    private CircleShape circle;
    private IControlGroup? grpShapeType;
    private IControlGroup? grpCircleCtrls;
    private IControlGroup? grpCircleClrGradCtrls;
    private IControlGroup? grpRectClrGradCtrls;
    private IControlGroup? grpRectCtrls;
    private IControlGroup? grpRectCornerRadiusCtrls;
    private Vector2 rectInstructionsPos;
    private Vector2 circleInstructionsPos;
    private BackgroundManager? backgroundManager;
    private ShapeType shapeType = ShapeType.Circle;
    private string rectInstructions = string.Empty;
    private string circleInstructions = string.Empty;
    private string? sldCircleDiameterName;
    private string? cmbCircleSolidColorName;
    private string? cmbRectSolidColorName;
    private string? sldRectWidthName;
    private string? sldRectHeightName;
    private string? sldTopLeftRadiusName;
    private string? sldTopRightRadiusName;
    private string? sldBottomRightRadiusName;
    private string? sldBottomLeftRadiusName;
    private bool isFirstRender = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeScene"/> class.
    /// </summary>
    public ShapeScene() => this.keyboard = HardwareFactory.GetKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.fontRenderer = RendererFactory.CreateFontRenderer();
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

        this.grpShapeType.Render();
        this.grpCircleCtrls.Render();
        this.grpCircleClrGradCtrls.Render();
        this.grpRectClrGradCtrls.Render();
        this.grpRectCtrls.Render();
        this.grpRectCornerRadiusCtrls.Render();

        if (this.isFirstRender)
        {
            this.grpShapeType.Position = new Point(WindowPadding, WindowPadding);
            this.grpCircleCtrls.Position = new Point(WindowPadding, this.grpShapeType.Bottom + WindowPadding);
            this.grpCircleClrGradCtrls.Position = new Point(WindowPadding, this.grpCircleCtrls.Bottom + WindowPadding + 50);

            this.grpRectCtrls.Position = new Point(WindowPadding, this.grpShapeType.Bottom + WindowPadding);
            this.grpRectCornerRadiusCtrls.Position = new Point(
                (int)WindowSize.Width - (this.grpRectCornerRadiusCtrls.Size.Width + WindowPadding),
                ((int)WindowSize.Height / 2) - (this.grpRectCornerRadiusCtrls.Size.Width / 2));

            this.isFirstRender = false;
        }

        base.Render();
    }

    private void CreateShapeTypeCtrls()
    {
        var cmbShapeType = TestingApp.Container.GetInstance<IComboBox>();
        cmbShapeType.Position = new Point(WindowPadding, WindowPadding);
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

            this.grpCircleCtrls.Visible = this.shapeType == ShapeType.Circle;
            this.grpCircleClrGradCtrls.Visible = this.shapeType == ShapeType.Circle;

            this.grpRectCtrls.Visible = this.shapeType == ShapeType.Rectangle;
            this.grpRectClrGradCtrls.Visible = this.shapeType == ShapeType.Rectangle;
            this.grpRectCornerRadiusCtrls.Visible = this.shapeType == ShapeType.Rectangle;
        };

        this.grpShapeType = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpShapeType.TitleBarVisible = false;
        this.grpShapeType.AutoSizeToFitContent = true;
        this.grpShapeType.Add(cmbShapeType);
    }

    private void CreateCircleCtrls()
    {
        var sldCircleBorderThickness = TestingApp.Container.GetInstance<ISlider>();
        sldCircleBorderThickness.Name = nameof(sldCircleBorderThickness);
        sldCircleBorderThickness.Text = "Border Thickness:";
        sldCircleBorderThickness.Value = DefaultBorderThickness;
        sldCircleBorderThickness.Min = 1;
        sldCircleBorderThickness.Max = DefaultCircleDiameter / 2;
        sldCircleBorderThickness.ValueChanged += (_, newValue) =>
        {
            this.circle.BorderThickness = newValue;
        };

        var sldCircleDiameter = TestingApp.Container.GetInstance<ISlider>();
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

        var cmbCircleSolidColor = TestingApp.Container.GetInstance<IComboBox>();
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

        var chkCircleIsSolid = TestingApp.Container.GetInstance<ICheckBox>();
        chkCircleIsSolid.Name = nameof(chkCircleIsSolid);
        chkCircleIsSolid.LabelWhenChecked = "Solid";
        chkCircleIsSolid.LabelWhenUnchecked = "Not Solid";
        chkCircleIsSolid.CheckedChanged += (_, isChecked) =>
        {
            this.circle.IsSolid = isChecked;
        };

        this.grpCircleCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpCircleCtrls.Title = "Circle Props";
        this.grpCircleCtrls.AutoSizeToFitContent = true;
        this.grpCircleCtrls.Add(chkCircleIsSolid);
        this.grpCircleCtrls.Add(cmbCircleSolidColor);
        this.grpCircleCtrls.Add(sldCircleBorderThickness);
        this.grpCircleCtrls.Add(sldCircleDiameter);
    }

    private void CreateCircleGradCtrls()
    {
        var cmbCircleGradType = TestingApp.Container.GetInstance<IComboBox>();
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

        var cmbCircleGradStartColor = TestingApp.Container.GetInstance<IComboBox>();
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
            switch (this.shapeType)
            {
                case ShapeType.Rectangle:
                    this.rectangle.GradientStart = this.clrList[selectedIndex];
                    break;
                case ShapeType.Circle:
                    this.circle.GradientStart = this.clrList[selectedIndex];
                    break;
            }
        };

        var cmbCircleGradStopColor = TestingApp.Container.GetInstance<IComboBox>();
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
            switch (this.shapeType)
            {
                case ShapeType.Rectangle:
                    this.rectangle.GradientStop = this.clrList[selectedIndex];
                    break;
                case ShapeType.Circle:
                    this.circle.GradientStop = this.clrList[selectedIndex];
                    break;
            }
        };
        cmbCircleGradStopColor.SelectedItemIndex = 1;

        this.grpCircleClrGradCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpCircleClrGradCtrls.Title = "Color Gradient Props";
        this.grpCircleClrGradCtrls.AutoSizeToFitContent = true;
        this.grpCircleClrGradCtrls.Add(cmbCircleGradType);
        this.grpCircleClrGradCtrls.Add(cmbCircleGradStopColor);
        this.grpCircleClrGradCtrls.Add(cmbCircleGradStartColor);
    }

    private void CreateRectGradCtrls()
    {
        var cmbRectGradType = TestingApp.Container.GetInstance<IComboBox>();
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

        var cmbRectGradStartColor = TestingApp.Container.GetInstance<IComboBox>();
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

        var cmbRectGradStopColor = TestingApp.Container.GetInstance<IComboBox>();
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

        this.grpRectClrGradCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpRectClrGradCtrls.Title = "Color Gradient Props";
        this.grpRectClrGradCtrls.AutoSizeToFitContent = true;
        this.grpRectClrGradCtrls.Visible = false;
        this.grpRectClrGradCtrls.Add(cmbRectGradType);
        this.grpRectClrGradCtrls.Add(cmbRectGradStopColor);
        this.grpRectClrGradCtrls.Add(cmbRectGradStartColor);
    }

    private void CreateRectCtrls()
    {
        var sldRectBorderThickness = TestingApp.Container.GetInstance<ISlider>();
        sldRectBorderThickness.Name = nameof(sldRectBorderThickness);
        sldRectBorderThickness.Text = "Border Thickness:";
        sldRectBorderThickness.Value = DefaultBorderThickness;
        sldRectBorderThickness.Min = 1;
        sldRectBorderThickness.Max = 125;
        sldRectBorderThickness.ValueChanged += (_, newValue) =>
        {
            this.rectangle.BorderThickness = newValue;
        };

        var sldRectWidth = TestingApp.Container.GetInstance<ISlider>();
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

        var sldRectHeight = TestingApp.Container.GetInstance<ISlider>();
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

        var cmbRectSolidColor = TestingApp.Container.GetInstance<IComboBox>();
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

        var chkRectIsSolid = TestingApp.Container.GetInstance<ICheckBox>();
        chkRectIsSolid.Name = nameof(chkRectIsSolid);
        chkRectIsSolid.LabelWhenChecked = "Solid";
        chkRectIsSolid.LabelWhenUnchecked = "Not Solid";
        chkRectIsSolid.CheckedChanged += (_, isChecked) =>
        {
            this.rectangle.IsSolid = isChecked;
        };

        this.grpRectCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpRectCtrls.Title = "Rectangle Props";
        this.grpRectCtrls.AutoSizeToFitContent = true;
        this.grpRectCtrls.Visible = false;
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

        var sldTopLeftRadius = TestingApp.Container.GetInstance<ISlider>();
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

        var sldTopRightRadius = TestingApp.Container.GetInstance<ISlider>();
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

        var sldBottomRightRadius = TestingApp.Container.GetInstance<ISlider>();
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

        var sldBottomLeftRadius = TestingApp.Container.GetInstance<ISlider>();
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

        this.grpRectCornerRadiusCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpRectCornerRadiusCtrls.Title = "Rect Radius Props";
        this.grpRectCornerRadiusCtrls.AutoSizeToFitContent = true;
        this.grpRectCornerRadiusCtrls.Visible = false;
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
