// <copyright file="NonAnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests that graphics properly render to the screen.
/// </summary>
public class NonAnimatedGraphicsScene : SceneBase
{
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private readonly IAppInput<KeyboardState> keyboard;
    private IAtlasData? mainAtlas;
    private ITextureRenderer? textureRenderer;
    private IFontRenderer? fontRenderer;
    private IFont? font;
    private AtlasSubTextureData octagonData;
    private KeyboardState prevKeyState;
    private BackgroundManager? backgroundManager;
    private RenderEffects renderEffects = RenderEffects.None;
    private string instructions = string.Empty;
    private SizeF textSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonAnimatedGraphicsScene"/> class.
    /// </summary>
    public NonAnimatedGraphicsScene() => this.keyboard = HardwareFactory.GetKeyboard();

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
        this.fontRenderer = renderFactory.CreateFontRenderer();

        this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);
        var textLines = new List<string>
        {
            "Use arrow keys to flip the texture horizontally and vertically.",
            $"{Environment.NewLine}Left: Flip Horizontally",
            "Right: Flip Horizontally",
            "Up: Flip Vertically",
            "Down: Flip Vertically",
        };
        this.instructions = string.Join(Environment.NewLine, textLines);

        this.textSize = this.font.Measure(this.instructions);

        this.mainAtlas = ContentLoader.LoadAtlas("Main-Atlas");
        this.octagonData = this.mainAtlas.GetFrames("octagon-flip")[0];

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager?.Unload();
        ContentLoader.UnloadAtlas(this.mainAtlas);
        ContentLoader.UnloadFont(this.font);
        this.renderEffects = RenderEffects.None;

        this.mainAtlas = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.IsKeyUp(KeyCode.Right) && this.prevKeyState.IsKeyDown(KeyCode.Right))
        {
            this.renderEffects = this.renderEffects switch
            {
                RenderEffects.FlipHorizontally => RenderEffects.None,
                RenderEffects.FlipBothDirections => RenderEffects.FlipVertically,
                _ => this.renderEffects
            };
        }

        if (currentKeyState.IsKeyUp(KeyCode.Left) && this.prevKeyState.IsKeyDown(KeyCode.Left))
        {
            this.renderEffects = this.renderEffects switch
            {
                RenderEffects.None => RenderEffects.FlipHorizontally,
                RenderEffects.FlipVertically => RenderEffects.FlipBothDirections,
                _ => this.renderEffects
            };
        }

        if (currentKeyState.IsKeyUp(KeyCode.Down) && this.prevKeyState.IsKeyDown(KeyCode.Down))
        {
            this.renderEffects = this.renderEffects switch
            {
                RenderEffects.None => RenderEffects.FlipVertically,
                RenderEffects.FlipHorizontally => RenderEffects.FlipBothDirections,
                _ => this.renderEffects
            };
        }

        if (currentKeyState.IsKeyUp(KeyCode.Up) && this.prevKeyState.IsKeyDown(KeyCode.Up))
        {
            this.renderEffects = this.renderEffects switch
            {
                RenderEffects.FlipVertically => RenderEffects.None,
                RenderEffects.FlipBothDirections => RenderEffects.FlipHorizontally,
                _ => this.renderEffects
            };
        }

        this.prevKeyState = currentKeyState;
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        var posX = WindowCenter.X - (this.octagonData.Bounds.Width / 2);
        var posY = WindowCenter.Y - (this.octagonData.Bounds.Height / 2);

        var instructionsX = (int)(this.textSize.Width / 2) + 25;
        var instructionsY = (int)(this.textSize.Height / 2) + 25;

        this.backgroundManager?.Render();
        this.fontRenderer.Render(this.font, this.instructions, instructionsX, instructionsY);

        this.textureRenderer.Render(
            this.mainAtlas.Texture,
            this.octagonData.Bounds,
            new Rectangle(posX, posY, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
            1f,
            0f,
            Color.White,
            this.renderEffects);

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
}
