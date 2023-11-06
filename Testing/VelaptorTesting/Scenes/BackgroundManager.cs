// <copyright file="BackgroundManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;

/// <summary>
/// Manages the background texture.
/// </summary>
public class BackgroundManager : IDrawable
{
    private const int BackgroundLayer = -50;
    private const string BackgroundTextureName = "layered-rendering-background";
    private ILoader<ITexture>? loader;
    private ITextureRenderer? textureRenderer;
    private Vector2 backgroundPos;
    private ITexture? background;

    /// <summary>
    /// Loads the background texture.
    /// </summary>
    /// <param name="position">The position to place the background.</param>
    public void Load(Vector2 position)
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.loader = ContentLoaderFactory.CreateTextureLoader();

        this.background = this.loader.Load(BackgroundTextureName);
        this.backgroundPos = position;
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public void Render()
    {
        // Render the checkerboard background
        this.textureRenderer.Render(this.background, (int)this.backgroundPos.X, (int)this.backgroundPos.Y, BackgroundLayer);
    }

    /// <summary>
    /// Unloads the background texture.
    /// </summary>
    public void Unload() => this.loader.Unload(BackgroundTextureName);
}
