// <copyright file="InternalExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    // ReSharper disable RedundantNameQualifier
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
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.GPUData;
    using NETColor = System.Drawing.Color;
    using NETRectF = System.Drawing.RectangleF;
    using NETSizeF = System.Drawing.SizeF;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    internal static class InternalExtensionMethods
    {
        /// <summary>
        /// Determines whether or not this string instance starts with the specified character.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="value">The character to compare.</param>
        /// <returns><c>true</c> if <paramref name="value"/> matches the beginning of this string; otherwise, <c>false</c>.</returns>
        public static bool DoesNotStartWith(this string stringToCheck, char value) => !stringToCheck.StartsWith(value);

        /// <summary>
        /// Determines whether or not this string instance starts with the specified string.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="value">The string to compare.</param>
        /// <returns><c>true</c> if <paramref name="value"/> matches the beginning of this string; otherwise, <c>false</c>.</returns>
        public static bool DoesNotStartWith(this string stringToCheck, string value) => !stringToCheck.StartsWith(value);

        /// <summary>
        /// Determines whether or not the end of this string instance matches the specified character.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="value">The character to compare to the character at the end of this instance.</param>
        /// <returns><c>true</c> if <paramref name="value"/> matches the end of this instance; otherwise, <c>false</c>.</returns>
        public static bool DoesNotEndWith(this string stringToCheck, char value) => !stringToCheck.EndsWith(value);

        /// <summary>
        /// Determines whether or not the end of this string instance matches the specified string.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="value">The string to compare to the character at the end of this instance.</param>
        /// <returns><c>true</c> if <paramref name="value"/> matches the end of this instance; otherwise, <c>false</c>.</returns>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Kept for future development.")]
        public static bool DoesNotEndWith(this string stringToCheck, string value) => !stringToCheck.EndsWith(value);

        /// <summary>
        /// Returns a value indicating whether or not the given file or directory path
        /// is only a root drive path with no directories or file names.
        /// </summary>
        /// <param name="fileOrDirPath">The path to check.</param>
        /// <returns><see langword="true"/> if there are no directories and is just a root drive.</returns>
        public static bool OnlyContainsDrive(this string fileOrDirPath)
        {
            if (string.IsNullOrEmpty(fileOrDirPath))
            {
                return false;
            }

            var onlyDirPath = Path.HasExtension(fileOrDirPath)
                ? Path.GetDirectoryName(fileOrDirPath) !
                : fileOrDirPath;

            var noExtension = !Path.HasExtension(fileOrDirPath);
            var onlySingleColon = onlyDirPath.Count(c => c == ':') == 1;
            var onlySinglePathSeparator = onlyDirPath.Count(c => c == '\\') == 1;
            var correctLen = onlyDirPath.Length == 3;

            return noExtension &&
                   onlySingleColon &&
                   onlySinglePathSeparator &&
                   correctLen;
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
        ///     Example: The path 'C:\temp\dirA\file.txt' will return 'dirA'.
        /// </para>
        /// <para>
        ///     If the <paramref name="fileOrDirPath"/> is a directory path, then the
        ///     last directory will be returned.
        /// </para>
        /// <para>
        ///     Example: The path 'C:\temp\dirA\dirB' will return the result 'dirB'.
        /// </para>
        /// </remarks>
        public static string GetLastDirName(this string fileOrDirPath)
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
            if (onlyDirPath.OnlyContainsDrive())
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
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
            => new (items.ToList());

        /// <summary>
        /// Converts the given list of <paramref name="items"/> to a read only dictionary where
        /// the key is the <paramref name="items"/> array item index.
        /// </summary>
        /// <param name="items">The list of items to convert.</param>
        /// <typeparam name="T">The type of values in the lists.</typeparam>
        /// <returns>A read only dictionary of the given <paramref name="items"/>.</returns>
        public static ReadOnlyDictionary<uint, T> ToReadOnlyDictionary<T>(this T[] items)
        {
            var result = new Dictionary<uint, T>();

            for (var i = 0u; i < items.Length; i++)
            {
                result.Add(i, items[i]);
            }

            return new ReadOnlyDictionary<uint, T>(result);
        }

        /// <summary>
        /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
        /// inherit from <see cref="IDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type to suppress against.</typeparam>
        /// <param name="container">The container that the suppression applies to.</param>
        [ExcludeFromCodeCoverage]
        public static void SuppressDisposableTransientWarning<T>(this Container container)
        {
            var registration = container.GetRegistration(typeof(T))?.Registration;
            registration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");
        }

        /// <summary>
        ///    Conditionally registers that a new instance of <typeparamref name="TImplementation"/> will be returned
        ///    every time a <typeparamref name="TService"/> is requested (transient) and where the supplied predicate
        ///    returns true. The predicate will only be evaluated a finite number of times;
        ///    the predicate is unsuited for making decisions based on runtime conditions.
        /// </summary>
        /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="predicate">
        ///     The predicate that determines whether or not the <typeparamref name="TImplementation"/> can be applied for
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
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
        public static void RegisterConditional<TService, TImplementation>(this Container container, Predicate<PredicateContext> predicate, bool suppressDisposal = false)
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
        /// Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
        /// a <typeparamref name="TService"/> is requested (transient).
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
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
        public static void Register<TService, TImplementation>(this Container container, bool suppressDisposal = false)
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
        /// Registers that a new instance of <typeparamref name="TImplementation"/> will be returned.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
        [ExcludeFromCodeCoverage]
        public static void Register<TImplementation>(this Container container, bool suppressDisposal = false)
            where TImplementation : class
        {
            container.Register<TImplementation>();

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TImplementation>(container);
            }
        }

        /// <summary>
        /// Registers the specified delegate that allows returning transient instances of
        /// <typeparamref name="TService" />. The delegate is expected to always return a new instance on
        /// each call.
        /// </summary>
        /// <remarks>
        /// This method uses the container's
        /// <see cref="P:SimpleInjector.ContainerOptions.LifestyleSelectionBehavior">LifestyleSelectionBehavior</see> to select
        /// the exact lifestyle for the specified type. By default this will be
        /// <see cref="F:SimpleInjector.Lifestyle.Transient">Transient</see>.
        /// </remarks>
        /// <typeparam name="TService">The interface or base type that can be used to retrieve instances.</typeparam>
        /// <param name="container">The container that the registration applies to.</param>
        /// <param name="instanceCreator">The delegate that allows building or creating new instances.</param>
        /// <param name="suppressDisposal"><see langword="true"/> to ignore dispose warnings if the original code invokes dispose.</param>
        /// <exception cref="T:System.InvalidOperationException">
        /// Thrown when this container instance is locked and cannot be altered, or when the
        /// <typeparamref name="TService" /> has already been registered.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when <paramref name="instanceCreator" /> is a null reference.</exception>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
        public static void Register<TService>(this Container container, Func<TService> instanceCreator, bool suppressDisposal = false)
            where TService : class
        {
            container.Register(instanceCreator);

            if (suppressDisposal)
            {
                SuppressDisposableTransientWarning<TService>(container);
            }
        }

        /// <summary>
        /// Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
        /// a <typeparamref name="TService"/> is requested (transient).
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
        /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
        [ExcludeFromCodeCoverage]
        public static void Register<TService, TImplementation>(this Container container, Lifestyle lifestyle, bool suppressDisposal = false)
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
        public static Image<Rgba32> ToSixLaborImage(in this ImageData image)
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
        public static ImageData ToImageData(this Image<Rgba32> image)
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

        /// <summary>
        /// Converts the given <paramref name="vector"/> components to an array of floats.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>An array of float values.</returns>
        public static IEnumerable<float> ToVertexArray(this Vector2 vector) => new[] { vector.X, vector.Y };

        /// <summary>
        /// Converts the given <paramref name="clr"/> components to an array of floats.
        /// </summary>
        /// <param name="clr">The color to convert.</param>
        /// <returns>An array of float values.</returns>
        /// <remarks>
        ///     The order of the color components are changed to meet OpenGL requirements.
        ///     Component order is Red, Green, Blue, Alpha.
        /// </remarks>
        public static IEnumerable<float> ToVertexArray(this NETColor clr) => new float[] { clr.R, clr.G, clr.B, clr.A };

        /// <summary>
        /// Converts the given <paramref name="vertexData"/> components to an array of floats.
        /// </summary>
        /// <param name="vertexData">The data to convert.</param>
        /// <returns>An array of float values.</returns>
        public static IEnumerable<float> ToVertexArray(this TextureVertexData vertexData)
        {
            // NOTE: The order of the array elements are extremely important.
            // They determine the layout of each stride of vertex data and the layout
            // here has to match the layout told to OpenGL using the VertexAttribLocation() calls
            var result = new List<float>();

            result.AddRange(vertexData.VertexPos.ToVertexArray());
            result.AddRange(vertexData.TextureCoord.ToVertexArray());
            result.AddRange(vertexData.TintColor.ToVertexArray());

            return result.ToArray();
        }

        /// <summary>
        /// Converts the given <paramref name="quad"/> components to an array of floats.
        /// </summary>
        /// <param name="quad">The quad to convert.</param>
        /// <returns>An array of float values.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
        public static IEnumerable<float> ToVertexArray(this TextureQuadData quad)
        {
            var result = new List<float>();

            result.AddRange(quad.Vertex1.ToVertexArray());
            result.AddRange(quad.Vertex2.ToVertexArray());
            result.AddRange(quad.Vertex3.ToVertexArray());
            result.AddRange(quad.Vertex4.ToVertexArray());

            return result.ToArray();
        }

        /// <summary>
        /// Converts the given list of <paramref name="quads"/> components to an array of floats.
        /// </summary>
        /// <param name="quads">The quads to convert.</param>
        /// <returns>An array of float values.</returns>
        public static float[] ToVertexArray(this IEnumerable<TextureQuadData> quads)
        {
            var result = new List<float>();

            foreach (var quad in quads)
            {
                result.AddRange(quad.ToVertexArray());
            }

            return result.ToArray();
        }

        /// <summary>
        /// Removes the given <paramref name="trimChar"/> from the end of all of the given string <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The string items to trim.</param>
        /// <param name="trimChar">The character to trim.</param>
        /// <returns>
        /// The string that remains for each item after all characters are removed from the end of each string.
        /// If no characters can be trimmed from an item, the method leaves the item unchanged.
        /// </returns>
        /// <remarks>
        ///     If no <paramref name="trimChar"/> value is provided, then the spaces will be trimmed.
        /// </remarks>
        public static string[] TrimAllEnds(this string[] items, char trimChar = ' ')
        {
            for (var i = 0; i < items.Length; i++)
            {
                items[i] = items[i].TrimEnd(trimChar);
            }

            return items;
        }

        /// <summary>
        /// Returns all of the <see cref="Vector4"/> components as a <see cref="float"/> array.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>The components in a <c>X</c> <c>Y</c> <c>Z</c> <c>W</c> order.</returns>
        public static float[] ToArray(this Vector4 vector) => new[] { vector.X, vector.Y, vector.Z, vector.W };
    }
}
