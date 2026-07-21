// <copyright file="LayeredRectRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using UILib;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests out layered rendering with rectangles.
/// </summary>
public class LayeredRectRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const float Speed = 200f;
    private const int RectWidth = 200;
    private const int RectHeight = 200;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly BackgroundManager backgroundManager;
    private RectShape orangeRect;
    private RectShape whiteRect;
    private RectShape blueRect;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private IShapeRenderer? shapeRenderer;
    private Label? lblInstructions;
    private Label? lblRectState;
    private RenderLayer whiteLayer = RenderLayer.One;
    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredRectRenderingScene"/> class.
    /// </summary>
    public LayeredRectRenderingScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.backgroundManager = new BackgroundManager();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();

        var textLines = new[]
        {
            "Use the arrow keys to move the white rectangle.",
            "Use the 'L' key to change the layer where the white rectangle is rendered.",
        };

        var instructions = string.Join(Environment.NewLine, textLines);

        this.lblInstructions = new Label { Text = instructions };
        this.lblRectState = new Label();

        this.lblInstructions.Load();
        this.lblRectState.Load();

        this.orangeRect = this.orangeRect with
        {
            Position = new Vector2(WindowCenter.X - 100, WindowCenter.Y),
            Width = RectWidth,
            Height = RectHeight,
            IsSolid = true,
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
            IsSolid = true,
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
            IsSolid = true,
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

        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);
        this.lblRectState.Position = new Vector2(WindowPadding, WindowCenter.Y - this.lblRectState.HalfHeight);

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.shapeRenderer.Render(this.blueRect, (int)BlueLayer);
        this.shapeRenderer.Render(this.orangeRect, (int)OrangeLayer);
        this.shapeRenderer.Render(this.whiteRect, (int)this.whiteLayer);

        this.backgroundManager.Render();

        this.lblInstructions.Render();
        this.lblRectState.Render();

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        this.backgroundManager.Unload();
        this.lblInstructions.Unload();
        this.lblRectState.Unload();

        base.UnloadContent();
    }

    /// <summary>
    /// Updates the text for the state of the white rectangle.
    /// </summary>
    private void UpdateRectStateText()
    {
        var textLines = new[]
        {
            $"White Rectangle Layer: {this.whiteLayer}",
            $"Orange Rectangle Layer: {OrangeLayer}",
            $"Blue Rectangle Layer: {BlueLayer}",
        };

        this.lblRectState.Text = string.Join(Environment.NewLine, textLines);
    }

    /// <summary>
    /// Updates the current layer of the white rectangle.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException">
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
                _ => throw new InvalidEnumArgumentException($"this.{nameof(this.whiteLayer)}", (int)this.whiteLayer, typeof(RenderLayer))
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
