using KDScorpionCore.Graphics;
using System;

namespace KDScorpionCore.Plugins
{
    /// <summary>
    /// Provides methods for rendering textures, text and primitives to the screen.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        #region Methods
        /// <summary>
        /// Starts the process of rendering a batch of <see cref="ITexture"/>s, <see cref="IText"/> items
        /// or primitives.  This method must be invoked before rendering.
        /// </summary>
        void Begin();


        /// <summary>
        /// Stops the batching process and renders all of the batched textures to the screen.
        /// </summary>
        void End();


        /// <summary>
        /// Clears the screen to the given color.
        /// </summary>
        /// <param name="color">The color to clear the screen to.</param>
        void Clear(GameColor color);


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        void Render(ITexture texture, float x, float y);


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        void Render(ITexture texture, float x, float y, float angle);


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        /// <param name="color">The color to apply to the texture.</param>
        void Render(ITexture texture, float x, float y, float angle, float size, GameColor color);


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        void Render(IText text, float x, float y);


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        void Render(IText text, float x, float y, GameColor color);


        /// <summary>
        /// Renders an area of the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="area">The area/section of the texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        void RenderTextureArea(ITexture texture, Rect area, float x, float y);


        /// <summary>
        /// Renders a line using the given start and stop coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        void RenderLine(float lineStartX, float lineStartY, float lineStopX, float lineStopY);


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        /// <param name="color">The color of the line.</param>
        void RenderLine(float startX, float startY, float endX, float endY, GameColor color);


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="x">The X coordinate on the screen of where to render the circle.</param>
        /// <param name="y">The Y coordinate on the screen of where to render the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        void FillCircle(float x, float y, float radius, GameColor color);


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        void FillRect(Rect rect, GameColor color);
        #endregion
    }
}
