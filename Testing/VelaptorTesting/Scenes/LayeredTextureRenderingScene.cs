// <copyright file="LayeredTextureRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using Velaptor.UI;

/// <summary>
/// Tests out layered rendering with textures.
/// </summary>
public class LayeredTextureRenderingScene : SceneBase
{
    private const string DefaultFont = "TimesNewRoman-Regular.ttf";
    private const float Speed = 200f;
    private const RenderLayer OrangeLayer = RenderLayer.Two;
    private const RenderLayer BlueLayer = RenderLayer.Four;
    private readonly IAppInput<KeyboardState> keyboard;
    private ITextureRenderer? textureRenderer;
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
    private BackgroundManager? backgroundManager;
    private ILoader<IFont>? fontLoader;
    private ILoader<IAtlasData>? atlasLoader;
    private Label? lblBoxState;
    private RenderLayer whiteLayer = RenderLayer.One;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredTextureRenderingScene"/> class.
    /// </summary>
    public LayeredTextureRenderingScene() => this.keyboard = HardwareFactory.GetKeyboard();

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

        this.textureRenderer = renderFactory.CreateTextureRenderer();

        this.fontLoader = ContentLoaderFactory.CreateFontLoader();
        this.font = this.fontLoader.Load(DefaultFont, 12);
        this.font.Style = FontStyle.Bold;

        var textLines = new[]
        {
            "Use the arrow keys to move the white box.",
            "Use the 'L' key to change the layer that the white box is rendered on.",
        };

        var lblInstructions = new Label
        {
            Color = Color.White,
            Text = string.Join(Environment.NewLine, textLines),
            Position = new Point(WindowCenter.X, 50),
        };

        this.atlasLoader = ContentLoaderFactory.CreateAtlasLoader();
        this.atlas = this.atlasLoader.Load("layered-rendering-atlas");

        this.whiteBoxData = this.atlas.GetFrames("white-box")[0];
        this.orangeBoxData = this.atlas.GetFrames("orange-box")[0];
        this.blueBoxData = this.atlas.GetFrames("blue-box")[0];

        // Set the default white box position
        this.orangeBoxPos.X = WindowCenter.X - 100;
        this.orangeBoxPos.Y = WindowCenter.Y;

        // Set the default blue box position
        this.blueBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 2f);
        this.blueBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 2f);

        // Set the default orange box position
        this.whiteBoxPos.X = this.orangeBoxPos.X - (this.orangeBoxData.Bounds.Width / 4f);
        this.whiteBoxPos.Y = this.orangeBoxPos.Y + (this.orangeBoxData.Bounds.Height / 4f);

        this.lblBoxState = new Label { Color = Color.White };

        AddControl(this.lblBoxState);
        AddControl(lblInstructions);

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
    public override void Render()
    {
        // BLUE
        this.textureRenderer.Render(
            this.atlas.Texture,
            this.blueBoxData.Bounds,
            new Rectangle((int)this.blueBoxPos.X, (int)this.blueBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)BlueLayer);

        // ORANGE
        this.textureRenderer.Render(
            this.atlas.Texture,
            this.orangeBoxData.Bounds,
            new Rectangle((int)this.orangeBoxPos.X, (int)this.orangeBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)OrangeLayer); // Neutral layer

        this.backgroundManager.Render();

        // WHITE
        this.textureRenderer.Render(
            this.atlas.Texture,
            this.whiteBoxData.Bounds,
            new Rectangle((int)this.whiteBoxPos.X, (int)this.whiteBoxPos.Y, (int)this.atlas.Width, (int)this.atlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            (int)this.whiteLayer);

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager.Unload();
        this.fontLoader.Unload(this.font);
        this.atlasLoader.Unload(this.atlas);

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
        this.lblBoxState.Text = string.Join(Environment.NewLine, textLines);

        this.lblBoxState.Left = 25;
        this.lblBoxState.Top = WindowCenter.Y - (int)(this.lblBoxState.Height / 2f);
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
