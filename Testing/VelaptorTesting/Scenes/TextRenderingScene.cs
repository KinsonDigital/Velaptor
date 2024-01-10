// <copyright file="TextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

/// <summary>
/// Used to test whether or not text is properly rendered to the screen.
/// </summary>
public class TextRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const string SingleLineText = "Change me using the font properties.";
    private readonly string multiLineText = $"Change me using{Environment.NewLine}the buttons to the left.";
    private readonly IControlGroup ctrlGroup;
    private IFontRenderer? fontRenderer;
    private IFont? textFont;
    private string text = SingleLineText;
    private BackgroundManager? backgroundManager;
    private ILoader<IFont>? fontLoader;
    private float renderSize = 1;
    private float angle;
    private bool isBlue;
    private bool isFirstRender = true;

    public TextRenderingScene()
    {
        // Rotate Button
        var udcRotate = TestingApp.Container.GetInstance<IUpDown>();
        udcRotate.Name = nameof(udcRotate);
        udcRotate.Text = "Rotate:";
        udcRotate.Increment = 2f;
        udcRotate.Decrement = 2f;
        udcRotate.Min = 0;
        udcRotate.Max = 360;
        udcRotate.ValueChanged += (_, newValue) => this.angle = newValue;

        // Increase Render Size Button
        var udcRenderSize = TestingApp.Container.GetInstance<IUpDown>();
        udcRenderSize.Name = nameof(udcRenderSize);
        udcRenderSize.Text = "Render Size:";
        udcRenderSize.Increment = 0.05f;
        udcRenderSize.Decrement = 0.05f;
        udcRenderSize.Min = 1;
        udcRenderSize.Max = 4;
        udcRenderSize.ValueChanged += (_, newValue) => this.renderSize = newValue;

        // Set Multi-Line
        var chkSetMultiLine = TestingApp.Container.GetInstance<ICheckBox>();
        chkSetMultiLine.Name = nameof(chkSetMultiLine);
        chkSetMultiLine.LabelWhenChecked = "Multi-Line";
        chkSetMultiLine.LabelWhenUnchecked = "Single-Line";
        chkSetMultiLine.CheckedChanged += (_, isChecked) => this.text = isChecked ? this.multiLineText : SingleLineText;

        // Set Color
        var chkSetColor = TestingApp.Container.GetInstance<ICheckBox>();
        chkSetColor.Name = nameof(chkSetColor);
        chkSetColor.LabelWhenChecked = "Color On";
        chkSetColor.LabelWhenUnchecked = "Color Off";
        chkSetColor.CheckedChanged += (_, isChecked) => this.isBlue = isChecked;

        // Font size
        var udcFontSize = TestingApp.Container.GetInstance<IUpDown>();
        udcFontSize.Name = nameof(udcFontSize);
        udcFontSize.Text = "Font Size:";
        udcFontSize.Value = 12;
        udcFontSize.ValueChanged += (_, value) =>
        {
            value = value > 100 ? 100 : value;
            value = value < 0 ? 0 : value;

            this.textFont.Size = (uint)value;
        };

        // Set the font style to bold
        var cmbSetStyle = TestingApp.Container.GetInstance<IComboBox>();
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

        this.ctrlGroup = TestingApp.Container.GetInstance<IControlGroup>();
        this.ctrlGroup.Title = "Font Properties";
        this.ctrlGroup.AutoSizeToFitContent = true;
        this.ctrlGroup.Add(udcRotate);
        this.ctrlGroup.Add(udcRenderSize);
        this.ctrlGroup.Add(chkSetMultiLine);
        this.ctrlGroup.Add(chkSetColor);
        this.ctrlGroup.Add(udcFontSize);
        this.ctrlGroup.Add(cmbSetStyle);
    }

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

        this.ctrlGroup.Dispose();
        base.UnloadContent();
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var xPos = WindowCenter.X;
        var yPos = WindowCenter.Y;

        this.ctrlGroup.Render();

        if (this.isFirstRender)
        {
            this.ctrlGroup.Position = new Point(WindowPadding, ((int)WindowSize.Height / 2) - this.ctrlGroup.HalfHeight);
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

        base.Render();
    }
}
