// <copyright file="ImageService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.IO;
using System.IO.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Velaptor.Graphics;
using Velaptor.Guards;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;

namespace Velaptor.Services;

/// <summary>
/// Saves, loads and manages image files.
/// </summary>
internal sealed class ImageService : IImageService
{
    private readonly IFile file;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageService"/> class.
    /// </summary>
    /// <param name="file">Performs operations with files.</param>
    public ImageService(IFile file)
    {
        EnsureThat.ParamIsNotNull(file);
        this.file = file;
    }

    /// <inheritdoc/>
    public ImageData Load(string filePath)
    {
        if (this.file.Exists(filePath) is false)
        {
            throw new FileNotFoundException("The image file was not found.", filePath);
        }

        var rgba32Image = Image.Load<Rgba32>(filePath);
        rgba32Image.Mutate(context => context.Flip(FlipMode.Vertical));

        var imageData = new ImageData(new NETColor[rgba32Image.Width, rgba32Image.Height], (uint)rgba32Image.Width, (uint)rgba32Image.Height);

        for (var y = 0; y < rgba32Image.Height; y++)
        {
            var row = y;
            rgba32Image.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);
                for (var x = 0; x < rgba32Image.Width; x++)
                {
                    imageData.Pixels[x, row] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            });
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
    public ImageData Draw(ImageData src, ImageData dest, NETPoint location) => dest.DrawImage(src, location);
}
