// <copyright file="ShapeScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Velum;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using Container = Velum.Container;
using VelUpdatable = Velaptor.IUpdatable;

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
    private const float MinRectWidth = 5;
    private const float MinRectHeight = 5;
    private const float MaxRectWidth = 500;
    private const float MaxRectHeight = 500;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly Dictionary<string, Color> clrList = new ()
    {
        { nameof(Color.Red), Color.IndianRed },
        { nameof(Color.Green), Color.SeaGreen },
        { nameof(Color.Blue), Color.CornflowerBlue },
    };
    private readonly IContentManager contentManager;
    private readonly IShapeRenderer shapeRenderer;
    private readonly BackgroundManager backgroundManager;
    private string circleInstructionText = string.Empty;
    private string rectInstructionText = string.Empty;
    private IFont? font;
    private KeyboardState currentKeyState;
    private RectShape rectangle;
    private CircleShape circle;
    private ShapeType shapeType;
    private Layout? layMain;
    private Container? conMain;
    private CheckBox? chkIsSolid;
    private DropDown? drpShapeType;
    private DropDown? drpSolidClr;
    private DropDown? drpGradType;
    private DropDown? drpGradStopClr;
    private DropDown? drpGradStartClr;
    private Label? lblInstructions;
    private Label? lblShapeType;
    private Label? lblSolidClr;
    private Label? lblBorderThickness;
    private Label? lblGradType;
    private Label? lblGradStopClr;
    private Label? lblGradStartClr;
    private Label? lblRectWidth;
    private Label? lblRectHeight;
    private Label? lblCircleDiameter;
    private Label? lblBottomLeftRadius;
    private Label? lblBottomRightRadius;
    private Label? lblTopRightRadius;
    private Label? lblTopLeftRadius;
    private Slider? sldBorderThickness;
    private Slider? sldRectWidth;
    private Slider? sldRectHeight;
    private Slider? sldCircleDiameter;
    private Slider? sldBottomLeftRadius;
    private Slider? sldBottomRightRadius;
    private Slider? sldTopRightRadius;
    private Slider? sldTopLeftRadius;
    private Layout? layRectHeight;
    private Layout? layCircleDiameter;
    private Layout? layBottomLeftRadius;
    private Layout? layBottomRightRadius;
    private Layout? layTopRightRadius;
    private Layout? layTopLeftRadius;
    private Layout? layShapeType;
    private Layout? laySolidClr;
    private Layout? layCircleProps;
    private Layout? layBorderThickness;
    private Layout? layGradType;
    private Layout? layGradStartClr;
    private Layout? layGradStopClr;
    private Layout? layRectWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeScene"/> class.
    /// </summary>
    public ShapeScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.contentManager = ContentManager.Create();
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.backgroundManager = new BackgroundManager();

        this.rectangle = new RectShape
        {
            Position = new Vector2(WindowCenter.X, WindowCenter.Y),
            Width = DefaultRectWidth,
            Height = DefaultRectHeight,
            Color = Color.CornflowerBlue,
            GradientType = ColorGradient.None,
            GradientStart = Color.IndianRed,
            GradientStop = Color.SeaGreen,
            IsSolid = true,
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
            IsSolid = true,
        };
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        CreateCtrls();

        this.shapeType = ShapeType.Circle;
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.font = this.contentManager.LoadFont(DefaultRegularFont, 12);

        this.lblInstructions.Load();

        this.conMain.Load();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.drpShapeType.SelectedItemChanged -= DrpShapeType_SelectedItemChanged;
        this.chkIsSolid.CheckedChanged -= ChkShapeIsSolid_CheckedChanged;
        this.drpSolidClr.SelectedItemChanged -= DrpSolidClr_SelectedItemChanged;
        this.sldBorderThickness.ValueChanged -= SldBorderThickness_ValueChanged;
        this.drpGradType.SelectedItemChanged -= DrpGradType_SelectedItemChanged;
        this.drpGradStartClr.SelectedItemChanged -= DrpGradStartClr_SelectedItemChanged;
        this.drpGradStopClr.SelectedItemChanged -= DrpGradStopClr_SelectedItemChanged;
        this.sldCircleDiameter.ValueChanged -= SldCircleDiameter_ValueChanged;
        this.sldRectWidth.ValueChanged -= SldRectWidth_ValueChanged;
        this.sldRectHeight.ValueChanged -= SldRectHeight_ValueChanged;
        this.sldBottomLeftRadius.ValueChanged -= SldBottomLeftRadius_ValueChanged;
        this.sldBottomRightRadius.ValueChanged -= SldBottomRightRadius_ValueChanged;
        this.sldTopRightRadius.ValueChanged -= SldTopRightRadius_ValueChanged;
        this.sldTopLeftRadius.ValueChanged -= SldTopLeftRadius_ValueChanged;

        this.backgroundManager.Unload();
        this.contentManager.Unload(this.font);
        this.lblInstructions.Unload();
        this.conMain.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        var isShiftDown = this.currentKeyState.IsKeyDown(KeyCode.LeftShift) || this.currentKeyState.IsKeyDown(KeyCode.RightShift);

        if (isShiftDown)
        {
            var delta = (float)frameTime.ElapsedTime.TotalSeconds * 150f;

            if (this.shapeType == ShapeType.Circle)
            {
                if (this.currentKeyState.IsKeyDown(KeyCode.Up) || this.currentKeyState.IsKeyDown(KeyCode.Right))
                {
                    this.circle.Diameter += delta;
                }

                if (this.currentKeyState.IsKeyDown(KeyCode.Down) || this.currentKeyState.IsKeyDown(KeyCode.Left))
                {
                    this.circle.Diameter -= delta;
                }
            }
            else
            {
                if (this.currentKeyState.IsKeyDown(KeyCode.Right))
                {
                    this.rectangle.Width += delta;
                    this.rectangle.Width = this.rectangle.Width > MaxRectWidth ? MaxRectWidth : this.rectangle.Width;
                }

                if (this.currentKeyState.IsKeyDown(KeyCode.Left))
                {
                    this.rectangle.Width -= delta;
                    this.rectangle.Width = this.rectangle.Width < MinRectWidth ? MinRectWidth : this.rectangle.Width;
                }

                if (this.currentKeyState.IsKeyDown(KeyCode.Up))
                {
                    this.rectangle.Height += delta;
                    this.rectangle.Height = this.rectangle.Height > MaxRectHeight ? MaxRectHeight : this.rectangle.Height;
                }

                if (this.currentKeyState.IsKeyDown(KeyCode.Down))
                {
                    this.rectangle.Height -= delta;
                    this.rectangle.Height = this.rectangle.Height < MinRectHeight ? MinRectHeight : this.rectangle.Height;
                }
            }
        }

        MoveShape(frameTime);

        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);
        this.conMain.Update();

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

        this.backgroundManager.Render();
        this.lblInstructions.Render(0);
        this.conMain.Render(0);

        base.Render();
    }

    private void CreateInstructions()
    {
        var circleTextLines = new[]
        {
            "----Circle Instructions----", "1. Movement: Arrow Keys", "2. Size:", "   - Increase Diameter: Shift + Up Or Right Arrow",
            "   - Decrease Diameter: Shift + Down Or Left Arrow",
        };
        var rectTextLines = new[]
        {
            "----Rectangle Instructions----", "1. Movement: Arrow Keys", "2. Size:", "   - Increase Width: Shift + Right Arrow",
            "   - Decrease Width: Shift + Left Arrow", "   - Increase Height: Shift + Up Arrow", "   - Decrease Height: Shift + Down Arrow",
        };

        this.circleInstructionText = string.Join(Environment.NewLine, circleTextLines);
        this.rectInstructionText = string.Join(Environment.NewLine, rectTextLines);
        this.lblInstructions = new Label
        {
            Text = this.circleInstructionText,
        };
    }

    private void CreateCtrls()
    {
        CreateInstructions();

        // Create shape type
        this.lblShapeType = new Label
        {
            Text = "Shape Type:",
        };
        this.drpShapeType = new DropDown();
        this.drpShapeType.AddItem(nameof(ShapeType.Circle));
        this.drpShapeType.AddItem(nameof(ShapeType.Rectangle));
        this.drpShapeType.SelectedItemChanged += DrpShapeType_SelectedItemChanged;

        this.layShapeType = new Layout();
        this.layShapeType.StackDirection = StackDirection.Horizontal;
        this.layShapeType.AddControl(this.lblShapeType);
        this.layShapeType.AddControl(this.drpShapeType);

        this.chkIsSolid = new CheckBox();
        this.chkIsSolid.IsChecked = true;
        this.chkIsSolid.Text = "Solid";
        this.chkIsSolid.CheckedChanged += ChkShapeIsSolid_CheckedChanged;

        // Circle color controls
        this.lblSolidClr = new Label();
        this.lblSolidClr.Text = "Solid Color:";

        this.drpSolidClr = new DropDown();
        this.drpSolidClr.AddItem("Blue");
        this.drpSolidClr.AddItem("Red");
        this.drpSolidClr.AddItem("Green");
        this.drpSolidClr.SelectedItemChanged += DrpSolidClr_SelectedItemChanged;

        // Circle color layout
        this.laySolidClr = new Layout();
        this.laySolidClr.StackDirection = StackDirection.Horizontal;
        this.laySolidClr.Centered = true;
        this.laySolidClr.AddControl(this.lblSolidClr);
        this.laySolidClr.AddControl(this.drpSolidClr);

        // Border thickness controls
        this.lblBorderThickness = new Label();
        this.lblBorderThickness.Text = "Border Thickness:";

        this.sldBorderThickness = new Slider();
        this.sldBorderThickness.Min = 1;
        this.sldBorderThickness.Max = DefaultCircleDiameter / 2;
        this.sldBorderThickness.ValueChanged += SldBorderThickness_ValueChanged;

        // Border thickness layout
        this.layBorderThickness = new Layout();
        this.layBorderThickness.StackDirection = StackDirection.Horizontal;
        this.layBorderThickness.Centered = true;
        this.layBorderThickness.AddControl(this.lblBorderThickness);
        this.layBorderThickness.AddControl(this.sldBorderThickness);

        // Circle properties layout
        this.layCircleProps = new Layout();
        this.layCircleProps.StackDirection = StackDirection.Vertical;
        this.layCircleProps.AddControl(this.chkIsSolid);
        this.layCircleProps.AddControl(this.laySolidClr);
        this.layCircleProps.AddControl(this.layBorderThickness);

        // Gradient type controls
        this.lblGradType = new Label();
        this.lblGradType.Text = "Gradient Type:";

        this.drpGradType = new DropDown();
        this.drpGradType.AddItem(nameof(ColorGradient.None));
        this.drpGradType.AddItem(nameof(ColorGradient.Horizontal));
        this.drpGradType.AddItem(nameof(ColorGradient.Vertical));
        this.drpGradType.SelectedItemChanged += DrpGradType_SelectedItemChanged;

        this.layGradType = new Layout();
        this.layGradType.StackDirection = StackDirection.Horizontal;
        this.layGradType.Centered = true;
        this.layGradType.Enabled = false;
        this.layGradType.AddControl(this.lblGradType);
        this.layGradType.AddControl(this.drpGradType);

        // Gradient start color
        this.lblGradStartClr = new Label();
        this.lblGradStartClr.Text = "Gradient Start Color:";

        this.drpGradStartClr = new DropDown();
        this.drpGradStartClr.AddItem(nameof(Color.Red));
        this.drpGradStartClr.AddItem(nameof(Color.Green));
        this.drpGradStartClr.AddItem(nameof(Color.Blue));
        this.drpGradStartClr.SelectedItemChanged += DrpGradStartClr_SelectedItemChanged;

        this.layGradStartClr = new Layout();
        this.layGradStartClr.StackDirection = StackDirection.Horizontal;
        this.layGradStartClr.Centered = true;
        this.layGradStartClr.Enabled = false;
        this.layGradStartClr.AddControl(this.lblGradStartClr);
        this.layGradStartClr.AddControl(this.drpGradStartClr);

        // Gradient stop color
        this.lblGradStopClr = new Label();
        this.lblGradStopClr.Text = "Gradient Stop Color:";

        this.drpGradStopClr = new DropDown();
        this.drpGradStopClr.SelectedItemChanged += DrpGradStopClr_SelectedItemChanged;
        this.drpGradStopClr.AddItem(nameof(Color.Green));
        this.drpGradStopClr.AddItem(nameof(Color.Red));
        this.drpGradStopClr.AddItem(nameof(Color.Blue));

        this.layGradStopClr = new Layout();
        this.layGradStopClr.StackDirection = StackDirection.Horizontal;
        this.layGradStopClr.Centered = true;
        this.layGradStopClr.Enabled = false;
        this.layGradStopClr.AddControl(this.lblGradStopClr);
        this.layGradStopClr.AddControl(this.drpGradStopClr);

        // Circle diameter
        this.lblCircleDiameter = new Label();
        this.lblCircleDiameter.Text = "Diameter:";

        this.sldCircleDiameter = new Slider();
        this.sldCircleDiameter.Min = 10;
        this.sldCircleDiameter.Max = 500;
        this.sldCircleDiameter.Value = DefaultCircleDiameter;
        this.sldCircleDiameter.ValueChanged += SldCircleDiameter_ValueChanged;

        this.layCircleDiameter = new Layout();
        this.layCircleDiameter.StackDirection = StackDirection.Horizontal;
        this.layCircleDiameter.Centered = true;
        this.layCircleDiameter.AddControl(this.lblCircleDiameter);
        this.layCircleDiameter.AddControl(this.sldCircleDiameter);

        // Rectangle width
        this.lblRectWidth = new Label();
        this.lblRectWidth.Text = "Width:";

        this.sldRectWidth = new Slider();
        this.sldRectWidth.Value = DefaultRectWidth;
        this.sldRectWidth.Min = MinRectWidth;
        this.sldRectWidth.Max = MaxRectHeight;
        this.sldRectWidth.ValueChanged += SldRectWidth_ValueChanged;

        this.layRectWidth = new Layout();
        this.layRectWidth.StackDirection = StackDirection.Horizontal;
        this.layRectWidth.Enabled = false;
        this.layRectWidth.Centered = true;
        this.layRectWidth.AddControl(this.lblRectWidth);
        this.layRectWidth.AddControl(this.sldRectWidth);

        this.lblRectHeight = new Label();
        this.lblRectHeight.Text = "Height:";

        // Rectangle height
        this.sldRectHeight = new Slider();
        this.sldRectHeight.Value = DefaultRectHeight;
        this.sldRectHeight.Min = MinRectHeight;
        this.sldRectHeight.Max = MaxRectHeight;
        this.sldRectHeight.ValueChanged += SldRectHeight_ValueChanged;

        this.layRectHeight = new Layout();
        this.layRectHeight.Name = "debug";
        this.layRectHeight.StackDirection = StackDirection.Horizontal;
        this.layRectHeight.Enabled = false;
        this.layRectHeight.Centered = true;
        this.layRectHeight.AddControl(this.lblRectHeight);
        this.layRectHeight.AddControl(this.sldRectHeight);

        // Bottom left radius
        this.lblBottomLeftRadius = new Label();
        this.lblBottomLeftRadius.Text = "Bottom Left:";

        this.sldBottomLeftRadius = new Slider();
        this.sldBottomLeftRadius.Min = 0;
        this.sldBottomLeftRadius.Max = this.rectangle.Width < this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height;
        this.sldBottomLeftRadius.ValueChanged += SldBottomLeftRadius_ValueChanged;

        this.layBottomLeftRadius = new Layout();
        this.layBottomLeftRadius.StackDirection = StackDirection.Horizontal;
        this.layBottomLeftRadius.Centered = true;
        this.layBottomLeftRadius.Enabled = false;
        this.layBottomLeftRadius.AddControl(this.lblBottomLeftRadius);
        this.layBottomLeftRadius.AddControl(this.sldBottomLeftRadius);

        // Bottom right radius
        this.lblBottomRightRadius = new Label();
        this.lblBottomRightRadius.Text = "Bottom Right:";

        this.sldBottomRightRadius = new Slider();
        this.sldBottomRightRadius.Min = 0;
        this.sldBottomRightRadius.Max = this.rectangle.Width < this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height;
        this.sldBottomRightRadius.ValueChanged += SldBottomRightRadius_ValueChanged;

        this.layBottomRightRadius = new Layout();
        this.layBottomRightRadius.StackDirection = StackDirection.Horizontal;
        this.layBottomRightRadius.Centered = true;
        this.layBottomRightRadius.Enabled = false;
        this.layBottomRightRadius.AddControl(this.lblBottomRightRadius);
        this.layBottomRightRadius.AddControl(this.sldBottomRightRadius);

        // Top right radius
        this.lblTopRightRadius = new Label();
        this.lblTopRightRadius.Text = "Top Right:";

        this.sldTopRightRadius = new Slider();
        this.sldTopRightRadius.Min = 0;
        this.sldTopRightRadius.Max = this.rectangle.Width < this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height;
        this.sldTopRightRadius.ValueChanged += SldTopRightRadius_ValueChanged;

        this.layTopRightRadius = new Layout();
        this.layTopRightRadius.StackDirection = StackDirection.Horizontal;
        this.layTopRightRadius.Centered = true;
        this.layTopRightRadius.Enabled = false;
        this.layTopRightRadius.AddControl(this.lblTopRightRadius);
        this.layTopRightRadius.AddControl(this.sldTopRightRadius);

        // Top left radius
        this.lblTopLeftRadius = new Label();
        this.lblTopLeftRadius.Text = "Top Left:";

        this.sldTopLeftRadius = new Slider();
        this.sldTopLeftRadius.Min = 0;
        this.sldTopLeftRadius.Max = this.rectangle.Width < this.rectangle.Height ? this.rectangle.Width : this.rectangle.Height;
        this.sldTopLeftRadius.ValueChanged += SldTopLeftRadius_ValueChanged;

        this.layTopLeftRadius = new Layout();
        this.layTopLeftRadius.StackDirection = StackDirection.Horizontal;
        this.layTopLeftRadius.Centered = true;
        this.layTopLeftRadius.Enabled = false;
        this.layTopLeftRadius.AddControl(this.lblTopLeftRadius);
        this.layTopLeftRadius.AddControl(this.sldTopLeftRadius);

        // Create main layout for the other layouts
        this.layMain = new Layout();
        this.layMain.Name = "Main Layout";
        this.layMain.AddControl(this.layShapeType);
        this.layMain.AddControl(this.layCircleProps);
        this.layMain.AddControl(this.layGradType);
        this.layMain.AddControl(this.layGradStartClr);
        this.layMain.AddControl(this.layGradStopClr);
        this.layMain.AddControl(this.layCircleDiameter);
        this.layMain.AddControl(this.layRectWidth);
        this.layMain.AddControl(this.layRectHeight);
        this.layMain.AddControl(this.layBottomLeftRadius);
        this.layMain.AddControl(this.layBottomRightRadius);
        this.layMain.AddControl(this.layTopRightRadius);
        this.layMain.AddControl(this.layTopLeftRadius);

        // Create main container
        this.conMain = new Container();
        this.conMain.Title = "Shape Settings";
        this.conMain.Position = new Vector2(15, 15);
        this.conMain.AddLayoutControl(this.layMain);
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
    }

    /// <summary>
    /// Invoked when the selected shape type changes in the shape type dropdown.
    /// </summary>
    private void DrpShapeType_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        this.shapeType = Enum.Parse<ShapeType>(e.NewValue);

        switch (this.shapeType)
        {
            case ShapeType.Circle:
                this.chkIsSolid.IsChecked = this.circle.IsSolid;
                this.lblInstructions.Text = this.circleInstructionText;
                this.chkIsSolid.IsChecked = this.circle.IsSolid;
                this.drpGradType.SelectItem(this.circle.GradientType.ToString());

                // Find the name of the color based on the actual color
                foreach (var color in this.clrList)
                {
                    if (this.circle.GradientStart == color.Value)
                    {
                        this.drpGradStartClr.SelectItem(color.Key);
                    }

                    if (this.circle.GradientStop == color.Value)
                    {
                        this.drpGradStopClr.SelectItem(color.Key);
                    }
                }

                this.sldBorderThickness.Value = this.circle.BorderThickness;
                this.layCircleDiameter.Enabled = true;
                this.layRectWidth.Enabled = false;
                this.layRectHeight.Enabled = false;
                this.layBottomLeftRadius.Enabled = false;
                this.layBottomRightRadius.Enabled = false;
                this.layTopRightRadius.Enabled = false;
                this.layTopLeftRadius.Enabled = false;

                break;
            case ShapeType.Rectangle:
                this.chkIsSolid.IsChecked = this.rectangle.IsSolid;
                this.lblInstructions.Text = this.rectInstructionText;
                this.sldBorderThickness.Value = this.rectangle.BorderThickness;
                this.drpGradType.SelectItem(this.rectangle.GradientType.ToString());

                // Find the name of the color based on the actual color
                foreach (var color in this.clrList)
                {
                    if (this.rectangle.GradientStart == color.Value)
                    {
                        this.drpGradStartClr.SelectItem(color.Key);
                    }

                    if (this.rectangle.GradientStop == color.Value)
                    {
                        this.drpGradStopClr.SelectItem(color.Key);
                    }
                }

                this.chkIsSolid.IsChecked = this.rectangle.IsSolid;
                this.layCircleDiameter.Enabled = false;
                this.layRectWidth.Enabled = true;
                this.layRectHeight.Enabled = true;
                this.layBottomLeftRadius.Enabled = true;
                this.layBottomRightRadius.Enabled = true;
                this.layTopRightRadius.Enabled = true;
                this.layTopLeftRadius.Enabled = true;

                break;
        }
    }

    /// <summary>
    /// Invoked when the solid shape checkbox check state changes.
    /// </summary>
    private void ChkShapeIsSolid_CheckedChanged(object? sender, CheckChangedEventArgs e)
    {
        if (this.shapeType == ShapeType.Circle)
        {
            this.circle.IsSolid = e.IsChecked;
        }
        else
        {
            this.rectangle.IsSolid = e.IsChecked;
        }

        this.layGradType.Enabled = !e.IsChecked;
        this.layGradStartClr.Enabled = !e.IsChecked;
        this.layGradStopClr.Enabled = !e.IsChecked;

        this.laySolidClr.Enabled = e.IsChecked;
    }

    /// <summary>
    /// Invoked when the selected solid color changes in the color dropdown.
    /// </summary>
    private void DrpSolidClr_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        switch (e.NewValue)
        {
            case "Red":
                this.circle.Color = Color.IndianRed;
                break;
            case "Green":
                this.circle.Color = Color.SeaGreen;
                break;
            case "Blue":
                this.circle.Color = Color.CornflowerBlue;
                break;
        }
    }

    /// <summary>
    /// Invoked when the border thickness slider value changes.
    /// </summary>
    private void SldBorderThickness_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (this.shapeType == ShapeType.Circle)
        {
            this.circle.BorderThickness = e.NewValue;
        }
        else
        {
            this.rectangle.BorderThickness = e.NewValue;
        }
    }

    /// <summary>
    /// Invoked when the selected gradient type changes in the gradient type dropdown.
    /// </summary>
    private void DrpGradType_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        var selectedGradType = Enum.Parse<ColorGradient>(e.NewValue);

        if (this.shapeType == ShapeType.Circle)
        {
            this.circle.GradientType = selectedGradType;
        }
        else
        {
            this.rectangle.GradientType = selectedGradType;
        }
    }

    /// <summary>
    /// Invoked when the selected gradient start color changes in the gradient start color dropdown.
    /// </summary>
    private void DrpGradStartClr_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        if (this.shapeType == ShapeType.Circle)
        {
            this.circle.GradientStart = this.clrList[e.NewValue];
        }
        else
        {
            this.rectangle.GradientStart = this.clrList[e.NewValue];
        }
    }

    /// <summary>
    /// Invoked when the selected gradient stop color changes in the gradient stop color dropdown.
    /// </summary>
    private void DrpGradStopClr_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        if (this.shapeType == ShapeType.Circle)
        {
            this.circle.GradientStop = this.clrList[e.NewValue];
        }
        else
        {
            this.rectangle.GradientStop = this.clrList[e.NewValue];
        }
    }

    /// <summary>
    /// Invoked when the circle diameter slider value changes.
    /// </summary>
    private void SldCircleDiameter_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.circle.Diameter = e.NewValue;
        this.sldBorderThickness.Max = e.NewValue / 2;
    }

    /// <summary>
    /// Invoked when the rectangle width slider value changes.
    /// </summary>
    private void SldRectWidth_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (this.shapeType != ShapeType.Rectangle)
        {
            return;
        }

        this.rectangle.Height = e.NewValue;
        var newMaxValue = (this.rectangle.Height < this.rectangle.Width
            ? this.rectangle.Height
            : this.rectangle.Width) / 2;

        this.sldBorderThickness.Max = newMaxValue;
        this.sldBottomLeftRadius.Max = newMaxValue;
        this.sldTopLeftRadius.Max = newMaxValue;
        this.sldTopRightRadius.Max = newMaxValue;
        this.sldBottomRightRadius.Max = newMaxValue;
    }

    /// <summary>
    /// Invoked when the rectangle height slider value changes.
    /// </summary>
    private void SldRectHeight_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.rectangle.Height = e.NewValue;
        var newMaxValue = (this.rectangle.Width < this.rectangle.Height
            ? this.rectangle.Width
            : this.rectangle.Height) / 2;

        this.sldBorderThickness.Max = newMaxValue;
        this.sldBottomLeftRadius.Max = newMaxValue;
        this.sldTopLeftRadius.Max = newMaxValue;
        this.sldTopRightRadius.Max = newMaxValue;
        this.sldBottomRightRadius.Max = newMaxValue;
    }

    /// <summary>
    /// Invoked when the bottom-left corner radius slider value changes.
    /// </summary>
    private void SldBottomLeftRadius_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.rectangle.CornerRadius = this.rectangle.CornerRadius with
        {
            BottomLeft = e.NewValue
        };
    }

    /// <summary>
    /// Invoked when the bottom-right corner radius slider value changes.
    /// </summary>
    private void SldBottomRightRadius_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.rectangle.CornerRadius = this.rectangle.CornerRadius with
        {
            BottomRight = e.NewValue
        };
    }

    /// <summary>
    /// Invoked when the top-right corner radius slider value changes.
    /// </summary>
    private void SldTopRightRadius_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.rectangle.CornerRadius = this.rectangle.CornerRadius with
        {
            TopRight = e.NewValue
        };
    }

    /// <summary>
    /// Invoked when the top-left corner radius slider value changes.
    /// </summary>
    private void SldTopLeftRadius_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.rectangle.CornerRadius = this.rectangle.CornerRadius with
        {
            TopLeft = e.NewValue
        };
    }
}
