// <copyright file="Playground.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace PlaygroundApp;

using System.Drawing;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.UI;

/// <summary>
/// Used to prototype, test, debug, and play around with Velaptor.
/// </summary>
public class Playground : Window
{
    private readonly IBatcher batcher;
    private readonly ITextureRenderer textureRenderer;
    private readonly ILoader<IAtlasData> atlasLoader;
    private IAtlasData? atlas;

    /// <summary>
    /// Initializes a new instance of the <see cref="Playground"/> class.
    /// </summary>
    public Playground()
    {
        this.batcher = RendererFactory.CreateBatcher();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.atlasLoader = ContentLoaderFactory.CreateAtlasLoader();
    }

    /// <summary>
    /// Loads the content.
    /// </summary>
    protected override void OnLoad()
    {
        this.atlas = this.atlasLoader.Load("logo");
        base.OnLoad();
    }

    /// <summary>
    /// Unloads the content.
    /// </summary>
    protected override void OnUnload()
    {
        this.atlasLoader.Unload(this.atlas);
        base.OnUnload();
    }

    /// <summary>
    /// Draws the content to the window.
    /// </summary>
    /// <param name="frameTime">The time it took to draw the last frame.</param>
    protected override void OnDraw(FrameTime frameTime)
    {
        this.batcher.Begin();

        this.textureRenderer.Render(
            this.atlas.Texture,
            new Rectangle(0, 0, 166, 116),
            new Rectangle(300, 300, (int)this.atlas.Texture.Width, (int)this.atlas.Texture.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None);

        this.batcher.End();

        base.OnDraw(frameTime);
    }
}
