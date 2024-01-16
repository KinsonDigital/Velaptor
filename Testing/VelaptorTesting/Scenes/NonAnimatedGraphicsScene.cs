// <copyright file="NonAnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
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
/// Tests that graphics properly render to the screen.
/// </summary>
public class NonAnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 10;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer? textureRenderer;
    private readonly ILoader<IAtlasData> atlasLoader;
    private IAtlasData? mainAtlas;
    private IControlGroup? grpControls;
    private AtlasSubTextureData octagonData;
    private KeyboardState prevKeyState;
    private BackgroundManager? backgroundManager;
    private RenderEffects renderEffects = RenderEffects.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonAnimatedGraphicsScene"/> class.
    /// </summary>
    public NonAnimatedGraphicsScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.atlasLoader = ContentLoaderFactory.CreateAtlasLoader();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var textLines = new List<string>
        {
            "Use arrow keys to flip the texture horizontally and vertically.",
            "1. Left to flip horizontally",
            "2. Right to flip horizontally",
            "3. Up to flip vertically",
            "4. Down to flip vertically",
        };

        var instructions = string.Join(Environment.NewLine, textLines);

        var lblInstructions = TestingApp.Container.GetInstance<ILabel>();
        lblInstructions.Name = nameof(lblInstructions);
        lblInstructions.Text = instructions;

        this.grpControls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpControls.Title = "Instructions";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowPadding);
        };
        this.grpControls.Add(lblInstructions);

        this.mainAtlas = this.atlasLoader.Load("Main-Atlas");
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
        this.atlasLoader.Unload(this.mainAtlas);
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
        this.backgroundManager?.Render();

        this.textureRenderer.Render(
            this.mainAtlas.Texture,
            this.octagonData.Bounds,
            new Rectangle(WindowCenter.X, WindowCenter.Y, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
            1f,
            0f,
            Color.White,
            this.renderEffects);

        this.grpControls.Render();

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
