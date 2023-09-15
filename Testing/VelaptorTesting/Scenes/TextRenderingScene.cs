// <copyright file="TextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;
using Velaptor.UI;

/// <summary>
/// Used to test whether or not text is properly rendered to the screen.
/// </summary>
public class TextRenderingScene : SceneBase
{
    private const int LeftMargin = 50;
    private const int VertButtonSpacing = 10;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const float AngularVelocity = 10f;
    private const float SizeChangeAmount = 0.5f;
    private const string SingleLineText = "Change me using the buttons to the left.";
    private readonly string multiLineText = $"Change me using{Environment.NewLine}the buttons to the left.";
    private IFontRenderer? fontRenderer;
    private IFont? textFont;
    private Button? btnRotateCW;
    private Button? btnRotateCCW;
    private Button? btnIncreaseRenderSize;
    private Button? btnDecreaseRenderSize;
    private Button? btnSetMultiLine;
    private Button? btnSetColor;
    private Button? btnSetStyle;
    private Button? btnIncreaseFontSize;
    private Button? btnDecreaseFontSize;
    private BackgroundManager? backgroundManager;
    private bool cwButtonDown;
    private bool ccwButtonDown;
    private bool increaseRenderSizeBtnDown;
    private bool decreaseRenderBtnDown;
    private bool isMultiLine = true;
    private bool isClrSet;
    private float angle;
    private float renderSize = 1f;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var renderFactory = new RendererFactory();

        this.fontRenderer = renderFactory.CreateFontRenderer();

        this.textFont = ContentLoader.LoadFont(DefaultRegularFont, 12);

        // Rotate CW Button
        this.btnRotateCW = new Button
        {
            Text = "CW",
            Name = nameof(this.btnRotateCW),
        };
        this.btnRotateCW.MouseDown += (_, _) =>
        {
            this.cwButtonDown = true;
            this.ccwButtonDown = false;
        };

        this.btnRotateCW.MouseUp += (_, _) =>
        {
            this.cwButtonDown = false;
            this.ccwButtonDown = false;
        };

        // Rotate CCW Button
        this.btnRotateCCW = new Button
        {
            Text = "CCW",
            Name = nameof(this.btnRotateCCW),
        };
        this.btnRotateCCW.MouseDown += (_, _) =>
        {
            this.ccwButtonDown = true;
            this.cwButtonDown = false;
        };
        this.btnRotateCCW.MouseUp += (_, _) =>
        {
            this.ccwButtonDown = false;
            this.cwButtonDown = false;
        };

        // Increase Render Size Button
        this.btnIncreaseRenderSize = new Button
        {
            Text = $"Render Size({Math.Round(this.renderSize, 2)}) +",
            Name = nameof(this.btnIncreaseRenderSize),
            AutoSize = false,
            Width = 250,
            Height = 42,
        };

        this.btnIncreaseRenderSize.MouseDown += (_, _) =>
        {
            this.increaseRenderSizeBtnDown = true;
            this.decreaseRenderBtnDown = false;
        };

        this.btnIncreaseRenderSize.MouseUp += (_, _) =>
        {
            this.increaseRenderSizeBtnDown = false;
            this.decreaseRenderBtnDown = false;
            this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
            this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
        };

        // Decrease Render Size Button
        this.btnDecreaseRenderSize = new Button
        {
            Text = $"Render Size({Math.Round(this.renderSize, 2)}) -",
            Name = nameof(this.btnDecreaseRenderSize),
            AutoSize = false,
            Width = 250,
            Height = 42,
        };

        this.btnDecreaseRenderSize.MouseDown += (_, _) =>
        {
            this.decreaseRenderBtnDown = true;
            this.increaseRenderSizeBtnDown = false;
        };

        this.btnDecreaseRenderSize.MouseUp += (_, _) =>
        {
            this.increaseRenderSizeBtnDown = false;
            this.decreaseRenderBtnDown = false;
            this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
            this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
        };

        // Set Multi-Line
        this.btnSetMultiLine = new Button
        {
            Text = $"Multi-Line: {this.isMultiLine}",
            Name = nameof(this.btnSetMultiLine),
        };

        this.btnSetMultiLine.MouseUp += (_, _) =>
        {
            this.isMultiLine = !this.isMultiLine;

            this.btnSetMultiLine.Text = $"Multi-Line: {this.isMultiLine}";
        };

