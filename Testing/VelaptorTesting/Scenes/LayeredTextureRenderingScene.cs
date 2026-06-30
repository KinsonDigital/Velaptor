// <copyright file="LayeredTextureRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests out layered rendering with textures.
/// </summary>
public class LayeredTextureRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const float Speed = 200f;
    private const RenderLayer OrangeLayer = RenderLayer.Two;
    private const RenderLayer BlueLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IContentManager contentManager;
    private readonly BackgroundManager backgroundManager;
    private readonly StringBuilder boxStateText = new (string.Empty);
    private readonly Label lblInstructions;
    private readonly Label lblBoxState;
    private IAtlasData? atlas;
    private Vector2 whiteBoxPos;
    private Vector2 orangeBoxPos;
    private Vector2 blueBoxPos;
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private AtlasSubTextureData whiteBoxData;
    private AtlasSubTextureData orangeBoxData;
    private RenderLayer whiteLayer = RenderLayer.One;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredTextureRenderingScene"/> class.
    /// </summary>
    public LayeredTextureRenderingScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.backgroundManager = new BackgroundManager();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.contentManager = ContentManager.Create();

        var textLines = new[]
        {
            "Use the arrow keys to move the white box.",
            "Use the 'L' key to change the layer that the white box is rendered on.",
        };

        var instructionsText = string.Join(Environment.NewLine, textLines);
        this.lblInstructions = new Label
        {
            Text = instructionsText,
        };

        this.lblBoxState = new Label();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblInstructions.Load();
        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);

        this.lblBoxState.Load();

        this.atlas = this.contentManager.Load<IAtlasData>("layered-rendering-atlas");

        this.whiteBoxData = this.atlas.GetFrames("white-box")[0];
        this.orangeBoxData = this.atlas.GetFrames("orange-box")[0];

        // Set the default white box position
        this.orangeBoxPos.X = WindowCenter.X - 100;
        this.orangeBoxPos.Y = WindowCenter.Y;

        // Set the default blue box position
        this.blueBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 2f);
        this.blueBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 2f);

        // Set the default orange box position
        this.whiteBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 4f);
        this.whiteBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 4f);

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager.Unload();
        this.contentManager.Unload(this.atlas);
        this.lblInstructions.Unload();
        this.lblBoxState.Unload();

        this.atlas = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.lblBoxState.Position = new Vector2(WindowPadding, WindowCenter.Y - this.lblBoxState.HalfHeight);
        this.currentKeyState = this.keyboard.GetState();

        UpdateWhiteBoxLayer();
        UpdateWhiteBoxStateText();

        MoveWhiteBox(frameTime);

        this.prevKeyState = this.currentKeyState;
        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        // BLUE
        this.textureRenderer.Render(this.atlas, "blue-box", this.blueBoxPos, 0, (int)BlueLayer);

        // ORANGE
        this.textureRenderer.Render(this.atlas, "orange-box", this.orangeBoxPos, 0, (int)OrangeLayer);

        // WHITE
        this.textureRenderer.Render(this.atlas, "white-box", this.whiteBoxPos, 0, (int)this.whiteLayer);

        // The instructions text
        this.lblInstructions.Render();
        this.lblBoxState.Render();

        base.Render();
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
    private void UpdateWhiteBoxStateText()
    {
        this.boxStateText.Clear();
        this.boxStateText.AppendLine("Texture State");
        this.boxStateText.AppendLine($"  - White Box Layer: {this.whiteLayer}");
        this.boxStateText.AppendLine($"  - Orange Box Layer: {OrangeLayer}");
        this.boxStateText.AppendLine($"  - Blue Box Layer: {BlueLayer}");
        this.lblBoxState.Text = this.boxStateText.ToString();
    }

    /// <summary>
    /// Updates the current layer of the white box.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException">
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
                _ => throw new InvalidEnumArgumentException(
                    $"this.{nameof(this.whiteLayer)}",
                    (int)this.whiteLayer,
                    typeof(RenderLayer)),
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
        if (this.whiteBoxPos.X > WindowSize.Width - halfWidth)
        {
            this.whiteBoxPos.X = WindowSize.Width - halfWidth;
        }

        // Top edge containment
        if (this.whiteBoxPos.Y < halfHeight)
        {
            this.whiteBoxPos.Y = halfHeight;
        }

        // Bottom edge containment
        if (this.whiteBoxPos.Y > WindowSize.Height - halfHeight)
        {
            this.whiteBoxPos.Y = WindowSize.Height - halfHeight;
        }
    }
}
