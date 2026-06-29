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
    private readonly UIContainer sliderContainer;
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
    private Label lblOther1;
    private Button btnOther2;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderingScene"/> class.
    /// </summary>
    public TextRenderingScene()
    {
        this.contentManager = ContentManager.Create();
        this.backgroundManager = new BackgroundManager();

        this.mainContainer = new UIContainer();
        this.mainContainer.Position = new Vector2(50, 50);
        this.mainContainer.Width = 500;
        this.mainContainer.Height = 500;
        // this.mainContainer.Position = new Point(WindowPadding, WindowCenter.Y - this.mainContainer.HalfHeight);
        this.mainContainer.Draggable = true;

        this.lblRotate = new Label();
        this.lblRotate.Text = "Rotate:";

        this.lblOther1 = new Label();
        this.lblOther1.Text = "Other 1:";

        this.btnOther2 = new Button();
        this.btnOther2.Text = "Other 2:";

        this.sldRotate = new Slider();

        this.sliderContainer = new UIContainer();
        this.sliderContainer.Position = new Vector2(WindowCenter.X, WindowCenter.Y);
        this.sliderContainer.AutoSize = true;
        this.sliderContainer.TitleBarVisible = false;
        this.sliderContainer.BorderVisible = false;
        this.sliderContainer.Layout = new LayoutSettings { StackDirection = StackDirection.Vertical };
        this.sliderContainer.Centered = true;
        // this.sliderContainer.AreaPadding = 0;
        // this.sliderContainer.VerticalSpacing = 0;
        // this.sliderContainer.HorizontalSpacing = 0;
        this.sliderContainer.BackgroundColor = Color.FromArgb(255, 30, 30, 30);

        this.sliderContainer.AddControl(this.lblRotate);
        this.sliderContainer.AddControl(this.sldRotate);
        this.sliderContainer.AddControl(this.lblOther1);
        this.sliderContainer.AddControl(this.btnOther2);

        this.mainContainer.AddControl(this.sliderContainer);
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
        // this.sliderContainer.Load();

        // TODO: Remove this
        // var ctrlFactory = new ControlFactory();

        // // Rotate Button
        // var sldRotate = ctrlFactory.CreateSlider();
        // sldRotate.Name = nameof(sldRotate);
        // sldRotate.Text = "Rotate:";
        // sldRotate.Value = 0;
        // sldRotate.Min = 0;
        // sldRotate.Max = 360;
        // sldRotate.ValueChanged += (_, newValue) => this.angle = newValue;

        // // Increase Render Size Button
        // var sldRenderSize = ctrlFactory.CreateSlider();
        // sldRenderSize.Name = nameof(sldRenderSize);
        // sldRenderSize.Text = "Render Size:";
        // sldRenderSize.Value = 1;
        // sldRenderSize.Min = 0.1f;
        // sldRenderSize.Max = 4;
        // sldRenderSize.ValueChanged += (_, newValue) => this.renderSize = newValue;

        // // Set Multi-Line
        // var chkSetMultiLine = ctrlFactory.CreateCheckbox();
        // chkSetMultiLine.Name = nameof(chkSetMultiLine);
        // chkSetMultiLine.LabelWhenChecked = "Multi-Line";
        // chkSetMultiLine.LabelWhenUnchecked = "Single-Line";
        // chkSetMultiLine.CheckedChanged += (_, isChecked) => this.text = isChecked ? this.multiLineText : SingleLineText;

        // // Set Color
        // var chkSetColor = ctrlFactory.CreateCheckbox();
        // chkSetColor.Name = nameof(chkSetColor);
        // chkSetColor.LabelWhenChecked = "Color On";
        // chkSetColor.LabelWhenUnchecked = "Color Off";
        // chkSetColor.CheckedChanged += (_, isChecked) => this.isBlue = isChecked;

        // // Font size
        // var sldFontSize = ctrlFactory.CreateSlider();
        // sldFontSize.Name = nameof(sldFontSize);
        // sldFontSize.Text = "Font Size:";
        // sldFontSize.Value = 12;
        // sldFontSize.Min = 1;
        // sldFontSize.Max = 50;
        // sldFontSize.ValueChanged += (_, value) =>
        // {
        //     value = value > 100 ? 100 : value;
        //     value = value < 0 ? 0 : value;

        //     this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, (uint)value);
        // };

        // // Set the font style to bold
        // var cmbSetStyle = ctrlFactory.CreateComboBox();
        // cmbSetStyle.Name = nameof(cmbSetStyle);
        // cmbSetStyle.Label = "Style:";
        // cmbSetStyle.Width = 150;
        // cmbSetStyle.Items = this.fontFileNames.Select(i => i.DisplayName).ToList();
        // cmbSetStyle.SelectedItemIndexChanged += (_, selectedIndex) =>
        // {
        //     this.currentChosenFontFileName = this.fontFileNames[selectedIndex].FileName;
        //     this.textFont = this.contentManager.LoadFont(this.currentChosenFontFileName, this.textFont.Size);
        // };

        // this.grpControls = ctrlFactory.CreateControlGroup();
        // this.grpControls.Title = "Font Properties";
        // this.grpControls.AutoSizeToFitContent = true;
        // this.grpControls.Initialized += (_, _) =>
        // {
        //     this.grpControls.Position = new Point(WindowPadding, WindowCenter.Y - this.grpControls.HalfHeight);
        // };
        // this.grpControls.Add(sldRotate);
        // this.grpControls.Add(sldRenderSize);
        // this.grpControls.Add(chkSetMultiLine);
        // this.grpControls.Add(chkSetColor);
        // this.grpControls.Add(sldFontSize);
        // this.grpControls.Add(cmbSetStyle);

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

        // TODO: Remove this
        // this.grpControls.Dispose();
        // this.grpControls = null;

        this.mainContainer.Unload();
        // this.sliderContainer.Unload();

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.mainContainer.AreaPadding = 10;
        this.mainContainer.Update();
        // this.lblRotate.Update();
        // this.lblRotate.Position = new Vector2(600, 50); // TODO: Remove me
        // var height = this.lblRotate.Height;
        // this.sliderContainer.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var xPos = WindowCenter.X;
        var yPos = WindowCenter.Y;

        if (this.isFirstRender)
        {
            // TODO: Remove this
            // this.grpControls.Position = new Point(WindowPadding, ((int)WindowSize.Height / 2) - this.grpControls.HalfHeight);
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
        // this.lblRotate.Render();
        // this.sliderContainer.Render();
        // this.grpControls.Render();

        base.Render();
    }
}
