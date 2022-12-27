// <copyright file="LayeredTextureRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;
using Core;

/// <summary>
/// Tests out layered rendering with textures.
/// </summary>
public class LayeredTextureRenderingScene : SceneBase
{
    private const string DefaultFont = "TimesNewRoman-Regular.ttf";
    private const float Speed = 200f;
    private const int BackgroundLayer = -50;
    private const RenderLayer OrangeLayer = RenderLayer.Two;
    private const RenderLayer BlueLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState>? keyboard;
    private readonly int windowHalfWidth;
    private readonly int windowHalfHeight;
    private ITexture? background;
    private IFont? font;
    private IAtlasData? atlas;
    private AtlasSubTextureData whiteBoxData;
    private Vector2 whiteBoxPos;
    private Vector2 orangeBoxPos;
    private Vector2 blueBoxPos;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private AtlasSubTextureData orangeBoxData;
    private AtlasSubTextureData blueBoxData;
    private Vector2 boxStateTextPos;
    private Vector2 backgroundPos;
    private SizeF instructionTextSize;
    private RenderLayer whiteLayer = RenderLayer.One;
    private int instructionsX;
    private int instructionsY;
    private string instructions = string.Empty;
    private string boxStateText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredTextureRenderingScene"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the scene.</param>
    public LayeredTextureRenderingScene(IContentLoader contentLoader)
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

        var textLines = new[]
        {
            "Use the arrow keys to move the white box.",
            "Use the 'L' key to change the layer that the white box is rendered on.",
        };

        this.instructions = string.Join(Environment.NewLine, textLines);

        this.instructionTextSize = this.font.Measure(this.instructions);
        this.instructionsX = (int)(this.instructionTextSize.Width / 2) + 25;
        this.instructionsY = (int)(this.instructionTextSize.Height / 2) + 25;

        this.atlas = ContentLoader.LoadAtlas("layered-rendering-atlas");

        this.whiteBoxData = this.atlas.GetFrames("white-box")[0];
        this.orangeBoxData = this.atlas.GetFrames("orange-box")[0];
        this.blueBoxData = this.atlas.GetFrames("blue-box")[0];

        // Set the default white box position
        this.orangeBoxPos.X = this.windowHalfWidth - 100;
        this.orangeBoxPos.Y = this.windowHalfHeight;

        // Set the default blue box position
        this.blueBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 2f);
        this.blueBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 2f);

        // Set the default orange box position
        this.whiteBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 4f);
        this.whiteBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 4f);

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
            this.atlas.Texture,
            this.blueBoxData.Bounds,
            new Rectangle((int)this.blueBoxPos.X, (int)this.blueBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)BlueLayer);

        // ORANGE
        renderer.Render(
            this.atlas.Texture,
            this.orangeBoxData.Bounds,
            new Rectangle((int)this.orangeBoxPos.X, (int)this.orangeBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)OrangeLayer); // Neutral layer

        // WHITE
        renderer.Render(
            this.atlas.Texture,
            this.whiteBoxData.Bounds,
            new Rectangle((int)this.whiteBoxPos.X, (int)this.whiteBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)this.whiteLayer);

        // Render the checkerboard background
        renderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        // Render the instructions
        renderer.Render(this.font, this.instructions, this.instructionsX, this.instructionsY, Color.White);

        // Render the box state text
        renderer.Render(this.font, this.boxStateText, (int)this.boxStateTextPos.X, (int)this.boxStateTextPos.Y);

        base.Render(renderer);
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        ContentLoader.UnloadTexture(this.background);
        ContentLoader.UnloadAtlas(this.atlas);
        ContentLoader.UnloadFont(this.font);

        this.atlas = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
    protected override void Dispose(bool disposing)
    {
        if (!IsLoaded || IsDisposed)
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
        this.boxStateText = string.Join(Environment.NewLine, textLines);

        var boxStateTextSize = this.font.Measure(this.boxStateText);

        this.boxStateTextPos = new Vector2
        {
            X = (int)(boxStateTextSize.Width / 2) + 25,
            Y = this.instructionsY +
                (int)this.instructionTextSize.Height +
                (int)(boxStateTextSize.Height / 2) +
                100,
        };
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
        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            this.whiteBoxPos.X -= Speed * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.whiteBoxPos.X += Speed * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.whiteBoxPos.Y -= Speed * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.whiteBoxPos.Y += Speed * (float)frameTime.ElapsedTime.TotalSeconds;
        }

        var halfWidth = this.whiteBoxData.Bounds.Width / 2f;
        var halfHeight = this.whiteBoxData.Bounds.Height / 2f;

        // Left edge containment
        if (this.whiteBoxPos.X < halfWidth)
        {
            this.whiteBoxPos.X = halfWidth;
        }

        // Right edge containment
        if (this.whiteBoxPos.X > MainWindow.WindowWidth - halfWidth)
        {
            this.whiteBoxPos.X = MainWindow.WindowWidth - halfWidth;
        }

        // Top edge containment
        if (this.whiteBoxPos.Y < halfHeight)
        {
            this.whiteBoxPos.Y = halfHeight;
        }

        // Bottom edge containment
        if (this.whiteBoxPos.Y > MainWindow.WindowHeight - halfHeight)
        {
            this.whiteBoxPos.Y = MainWindow.WindowHeight - halfHeight;
        }
    }
}
