// <copyright file="NonAnimatedGraphicsScene.cs" company="KinsonDigital">
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
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests that graphics properly render to the screen.
/// </summary>
public class NonAnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 100;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly BackgroundManager backgroundManager;
    private IAtlasData? mainAtlas;
    private IFont? font;
    private KeyboardState prevKeyState;
    private RenderEffects renderEffects = RenderEffects.None;
    private string instructions = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonAnimatedGraphicsScene"/> class.
    /// </summary>
    public NonAnimatedGraphicsScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.backgroundManager = new BackgroundManager();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.mainAtlas = this.contentManager.Load<IAtlasData>("Main-Atlas");
        this.font = this.contentManager.LoadFont(Program.DefaultFontRegular, 12);

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var textLines = new string[]
        {
            "Use arrow keys to flip the texture horizontally and vertically.",
            "1. Left to flip horizontally",
            "2. Right to flip horizontally",
            "3. Up to flip vertically",
            "4. Down to flip vertically",
        };

        this.instructions = string.Join(Environment.NewLine, textLines);

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

        this.renderEffects = RenderEffects.None;

        this.contentManager.Unload(this.mainAtlas);
        this.mainAtlas = null;

        this.contentManager.Unload(this.font);
        this.font = null;

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
        if (this.mainAtlas is null)
        {
            return;
        }

        this.backgroundManager.Render();

        this.textureRenderer.Render(
            this.mainAtlas,
            "logo-flip",
            new Vector2(WindowCenter.X, WindowCenter.Y),
            0F,
            2f,
            Color.White,
            this.renderEffects);

        this.fontRenderer.Render(this.font, this.instructions, new Vector2(WindowCenter.X, WindowPadding));

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
