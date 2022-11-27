﻿// <copyright file="LayeredTextRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;
using VelaptorTesting.Core;

namespace VelaptorTesting.Scenes;

/// <summary>
/// Tests out layered rendering with text.
/// </summary>
public class LayeredTextRenderingScene : SceneBase
{
    private const string DefaultFont = "TimesNewRoman-Regular.ttf";
    private const float Speed = 100f;
    private const int BackgroundLayer = -50;
    private const RenderLayer OrangeLayer = RenderLayer.Two;
    private const RenderLayer BlueLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState>? keyboard;
    private readonly int windowHalfWidth;
    private readonly int windowHalfHeight;
    private ITexture? background;
    private IFont? font;
    private Vector2 whiteTextPos;
    private Vector2 orangeTextPos;
    private Vector2 blueTextPos;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Vector2 backgroundPos;
    private SizeF whiteTextSize;
    private RenderLayer whiteLayer = RenderLayer.One;
    private string whiteText = string.Empty;
    private string orangeText = string.Empty;
    private string blueText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredTextRenderingScene"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the scene.</param>
    public LayeredTextRenderingScene(IContentLoader contentLoader)
        : base(contentLoader)
    {
        this.keyboard = AppInputFactory.CreateKeyboard();
        this.windowHalfWidth = (int)MainWindow.WindowWidth / 2;
        this.windowHalfHeight = (int)MainWindow.WindowHeight / 2;
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.background = ContentLoader.LoadTexture("layered-rendering-background");
        this.backgroundPos = new Vector2(this.windowHalfWidth, this.windowHalfHeight);

        this.font = ContentLoader.LoadFont(DefaultFont, 12);
        this.font.Style = FontStyle.Bold;
        this.font.Size = 24;

        var whiteLines = new[]
        {
            "Use the arrow keys to move the white text.",
            "Use the 'L' key to change the layer that the",
            "white text is rendered on.",
        };

        this.whiteText = string.Join(Environment.NewLine, whiteLines);

        this.whiteTextSize = this.font.Measure(this.whiteText);
        this.whiteTextPos.X = this.windowHalfWidth;
        this.whiteTextPos.Y = this.windowHalfHeight - (MainWindow.WindowHeight / 6f);

        var orangeLines = new[]
        {
            $"White Box Layer: {this.whiteLayer}",
            $"Orange Box Layer: {OrangeLayer}",
            $"Blue Box Layer: {BlueLayer}",
        };

        this.orangeText = string.Join(Environment.NewLine, orangeLines);

        // Set the default white box position
        this.orangeTextPos.X = this.windowHalfWidth;
        this.orangeTextPos.Y = this.windowHalfHeight;

        var blueLines = new[]
        {
            "This is the first line of the blue text.",
            "This is the second line of the blue text.",
        };
        this.blueText = string.Join(Environment.NewLine, blueLines);

        // Set the default blue box position
        this.blueTextPos.X = this.windowHalfWidth;
        this.blueTextPos.Y = this.windowHalfHeight + (MainWindow.WindowHeight / 6f);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        UpdateWhiteBoxLayer();
        UpdateBoxStateText();

        MoveWhiteBox(frameTime);

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render(IRenderer renderer)
    {
        // BLUE
        renderer.Render(
            this.font,
            this.blueText,
            (int)this.blueTextPos.X,
            (int)this.blueTextPos.Y,
            Color.SteelBlue,
            (int)BlueLayer);

        // ORANGE
        renderer.Render(
            this.font,
            this.orangeText,
            (int)this.orangeTextPos.X,
            (int)this.orangeTextPos.Y,
            Color.FromArgb(255, 193, 105, 46),
            (int)OrangeLayer);

        // WHITE
        renderer.Render(
            this.font,
            this.whiteText,
            (int)this.whiteTextPos.X,
            (int)this.whiteTextPos.Y,
            Color.AntiqueWhite,
            (int)this.whiteLayer);

        // Render the checkerboard background
        renderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        base.Render(renderer);
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (IsLoaded || IsDisposed)
        {
            return;
        }

        ContentLoader.UnloadTexture(this.background);
        ContentLoader.UnloadFont(this.font);

        base.UnloadContent();
    }

    /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed || !IsLoaded)
        {
            return;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Updates the text for the state of the white box.
    /// </summary>
    private void UpdateBoxStateText()
    {
        // Render the current enabled box text
        var textLines = new[]
        {
            $"White Box Layer: {this.whiteLayer}",
            $"Orange Box Layer: {OrangeLayer}",
            $"Blue Box Layer: {BlueLayer}",
        };
        this.orangeText = string.Join(Environment.NewLine, textLines);
    }

    /// <summary>
    /// Updates the current layer of the white box.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Occurs if the <see cref="RenderLayer"/> is out of range.
    /// </exception>
    private void UpdateWhiteBoxLayer()
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
    /// Moves the white box.
    /// </summary>
    /// <param name="frameTime">The current frame time.</param>
    private void MoveWhiteBox(FrameTime frameTime)
    {
        var amount = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            this.whiteTextPos.X -= amount;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.whiteTextPos.X += amount;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.whiteTextPos.Y -= amount;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.whiteTextPos.Y += amount;
        }

        var halfWidth = this.whiteTextSize.Width / 2f;
        var halfHeight = this.whiteTextSize.Height / 2f;

        // Left edge containment
        if (this.whiteTextPos.X < halfWidth)
        {
            this.whiteTextPos.X = halfWidth;
        }

        // Right edge containment
        if (this.whiteTextPos.X > MainWindow.WindowWidth - halfWidth)
        {
            this.whiteTextPos.X = MainWindow.WindowWidth - halfWidth;
        }

        // Top edge containment
        if (this.whiteTextPos.Y < halfHeight)
        {
            this.whiteTextPos.Y = halfHeight;
        }

        // Bottom edge containment
        if (this.whiteTextPos.Y > MainWindow.WindowHeight - halfHeight)
        {
            this.whiteTextPos.Y = MainWindow.WindowHeight - halfHeight;
        }
    }
}