// <copyright file="TextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

/// <summary>
/// Used to test whether text is properly rendered to the screen.
/// </summary>
public class TextRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const string SingleLineText = "Change me using the font properties.";
    private readonly string multiLineText = $"Change me using{Environment.NewLine}the buttons to the left.";
    private IControlGroup? grpControls;
    private IFontRenderer? fontRenderer;
    private IFont? textFont;
    private string text = SingleLineText;
    private BackgroundManager? backgroundManager;
    private ILoader<IFont>? fontLoader;
    private float renderSize = 1;
    private float angle;
    private bool isBlue;
    private bool isFirstRender = true;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.fontLoader = ContentLoaderFactory.CreateFontLoader();
        this.textFont = this.fontLoader.Load(DefaultRegularFont, 12);

        var ctrlFactory = new ControlFactory();

        // Rotate Button
        var sldRotate = ctrlFactory.CreateSlider();
        sldRotate.Name = nameof(sldRotate);
        sldRotate.Text = "Rotate:";
        sldRotate.Value = 0;
        sldRotate.Min = 0;
        sldRotate.Max = 360;
        sldRotate.ValueChanged += (_, newValue) => this.angle = newValue;

        // Increase Render Size Button
        var sldRenderSize = ctrlFactory.CreateSlider();
        sldRenderSize.Name = nameof(sldRenderSize);
        sldRenderSize.Text = "Render Size:";
        sldRenderSize.Value = 1;
        sldRenderSize.Min = 0.1f;
        sldRenderSize.Max = 4;
        sldRenderSize.ValueChanged += (_, newValue) => this.renderSize = newValue;

        // Set Multi-Line
        var chkSetMultiLine = ctrlFactory.CreateCheckbox();
        chkSetMultiLine.Name = nameof(chkSetMultiLine);
        chkSetMultiLine.LabelWhenChecked = "Multi-Line";
        chkSetMultiLine.LabelWhenUnchecked = "Single-Line";
        chkSetMultiLine.CheckedChanged += (_, isChecked) => this.text = isChecked ? this.multiLineText : SingleLineText;

        // Set Color
        var chkSetColor = ctrlFactory.CreateCheckbox();
        chkSetColor.Name = nameof(chkSetColor);
        chkSetColor.LabelWhenChecked = "Color On";
        chkSetColor.LabelWhenUnchecked = "Color Off";
        chkSetColor.CheckedChanged += (_, isChecked) => this.isBlue = isChecked;

        // Font size
        var sldFontSize = ctrlFactory.CreateSlider();
        sldFontSize.Name = nameof(sldFontSize);
        sldFontSize.Text = "Font Size:";
        sldFontSize.Value = 12;
        sldFontSize.Min = 1;
        sldFontSize.Max = 50;
        sldFontSize.ValueChanged += (_, value) =>
        {
            value = value > 100 ? 100 : value;
            value = value < 0 ? 0 : value;

            this.textFont.Size = (uint)value;
        };

        // Set the font style to bold
        var cmbSetStyle = ctrlFactory.CreateComboBox();
        cmbSetStyle.Name = nameof(cmbSetStyle);
        cmbSetStyle.Label = "Style:";
        cmbSetStyle.Width = 150;
        cmbSetStyle.Items =
        [
            FontStyle.Regular.ToString(),
            FontStyle.Italic.ToString(),
            FontStyle.Bold.ToString(),
            $"{FontStyle.Italic.ToString()} & {FontStyle.Bold.ToString()}"
        ];
        cmbSetStyle.SelectedItemIndexChanged += (_, selectedIndex) =>
        {
            this.textFont.Style = (FontStyle)selectedIndex;
        };

        this.grpControls = ctrlFactory.CreateControlGroup();
        this.grpControls.Title = "Font Properties";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowPadding, WindowCenter.Y - this.grpControls.HalfHeight);
        };
        this.grpControls.Add(sldRotate);
        this.grpControls.Add(sldRenderSize);
        this.grpControls.Add(chkSetMultiLine);
        this.grpControls.Add(chkSetColor);
        this.grpControls.Add(sldFontSize);
        this.grpControls.Add(cmbSetStyle);

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager?.Unload();
        this.fontLoader.Unload(this.textFont);
        this.grpControls.Dispose();
        this.grpControls = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var xPos = WindowCenter.X;
        var yPos = WindowCenter.Y;

        if (this.isFirstRender)
        {
            this.grpControls.Position = new Point(WindowPadding, ((int)WindowSize.Height / 2) - this.grpControls.HalfHeight);
            this.isFirstRender = false;
        }

        this.backgroundManager?.Render();
        this.fontRenderer.Render(
            this.textFont,
            this.text,
            xPos,
            yPos,
            this.renderSize,
            this.angle,
            this.isBlue ? Color.CornflowerBlue : Color.White);

        this.grpControls.Render();

        base.Render();
    }
}
