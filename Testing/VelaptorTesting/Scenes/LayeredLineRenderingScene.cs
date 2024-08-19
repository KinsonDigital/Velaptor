// <copyright file="LayeredLineRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
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
    private const int BackgroundLayer = -50;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState>? keyboard;
    private ITexture? background;
    private ITextureRenderer? textureRenderer;
    private ILineRenderer? lineRenderer;
    private Line whiteLine;
    private Line orangeLine;
    private Line blueLine;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Vector2 backgroundPos;
    private ILoader<ITexture>? textureLoader;
    private IControlGroup? grpInstructions;
    private IControlGroup? grpLineState;
    private RenderLayer whiteLayer = RenderLayer.One;
    private string? lblLineStateName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredLineRenderingScene"/> class.
    /// </summary>
    public LayeredLineRenderingScene() => this.keyboard = HardwareFactory.GetKeyboard();

    /// <inheritdoc cref="IContentLoadable.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();

        this.textureLoader = ContentLoaderFactory.CreateTextureLoader();

        this.background = this.textureLoader.Load("layered-rendering-background");
        this.backgroundPos = new Vector2(WindowCenter.X, WindowCenter.Y);

        var textLines = new[]
        {
            "Use the arrow keys to move the white line.",
            "Use the 'L' key to change the layer where the white line is rendered.",
        };

        var instructions = string.Join(Environment.NewLine, textLines);

        var ctrlFactory = new ControlFactory();

        var lblInstructions = ctrlFactory.CreateLabel();
        lblInstructions.Name = nameof(lblInstructions);
        lblInstructions.Text = instructions;

        this.grpInstructions = ctrlFactory.CreateControlGroup();
        this.grpInstructions.Title = "Instructions";
        this.grpInstructions.AutoSizeToFitContent = true;
        this.grpInstructions.TitleBarVisible = false;
        this.grpInstructions.Initialized += (_, _) =>
        {
            this.grpInstructions.Position = new Point(WindowCenter.X - this.grpInstructions.HalfWidth, WindowPadding);
        };
        this.grpInstructions.Add(lblInstructions);

        var lblLineState = ctrlFactory.CreateLabel();
        lblLineState.Name = nameof(lblLineState);
        this.lblLineStateName = nameof(lblLineState);

        this.grpLineState = ctrlFactory.CreateControlGroup();
        this.grpLineState.Title = "Line State";
        this.grpLineState.AutoSizeToFitContent = true;
        this.grpLineState.Initialized += (_, _) =>
        {
            this.grpLineState.Position = new Point(WindowPadding, WindowCenter.Y - this.grpLineState.HalfHeight);
        };
        this.grpLineState.Add(lblLineState);

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

        // Render the current enabled box text
        var textLines = new[]
        {
            $"White Line Layer: {this.whiteLayer}",
            $"Orange Line Layer: {OrangeLayer}",
            $"Blue Line Layer: {BlueLayer}",
        };

        var lblLineStateCtrl = this.grpLineState.GetControl<ILabel>(this.lblLineStateName);
        lblLineStateCtrl.Text = string.Join(Environment.NewLine, textLines);

        MoveWhiteLine(frameTime);

        this.prevKeyState = this.currentKeyState;

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.lineRenderer.Render(this.blueLine, (int)BlueLayer);
        this.lineRenderer.Render(this.orangeLine, (int)OrangeLayer);
        this.lineRenderer.Render(this.whiteLine, (int)this.whiteLayer);

        // Render the checkerboard background
        this.textureRenderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        this.grpInstructions.Render();
        this.grpLineState.Render();

        base.Render();
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.textureLoader.Unload(this.background);
        this.grpInstructions.Dispose();
        this.grpLineState.Dispose();
        this.grpInstructions = null;
        this.grpLineState = null;

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
