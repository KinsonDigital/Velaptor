// <copyright file="AnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;
using Velaptor.UI;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class AnimatedGraphicsScene : SceneBase
{
    private const int TopMargin = 50;
    private IAtlasData? mainAtlas;
    private ITextureRenderer? textureRenderer;
    private AtlasSubTextureData[]? frames;
    private BackgroundManager? backgroundManager;
    private ILoader<IAtlasData>? atlasLoader;
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

        var instructions = new Label();
        instructions.Text = "Verify that the Kinson Digital logo is rotating clockwise.";
        instructions.Color = Color.White;

        AddControl(instructions);

        instructions.Left = WindowCenter.X - (int)(instructions.Width / 2);
        instructions.Top = TopMargin;

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
        var posX = WindowCenter.X - (this.frames[this.currentFrame].Bounds.Width / 2);
        var posY = WindowCenter.Y - (this.frames[this.currentFrame].Bounds.Height / 2);

        this.backgroundManager?.Render();
        this.textureRenderer.Render(
            this.mainAtlas.Texture,
            this.frames[this.currentFrame].Bounds,
            new Rectangle(posX, posY, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None);
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
