// <copyright file="BackgroundManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
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
    private readonly IContentManager contentManager;
    private ITextureRenderer? textureRenderer;
    private Vector2 backgroundPos;
    private ITexture? background;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundManager"/> class.
    /// </summary>
    public BackgroundManager() => this.contentManager = ContentManager.Create();

    /// <summary>
    /// Loads the background texture.
    /// </summary>
    /// <param name="position">The position to place the background.</param>
    public void Load(Vector2 position)
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();

        this.background = this.contentManager.Load<ITexture>(BackgroundTextureName);
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
    public void Unload()
    {
        if (this.background is null)
        {
            throw new NullReferenceException("The background texture cannot be null");
        }

        this.contentManager.Unload(this.background);
    }
}
