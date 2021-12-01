// <copyright file="SizeU.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    using System;

    /// <summary>
    /// Stores an ordered pair of integers, which specify a <see cref="Width"/> and <see cref="Height"/>.
    /// </summary>
    public struct SizeU : IEquatable<SizeU>
    {
        /// <summary>
        /// Gets or sets the width of the size.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the size.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Returns true if the left operator is equal to the right operator.
        /// </summary>
        /// <param name="left">The left operator.</param>
        /// <param name="right">The right operator.</param>
        /// <returns>True if both operators are equal.</returns>
        public static bool operator ==(SizeU left, SizeU right) => left.Equals(right);

        /// <summary>
        /// Returns true if the left operator is not equal to the right operator.
        /// </summary>
        /// <param name="left">The left operator.</param>
        /// <param name="right">The right operator.</param>
        /// <returns>True if both operators are not equal.</returns>
        public static bool operator !=(SizeU left, SizeU right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(SizeU other) => Width == other.Width && Height == other.Height;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is SizeU size && Equals(size);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Width, Height);
    }
}
