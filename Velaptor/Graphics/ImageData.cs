// <copyright file="ImageData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

/// <summary>
/// Holds image data such as the pixel colors for each X and Y location, the image width, and height.
/// </summary>
public readonly record struct ImageData
{
    private readonly bool[] flipState = { false, false };

    /// <summary>
    /// Gets the pixel colors of the image.
    /// </summary>
    /// <remarks>
    ///     The first dimension is the X location of the pixel and the second
    ///     dimension is the Y location of the pixel.
    /// <para>
    ///     The 32-bit color component byte layout is ARGB.
    /// </para>
    /// </remarks>
    public Color[,] Pixels { get; }

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    public uint Width { get; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    public uint Height { get; }

    /// <summary>
    /// Gets the file path of where the image exists.
    /// </summary>
    /// <remarks>This is only for reference.</remarks>
    public string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether or not the image is flipped horizontally.
    /// </summary>
    public bool IsFlippedHorizontally => this.flipState[0];

    /// <summary>
    /// Gets a value indicating whether or not the image is flipped vertically.
    /// </summary>
    public bool IsFlippedVertically => this.flipState[1];

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageData"/> struct.
    /// </summary>
    /// <param name="pixels">The pixel data of the image.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="filePath">The file path of where the image exists.</param>
    /// <remarks>
    ///     The <paramref name="filePath"/> is used for debugging purposes only.
    /// </remarks>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The summary is correct but StyleCop is not recognizing it.")]
    public ImageData(Color[,]? pixels, uint width, uint height, string filePath = "")
    {
        if (pixels is null)
        {
            Pixels = new Color[width, height];

            // Makes all the pixels white
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Pixels[x, y] = Color.White;
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

            Pixels = pixels;
        }

        Width = width;
        Height = height;
        FilePath = string.IsNullOrEmpty(filePath) ? string.Empty : filePath;
    }

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
            if (x + location.X > Width - 1)
            {
                continue;
            }

            for (var y = 0; y < image.Height; y++)
            {
                if (y + location.Y > Height - 1)
                {
                    continue;
                }

                Pixels[location.X + x, location.Y + y] = image.Pixels[x, y];
            }
        }

        return this;
    }

    /// <summary>
    /// Flips the image horizontally.
    /// </summary>
    public void FlipHorizontally()
    {
        var newPixels = new Color[Width, Height];

        // Flip the pixels horizontally
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var newX = Width - 1 - x;

                newPixels[newX, y] = Pixels[x, y];
            }
        }

        // Set the new pixels
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Pixels[x, y] = newPixels[x, y];
            }
        }

        this.flipState[0] = !this.flipState[0];
    }

    /// <summary>
    /// Flips the image vertically.
    /// </summary>
    public void FlipVertically()
    {
        var newPixels = new Color[Width, Height];

        // Flip the pixels horizontally
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var newY = Height - 1 - y;

                newPixels[x, newY] = Pixels[x, y];
            }
        }

        // Set the new pixels
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Pixels[x, y] = newPixels[x, y];
            }
        }

        this.flipState[1] = !this.flipState[1];
    }

    /// <summary>
    /// Returns a value indicating whether or not this instance is equal to the given <see cref="ImageData"/>.
    /// </summary>
    /// <param name="other">The other data to compare.</param>
    /// <returns><c>true</c> if equal.</returns>
    public bool Equals(ImageData other)
    {
        if (Pixels.Length != other.Pixels.Length)
        {
            return false;
        }

        if (FilePath != other.FilePath)
        {
            return false;
        }

        if (Width != other.Width)
        {
            return false;
        }

        if (Height != other.Height)
        {
            return false;
        }

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (Pixels[x, y] != other.Pixels[x, y])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        var filePathContent = string.IsNullOrEmpty(FilePath) ? string.Empty : $" | {FilePath}";

        sb.Append($"{Width} x {Height}{filePathContent}");

        return sb.ToString();
    }

    /// <summary>
    /// Returns a value indicating whether or not the <see cref="ImageData"/> contents are empty.
    /// </summary>
    /// <returns><c>true</c> if empty.</returns>
    public bool IsEmpty() => Width == 0 && Height == 0;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => HashCode.Combine(Pixels, Width, Height, FilePath);
}
