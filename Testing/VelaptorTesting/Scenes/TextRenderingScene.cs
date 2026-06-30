// <copyright file="TextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using UILib;
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
    private readonly Layout layMain;
    private Slider sldRenderSize;
    private Layout layRenderSize;
    private Layout layRotate;
    private IFontRenderer? fontRenderer;
    private IFont? textFont;
    private UIContainer mainContainer;
    private Label lblRotate;
    private Slider sldRotate;
    private string text = SingleLineText;
    private float renderSize = 1;
    private float angle;
    private bool isBlue;
    private bool isFirstRender = true;
    private string currentChosenFontFileName = $"{DefaultFontName}-{nameof(FontStyle.Regular)}.ttf";
    private Label lblRenderSize;
    private CheckBox chkSingleLine;
    private Layout laySingleLine;
    private CheckBox chkColor;
    private Layout layColor;
    private Slider sldFontSize;
    private Label lblFontSize;
    private Layout layFontSize;
    private Label lblStyle;
    private DropDown drpStyle;
    private Layout layFontStyle;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderingScene"/> class.
    /// </summary>
    public TextRenderingScene()
    {
        this.contentManager = ContentManager.Create();
        this.backgroundManager = new BackgroundManager();

        this.mainContainer = new UIContainer();
        this.mainContainer.Position = new Vector2(50, 50);
        this.mainContainer.Position = new Vector2(WindowPadding, WindowCenter.Y - this.mainContainer.HalfHeight);
        this.mainContainer.Draggable = true;

        CreateRotateCtrls();
        CreateRenderSizeCtrls();
        CreateSingleLineCtrls();
        CreateColorCtrls();
        CreateFontSizeCtrls();
        CreateFontStyleCtrls();

        this.layMain = new Layout();
        this.layMain.AddControl(this.layRotate);
        this.layMain.AddControl(this.layRenderSize);
        this.layMain.AddControl(this.laySingleLine);
        this.layMain.AddControl(this.layColor);
        this.layMain.AddControl(this.layFontSize);
        this.layMain.AddControl(this.layFontStyle);

        this.mainContainer.AddLayoutControl(this.layMain);
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
        if (!IsLoaded || IsDisposed)
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
        this.lblRotate = new Label();
        this.lblRotate.Text = "Rotate:";

        this.sldRotate = new Slider();
        this.sldRotate.Max = 360f;
        this.sldRotate.ValueChanged += (_, e) => this.angle = e.NewValue;

        this.layRotate = new Layout();
        this.layRotate.StackDirection = StackDirection.Horizontal;
        this.layRotate.Centered = true;
        this.layRotate.HorizontalSpacing = 5;

        this.layRotate.AddControl(this.lblRotate);
        this.layRotate.AddControl(this.sldRotate);
    }

    private void CreateRenderSizeCtrls()
    {
        this.lblRenderSize = new Label();
        this.lblRenderSize.Text = "Render Size:";

        this.sldRenderSize = new Slider();
        this.sldRenderSize.Value = 1;
        this.sldRenderSize.Min = 0.1f;
        this.sldRenderSize.Max = 4;
        this.sldRenderSize.ValueChanged += (_, e) => this.renderSize = e.NewValue;

        this.layRenderSize = new Layout();
        this.layRenderSize.StackDirection = StackDirection.Horizontal;
        this.layRenderSize.Centered = true;
        this.layRenderSize.HorizontalSpacing = 5;

        this.layRenderSize.AddControl(this.lblRenderSize);
        this.layRenderSize.AddControl(this.sldRenderSize);
    }

    private void CreateSingleLineCtrls()
    {
        this.chkSingleLine = new CheckBox();
        this.chkSingleLine.Text = "Single-Line";
        this.chkSingleLine.IsChecked = true;
        this.chkSingleLine.CheckedChanged += (_, e) =>
        {
            this.text = e.IsChecked ? SingleLineText : this.multiLineText;
            this.chkSingleLine.Text = e.IsChecked ? "Single-Line" : "Multi-Line";
        };

        this.laySingleLine = new Layout();
        this.laySingleLine.StackDirection = StackDirection.Horizontal;
        this.laySingleLine.Centered = true;
        this.laySingleLine.HorizontalSpacing = 5;

        this.laySingleLine.AddControl(this.chkSingleLine);
    }

    private void CreateColorCtrls()
    {
        this.chkColor = new CheckBox();
        this.chkColor.Text = "Color Off";
        this.chkColor.CheckedChanged += (_, e) =>
        {
            this.isBlue = e.IsChecked;
            this.chkColor.Text = e.IsChecked ? "Color On" : "Color Off";
        };

        this.layColor = new Layout();
        this.layColor.StackDirection = StackDirection.Horizontal;
        this.layColor.Centered = true;
        this.layColor.HorizontalSpacing = 5;

        this.layColor.AddControl(this.chkColor);
    }

    private void CreateFontSizeCtrls()
    {
        this.lblFontSize = new Label();
        this.lblFontSize.Text = "Font Size:";

        this.sldFontSize = new Slider();
        this.sldFontSize.Value = 12;
        this.sldFontSize.Min = 1;
        this.sldFontSize.Max = 50;
        this.sldFontSize.ValueChanged += (_, e) =>
        {
            var value = e.NewValue;
            value = value > 100 ? 100 : value;
            value = value < 0 ? 0 : value;

            this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, (uint)value);
        };

        this.layFontSize = new Layout();
        this.layFontSize.StackDirection = StackDirection.Horizontal;
        this.layFontSize.Centered = true;
        this.layFontSize.HorizontalSpacing = 5;

        this.layFontSize.AddControl(this.lblFontSize);
        this.layFontSize.AddControl(this.sldFontSize);
    }

    private void CreateFontStyleCtrls()
    {
        this.lblStyle = new Label();
        this.lblStyle.Text = "Style:";

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

        this.layFontStyle = new Layout();
        this.layFontStyle.StackDirection = StackDirection.Horizontal;
        this.layFontStyle.Centered = true;
        this.layFontStyle.HorizontalSpacing = 5;

        this.layFontStyle.AddControl(this.lblStyle);
        this.layFontStyle.AddControl(this.drpStyle);
    }
}
