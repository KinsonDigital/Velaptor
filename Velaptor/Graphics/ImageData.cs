// <copyright file="ImageData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

/// <summary>
/// Holds image data such as the pixel colors for each X and Y location, the image width, and height.
/// </summary>
public readonly struct ImageData : IEquatable<ImageData>
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
    public readonly Color[,] Pixels;

    /// <summary>
    /// Gets or sets the width of the image.
    /// </summary>
    public readonly uint Width;

    /// <summary>
    /// Gets or sets the height of the image.
    /// </summary>
    public readonly uint Height;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageData"/> struct.
    /// </summary>
    /// <param name="pixels">The pixel data of the image.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public ImageData(Color[,]? pixels, uint width, uint height)
    {
        if (pixels is null)
        {
            this.Pixels = new Color[width, height];

            // Makes all the pixels white
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    this.Pixels[x, y] = Color.White;
                }
            }
        }
        else
        {
            if (pixels.GetUpperBound(0) != width - 1)
            {
                var exceptionMsg = $"The length of the 1st dimension of the '{nameof(pixels)}' parameter";
                exceptionMsg += $" must match the '{nameof(width)}' parameter.";

                throw new ArgumentException(exceptionMsg);
            }

            if (pixels.GetUpperBound(1) != height - 1)
            {
                var exceptionMsg = $"The length of the 1st dimension of the '{nameof(pixels)}' parameter";
                exceptionMsg += $" must match the '{nameof(height)}' parameter.";

                throw new ArgumentException(exceptionMsg);
            }

            this.Pixels = pixels;
        }

        this.Width = width;
        this.Height = height;
    }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns><c>true</c> if both are equal.</returns>
    public static bool operator ==(ImageData left, ImageData right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns><c>true</c> if both are not equal.</returns>
    public static bool operator !=(ImageData left, ImageData right) => !(left == right);

    /// <summary>
    /// Draws the given <paramref name="image"/> onto this image,
    /// starting at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="image">The image to draw onto this one.</param>
    /// <param name="location">
    ///     The location of where to draw the image.
    ///     References the top left corner of the given <paramref name="image"/>.
    /// </param>
    /// <remarks>
    /// If a pixel of the given <paramref name="image"/> is out side of the bounds of this
    /// image, it will be skipped.
    /// </remarks>
    /// <returns>This current image with the given <paramref name="image"/> painted onto it.</returns>
    public ImageData DrawImage(ImageData image, Point location)
    {
        for (var x = 0; x < image.Width; x++)
        {
            if (x + location.X > this.Width - 1)
            {
                continue;
            }

            for (var y = 0; y < image.Height; y++)
            {
                if (y + location.Y > this.Height - 1)
                {
                    continue;
                }

                this.Pixels[location.X + x, location.Y + y] = image.Pixels[x, y];
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public bool Equals(ImageData other)
    {
        if (this.Pixels.Length != other.Pixels.Length)
        {
            return false;
        }

        var arePixelsTrue = true;

        var breakOuterLoop = false;

        for (var x = 0; x < this.Width; x++)
        {
            for (var y = 0; y < this.Height; y++)
            {
                if (this.Pixels[x, y] == other.Pixels[x, y])
                {
                    continue;
                }

                arePixelsTrue = false;
                breakOuterLoop = true;
                break;
            }

            if (breakOuterLoop)
            {
                break;
            }
        }

        return arePixelsTrue && this.Width == other.Width && this.Height == other.Height;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ImageData imageData && Equals(imageData);

    /// <summary>
    /// Returns a value indicating whether or not the <see cref="ImageData"/> contents are empty.
    /// </summary>
    /// <returns><c>true</c> if empty.</returns>
    public bool IsEmpty() => this.Width == 0 && this.Height == 0;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => HashCode.Combine(this.Pixels, this.Width, this.Height);
}
