// <copyright file="NonAnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
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
using VelUpdatable = Velaptor.IUpdatable;

/// <summary>
/// Tests that graphics properly render to the screen.
/// </summary>
public class NonAnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 100;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IContentManager contentManager;
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblInstructions;
    private IAtlasData? mainAtlas;
    private KeyboardState prevKeyState;
    private RenderEffects renderEffects = RenderEffects.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonAnimatedGraphicsScene"/> class.
    /// </summary>
    public NonAnimatedGraphicsScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.contentManager = ContentManager.Create();
        this.backgroundManager = new BackgroundManager();

        this.lblInstructions = new Label();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.mainAtlas = this.contentManager.Load<IAtlasData>("Main-Atlas");
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblInstructions.Load();

        var textLines = new string[]
        {
            "Use arrow keys to flip the texture horizontally and vertically.",
            "1. Left to flip horizontally",
            "2. Right to flip horizontally",
            "3. Up to flip vertically",
            "4. Down to flip vertically",
        };

        this.lblInstructions.Text = string.Join(Environment.NewLine, textLines);
        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);

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
        this.lblInstructions.Unload();

        this.renderEffects = RenderEffects.None;

        this.contentManager.Unload(this.mainAtlas);
        this.mainAtlas = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>
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

        this.lblInstructions.Render(0);

        base.Render();
    }
}
