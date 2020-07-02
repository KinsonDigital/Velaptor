// <copyright file="SpriteBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System.Drawing;

    /// <summary>
    /// Represents a single texture to be rendered to the screen as part of a batch.
    /// </summary>
    internal struct SpriteBatchItem
    {
        /// <summary>
        /// The texture ID of the texture to be rendered.
        /// </summary>
        public uint TextureID;

        /// <summary>
        /// The source rectangle inside of the texture to render.
        /// </summary>
        public Rectangle SrcRect;

        /// <summary>
        /// The destination rectangular area of where to render the texture on the screen.
        /// </summary>
        public Rectangle DestRect;

        /// <summary>
        /// The size of the texture to be rendered.
        /// </summary>
        /// <remarks>This must be a value between 0 and 1.</remarks>
        public float Size;

        /// <summary>
        /// The angle of the texture to be rendered.
        /// </summary>
        public float Angle;

        /// <summary>
        /// The color to tint the entire texture to be rendered.
        /// </summary>
        public Color TintColor;

        /// <summary>
        /// Gets an empty sprite batch item.
        /// </summary>
        /// <returns>An empty sprite batch item.</returns>
        public static SpriteBatchItem Empty
        {
            get
            {
                SpriteBatchItem result;

                result.TextureID = 0;
                result.Angle = 0;
                result.Size = 0;
                result.SrcRect = Rectangle.Empty;
                result.DestRect = Rectangle.Empty;
                result.TintColor = Color.Empty;

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="SpriteBatchItem"/> is empty.
        /// </summary>
        public bool IsEmpty => this.TextureID == 0 &&
                    this.SrcRect.IsEmpty &&
                    this.DestRect.IsEmpty &&
                    this.Size == 0f &&
                    this.Angle == 0f &&
                    this.TintColor.IsEmpty;
    }
}
