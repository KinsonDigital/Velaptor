// <copyright file="ImageFile.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Saves, loads and manages image files.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ImageFileService : IImageFileService
    {
        /// <inheritdoc/>
        public (byte[] data, int width, int height) Load(string path)
        {
            var image = (Image<Rgba32>)Image.Load(path);

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var tempPixels = new List<Rgba32>();

            for (var i = 0; i < image.Height; i++)
            {
                tempPixels.AddRange(image.GetPixelRowSpan(i).ToArray());
            }

            var pixels = new List<byte>();

            foreach (var pixel in tempPixels)
            {
                pixels.Add(pixel.R);
                pixels.Add(pixel.G);
                pixels.Add(pixel.B);
                pixels.Add(pixel.A);
            }

            var width = image.Width;
            var height = image.Height;

            image.Dispose();

            return (pixels.ToArray(), width, height);
        }

        /// <inheritdoc/>
        public void Save(string path, byte[] imageData, int width, int height)
        {
            var img = Image.LoadPixelData<Rgba32>(imageData, width, height);

            img.Mutate(x => x.Flip(FlipMode.Vertical));

            using var fileStream = new FileStream(path, FileMode.Create);

            img.SaveAsPng(fileStream);
        }
    }
}
