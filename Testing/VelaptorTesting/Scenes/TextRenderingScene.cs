// <copyright file="TextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velum;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

/// <summary>
/// Used to test whether text is properly rendered to the screen.
/// </summary>
public class TextRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const string DefaultFontName = "TimesNewRoman";
    private const string SingleLineText = "Change me using the font properties.";
    private readonly string multiLineText = $"Change me using{Environment.NewLine}the buttons to the left.";
    private readonly (string DisplayName, string FileName)[] fontFileNames =
    [
        (nameof(FontStyle.Regular), $"{DefaultFontName}-{nameof(FontStyle.Regular)}.ttf"),
        (nameof(FontStyle.Italic), $"{DefaultFontName}-{nameof(FontStyle.Italic)}.ttf"),
        (nameof(FontStyle.Bold), $"{DefaultFontName}-{nameof(FontStyle.Bold)}.ttf"),
        ($"{nameof(FontStyle.Bold)} & {nameof(FontStyle.Italic)}", $"{DefaultFontName}-{nameof(FontStyle.Bold)}{nameof(FontStyle.Italic)}.ttf"),
    ];
    private readonly IContentManager contentManager;
    private readonly BackgroundManager backgroundManager;
    private readonly Container mainContainer;
    private IFontRenderer? fontRenderer;
    private IFont? textFont;
    private Layout layRenderSize;
    private Layout layRotate;
    private Layout laySingleLine;
    private Layout layColor;
    private Layout layFontSize;
    private Layout layFontStyle;
    private Label? lblRotate;
    private Label? lblRenderSize;
    private Label? lblFontSize;
    private Label? lblStyle;
    private Slider? sldRenderSize;
    private Slider? sldRotate;
    private Slider? sldFontSize;
    private CheckBox? chkSingleLine;
    private CheckBox? chkColor;
    private DropDown? drpStyle;
    private string text = SingleLineText;
    private float renderSize = 1;
    private float angle;
    private bool isBlue;
    private bool isFirstRender = true;
    private string currentChosenFontFileName = $"{DefaultFontName}-{nameof(FontStyle.Regular)}.ttf";

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderingScene"/> class.
    /// </summary>
    public TextRenderingScene()
    {
        this.contentManager = ContentManager.Create();
        this.backgroundManager = new BackgroundManager();

        this.mainContainer = new Container { Position = new Vector2(50, 50) };
        this.mainContainer.Position = new Vector2(WindowPadding, WindowCenter.Y - this.mainContainer.HalfHeight);
        this.mainContainer.Draggable = true;

        CreateRotateCtrls();
        CreateRenderSizeCtrls();
        CreateSingleLineCtrls();
        CreateColorCtrls();
        CreateFontSizeCtrls();
        CreateFontStyleCtrls();

        var layMain = new Layout();
        layMain.AddControl(this.layRotate);
        layMain.AddControl(this.layRenderSize);
        layMain.AddControl(this.laySingleLine);
        layMain.AddControl(this.layColor);
        layMain.AddControl(this.layFontSize);
        layMain.AddControl(this.layFontStyle);

        this.mainContainer.AddLayoutControl(layMain);
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, 12);
        this.mainContainer.Load();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.backgroundManager.Unload();
        this.contentManager.Unload(this.textFont);
        this.mainContainer.Unload();

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.mainContainer.AreaPadding = 10;
        this.mainContainer.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var xPos = WindowCenter.X;
        var yPos = WindowCenter.Y;

        if (this.isFirstRender)
        {
            this.isFirstRender = false;
        }

        this.backgroundManager.Render();
        this.fontRenderer.Render(
            this.textFont,
            this.text,
            xPos,
            yPos,
            this.renderSize,
            this.angle,
            this.isBlue ? Color.CornflowerBlue : Color.White);

        this.mainContainer.Render();

        base.Render();
    }

    private void CreateRotateCtrls()
    {
        this.lblRotate = new Label { Text = "Rotate:" };

        this.sldRotate = new Slider { Value = 0, Max = 360f };
        this.sldRotate.ValueChanged += (_, e) => this.angle = e.NewValue;

        this.layRotate = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.layRotate.AddControl(this.lblRotate);
        this.layRotate.AddControl(this.sldRotate);
    }

    private void CreateRenderSizeCtrls()
    {
        this.lblRenderSize = new Label { Text = "Render Size:" };

        this.sldRenderSize = new Slider { Value = 1, Min = 0.1f, Max = 4 };
        this.sldRenderSize.ValueChanged += (_, e) => this.renderSize = e.NewValue;

        this.layRenderSize = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.layRenderSize.AddControl(this.lblRenderSize);
        this.layRenderSize.AddControl(this.sldRenderSize);
    }

    private void CreateSingleLineCtrls()
    {
        this.chkSingleLine = new CheckBox { Text = "Single-Line", IsChecked = true };
        this.chkSingleLine.CheckedChanged += (_, e) =>
        {
            this.text = e.IsChecked ? SingleLineText : this.multiLineText;
            this.chkSingleLine.Text = e.IsChecked ? "Single-Line" : "Multi-Line";
        };

        this.laySingleLine = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.laySingleLine.AddControl(this.chkSingleLine);
    }

    private void CreateColorCtrls()
    {
        this.chkColor = new CheckBox { Text = "Color Off" };
        this.chkColor.CheckedChanged += (_, e) =>
        {
            this.isBlue = e.IsChecked;
            this.chkColor.Text = e.IsChecked ? "Color On" : "Color Off";
        };

        this.layColor = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.layColor.AddControl(this.chkColor);
    }

    private void CreateFontSizeCtrls()
    {
        this.lblFontSize = new Label { Text = "Font Size:" };

        this.sldFontSize = new Slider { Value = 12, Min = 1, Max = 50 };
        this.sldFontSize.ValueChanged += (_, e) =>
        {
            var value = e.NewValue;
            value = value > 100 ? 100 : value;
            value = value < 0 ? 0 : value;

            this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, (uint)value);
        };

        this.layFontSize = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.layFontSize.AddControl(this.lblFontSize);
        this.layFontSize.AddControl(this.sldFontSize);
    }

    private void CreateFontStyleCtrls()
    {
        this.lblStyle = new Label { Text = "Style:" };

        this.drpStyle = new DropDown();
        var items = this.fontFileNames.Select(i => i.DisplayName);

        foreach (var item in items)
        {
            this.drpStyle.AddItem(item);
        }

        this.drpStyle.SelectedItemChanged += (_, e) =>
        {
            this.currentChosenFontFileName = this.fontFileNames.First(f => f.DisplayName == e.NewValue).FileName;
            this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, this.textFont.Size);
        };

        this.layFontStyle = new Layout { StackDirection = StackDirection.Horizontal, Centered = true, HorizontalSpacing = 5 };

        this.layFontStyle.AddControl(this.lblStyle);
        this.layFontStyle.AddControl(this.drpStyle);
    }
}
