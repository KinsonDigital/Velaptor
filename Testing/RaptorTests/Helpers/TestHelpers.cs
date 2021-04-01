// <copyright file="TestHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Raptor.Graphics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using NETColor = System.Drawing.Color;

    [ExcludeFromCodeCoverage]
    public static class TestHelpers
    {
        private static readonly string BasePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private static readonly string TestResultDirName = "ImageTestResults";
        private static readonly string TestResultDirPath = @$"{BasePath}{TestResultDirName}\";

        /// <summary>
        /// Creates a new <see cref="ImageData"/> struct for the purpose of testing.
        /// </summary>
        /// <param name="color">The color to set all of the <see cref="ImageData.Pixels"/>.</param>
        /// <param name="width">The width of the image that the pixels represent.</param>
        /// <param name="height">The heightof the image that the pixels represent.</param>
        /// <returns>The struct to test.</returns>
        public static ImageData CreateImageData(NETColor color, int width, int height)
        {
            ImageData result = default;

            result.Pixels = CreatePixels(color, width, height);
            result.Width = width;
            result.Height = height;

            return result;
        }

        /// <summary>
        /// Converts the given <paramref name="imageData"/> of type <see cref="ImageData"/>
        /// to the type of <see cref="Image{Rgba32}"/>.
        /// </summary>
        /// <param name="imageData">The image data to convert.</param>
        /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
        public static Image<Rgba32> ToSixLaborImage(ImageData imageData)
        {
            var result = new Image<Rgba32>(imageData.Width, imageData.Height);

            for (var y = 0; y < result.Height; y++)
            {
                var pixelRowSpan = result.GetPixelRowSpan(y);

                for (var x = 0; x < result.Width; x++)
                {
                    pixelRowSpan[x] = new Rgba32(
                        imageData.Pixels[x, y].R,
                        imageData.Pixels[x, y].G,
                        imageData.Pixels[x, y].B,
                        imageData.Pixels[x, y].A);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given <paramref name="image"/> of type <see cref="Image{Rgba32}"/>
        /// to the type of <see cref="ImageData"/>.
        /// </summary>
        /// <param name="image">The image to convert.</param>
        /// <returns>The image data of type <see cref="ImageData"/>.</returns>
        public static ImageData ToImageData(Image<Rgba32> image)
        {
            ImageData result = default;

            result.Pixels = new NETColor[image.Width, image.Height];
            result.Width = image.Width;
            result.Height = image.Height;

            for (var y = 0; y < image.Height; y++)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);

                for (var x = 0; x < image.Width; x++)
                {
                    result.Pixels[x, y] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            }

            return result;
        }

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
                var pixelRowSpan = image.GetPixelRowSpan(y);

                for (var x = 0; x < image.Width; x++)
                {
                    result[x, y] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            }

            return result;
        }

        /// <summary>
        /// Saves the given <paramref name="image"/> into the test result directory
        /// with a PNG file name that matches the given <paramref name="unitTestName"/>.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="unitTestName">The name of the unit test requesting the name.</param>
        public static void SaveImageForTest(ImageData image, [CallerMemberName]string unitTestName = "NOT-NAME-SET")
        {
            unitTestName = Path.HasExtension(unitTestName)
                ? unitTestName.Split('.')[0]
                : unitTestName;

            var imageResultPath = $"{TestResultDirPath}{unitTestName}.png";

            ToSixLaborImage(image).SaveAsPng(imageResultPath);
        }

        /// <summary>
        /// Creates a new 2 dimensional array of pixel colors using the given <paramref name="color"/>
        /// with enough pixels to fill an image that has the given <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="color">The color of all the pixels.</param>
        /// <param name="width">The width of the image represented by the <see cref="ImageData.Pixels"/>.</param>
        /// <param name="height">The height of the image represented by the <see cref="ImageData.Pixels"/>.</param>
        /// <returns>The 2 dimensional array of pixels to test.</returns>
        private static NETColor[,] CreatePixels(NETColor color, int width, int height)
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
    }
}
