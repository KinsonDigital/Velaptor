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
    private readonly bool[]? flipState = [false, false];
    private readonly Color[,]? pixels;

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
    public Color[,] Pixels => this.pixels ?? new Color[0, 0];

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    public uint Width { get; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    public uint Height { get; }

    /// <summary>
    /// Gets the file path of the image.
    /// </summary>
    /// <remarks>This is only for reference.</remarks>
    public string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the image is flipped horizontally.
    /// </summary>
    public bool IsFlippedHorizontally => this.flipState?[0] ?? false;

    /// <summary>
    /// Gets a value indicating whether the image is flipped vertically.
    /// </summary>
    public bool IsFlippedVertically => this.flipState?[1] ?? false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageData"/> struct.
    /// </summary>
    /// <param name="pixels">The pixel data of the image.</param>
    /// <param name="filePath">The file path of where the image exists.</param>
    /// <remarks>
    ///     The <paramref name="filePath"/> is used for reference only.
    /// </remarks>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The summary is correct but StyleCop is not recognizing it.")]
    public ImageData(Color[,] pixels, string filePath = "")
    {
        ArgumentNullException.ThrowIfNull(pixels);

        this.pixels = pixels;

        Width = (uint)pixels.GetUpperBound(0) + 1;
        Height = (uint)pixels.GetUpperBound(1) + 1;
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
    /// If a pixel of the given <paramref name="image"/> is outside of the bounds of this
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

        if (this.flipState is not null)
        {
            this.flipState[0] = !this.flipState[0];
        }
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

        if (this.flipState is not null)
        {
            this.flipState[1] = !this.flipState[1];
        }
    }

    /// <summary>
    /// Returns a value indicating whether this instance is equal to the given <see cref="ImageData"/>.
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
    /// Returns a value indicating whether the <see cref="ImageData"/> contents are empty.
    /// </summary>
    /// <returns><c>true</c> if empty.</returns>
    public bool IsEmpty() => Width == 0 && Height == 0;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => HashCode.Combine(Pixels, Width, Height, FilePath);
}
