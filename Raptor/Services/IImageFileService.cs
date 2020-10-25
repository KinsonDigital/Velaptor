// <copyright file="IImageFile.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    /// <summary>
    /// Saves, loads and manages image files.
    /// </summary>
    public interface IImageFileService
    {
        /// <summary>
        /// Loads a image file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The directory path to the file.</param>
        /// <returns>The image data to return.</returns>
        (byte[] data, int width, int height) Load(string path);

        /// <summary>
        /// Saves the given <paramref name="imageData"/> to the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of where to save the file.</param>
        /// <param name="imageData">The image data to save in the file.</param>
        /// <param name="width">The width of the image being saved.</param>
        /// <param name="height">The height of the image being saved.</param>
        void Save(string path, byte[] imageData, int width, int height);
    }
}
