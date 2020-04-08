using Raptor.Plugins;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Raptor.Graphics
{
    /// <summary>
    /// Used to render graphics to the graphics surface.
    /// </summary>
    public class Renderer
    {
        #region Private Fields
        private readonly IDebugDraw _debugDraw;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Renderer"/>.
        /// USED FOR UNIT TESTING.
        /// </summary>
        /// <param name="mockedRenderer">The mocked renderer to inject.</param>
        /// <param name="mockedDebugDraw">The mocked debug draw object to inject.</param>
        public Renderer(IRenderer mockedRenderer, IDebugDraw mockedDebugDraw)
        {
            InternalRenderer = mockedRenderer;
            _debugDraw = mockedDebugDraw;
        }


        /// <summary>
        /// Creates a new instance of <see cref="Renderer"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Renderer()
        {
            //TODO: Figure out how to get the proper implementation inside of this class
        }
        #endregion


        #region Props
        /// <summary>
        /// The internal renderer plugin implementation.
        /// </summary>
        public IRenderer InternalRenderer { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Clears the graphics surface to a color described by the given color components.
        /// </summary>
        /// <param name="alpha">The alpha component of the color.</param>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public void Clear(byte alpha, byte red, byte green, byte blue) => Clear(new GameColor(alpha, red, green, blue));


        /// <summary>
        /// Clears the graphics surface to the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">The color to clear the surface to.</param>
        public void Clear(GameColor color) => InternalRenderer.Clear(color);


        /// <summary>
        /// Starts the process of rendering a batch of <see cref="Texture"/>s, <see cref="GameText"/> items
        /// or primitives.  This method must be invoked before rendering.
        /// </summary>
        public void Begin() => InternalRenderer.Begin();


        /// <summary>
        /// Stops the batching process and renders all of the batched textures to the screen.
        /// </summary>
        public void End() => InternalRenderer.End();


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        public void Render(Texture texture, float x, float y) => InternalRenderer.Render(texture.InternalTexture, x, y);


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(Texture texture, Vector2 position) => Render(texture, position.X, position.Y);



        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        public void Render(Texture texture, float x, float y, float angle) => 
            InternalRenderer.Render(texture.InternalTexture, x, y, angle);


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
        public void Render(Texture texture, float x, float y, float angle, float size, GameColor color) => 
            InternalRenderer.Render(texture.InternalTexture, x, y, angle, size, color);


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        public void Render(GameText text, float x, float y) =>
            InternalRenderer.Render(text.InternalText, x, y);


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(GameText text, Vector2 position) => Render(text, position.X, position.Y);


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(GameText text, float x, float y, GameColor color) => InternalRenderer.Render(text.InternalText, x, y, color);


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(GameText text, Vector2 position, GameColor color) => Render(text, position.X, position.Y, color);


        /// <summary>
        /// Renders an area of the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="area">The area/section of the texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void RenderTextureArea(Texture texture, Rect area, Vector2 position) => 
            InternalRenderer.RenderTextureArea(texture.InternalTexture, area, position.X, position.Y);


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        public void FillCircle(Vector2 position, float radius, GameColor color) => 
            InternalRenderer.FillCircle(position.X, position.Y, radius, color);


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void FillRect(Rect rect, GameColor color) => InternalRenderer.FillRect(rect, color);


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void Line(Vector2 start, Vector2 end, GameColor color) => InternalRenderer.RenderLine(start.X, start.Y, end.X, end.Y, color);


        /// <summary>
        /// Renders the outline/frame of the physics body on the screen for debugging purposes.
        /// </summary>
        /// <param name="body">The physics body to render.</param>
        /// <param name="color">The color to render the outline/frame.</param>
        public void RenderDebugDraw(IPhysicsBody body, GameColor color) => _debugDraw.Draw(InternalRenderer, body, color);
        #endregion
    }
}
