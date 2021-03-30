// <copyright file="ImageService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional | Reason: The pixel array does not waist space
namespace Raptor.Services
{
    using Raptor.Graphics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using NETColor = System.Drawing.Color;

    /// <summary>
    /// Saves, loads and manages image files.
    /// </summary>
    public class ImageService : IImageService
    {
        /// <inheritdoc/>
        public ImageData Load(string path)
        {
            ImageData imageData = default;

            var rgba32Image = Image.Load<Rgba32>(path);
            rgba32Image.Mutate(context => context.Flip(FlipMode.Vertical));

            imageData.Pixels = new NETColor[rgba32Image.Width, rgba32Image.Height];
            imageData.Width = rgba32Image.Width;
            imageData.Height = rgba32Image.Height;

            for (var y = 0; y < rgba32Image.Height; y++)
            {
                var pixelRowSpan = rgba32Image.GetPixelRowSpan(y);

                for (var x = 0; x < rgba32Image.Width; x++)
                {
                    imageData.Pixels[x, y] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            }

            return imageData;
        }

        /// <inheritdoc/>
        public void Save(string path, ImageData imageData)
        {
            var rgba32Image = imageData.ToSixLaborImage();

            rgba32Image.SaveAsPng(path);
            rgba32Image.Dispose();
        }
    }
}
