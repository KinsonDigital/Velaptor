// <copyright file="AtlasSubRect.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a rectangular region of a texture atlas.
    /// </summary>
    public struct AtlasSubRect : System.IEquatable<AtlasSubRect>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasSubRect"/> struct.
        /// </summary>
        /// <param name="name">The name of the rectangle.</param>
        /// <param name="x">The X position of the rectangle..</param>
        /// <param name="y">The Y position of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public AtlasSubRect(string name, int x, int y, int width, int height)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets or sets the name of the rectangle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X position of the rectangle.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the rectangle.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Returns a value indicating if both <see cref="AtlasSubRect"/>s are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both are equal to eachother.</returns>
        public static bool operator ==(AtlasSubRect left, AtlasSubRect right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating if both <see cref="AtlasSubRect"/>s are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both are not equal to eachother.</returns>
        public static bool operator !=(AtlasSubRect left, AtlasSubRect right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(AtlasSubRect other)
            => other.Name == Name &&
               other.X == X &&
               other.Y == Y &&
               other.Width == Width &&
               other.Height == Height;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is AtlasSubRect rect))
                return false;

            return rect == this;
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
            => System.HashCode.Combine(Name, X, Y, Width, Height);
    }
}
