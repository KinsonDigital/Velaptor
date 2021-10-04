// <copyright file="SpriteBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System.Drawing;
    using Velaptor.Graphics;

    /// <summary>
    /// A single batch item in a batch of items to be rendered to the screen with a single OpenGL call.
    /// </summary>
    internal struct SpriteBatchItem
    {
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
        /// The angle in degress of the texture.
        /// </summary>
        /// <remarks>Needs to be a value between 0 and 360.</remarks>
        public float Angle;

        /// <summary>
        /// The color to apply to the entire textrue.
        /// </summary>
        public Color TintColor;

        /// <summary>
        /// The type of effects to apply to the texture when rendering.
        /// </summary>
        public RenderEffects Effects;

        /// <summary>
        /// The ID of the texture.
        /// </summary>
        public uint TextureId;

        /// <summary>
        /// Gets an empty <see cref="SpriteBatchItem"/>.
        /// </summary>
        /// <returns>An empty sprite batch item.</returns>
        public static SpriteBatchItem Empty
        {
            get
            {
                SpriteBatchItem result;
                result.TextureId = 0;
                result.Size = 0f;
                result.Angle = 0f;
                result.SrcRect = Rectangle.Empty;
                result.DestRect = Rectangle.Empty;
                result.TintColor = Color.Empty;
                result.Effects = RenderEffects.None;

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="SpriteBatchItem"/> is empty.
        /// </summary>
        public bool IsEmpty => this.TextureId == 0 &&
                    this.SrcRect.IsEmpty &&
                    this.DestRect.IsEmpty &&
                    this.Size == 0f &&
                    this.Angle == 0f &&
                    this.TintColor.IsEmpty &&
                    (this.Effects == 0 || this.Effects == RenderEffects.None);
    }
}
