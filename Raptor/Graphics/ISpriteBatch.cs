// <copyright file="ISpriteBatch.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Renders a single or batch of textures.
    /// </summary>
    public interface ISpriteBatch : IDisposable
    {
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        uint BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the render surface width.
        /// </summary>
        /// <remarks>This is the width of the viewport.</remarks>
        int RenderSurfaceWidth { get; set; }

        /// <summary>
        /// Gets or sets the render surface height.
        /// </summary>
        /// <remarks>This is the height of the viewport.</remarks>
        int RenderSurfaceHeight { get; set; }

        /// <summary>
        /// Gets or sets the color that the back buffer will be cleared to.
        /// </summary>
        Color ClearColor { get; set; }

        /// <summary>
        /// Starts the batch rendering process.  Must be called before calling
        /// the <see cref="Render()"/> methods.
        /// </summary>
        void BeginBatch();

        /// <summary>
        /// Clears the buffers.
        /// </summary>
        /// <remarks>
        ///     It is best to clear the buffer before rendering all of the textures.
        ///     This is to make sure smearing does not occur during texture
        ///     movement or animation.
        /// </remarks>
        void Clear();

        /// <summary>
        /// Ends the sprite batch process.  Calling this will render any textures
        /// still in the batch.
        /// </summary>
        void EndBatch();

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <exception cref="Exception">Thrown if the <see cref="BeginBatch"/>() method has not been called.</exception>
        void Render(ITexture texture, int x, int y);

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
        /// <exception cref="Exception">Thrown if the <see cref="BeginBatch"/>() method has not been called.</exception>
        void Render(ITexture texture, int x, int y, RenderEffects effects);

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture.</param>
        /// <exception cref="Exception">Thrown if the <see cref="BeginBatch"/>() method has not been called.</exception>
        void Render(ITexture texture, int x, int y, Color tintColor);

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture.</param>
        /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
        /// <exception cref="Exception">Thrown if the <see cref="BeginBatch"/>() method has not been called.</exception>
        void Render(ITexture texture, int x, int y, Color tintColor, RenderEffects effects);

        /// <summary>
        /// Renders the given <see cref="Texture"/> using the given parametters.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="srcRect">The rectangle of the sub texture within the texture to render.</param>
        /// <param name="destRect">The destination rectangle of rendering.</param>
        /// <param name="size">The size to render the texture at. 1 is for 100%/normal size.</param>
        /// <param name="angle">The angle of rotation in degrees of the rendering.</param>
        /// <param name="tintColor">The color to apply to the rendering.</param>
        /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
        /// <exception cref="Exception">Thrown if the <see cref="BeginBatch"/>() method has not been called.</exception>
        void Render(ITexture texture, Rectangle srcRect, Rectangle destRect, float size, float angle, Color tintColor, RenderEffects effects);

        /// <summary>
        /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
        /// at the position described by the given <paramref name="x"/> and <paramref name="y"/>
        /// coordinates.
        /// </summary>
        /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location to render the text.</param>
        /// <param name="y">The Y coordinate location to render the text.</param>
        /// <remarks>
        ///     The X and Y coordinates is the bottom left corner of the string of text being rendered.
        /// <para>
        ///     The color of the text will be white.
        /// </para>
        /// </remarks>
        void Render(IFont font, string text, int x, int y);

        /// <summary>
        /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
        /// with the given <paramref name="tintColor"/> at the position described by
        /// the given <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </summary>
        /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location to render the text.</param>
        /// <param name="y">The Y coordinate location to render the text.</param>
        /// <param name="tintColor">The color of the text.</param>
        /// <remarks>
        ///     The X and Y coordinates is the bottom left corner of the string of text being rendered.
        /// </remarks>
        void Render(IFont font, string text, int x, int y, Color tintColor);
    }
}
