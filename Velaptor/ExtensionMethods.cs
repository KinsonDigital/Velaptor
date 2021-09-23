// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using SimpleInjector;
    using SimpleInjector.Diagnostics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Velaptor.Graphics;
    using NETColor = System.Drawing.Color;

    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts the given <paramref name="radians"/> value into degrees.
        /// </summary>
        /// <param name="radians">The value to convert.</param>
        /// <returns>The radians converted into degrees.</returns>
        public static float ToDegrees(this float radians) => radians * 180.0f / (float)Math.PI;

        /// <summary>
        /// Converts the given <paramref name="degrees"/> value into radians.
        /// </summary>
        /// <param name="degrees">The value to convert.</param>
        /// <returns>The degrees converted into radians.</returns>
        public static float ToRadians(this float degrees) => degrees * (float)Math.PI / 180f;

        /// <summary>
        /// Sets the value to positive if its negative.
        /// </summary>
        /// <param name="value">The value to force.</param>
        /// <returns>The value as a positive number.</returns>
        public static float ForcePositive(this float value) => value < 0 ? value * -1 : value;

        /// <summary>
        /// Sets the value to negative if its positive.
        /// </summary>
        /// <param name="value">The value to force.</param>
        /// <returns>The value as a negative number.</returns>
        public static float ForceNegative(this float value) => value > 0 ? value * -1 : value;

        /// <summary>
        /// Maps the given <paramref name="value"/> from one range to another.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="fromStart">The from starting range value.</param>
        /// <param name="fromStop">The from ending range value.</param>
        /// <param name="toStart">The to starting range value.</param>
        /// <param name="toStop">The to ending range value.</param>
        /// <returns>A value that has been mapped to a range between <paramref name="toStart"/> and <paramref name="toStop"/>.</returns>
        public static float MapValue(this int value, float fromStart, float fromStop, float toStart, float toStop)
            => MapValue((float)value, fromStart, fromStop, toStart, toStop);

        /// <summary>
        /// Maps the given <paramref name="value"/> from one range to another.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="fromStart">The from starting range value.</param>
        /// <param name="fromStop">The from ending range value.</param>
        /// <param name="toStart">The to starting range value.</param>
        /// <param name="toStop">The to ending range value.</param>
        /// <returns>A value that has been mapped to a range between <paramref name="toStart"/> and <paramref name="toStop"/>.</returns>
        public static float MapValue(this float value, float fromStart, float fromStop, float toStart, float toStop)
            => toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));

        /// <summary>
        /// Maps the given <paramref name="value"/> from one range to another.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="fromStart">The from starting range value.</param>
        /// <param name="fromStop">The from ending range value.</param>
        /// <param name="toStart">The to starting range value.</param>
        /// <param name="toStop">The to ending range value.</param>
        /// <returns>A value that has been mapped to a range between <paramref name="toStart"/> and <paramref name="toStop"/>.</returns>
        /// <remarks>
        ///     Be careful when restricting the 'to' values to a value between 0 and 1.  This will always return a value
        ///     of 0.  This is because the return type is a byte and any value between the values of 0 and 1 is
        ///     a floating point value and floating point values cannot be represented with a byte data type.
        ///
        ///     This results in a value of 0 with a loss of information.  If you need to return a value that
        ///     is between the values of 0 and 1, use the method overload <see cref="MapValue(int,float,float,float,float)"/>.
        /// </remarks>
        public static byte MapValue(this byte value, byte fromStart, byte fromStop, byte toStart, byte toStop)
            => (byte)(toStart + ((toStop - (float)toStart) * ((value - (float)fromStart) / (fromStop - (float)fromStart))));

        /// <summary>
        /// Maps the given <paramref name="value"/> from one range to another.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="fromStart">The from starting range value.</param>
        /// <param name="fromStop">The from ending range value.</param>
        /// <param name="toStart">The to starting range value.</param>
        /// <param name="toStop">The to ending range value.</param>
        /// <returns>A value that has been mapped to a range between <paramref name="toStart"/> and <paramref name="toStop"/>.</returns>
        public static float MapValue(this byte value, float fromStart, float fromStop, float toStart, float toStop)
            => toStart + ((toStop - (float)toStart) * ((value - (float)fromStart) / (fromStop - (float)fromStart)));

        /// <summary>
        /// Rotates the <paramref name="vector"/> around the <paramref name="origin"/> at the given <paramref name="angle"/>.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="origin">The origin to rotate the <paramref name="vector"/> around.</param>
        /// <param name="angle">The angle in degrees to rotate <paramref name="vector"/>.  Value must be positive.</param>
        /// <param name="clockWise">Determines the direction the given <paramref name="vector"/> should rotate around the <paramref name="origin"/>.</param>
        /// <returns>The <paramref name="vector"/> rotated around the <paramref name="origin"/>.</returns>
        public static Vector2 RotateAround(this Vector2 vector, Vector2 origin, float angle, bool clockWise = true)
        {
            var angleRadians = clockWise ? angle.ToRadians() : angle.ToRadians() * -1;

            var cos = (float)Math.Cos(angleRadians);
            var sin = (float)Math.Sin(angleRadians);

            var dx = vector.X - origin.X; // The delta x
            var dy = vector.Y - origin.Y; // The delta y

            var tempX = (dx * cos) - (dy * sin);
            var tempY = (dx * sin) + (dy * cos);

            var x = tempX + origin.X;
            var y = tempY + origin.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        ///     Converts the given <see cref="System.Drawing.Color"/> to a <see cref="Vector4"/>
        ///     with each component holding the color component values.
        /// </summary>
        /// <param name="clr">The color to convert.</param>
        /// <returns>
        ///     A 4 component vector of color values.
        ///     X = red.
        ///     Y = green.
        ///     Z = blue.
        ///     W = alpha.
        /// </returns>
        public static Vector4 ToVector4(this NETColor clr) => new (clr.R, clr.G, clr.B, clr.A);

        /// <summary>
        /// Returns a value indicating whether the given file or directory path
        /// only contains a root drive path with no directories.
        /// </summary>
        /// <param name="fileOrDirPath">The path to check.</param>
        /// <returns><see langword="true"/> if there are no directories and is just a root drive.</returns>
        internal static bool IsDirectoryRootDrive(this string fileOrDirPath)
        {
            if (string.IsNullOrEmpty(fileOrDirPath))
            {
                return false;
            }

            var onlyDirPath = Path.HasExtension(fileOrDirPath)
                ? Path.GetDirectoryName(fileOrDirPath)
                : fileOrDirPath;

            if (onlyDirPath is null)
            {
                return false;
            }

            if (onlyDirPath.Count(c => c == ':') == 1 && onlyDirPath.Count(c => c == '\\') == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the last directory name in the given directory or file path.
        /// </summary>
        /// <param name="fileOrDirPath">The path to check.</param>
        /// <returns>The last directory name.</returns>
        /// <remarks>
        /// <para>
        ///     If the <paramref name="fileOrDirPath"/> is a file path, then the file name
        ///     will be stripped and the last directory will be returned.
        /// </para>
        /// <para>
        ///     Example: The path 'C:\temp\dirA\myfile.txt' will return 'dirA'.
        /// </para>
        /// <para>
        ///     If the <paramref name="fileOrDirPath"/> is a directory path, then the
        ///     last directory will be returned.
        /// </para>
        /// <para>
        ///     Example: The path 'C:\temp\dirA\dirB' will return the result 'dirB'.
        /// </para>
        /// </remarks>
        internal static string GetLastDirName(this string fileOrDirPath)
        {
            if (string.IsNullOrEmpty(fileOrDirPath))
            {
                return string.Empty;
            }

            var onlyDirPath = Path.HasExtension(fileOrDirPath)
                ? Path.GetDirectoryName(fileOrDirPath)
                : fileOrDirPath;

            if (string.IsNullOrEmpty(onlyDirPath))
            {
                return string.Empty;
            }

            // If the directory path is just a root drive path
            if (onlyDirPath.IsDirectoryRootDrive())
            {
                var sections = onlyDirPath.Split(':', StringSplitOptions.RemoveEmptyEntries);

                return sections[^1] == Path.DirectorySeparatorChar.ToString()
                    ? onlyDirPath
                    : sections[^1].TrimStart(Path.DirectorySeparatorChar);
            }

            var dirNames = onlyDirPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            return dirNames[^1];
        }

        /// <summary>
        /// Converts the items of type <see cref="IEnumerable{T}"/> to type <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the <see cref="IEnumerable{T}"/> list.</typeparam>
        /// <param name="items">The items to convert.</param>
        /// <returns>The items as a read only collection.</returns>
        internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
            => new (items.ToList());

        /// <summary>
        /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
        /// inherit from <see cref="IDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type to suppress against.</typeparam>
        /// <param name="container">The container that the suppression applies to.</param>
        [ExcludeFromCodeCoverage]
        internal static void SuppressDisposableTransientWarning<T>(this Container container)
        {
            var registration = container.GetRegistration(typeof(T))?.Registration;
            registration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");
        }

        /// <summary>
        ///     Conditionally registers that a new instance of <typeparamref name="TImplementation"/> will be returned
        ///     every time a <typeparamref name="TService"/> is requested (transient) and where the supplied predicate
        ///     returns true. The predicate will only be evaluated a finite number of times;
        ///     the predicate is unsuited for making decisions based on runtime conditions.
        /// </summary>
        /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="predicate">
        ///     The predicate that determines whether the <typeparamref name="TImplementation"/> can be applied for
        ///     the requested service type. This predicate can be used to build a fallback mechanism
        ///     where multiple registrations for the same service type are made. Note that the
        ///     predicate will be called a finite number of times and its result will be cached
        ///     for the lifetime of the container. It can't be used for selecting a type based
        ///     on runtime conditions.
        /// </param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <remarks>
        ///     This method uses the container's LifestyleSelectionBehavior to select the exact
        ///     lifestyle for the specified type. By default this will be Transient.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and can not be altered.</exception>
        [ExcludeFromCodeCoverage]
        internal static void RegisterConditional<TService, TImplementation>(this Container container, Predicate<PredicateContext> predicate, bool suppressDisposal = false)
            where TService : class
            where TImplementation : class, TService
        {
            container.RegisterConditional<TService, TImplementation>(predicate);

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TService>(container);
            }
        }

        /// <summary>
        ///     Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
        ///     a <typeparamref name="TService"/> is requested (transient).
        /// </summary>
        /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <remarks>
        ///     This method uses the container's LifestyleSelectionBehavior to select the exact
        ///     lifestyle for the specified type. By default this will be Transient.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and can not be altered.</exception>
        [ExcludeFromCodeCoverage]
        internal static void Register<TService, TImplementation>(this Container container, bool suppressDisposal = false)
            where TService : class
            where TImplementation : class, TService
        {
            container.Register<TService, TImplementation>();

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TService>(container);
            }
        }

        /// <summary>
        ///     Registers that a new instance of <typeparamref name="TImplementation"/> will be returned.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and can not be altered.</exception>
        [ExcludeFromCodeCoverage]
        internal static void Register<TImplementation>(this Container container, bool suppressDisposal = false)
            where TImplementation : class
        {
            container.Register<TImplementation>();

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TImplementation>(container);
            }
        }

        /// <summary>
        ///     Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
        ///     a <typeparamref name="TService"/> is requested (transient).
        /// </summary>
        /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="lifestyle">The lifestyle that specifies how the returned instance will be cached.</param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <remarks>
        ///     This method uses the container's LifestyleSelectionBehavior to select the exact
        ///     lifestyle for the specified type. By default this will be Transient.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and can not be altered.</exception>
        [ExcludeFromCodeCoverage]
        internal static void Register<TService, TImplementation>(this Container container, Lifestyle lifestyle, bool suppressDisposal = false)
            where TService : class
            where TImplementation : class, TService
        {
            container.Register<TService, TImplementation>(lifestyle);

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TService>(container);
            }
        }

        /// <summary>
        /// Converts the given <paramref name="image"/> of type <see cref="ImageData"/>
        /// to the type of <see cref="Image{Rgba32}"/>.
        /// </summary>
        /// <param name="image">The image data to convert.</param>
        /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
        internal static Image<Rgba32> ToSixLaborImage(in this ImageData image)
        {
            var result = new Image<Rgba32>((int)image.Width, (int)image.Height);

            for (var y = 0; y < result.Height; y++)
            {
                var pixelRowSpan = result.GetPixelRowSpan(y);

                for (var x = 0; x < result.Width; x++)
                {
                    var pixel = image.Pixels[x, y];

                    pixelRowSpan[x] = new Rgba32(
                        pixel.R,
                        pixel.G,
                        pixel.B,
                        pixel.A);
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
        internal static ImageData ToImageData(this Image<Rgba32> image)
        {
            var pixelData = new NETColor[image.Width, image.Height];

            for (var y = 0; y < image.Height; y++)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);

                for (var x = 0; x < image.Width; x++)
                {
                    pixelData[x, y] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            }

            return new ImageData(pixelData, (uint)image.Width, (uint)image.Height);
        }
    }
}
