using Raptor.OpenGLImp;
using Raptor.Plugins;
using System;
using System.Collections.Generic;
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
        private readonly IDebugDraw? _debugDraw = null;
        private readonly IRenderer _renderer;
        private bool _hasBegun;
        private readonly List<TextureData> _texturesToRender = new List<TextureData>();
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Renderer"/>.
        /// </summary>
        /// <param name="renderer">The renderer implementation.</param>
        public Renderer(IRenderer renderer) => _renderer = renderer;


        /// <summary>
        /// Creates a new instance of <see cref="Renderer"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Renderer() => _renderer = new GLRenderer(800, 600);
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public int ViewportWidth { get => _renderer.ViewportWidth; set => _renderer.ViewportWidth = value; }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public int ViewportHeight { get => _renderer.ViewportHeight; set => _renderer.ViewportHeight = value; }
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
        public void Clear(GameColor color)
        {
            if (_renderer is null)
                return;

            _renderer.Clear(color);
        }


        public void Begin()
        {
            _hasBegun = true;
        }


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(Texture texture, float x, float y)
            => Render(texture, x, y, 0, 1, new GameColor(255, 255, 255, 255));


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(Texture texture, Vector2 position)
            => Render(texture, position.X, position.Y, 0, 1, new GameColor(255, 255, 255, 255));


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(Texture texture, float x, float y, float angle)
            => Render(texture, x, y, angle, 1, new GameColor(255, 255, 255, 255));


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
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(Texture texture, float x, float y, float angle, float size, GameColor color)
        {
            if (texture is null)
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");

            if (_renderer is null)
                return;


            var textureData = new TextureData()
            {
                ID = texture.ID,
                Layer = texture.Layer,
                X = x,
                Y = y,
                Width = texture.Width,
                Height = texture.Height,
                Angle = angle,
                Size = size,
                TintColorAlpha = color.Alpha,
                TintColorRed = color.Red,
                TintColorGreen = color.Green,
                TintColorBlue = color.Blue
            };


            _texturesToRender.Add(textureData);
        }


        private static bool Contains<T>(Memory<T> memory, Predicate<T> predicate) where T : struct
        {
            var items = memory.Span;

            for (int i = 0; i < memory.Length; i++)
            {
                if (predicate.Invoke(items[i]))
                    return true;
            }


            return false;
        }


        private static int IndexOf<T>(Memory<T> memory, Predicate<T> predicate) where T : struct
        {
            var items = memory.Span;

            for (int i = 0; i < memory.Length; i++)
            {
                if (predicate.Invoke(items[i]))
                    return i;
            }


            return -1;
        }


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(GameText text, float x, float y)
            => Render(text, x, y, new GameColor(255, 0, 0, 0));


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(GameText text, Vector2 position)
            => Render(text, position.X, position.Y, new GameColor(255, 0, 0, 0));


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(GameText text, float x, float y, GameColor color)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text), "The text must not be null.");

            if (_renderer is null || text.InternalText is null)
                return;

            _renderer.Render(text.InternalText, x, y, color);
        }


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(GameText text, Vector2 position, GameColor color)
            => Render(text, position.X, position.Y, color);


        /// <summary>
        /// Renders an area of the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="area">The area/section of the texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void RenderTextureArea(Texture texture, Rect area, Vector2 position)
        {
            if (_renderer is null || texture is null)
                return;

            _renderer.RenderTextureArea(texture.InternalTexture, area, position.X, position.Y);
        }


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        public void FillCircle(Vector2 position, float radius, GameColor color)
        {
            if (_renderer is null)
                return;

            _renderer.FillCircle(position.X, position.Y, radius, color);
        }


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void FillRect(Rect rect, GameColor color)
        {
            if (_renderer is null)
                return;

            _renderer.FillRect(rect, color);
        }


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void Line(Vector2 start, Vector2 end, GameColor color)
        {
            if (_renderer is null)
                return;

            _renderer.RenderLine(start.X, start.Y, end.X, end.Y, color);
        }


        /// <summary>
        /// Renders the outline/frame of the physics body on the screen for debugging purposes.
        /// </summary>
        /// <param name="body">The physics body to render.</param>
        /// <param name="color">The color to render the outline/frame.</param>
        public void RenderDebugDraw(IPhysicsBody body, GameColor color)
        {
            if (_debugDraw is null || _renderer is null)
                return;

            _debugDraw.Draw(_renderer, body, color);
        }


        public void End()
        {
            if (!_hasBegun)
                throw new Exception($"The '{nameof(Renderer)}.{nameof(Begin)}()' method must be invoke first.");

            //First sort the textures to render by layer
            //Lower layer values will render before higher layer numbers
            _texturesToRender.Sort();

            foreach (var textureData in _texturesToRender)
                _renderer.Render(textureData);

            _texturesToRender.Clear();
            _hasBegun = false;
        }
        #endregion
    }
}
