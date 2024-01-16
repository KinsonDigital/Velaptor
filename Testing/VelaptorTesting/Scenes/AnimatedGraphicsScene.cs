// <copyright file="AnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Drawing;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class AnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 10;
    private IAtlasData? mainAtlas;
    private ITextureRenderer? textureRenderer;
    private AtlasSubTextureData[]? frames;
    private BackgroundManager? backgroundManager;
    private ILoader<IAtlasData>? atlasLoader;
    private IControlGroup? grpControls;
    private int elapsedTime;
    private int currentFrame;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.textureRenderer = RendererFactory.CreateTextureRenderer();

        this.atlasLoader = ContentLoaderFactory.CreateAtlasLoader();
        this.mainAtlas = this.atlasLoader.Load("Main-Atlas");
        this.frames = this.mainAtlas.GetFrames("circle");

        var instructions = TestingApp.Container.GetInstance<ILabel>();
        instructions.Text = "Verify that the Kinson Digital logo is rotating clockwise.";

        this.grpControls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpControls.Title = "Instructions";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowPadding);
        };
        this.grpControls.Add(instructions);

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
        this.grpControls.Dispose();
        this.grpControls = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        if (this.elapsedTime >= 124)
        {
            this.elapsedTime = 0;

            this.currentFrame = this.currentFrame >= this.frames.Length - 1
                ? 0
                : this.currentFrame + 1;
        }
        else
        {
            this.elapsedTime += frameTime.ElapsedTime.Milliseconds;
        }
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager?.Render();
        this.textureRenderer.Render(
            this.mainAtlas.Texture,
            this.frames[this.currentFrame].Bounds,
            new Rectangle(WindowCenter.X, WindowCenter.Y, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None);

        this.grpControls.Render();

        base.Render();
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
}
