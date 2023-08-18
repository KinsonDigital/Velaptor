// <copyright file="IRendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using Batching;

/// <summary>
/// Creates renderer instances.
/// </summary>
public interface IRendererFactory
{
    /// <summary>
    /// Creates an instance of the <see cref="ITextureRenderer"/>.
    /// </summary>
    /// <returns>The texture renderer.</returns>
    /// <remarks><c>NOTE:</c> the renderer is a singleton.</remarks>
    ITextureRenderer CreateTextureRenderer();

    /// <summary>
    /// Creates an instance of the <see cref="IFontRenderer"/>.
    /// </summary>
    /// <returns>The font renderer.</returns>
    /// <remarks><c>NOTE:</c> the renderer is a singleton.</remarks>
    IFontRenderer CreateFontRenderer();

    /// <summary>
    /// Creates an instance of the <see cref="IShapeRenderer"/>.
    /// </summary>
    /// <returns>The rectangle renderer.</returns>
    /// <remarks><c>NOTE:</c> the renderer is a singleton.</remarks>
    IShapeRenderer CreateShapeRenderer();

    /// <summary>
    /// Creates an instance of the <see cref="ILineRenderer"/>.
    /// </summary>
    /// <returns>The line renderer.</returns>
    /// <remarks><c>NOTE:</c> the renderer is a singleton.</remarks>
    ILineRenderer CreateLineRenderer();

    /// <summary>
    /// Creates an instance of <see cref="IBatcher"/> to start and stop batching.
    /// </summary>
    /// <returns>The batcher instance.</returns>
    IBatcher CreateBatcher();
}
