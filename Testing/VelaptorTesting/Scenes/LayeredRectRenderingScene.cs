// <copyright file="LayeredRectRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
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
    private const int BackgroundLayer = -50;
    private const int RectWidth = 200;
    private const int RectHeight = 200;
    private const RenderLayer BlueLayer = RenderLayer.Two;
    private const RenderLayer OrangeLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState> keyboard;
    private ITexture? background;
    private RectShape orangeRect;
    private RectShape whiteRect;
    private RectShape blueRect;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private Vector2 backgroundPos;
    private ITextureRenderer? textureRenderer;
    private IShapeRenderer? shapeRenderer;
    private ILoader<ITexture>? textureLoader;
    private IControlGroup? grpInstructions;
    private IControlGroup? grpRectState;
    private RenderLayer whiteLayer = RenderLayer.One;
    private string? lblRectStateName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredRectRenderingScene"/> class.
    /// </summary>
    public LayeredRectRenderingScene() => this.keyboard = HardwareFactory.GetKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();

        this.textureLoader = ContentLoaderFactory.CreateTextureLoader();

        this.background = this.textureLoader.Load("layered-rendering-background");
        this.backgroundPos = new Vector2(WindowCenter.X, WindowCenter.Y);

        var textLines = new[]
        {
            "Use the arrow keys to move the white rectangle.",
            "Use the 'L' key to change the layer where the white rectangle is rendered.",
        };

        var instructions = string.Join(Environment.NewLine, textLines);

        var lblInstructions = TestingApp.Container.GetInstance<ILabel>();
        lblInstructions.Name = nameof(lblInstructions);
        lblInstructions.Text = instructions;

        var lblRectState = TestingApp.Container.GetInstance<ILabel>();
        lblRectState.Name = nameof(lblRectState);
        this.lblRectStateName = nameof(lblRectState);

        this.grpInstructions = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpInstructions.Title = "Instructions";
        this.grpInstructions.AutoSizeToFitContent = true;
        this.grpInstructions.TitleBarVisible = false;
        this.grpInstructions.Initialized += (_, _) =>
        {
            this.grpInstructions.Position = new Point(WindowCenter.X - this.grpInstructions.HalfWidth, WindowPadding);
        };
        this.grpInstructions.Add(lblInstructions);

        this.grpRectState = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpRectState.Title = "Rect State";
        this.grpRectState.AutoSizeToFitContent = true;
        this.grpRectState.Initialized += (_, _) =>
        {
            this.grpRectState.Position = new Point(WindowPadding, WindowCenter.Y - this.grpRectState.HalfHeight);
        };
        this.grpRectState.Add(lblRectState);

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

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.shapeRenderer.Render(this.blueRect, (int)BlueLayer);
        this.shapeRenderer.Render(this.orangeRect, (int)OrangeLayer);
        this.shapeRenderer.Render(this.whiteRect, (int)this.whiteLayer);

        // Render the checkerboard background
        this.textureRenderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);

        this.grpInstructions.Render();
        this.grpRectState.Render();

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.textureLoader.Unload(this.background);
        this.grpInstructions.Dispose();
        this.grpRectState.Dispose();
        this.grpInstructions = null;
        this.grpRectState = null;

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
        var rectStateText = string.Join(Environment.NewLine, textLines);

        var lblRectStateCtl = this.grpRectState.GetControl<ILabel>(this.lblRectStateName);
        lblRectStateCtl.Text = rectStateText;
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
