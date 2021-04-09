// <copyright file="TestHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Raptor.Graphics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using NETColor = System.Drawing.Color;
    using NETPoint = System.Drawing.Point;

    [ExcludeFromCodeCoverage]
    public static unsafe class TestHelpers
    {
        private static readonly string BasePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private static readonly string TestResultDirName = "ImageTestResults";
        private static readonly string TestResultDirPath = @$"{BasePath}{TestResultDirName}\";

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
        /// <returns>The resulting image after the draw operationi.</returns>
        public static ImageData Draw(ImageData src, ImageData dest, NETPoint location)
        {
            var srcImage = src.ToSixLaborImage();
            var destImage = dest.ToSixLaborImage();

            destImage.Mutate(context => context.DrawImage(srcImage, new Point(location.X, location.Y), 1));

            srcImage.Dispose();

            return destImage.ToImageData();
        }

        /// <summary>
        /// Converts the given <paramref name="image"/> of type <see cref="ImageData"/>
        /// to the type of <see cref="Image{Rgba32}"/>.
        /// </summary>
        /// <param name="image">The image data to convert.</param>
        /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
        public static Image<Rgba32> ToSixLaborImage(this ImageData image)
        {
            var result = new Image<Rgba32>(image.Width, image.Height);

            for (var y = 0; y < result.Height; y++)
            {
                var pixelRowSpan = result.GetPixelRowSpan(y);

                for (var x = 0; x < result.Width; x++)
                {
                    pixelRowSpan[x] = new Rgba32(
                        image.Pixels[x, y].R,
                        image.Pixels[x, y].G,
                        image.Pixels[x, y].B,
                        image.Pixels[x, y].A);
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
        public static ImageData ToImageData(this Image<Rgba32> image)
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
        /// Returns an <see cref="IntPtr"/> to the given <paramref name="unmanagedData"/>.
        /// </summary>
        /// <typeparam name="T">The unmanaged type.</typeparam>
        /// <param name="unmanagedData">The unmanaged data to return a pointer to.</param>
        /// <returns>A pointer to the unmanaged data.</returns>
        public static IntPtr ToIntPtr<T>(ref T unmanagedData)
            where T : unmanaged
        {
            fixed (T* faceRecPtr = &unmanagedData)
            {
                return (IntPtr)faceRecPtr;
            }
        }

        /// <summary>
        /// Returns an unsafe pointer to the given <paramref name="unmanagedData"/>.
        /// </summary>
        /// <typeparam name="T">The type of unmanaged data.</typeparam>
        /// <param name="unmanagedData">The unmanaged data to conver.</param>
        /// <returns>The unsafe pointer to the unmanaged data.</returns>
        public static T* ToUnsafePointer<T>(ref T unmanagedData)
            where T : unmanaged
        {
            fixed (T* slotRecPtr = &unmanagedData)
            {
                return slotRecPtr;
            }
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
