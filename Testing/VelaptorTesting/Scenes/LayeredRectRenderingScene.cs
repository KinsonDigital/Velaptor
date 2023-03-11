// <copyright file="LayeredRectRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using Velaptor.Scene;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

/// <summary>
/// Tests out layered rendering with rectangles.
/// </summary>
public class LayeredRectRenderingScene : SceneBase
{
    private const string DefaultFont = "TimesNewRoman-Regular.ttf";
    private const float Speed = 200f;
    private const int BackgroundLayer = -50;
    private const int RectWidth = 200;
    private const int RectHeight = 200;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState> keyboard;
    private ITexture? background;
    private IFont? font;
    private RectShape orangeRect;
    private RectShape whiteRect;
    private RectShape blueRect;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Vector2 backgroundPos;
    private Vector2 rectStateTextPos;
    private SizeF instructionTextSize;
    private ITextureRenderer? textureRenderer;
    private IRectangleRenderer? rectRenderer;
    private IFontRenderer? fontRenderer;
    private RenderLayer whiteLayer = RenderLayer.One;
    private int instructionsX;
    private int instructionsY;
    private string instructions = string.Empty;
    private string rectStateText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredRectRenderingScene"/> class.
    /// </summary>
    public LayeredRectRenderingScene() => this.keyboard = InputFactory.CreateKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        var renderFactory = new RendererFactory();
        this.textureRenderer = renderFactory.CreateTextureRenderer();
        this.rectRenderer = renderFactory.CreateRectangleRenderer();
        this.fontRenderer = renderFactory.CreateFontRenderer();

        this.background = ContentLoader.LoadTexture("layered-rendering-background");
        this.backgroundPos = new Vector2(WindowCenter.X, WindowCenter.Y);

        this.font = ContentLoader.LoadFont(DefaultFont, 12);
        this.font.Style = FontStyle.Bold;

        var textLines = new[]
        {
            "Use the arrow keys to move the white rectangle.",
            "Use the 'L' key to change the layer where the white rectangle is rendered.",
        };

        this.instructions = string.Join(Environment.NewLine, textLines);

        this.instructionTextSize = this.font.Measure(this.instructions);
        this.instructionsX = (int)(this.instructionTextSize.Width / 2) + 25;
        this.instructionsY = (int)(this.instructionTextSize.Height / 2) + 25;

        this.orangeRect = this.orangeRect with
        {
            Position = new Vector2(WindowCenter.X - 100, WindowCenter.Y),
            Width = RectWidth,
            Height = RectHeight,
            IsFilled = true,
            Color = Color.FromArgb(255, 193, 105, 46),
            CornerRadius = new CornerRadius(15f, 50f, 15f, 50f),
        };

        this.blueRect = this.blueRect with
        {
            Position = new Vector2(
            this.orangeRect.Position.X - this.orangeRect.HalfWidth,
            this.orangeRect.Position.Y + this.orangeRect.HalfHeight),
            Width = RectWidth,
            Height = RectHeight,
            IsFilled = true,
            Color = Color.SteelBlue,
            CornerRadius = new CornerRadius(40f, 10f, 40f, 10f),
        };

        this.whiteRect = this.whiteRect with
        {
            Position = new Vector2(
                this.orangeRect.Position.X - (this.orangeRect.HalfWidth / 2f),
                this.orangeRect.Position.Y + (this.orangeRect.HalfHeight / 2f)),
            Width = RectWidth,
            Height = RectHeight,
            IsFilled = true,
            Color = Color.AntiqueWhite,
            CornerRadius = new CornerRadius(20f, 20f, 20f, 20f),
        };

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        UpdateWhiteRectLayer();
        UpdateRectStateText();
        MoveWhiteRect(frameTime);

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.rectRenderer.Render(this.blueRect, (int)BlueLayer);
        this.rectRenderer.Render(this.orangeRect, (int)OrangeLayer);
        this.rectRenderer.Render(this.whiteRect, (int)this.whiteLayer);

        // Render the checkerboard background
        this.textureRenderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        // Render the instructions
        this.fontRenderer.Render(this.font, this.instructions, this.instructionsX, this.instructionsY, Color.White);

        // Render the rectangle state text
        this.fontRenderer.Render(this.font, this.rectStateText, (int)this.rectStateTextPos.X, (int)this.rectStateTextPos.Y);

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
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
    /// Updates the text for the state of the white rectangle.
    /// </summary>
    private void UpdateRectStateText()
    {
        // Render the current enabled box text
        var textLines = new[]
        {
            $"White Rectangle Layer: {this.whiteLayer}",
            $"Orange Rectangle Layer: {OrangeLayer}",
            $"Blue Rectangle Layer: {BlueLayer}",
        };
        this.rectStateText = string.Join(Environment.NewLine, textLines);

        var boxStateTextSize = this.font.Measure(this.rectStateText);

        this.rectStateTextPos = new Vector2
        {
            X = (int)(boxStateTextSize.Width / 2) + 25,
            Y = this.instructionsY +
                (int)this.instructionTextSize.Height +
                (int)(boxStateTextSize.Height / 2) +
                10,
        };
    }

    /// <summary>
    /// Updates the current layer of the white rectangle.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Occurs if the <see cref="RenderLayer"/> is out of range.
    /// </exception>
    private void UpdateWhiteRectLayer()
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
    /// Moves the white rectangle.
    /// </summary>
    /// <param name="frameTime">The current frame time.</param>
    private void MoveWhiteRect(FrameTime frameTime)
    {
        var amount = Speed * (float)frameTime.ElapsedTime.TotalSeconds;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            this.whiteRect.Position -= new Vector2(amount, 0f);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.whiteRect.Position += new Vector2(amount, 0f);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.whiteRect.Position -= new Vector2(0f, amount);
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.whiteRect.Position += new Vector2(0f, amount);
        }

        // Left edge containment
        if (this.whiteRect.Position.X < this.whiteRect.HalfWidth)
        {
            this.whiteRect.Position = new Vector2(this.whiteRect.HalfWidth, this.whiteRect.Position.Y);
        }

        // Right edge containment
        if (this.whiteRect.Position.X > WindowSize.Width - this.whiteRect.HalfWidth)
        {
            this.whiteRect.Position = new Vector2(WindowSize.Width - this.whiteRect.HalfWidth, this.whiteRect.Position.Y);
        }

        // Top edge containment
        if (this.whiteRect.Position.Y < this.whiteRect.HalfHeight)
        {
            this.whiteRect.Position = new Vector2(this.whiteRect.Position.X, this.whiteRect.HalfHeight);
        }

        // Bottom edge containment
        if (this.whiteRect.Position.Y > WindowSize.Height - this.whiteRect.HalfHeight)
        {
            this.whiteRect.Position = new Vector2(this.whiteRect.Position.X, WindowSize.Height - this.whiteRect.HalfHeight);
        }
    }
}
