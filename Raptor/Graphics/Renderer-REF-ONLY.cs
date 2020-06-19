using Raptor.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

namespace Raptor.Graphics
{
    //!!NOTE!! - This is only going to be used as a reference for creating the SpriteBatch class



    /// <summary>
    /// Used to render graphics to the graphics surface.
    /// </summary>
    public class RendererREFONLY
    {
        #region Private Fields
        private bool _hasBegun;
        private readonly List<TextureData> _texturesToRender = new List<TextureData>();
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="RendererREFONLY"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public RendererREFONLY() { }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public int ViewportWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public int ViewportHeight { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Clears the graphics surface to a color described by the given color components.
        /// </summary>
        /// <param name="alpha">The alpha component of the color.</param>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public void Clear(byte alpha, byte red, byte green, byte blue) => Clear(Color.FromArgb(alpha, red, green, blue));


        /// <summary>
        /// Clears the graphics surface to the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">The color to clear the surface to.</param>
        public void Clear(Color color)
        {
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
            => Render(texture, x, y, 0, 1, Color.FromArgb(255, 255, 255, 255));


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(Texture texture, Vector2 position)
            => Render(texture, position.X, position.Y, 0, 1, Color.FromArgb(255, 255, 255, 255));


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
            => Render(texture, x, y, angle, 1, Color.FromArgb(255, 255, 255, 255));


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
        public void Render(Texture texture, float x, float y, float angle, float size, Color color)
        {
            if (texture is null)
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");

            var textureData = new TextureData()
            {
                ID = texture.ID,
                X = x,
                Y = y,
                Width = texture.Width,
                Height = texture.Height,
                Angle = angle,
                Size = size,
                TintColorAlpha = color.A,
                TintColorRed = color.R,
                TintColorGreen = color.G,
                TintColorBlue = color.B
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
        public void Render(RenderText text, float x, float y)
            => Render(text, x, y, Color.FromArgb(255, 0, 0, 0));


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void Render(RenderText text, Vector2 position)
            => Render(text, position.X, position.Y, Color.FromArgb(255, 0, 0, 0));


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public void Render(RenderText text, float x, float y, Color color)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text), "The text must not be null.");
        }


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(RenderText text, Vector2 position, Color color)
            => Render(text, position.X, position.Y, color);


        /// <summary>
        /// Renders an area of the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="area">The area/section of the texture to render.</param>
        /// <param name="position">The position on the surface to render.</param>
        public void RenderTextureArea(Texture texture, Rectangle area, Vector2 position)
        {
        }


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="position">The position on the surface to render.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        public void FillCircle(Vector2 position, float radius, Color color)
        {
        }


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void FillRect(Rectangle rect, Color color)
        {
        }


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void Line(Vector2 start, Vector2 end, Color color)
        {
        }


        /// <summary>
        /// Renders the outline/frame of the physics body on the screen for debugging purposes.
        /// </summary>
        /// <param name="body">The physics body to render.</param>
        /// <param name="color">The color to render the outline/frame.</param>
        public void RenderDebugDraw(Color color)
        {
        }


        public void End()
        {
            if (!_hasBegun)
                throw new Exception($"The '{nameof(RendererREFONLY)}.{nameof(Begin)}()' method must be invoke first.");

            _hasBegun = false;
        }
        #endregion
    }
}
