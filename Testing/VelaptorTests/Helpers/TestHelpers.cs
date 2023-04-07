// <copyright file="TestHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Velaptor;
using Velaptor.Graphics;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;

[ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
public static class TestHelpers
{
    private const string TestResultDirName = "ImageTestResults";
    private static readonly string BasePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .ToCrossPlatPath();
    private static readonly string TestResultDirPath = @$"{BasePath}/{TestResultDirName}";

    /// <summary>
    /// Returns the directory path to the test result directory.
    /// </summary>
    /// <returns>The directory path.</returns>
    public static string GetTestResultDirPath() => TestResultDirPath;

    /// <summary>
    /// Sets up the directory of where to store image test results.
    /// </summary>
    public static void SetupTestResultDirPath()
    {
        if (Directory.Exists(TestResultDirPath) is false)
        {
            Directory.CreateDirectory(TestResultDirPath);
        }
    }

    /// <summary>
    /// Creates a new <see cref="ImageData"/> struct for the purpose of testing.
    /// </summary>
    /// <param name="color">The color to set all of the <see cref="ImageData.Pixels"/>.</param>
    /// <param name="width">The width of the image that the pixels represent.</param>
    /// <param name="height">The height of the image that the pixels represent.</param>
    /// <param name="filePath">The path to the file where the data came from.</param>
    /// <returns>The struct to test.</returns>
    public static ImageData CreateImageData(NETColor color, uint width, uint height, string filePath = "")
        => new (CreatePixels(color, width, height), width, height, filePath);

    /// <summary>
    /// Returns all of the pixels from the given <paramref name="image"/>
    /// and returns it as a 2-dimensional array of pixels represented by <see cref="NETColor"/>.
    /// </summary>
    /// <param name="image">The image to convert to pixels.</param>
    /// <returns>The pixel data from the test comparison image.</returns>
    public static NETColor[,] ToPixelColors(Image<Rgba32> image)
    {
        var result = new NETColor[image.Width, image.Height];

        for (var y = 0; y < image.Height; y++)
        {
            var row = y;
            image.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < image.Width; x++)
                {
                    result[x, row] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Saves the given <paramref name="image"/> into the test result directory
    /// with a PNG file name that matches the given <paramref name="unitTestName"/>.
    /// </summary>
    /// <param name="image">The image to save.</param>
    /// <param name="unitTestName">The name of the unit test requesting the name.</param>
    public static void SaveImageForTest(ImageData image, [CallerMemberName] string unitTestName = "NOT-NAME-SET")
    {
        unitTestName = Path.HasExtension(unitTestName)
            ? unitTestName.Split('.')[0]
            : unitTestName;

        var imageResultPath = $"{TestResultDirPath}{unitTestName}.png";

        ToSixLaborImage(image).SaveAsPng(imageResultPath);
    }

    /// <summary>
    /// Draws the given <paramref name="src"/> image onto the given <paramref name="dest"/>
    /// at the given <paramref name="location"/>.
    /// </summary>
    /// <param name="src">The source image to draw.</param>
    /// <param name="dest">The destination image to draw the source onto.</param>
    /// <param name="location">The location of where to draw the <paramref name="src"/> image.</param>
    /// <returns>The resulting image after the draw operation.</returns>
    [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Action is safe to perform.")]
    public static ImageData Draw(ImageData src, ImageData dest, NETPoint location)
    {
        if ((location.X < 0 && location.X > dest.Width) ||
            (location.Y < 0 && location.Y > dest.Height))
        {
            var exMsg = "The location to draw is outside of the bounds of the destination image.";
            exMsg += $"{Environment.NewLine}\tDestination Size: W:{dest.Width}, H: {dest.Height}";
            exMsg += $"{Environment.NewLine}\tDestination Location: X:{location.X}, Y: {location.Y}";

            throw new ArgumentException(exMsg, nameof(location));
        }

        var srcImage = src.ToSixLaborImage();
        if (srcImage is null)
        {
            throw new NullReferenceException("Failed to create six labor image.");
        }

        var destImage = dest.ToSixLaborImage();
        if (destImage is null)
        {
            throw new NullReferenceException("Failed to create six labor image.");
        }

        destImage.Mutate(context => context.DrawImage(srcImage, new Point(location.X, location.Y), 1));

        srcImage.Dispose();

        return ToImageData(destImage);
    }

    /// <summary>
    /// Returns the color of all of the pixels in the given <paramref name="row"/>.
    /// </summary>
    /// <param name="image">The image data that contains the row.</param>
    /// <param name="row">The row of pixels to return.</param>
    /// <returns>The list of row pixel colors.</returns>
    /// <remarks>The row is 0 index based.</remarks>
    public static IEnumerable<NETColor> GetRow(ImageData image, uint row) => GetRow(image, row, 0, image.Width);

    /// <summary>
    /// Returns the color of all of the pixels in the given <paramref name="row"/>.
    /// </summary>
    /// <param name="image">The image data that contains the row.</param>
    /// <param name="row">The row of pixels to return.</param>
    /// <param name="colStart">The column in the row where to start.</param>
    /// <param name="colStop">The column in the row where to stop.</param>
    /// <returns>The list of row pixel colors.</returns>
    /// <remarks>The row, colStart, and colStop are 0 index based.</remarks>
    public static IEnumerable<NETColor> GetRow(ImageData image, uint row, uint colStart, uint colStop)
    {
        if (row > image.Height - 1)
        {
            throw new Exception($"The row '{row}' does not exist.");
        }

        var rowPixels = new List<NETColor>(0);

        for (var x = 0; x < image.Width; x++)
        {
            // If the current column is within the range between column start and stop
            if (x >= colStart && x <= colStop)
            {
                rowPixels.Add(image.Pixels[x, row]);
            }
        }

        return rowPixels.ToArray();
    }

    /// <summary>
    /// Returns the color of all of the pixels in the given <paramref name="column"/>.
    /// </summary>
    /// <param name="image">The image data that contains the column.</param>
    /// <param name="column">The column of pixels to return.</param>
    /// <returns>The list of column pixel colors.</returns>
    /// <remarks>The column is 0 index based.</remarks>
    public static IEnumerable<NETColor> GetColumn(ImageData image, int column)
    {
        if (column < 0 || column > image.Width - 1)
        {
            throw new Exception($"The column '{column}' does not exist.");
        }

        var columnPixels = new List<NETColor>(0);

        for (var y = 0; y < image.Height; y++)
        {
            columnPixels.Add(image.Pixels[column, y]);
        }

        return columnPixels.ToArray();
    }

    /// <summary>
    /// Creates a new two dimensional array of pixel colors using the given <paramref name="color"/>
    /// with enough pixels to fill an image that has the given <paramref name="width"/> and <paramref name="height"/>.
    /// </summary>
    /// <param name="color">The color of all the pixels.</param>
    /// <param name="width">The width of the image represented by the <see cref="ImageData.Pixels"/>.</param>
    /// <param name="height">The height of the image represented by the <see cref="ImageData.Pixels"/>.</param>
    /// <returns>The two dimensional array of pixels to test.</returns>
    private static NETColor[,] CreatePixels(NETColor color, uint width, uint height)
    {
        var result = new NETColor[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                result[x, y] = color;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts the given <paramref name="image"/> of type <see cref="ImageData"/>
    /// to the type of <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="image">The image data to convert.</param>
    /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
    private static Image<Rgba32> ToSixLaborImage(this ImageData image)
    {
        var result = new Image<Rgba32>((int)image.Width, (int)image.Height);

        for (var y = 0; y < result.Height; y++)
        {
            var row = y;
            result.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < result.Width; x++)
                {
                    pixelRowSpan[x] = new Rgba32(
                        image.Pixels[x, row].R,
                        image.Pixels[x, row].G,
                        image.Pixels[x, row].B,
                        image.Pixels[x, row].A);
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Converts the given <paramref name="image"/> of type <see cref="Image{Rgba32}"/>
    /// to the type <see cref="ImageData"/>.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>The image data of type <see cref="ImageData"/>.</returns>
    private static ImageData ToImageData(Image<Rgba32> image)
    {
        var pixelData = new NETColor[image.Width, image.Height];

        for (var y = 0; y < image.Height; y++)
        {
            var row = y;
            image.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < image.Width; x++)
                {
                    pixelData[x, row] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            });
        }

        return new ImageData(pixelData, (uint)image.Width, (uint)image.Height);
    }
}
