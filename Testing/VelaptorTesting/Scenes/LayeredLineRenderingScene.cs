// <copyright file="LayeredLineRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Velum;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests layered rendering with lines.
/// </summary>
public class LayeredLineRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const float LineMoveSpeed = 200f;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState>? keyboard;
    private readonly BackgroundManager backgroundManager;
    private readonly IShapeRenderer shapeRenderer;
    private Line whiteLine;
    private Line orangeLine;
    private Line blueLine;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Label? lblInstructions;
    private Label? lblLineState;
    private RenderLayer whiteLayer = RenderLayer.One;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredLineRenderingScene"/> class.
    /// </summary>
    public LayeredLineRenderingScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.backgroundManager = new BackgroundManager();
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
    }

    /// <inheritdoc cref="IContentLoadable.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var textLines = new[]
        {
            "Use the arrow keys to move the white line.",
            "Use the 'L' key to change the layer where the white line is rendered.",
        };

        var instructions = string.Join(Environment.NewLine, textLines);

        this.lblInstructions = new Label { Text = instructions };
        this.lblLineState = new Label();

        this.lblInstructions.Load();
        this.lblLineState.Load();

        this.orangeLine = default;
        this.orangeLine.Color = Color.FromArgb(255, 193, 105, 46);
        this.orangeLine.Thickness = 20;
        this.orangeLine.P1 = new Vector2(WindowCenter.X, WindowCenter.Y - 100);
        this.orangeLine.P2 = new Vector2(WindowCenter.X, WindowCenter.Y + 100);

        this.blueLine = default;
        this.blueLine.Color = Color.SteelBlue;
        this.blueLine.Thickness = 15;
        this.blueLine.P1 = new Vector2(WindowCenter.X - 100, WindowCenter.Y - 100);
        this.blueLine.P2 = new Vector2(WindowCenter.X + 100, WindowCenter.Y + 100);

        this.whiteLine = default;
        this.whiteLine.Color = Color.AntiqueWhite;
        this.whiteLine.Thickness = 10;
        this.whiteLine.P1 = new Vector2(WindowCenter.X - 100, WindowCenter.Y);
        this.whiteLine.P2 = new Vector2(WindowCenter.X + 100, WindowCenter.Y);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();

        UpdateWhiteLineLayer();
        UpdateLineStateText();
        MoveWhiteLine(frameTime);

        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);
        this.lblLineState.Position = new Vector2(WindowPadding, WindowCenter.Y - this.lblLineState.HalfHeight);

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        this.shapeRenderer.Render(this.blueLine, (int)BlueLayer);
        this.shapeRenderer.Render(this.orangeLine, (int)OrangeLayer);
        this.shapeRenderer.Render(this.whiteLine, (int)this.whiteLayer);

        // Render the background
        this.lblInstructions.Render(0);
        this.lblLineState.Render(0);
        base.Render();
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        this.backgroundManager.Unload();
        this.lblInstructions.Unload();
        this.lblLineState.Unload();

        base.UnloadContent();
    }

    /// <summary>
    /// Updates the text for the state of the white line.
    /// </summary>
    private void UpdateLineStateText()
    {
        var textLines = new[]
        {
            $"White Line Layer: {this.whiteLayer}",
            $"Orange Line Layer: {OrangeLayer}",
            $"Blue Line Layer: {BlueLayer}",
        };

        this.lblLineState.Text = string.Join(Environment.NewLine, textLines);
    }

    /// <summary>
    /// Updates the current layer of the white rectangle.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException">
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
                _ => throw new InvalidEnumArgumentException($"this.{nameof(this.whiteLayer)}", (int)this.whiteLayer, typeof(RenderLayer))
            };
        }
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
