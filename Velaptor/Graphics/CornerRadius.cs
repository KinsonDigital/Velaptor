// <copyright file="CornerRadius.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds all of the radius values for each corner of a rectangle.
    /// </summary>
    public readonly struct CornerRadius : IEquatable<CornerRadius>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CornerRadius"/> struct.
        /// </summary>
        /// <param name="topLeft">The top left corner radius.</param>
        /// <param name="bottomLeft">The bottom left corner radius.</param>
        /// <param name="bottomRight">The bottom right corner radius.</param>
        /// <param name="topRight">The top right corner radius.</param>
        public CornerRadius(float topLeft, float bottomLeft,  float bottomRight, float topRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornerRadius"/> struct.
        /// </summary>
        /// <param name="value">The value to set all corner radius values.</param>
        public CornerRadius(float value)
        {
            TopLeft = value;
            TopRight = value;
            BottomRight = value;
            BottomLeft = value;
        }

        /// <summary>
        /// Gets the top left corner radius.
        /// </summary>
        public float TopLeft { get; }

        /// <summary>
        /// Gets the bottom left corner radius.
        /// </summary>
        public float BottomLeft { get; }

        /// <summary>
        /// Gets the bottom right corner radius.
        /// </summary>
        public float BottomRight { get; }

        /// <summary>
        /// Gets the top right corner radius.
        /// </summary>
        public float TopRight { get; }

        /// <summary>
        /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><c>true</c> if the operands are equal.</returns>
        public static bool operator ==(CornerRadius left, CornerRadius right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><c>true</c> if the operands are not equal.</returns>
        public static bool operator !=(CornerRadius left, CornerRadius right) => !(left == right);

        /// <summary>
        /// Returns an empty <see cref="CornerRadius"/> instance.
        /// </summary>
        /// <returns>The empty instance.</returns>
        public static CornerRadius Empty() => new (0f, 0f, 0f, 0f);

        /// <summary>
        /// Returns a value indicating if the <see cref="CornerRadius"/> is empty.
        /// </summary>
        /// <returns>True if empty.</returns>
        public bool IsEmpty() => TopLeft == 0f &&
                                 BottomLeft == 0f &&
                                 BottomRight == 0f &&
                                 TopRight == 0f;

        /// <inheritdoc/>
        public bool Equals(CornerRadius other) =>
            TopLeft.Equals(other.TopLeft) &&
            BottomLeft.Equals(other.BottomLeft) &&
            BottomRight.Equals(other.BottomRight) &&
            TopRight.Equals(other.TopRight);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CornerRadius other && Equals(other);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => HashCode.Combine(TopLeft, BottomLeft, BottomRight, TopRight);
    }
}
