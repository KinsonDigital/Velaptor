// <copyright file="ImageService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using Raptor.Graphics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using NETColor = System.Drawing.Color;
    using NETPoint = System.Drawing.Point;

    /// <summary>
    /// Saves, loads and manages image files.
    /// </summary>
    public class ImageService : IImageService
    {
        /// <inheritdoc/>
        public ImageData Load(string path)
        {
            var rgba32Image = Image.Load<Rgba32>(path);
            rgba32Image.Mutate(context => context.Flip(FlipMode.Vertical));

            var imageData = new ImageData(new NETColor[rgba32Image.Width, rgba32Image.Height], rgba32Image.Width, rgba32Image.Height);

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

        /// <inheritdoc/>
        public ImageData FlipVertically(ImageData image)
        {
            var sixLaborImage = image.ToSixLaborImage();
            sixLaborImage.Mutate(context => context.Flip(FlipMode.Vertical));

            return sixLaborImage.ToImageData();
        }

        /// <inheritdoc/>
        public ImageData FlipHorizontally(ImageData image)
        {
            var sixLaborImage = image.ToSixLaborImage();
            sixLaborImage.Mutate(context => context.Flip(FlipMode.Horizontal));

            return sixLaborImage.ToImageData();
        }

        /// <inheritdoc/>
        public ImageData Draw(ImageData src, ImageData dest, NETPoint location)
        {
            var srcImage = src.ToSixLaborImage();
            var destImage = dest.ToSixLaborImage();

            destImage.Mutate(context => context.DrawImage(srcImage, new Point(location.X, location.Y), 1));

            srcImage.Dispose();

            return destImage.ToImageData();
        }
    }
}
