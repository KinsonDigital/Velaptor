// <copyright file="IImageService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Drawing;
using Graphics;

/// <summary>
/// Saves, loads and manages image files.
/// </summary>
internal interface IImageService
{
    /// <summary>
    /// Loads an image file at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The file path to the file.</param>
    /// <returns>The image data to return.</returns>
    ImageData Load(string filePath);

    /// <summary>
    /// Saves the given <paramref name="image"/> to the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="image">The image to save to a file.</param>
    /// <remarks>
    ///     Saves the image in the PNG file format.
    /// </remarks>
    void Save(string path, ImageData image);

    /// <summary>
    /// Flips the given <paramref name="image"/> vertically.
    /// </summary>
    /// <param name="image">The image to flip.</param>
    /// <returns>The image flipped.</returns>
    ImageData FlipVertically(ImageData image);

    /// <summary>
    /// Flips the given <paramref name="image"/> horizontally.
    /// </summary>
    /// <param name="image">The image to flip.</param>
    /// <returns>The image flipped.</returns>
    ImageData FlipHorizontally(ImageData image);

    /// <summary>
    /// Draws the given <paramref name="src"/> image on top of the given
    /// <paramref name="dest"/> image at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="src">The source image to draw onto the destination.</param>
    /// <param name="dest">The destination image/canvas to draw the source image onto.</param>
    /// <param name="location">The top left location of where to draw the <paramref name="src"/> image.</param>
    /// <returns>The destination image with the source drawn onto its surface.</returns>
    ImageData Draw(ImageData src, ImageData dest, Point location);
}
