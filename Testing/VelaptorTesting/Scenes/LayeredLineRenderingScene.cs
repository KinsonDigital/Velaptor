﻿// <copyright file="LayeredLineRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using Core;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;

/// <summary>
/// Tests layered rendering with lines.
/// </summary>
public class LayeredLineRenderingScene : SceneBase
{
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const float LineMoveSpeed = 200f;
    private const int BackgroundLayer = -50;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState>? keyboard;
    private readonly float windowHalfWidth;
    private readonly float windowHalfHeight;
    private ITexture? background;
    private IFont? font;
    private Line whiteLine;
    private Line orangeLine;
    private Line blueLine;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Vector2 instructionsPos;
    private Vector2 lineStateTextPos;
    private Vector2 backgroundPos;
    private SizeF instructionTextSize;
    private RenderLayer whiteLayer = RenderLayer.One;
    private string instructions = string.Empty;
    private string lineStateText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredLineRenderingScene"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the scene.</param>
    public LayeredLineRenderingScene(IContentLoader contentLoader)
        : base(contentLoader)
    {
        this.keyboard = AppInputFactory.CreateKeyboard();
        this.windowHalfWidth = MainWindow.WindowWidth / 2f;
        this.windowHalfHeight = MainWindow.WindowHeight / 2f;
    }

    /// <inheritdoc cref="IContentLoadable.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.background = ContentLoader.LoadTexture("layered-rendering-background");
        this.backgroundPos = new Vector2(this.windowHalfWidth, this.windowHalfHeight);

        this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);
        this.font.Style = FontStyle.Bold;

        var textLines = new[]
        {
            "Use the arrow keys to move the white line.",
            "Use the 'L' key to change the layer where the white line is rendered.",
        };

        this.instructions = string.Join(Environment.NewLine, textLines);

        this.instructionTextSize = this.font.Measure(this.instructions);
        this.instructionsPos = new Vector2(
            this.windowHalfWidth,
            (this.instructionTextSize.Height / 2) + 25);

        this.orangeLine = default;
        this.orangeLine.Color = Color.FromArgb(255, 193, 105, 46);
        this.orangeLine.Thickness = 20;
        this.orangeLine.P1 = new Vector2(this.windowHalfWidth, this.windowHalfHeight - 100);
        this.orangeLine.P2 = new Vector2(this.windowHalfWidth, this.windowHalfHeight + 100);

        this.blueLine = default;
        this.blueLine.Color = Color.SteelBlue;
        this.blueLine.Thickness = 15;
        this.blueLine.P1 = new Vector2(this.windowHalfWidth - 100, this.windowHalfHeight - 100);
        this.blueLine.P2 = new Vector2(this.windowHalfWidth + 100, this.windowHalfHeight + 100);

        this.whiteLine = default;
        this.whiteLine.Color = Color.AntiqueWhite;
        this.whiteLine.Thickness = 10;
        this.whiteLine.P1 = new Vector2(this.windowHalfWidth - 100, this.windowHalfHeight);
        this.whiteLine.P2 = new Vector2(this.windowHalfWidth + 100, this.windowHalfHeight);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        UpdateWhiteLineLayer();
        UpdateRectStateText();
        MoveWhiteLine(frameTime);

        this.prevKeyState = this.currentKeyState;

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render(IRenderer renderer)
    {
        renderer.Render(this.blueLine, (int)BlueLayer);
        renderer.Render(this.orangeLine, (int)OrangeLayer);
        renderer.Render(this.whiteLine, (int)this.whiteLayer);

        // Render the checkerboard background
        renderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        // Render the instructions
        renderer.Render(this.font, this.instructions, this.instructionsPos, Color.White);

        // Render the rectangle state text
        renderer.Render(this.font, this.lineStateText, (int)this.lineStateTextPos.X, (int)this.lineStateTextPos.Y);

        base.Render(renderer);
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        ContentLoader.UnloadFont(this.font);
        ContentLoader.UnloadTexture(this.background);

        base.UnloadContent();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
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

    /// <summary>
    /// Updates the current layer of the white rectangle.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Occurs if the <see cref="RenderLayer"/> is out of range.
    /// </exception>
    private void UpdateWhiteLineLayer()
    {
        if (this.currentKeyState.IsKeyDown(KeyCode.L) && this.prevKeyState.IsKeyUp(KeyCode.L))
        {
            this.whiteLayer = this.whiteLayer switch
            {
                RenderLayer.One => RenderLayer.Three,
                RenderLayer.Three => RenderLayer.Five,
                RenderLayer.Five => RenderLayer.One,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    /// <summary>
    /// Updates the text for the state of the white rectangle.
    /// </summary>
    private void UpdateRectStateText()
    {
        // Render the current enabled box text
        var textLines = new[]
        {
            $"White Line Layer: {this.whiteLayer}",
            $"Orange Line Layer: {OrangeLayer}",
            $"Blue Line Layer: {BlueLayer}",
        };
        this.lineStateText = string.Join(Environment.NewLine, textLines);

        var boxStateTextSize = this.font.Measure(this.lineStateText);

        this.lineStateTextPos = new Vector2
        {
            X = (int)(boxStateTextSize.Width / 2) + 25,
            Y = this.instructionsPos.Y +
                (int)this.instructionTextSize.Height +
                (int)(boxStateTextSize.Height / 2) +
                10,
        };
    }

    private void MoveWhiteLine(FrameTime frameTime)
    {
        var changeAmount = LineMoveSpeed * (float)frameTime.ElapsedTime.TotalSeconds;

        var velocity = Vector2.Zero;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            velocity.X = changeAmount * -1;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            velocity.X = changeAmount;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            velocity.Y = changeAmount * -1;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            velocity.Y = changeAmount;
        }

        this.whiteLine.P1 += velocity;
        this.whiteLine.P2 += velocity;
    }
}