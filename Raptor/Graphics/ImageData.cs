// <copyright file="ImageData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional | Reason: The array does not waist space
#pragma warning disable CA1819 // Properties should not return arrays | Reason: pixel data is immutable
namespace Raptor.Graphics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    /// <summary>
    /// Holds image data such as the pixel colors for each X and Y location, the image width, and height.
    /// </summary>
    public struct ImageData : IEquatable<ImageData>
    {
        /// <summary>
        /// Gets or sets the pixel colors of the image.
        /// </summary>
        /// <remarks>
        ///     The first dimension is the X location of the pixel and the second
        ///     dimension is the Y location of the pixel.
        /// <para>
        ///     The 32-bit color component byte layout is ARGB.
        /// </para>
        /// </remarks>
        public Color[,] Pixels { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the iamge.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Returns a value indicating whether 2 <see cref="ImageData"/> types are equal.
        /// </summary>
        /// <param name="left">The left operator.</param>
        /// <param name="right">The right operator.</param>
        /// <returns>True if both are equal.</returns>
        public static bool operator ==(ImageData left, ImageData right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating whether 2 <see cref="ImageData"/> types are not equal.
        /// </summary>
        /// <param name="left">The left operator.</param>
        /// <param name="right">The right operator.</param>
        /// <returns>True if both are not equal.</returns>
        public static bool operator !=(ImageData left, ImageData right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(ImageData other)
        {
            if (Pixels.Length != other.Pixels.Length)
            {
                return false;
            }

            var arePixelsTrue = true;

            if (Pixels.Length > 0 && other.Pixels.Length > 0)
            {
                var breakOuterLoop = false;

                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (Pixels[x, y] != other.Pixels[x, y])
                        {
                            arePixelsTrue = false;
                            breakOuterLoop = true;
                            break;
                        }
                    }

                    if (breakOuterLoop)
                    {
                        break;
                    }
                }
            }
            else
            {
                arePixelsTrue = true;
            }

            return arePixelsTrue && Width == other.Width && Height == other.Height;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is ImageData imageData))
            {
                return false;
            }

            return Equals(imageData);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => HashCode.Combine(Pixels, Width, Height);
    }
}
