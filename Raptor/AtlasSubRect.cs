// <copyright file="AtlasSubRect.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    public struct AtlasSubRect : System.IEquatable<AtlasSubRect>
    {
        public AtlasSubRect(string name, int x, int y, int width, int height)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public string Name { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public static bool operator ==(AtlasSubRect left, AtlasSubRect right) => left.Equals(right);

        public static bool operator !=(AtlasSubRect left, AtlasSubRect right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(AtlasSubRect other) => other == this;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is AtlasSubRect rect))
                return false;

            return rect == this;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => System.HashCode.Combine(Name, X, Y, Width, Height);
    }
}
