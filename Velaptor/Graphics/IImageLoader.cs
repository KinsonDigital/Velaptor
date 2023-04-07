// <copyright file="IImageLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

/// <summary>
/// Loads images from disk.
/// </summary>
public interface IImageLoader
{
    /// <summary>
    /// Loads an image from the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the image file.</param>
    /// <returns>The image data.</returns>
    ImageData LoadImage(string filePath);
}
