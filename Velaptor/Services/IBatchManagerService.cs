// <copyright file="IBatchManagerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Numerics;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.OpenGL;

    /// <summary>
    /// Manages the process of batching textures together when rendering them.
    /// </summary>
    internal interface IBatchManagerService
    {
        /// <summary>
        /// Occurs when a batch is ready to be rendered.
        /// </summary>
        /// <remarks>
        /// Scenarios When The Batch Is Ready:
        /// <para>
        ///     1. The batch is ready when draw calls switch to another texture
        /// </para>
        /// <para>
        ///     2. The batch is ready when the total amount of render calls have
        ///     reached the <see cref="BatchSize"/>.
        /// </para>
        /// </remarks>
        event EventHandler<EventArgs>? BatchReady;

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        uint BatchSize { get; set; }

        /// <summary>
        /// Gets the list of batch items.
        /// </summary>
        /// <remarks>
        ///     Represents a list of items that are ready or not ready to be rendered.
        /// </remarks>
        ReadOnlyDictionary<uint, SpriteBatchItem> BatchItems { get; }

        /// <summary>
        /// Gets the total number of batch items that are ready for rendering.
        /// </summary>
        uint TotalItemsToRender { get; }

        /// <summary>
        /// Gets a value indicating whether the entire batch is ready for rendering.
        /// </summary>
        /// <returns>True if every batch item is ready.</returns>
        bool EntireBatchEmpty { get; }

        /// <summary>
        /// Updates the batch using the given <paramref name="texture"/> and other parameters about
        /// where to render a section of the texture to the screen with a given <paramref name="size"/>,
        /// <paramref name="angle"/>, and <paramref name="tintColor"/>.
        /// </summary>
        /// <param name="texture">The texture to add to the batch.</param>
        /// <param name="srcRect">The rectangle of the texture area to render.</param>
        /// <param name="destRect">The rectangle of the destination of where to render the texture.</param>
        /// <param name="size">The size of the texture.</param>
        /// <param name="angle">The angle of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture.</param>
        /// <param name="effects">The effects to apply to the texture.</param>
        void UpdateBatch(ITexture texture, Rectangle srcRect, Rectangle destRect, float size, float angle, Color tintColor, RenderEffects effects);

        /// <summary>
        /// Empties the entire batch.
        /// </summary>
        void EmptyBatch();

        /// <summary>
        /// Builds a transformation matrix in NDC(Normalized Device Coordinates) using the given parameters.
        /// </summary>
        /// <param name="portSize">The size of the port where rendering will occur.</param>
        /// <param name="x">The X position of the render.</param>
        /// <param name="y">The Y position of the render.</param>
        /// <param name="width">The width of the render.</param>
        /// <param name="height">The height of the render.</param>
        /// <param name="size">The size of the render.</param>
        /// <param name="angle">The angle of the render.</param>
        /// <returns>A 4x4 matrix result of all the parameters to send to the GPU.</returns>
        Matrix4x4 BuildTransformationMatrix(Vector2 portSize, float x, float y, int width, int height, float size, float angle);
    }
}