        // Set Color
        this.btnSetColor = new Button
        {
            Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}",
            Name = nameof(this.btnSetColor),
        };

        this.btnSetColor.MouseUp += (_, _) =>
        {
            this.isClrSet = !this.isClrSet;
            this.btnSetColor.Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}";
        };

        // Set the font style to bold
        this.btnSetStyle = new Button
        {
            Text = $"Style: {this.textFont.Style}",
            Name = nameof(this.btnSetStyle),
        };

        this.btnSetStyle.MouseUp += (_, _) =>
        {
            const string argName = $"{nameof(TextRenderingScene)}.{nameof(this.textFont)}.{nameof(this.textFont.Style)}";

            this.textFont.Style = this.textFont.Style switch
            {
                FontStyle.Regular => FontStyle.Bold,
                FontStyle.Bold => FontStyle.Italic,
                FontStyle.Italic => FontStyle.Bold | FontStyle.Italic,
                FontStyle.Bold | FontStyle.Italic => FontStyle.Regular,
                _ => throw new InvalidEnumArgumentException(argName, (int)this.textFont.Style, typeof(FontStyle)),
            };

            this.btnSetStyle.Text = $"Style: {this.textFont.Style}";
        };

        // Increase font size
        this.btnIncreaseFontSize = new Button
        {
            Text = $"Font Size({this.textFont.Size}) +",
            Name = nameof(this.btnIncreaseFontSize),
        };
        this.btnIncreaseFontSize.MouseUp += (_, _) =>
        {
            this.textFont.Size += 1;
            this.btnIncreaseFontSize.Text = $"Font Size({this.textFont.Size}) +";
            this.btnDecreaseFontSize.Text = $"Font Size({this.textFont.Size}) -";
        };

        // Decrease font size
        this.btnDecreaseFontSize = new Button
        {
            Text = $"Font Size({this.textFont.Size}) -",
            Name = nameof(this.btnDecreaseFontSize),
        };
        this.btnDecreaseFontSize.MouseUp += (_, _) =>
        {
            this.textFont.Size -= this.textFont.Size == 0u ? 0u : 1u;
            this.btnIncreaseFontSize.Text = $"Font Size({this.textFont.Size}) +";
            this.btnDecreaseFontSize.Text = $"Font Size({this.textFont.Size}) -";
        };

        AddControl(this.btnRotateCW);
        AddControl(this.btnRotateCCW);
        AddControl(this.btnIncreaseRenderSize);
        AddControl(this.btnDecreaseRenderSize);
        AddControl(this.btnSetMultiLine);
        AddControl(this.btnSetColor);
        AddControl(this.btnSetStyle);
        AddControl(this.btnIncreaseFontSize);
        AddControl(this.btnDecreaseFontSize);

        base.LoadContent();

        LayoutButtonsLeftSide();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager?.Unload();
        ContentLoader.UnloadFont(this.textFont);

        this.btnRotateCW = null;
        this.btnRotateCCW = null;
        this.btnIncreaseRenderSize = null;
        this.btnDecreaseRenderSize = null;
        this.btnSetMultiLine = null;
        this.btnSetColor = null;
        this.btnSetStyle = null;
        this.btnIncreaseFontSize = null;
        this.btnDecreaseFontSize = null;
        this.cwButtonDown = false;
        this.ccwButtonDown = false;
        this.increaseRenderSizeBtnDown = false;
        this.decreaseRenderBtnDown = false;
        this.isMultiLine = true;
        this.isClrSet = false;
        this.angle = 0f;
        this.renderSize = 1f;

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        // Rotate CW
        if (this.cwButtonDown && !this.ccwButtonDown)
        {
            this.angle += AngularVelocity * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        // Rotate CCW
        if (this.ccwButtonDown && !this.cwButtonDown)
        {
            this.angle -= AngularVelocity * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        // Increase Render Size
        if (this.increaseRenderSizeBtnDown && !this.decreaseRenderBtnDown)
        {
            this.renderSize += SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
            this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
        }

        // Decrease Render Size
        if (this.decreaseRenderBtnDown && !this.increaseRenderSizeBtnDown)
        {
            this.renderSize -= SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
            this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
        }

        this.renderSize = this.renderSize < 0f ? 0f : this.renderSize;

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var xPos = WindowCenter.X;
        var yPos = WindowCenter.Y;

        this.backgroundManager?.Render();
        this.fontRenderer.Render(
            this.textFont,
            this.isMultiLine ? this.multiLineText : SingleLineText,
            xPos,
            yPos,
            this.renderSize,
            this.angle,
            this.isClrSet ? Color.CornflowerBlue : Color.White);

        base.Render();
    }

    /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed || !IsLoaded)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
        }

        base.Dispose(disposing);
    }

    private void LayoutButtonsLeftSide()
    {
        var excludeList = new[]
        {
            nameof(this.btnRotateCW),
            nameof(this.btnRotateCCW),
            nameof(this.btnIncreaseRenderSize),
            nameof(this.btnDecreaseRenderSize),
            nameof(this.btnSetMultiLine),
            nameof(this.btnSetColor),
            nameof(this.btnSetStyle),
            nameof(this.btnIncreaseFontSize),
            nameof(this.btnDecreaseFontSize),
        };

        var buttons = Controls.Where(c => excludeList.Contains(c.Name)).ToImmutableArray();

        if (buttons.Length <= 0)
        {
            return;
        }

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

        // Center all of the buttons horizontally relative to each other
        var largestBtnWidth = buttons.Max(b => b.Width);
        var desiredPosition = (from b in buttons
            where b.Width == largestBtnWidth
            select b.Position).FirstOrDefault();

        foreach (var button in buttons)
        {
            button.Position = new Point(desiredPosition.X, button.Position.Y);
        }
    }
}
