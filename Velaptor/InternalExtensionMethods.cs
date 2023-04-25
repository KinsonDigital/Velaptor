// <copyright file="InternalExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Graphics;
using OpenGL;
using OpenGL.Batching;
using OpenGL.GPUData;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectF = System.Drawing.RectangleF;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Provides extensions to various things to help make better code.
/// </summary>
internal static class InternalExtensionMethods
{
    private const char WinDirSeparatorChar = '\\';
    private const char CrossPlatDirSeparatorChar = '/';

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
    /// Removes all occurrences of the given <paramref name="trimChar"/> from the left
    /// side of all occurrences of the given <paramref name="value"/>, inside of this string.
    /// </summary>
    /// <param name="content">The string data containing the values to trim.</param>
    /// <param name="value">The value to trim the characters from.</param>
    /// <param name="trimChar">The character to trim off the <paramref name="value"/>.</param>
    /// <returns>
    ///     The content with all occurrences of the <paramref name="value"/> trimmed.
    /// </returns>
    public static string TrimLeftOf(this string content, string value, char trimChar)
    {
        var result = new StringBuilder(content);

        while (result.ToString().Contains($"{trimChar}{value}"))
        {
            result.Replace($"{trimChar}{value}", value);
        }

        return result.ToString();
    }

    /// <summary>
    /// Removes all occurrences of the given <paramref name="trimChar"/> from the right
    /// side of all occurrences of the given <paramref name="value"/>, inside of this string.
    /// </summary>
    /// <param name="content">The string data containing the values to trim.</param>
    /// <param name="value">The value to trim the characters from.</param>
    /// <param name="trimChar">The character to trim off the <paramref name="value"/>.</param>
    /// <returns>
    ///     The content with all occurrences of the <paramref name="value"/> trimmed.
    /// </returns>
    public static string TrimRightOf(this string content, string value, char trimChar)
    {
        var result = new StringBuilder(content);

        while (result.ToString().Contains($"{value}{trimChar}"))
        {
            result.Replace($"{value}{trimChar}", value);
        }

        return result.ToString();
    }

    /// <summary>
    /// Returns a value indicating whether or not the given file or directory path
    /// is only a root drive path with no directories or file names.
    /// </summary>
    /// <param name="fileOrDirPath">The path to check.</param>
    /// <returns><c>true</c> if there are no directories and is just a root drive.</returns>
    public static bool OnlyContainsDrive(this string fileOrDirPath)
    {
        if (string.IsNullOrEmpty(fileOrDirPath))
        {
            return false;
        }

        var onlyDirPath = Path.HasExtension(fileOrDirPath)
            ? Path.GetDirectoryName(fileOrDirPath)?.ToCrossPlatPath() ?? string.Empty
            : fileOrDirPath.ToCrossPlatPath();

        var noExtension = !Path.HasExtension(fileOrDirPath);
        var onlySingleColon = onlyDirPath.Count(c => c == ':') == 1;
        var onlySinglePathSeparator = onlyDirPath.Count(c => c is CrossPlatDirSeparatorChar) == 1;
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

        fileOrDirPath = fileOrDirPath.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);

        var onlyDirPath = Path.HasExtension(fileOrDirPath)
            ? Path.GetDirectoryName(fileOrDirPath)?.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar) ?? string.Empty
            : fileOrDirPath.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);

        if (string.IsNullOrEmpty(onlyDirPath))
        {
            return string.Empty;
        }

        var dirName = new DirectoryInfo(onlyDirPath).Name.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);

        return dirName;
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="path"/> is a valid drive.
    /// </summary>
    /// <param name="path">Processes directory and file paths.</param>
    /// <returns>True if <paramref name="path"/> contains a valid drive.</returns>
    /// <example>
    /// <list type="number">
    ///     <item>Value of <c>C:/my-dir</c> will return <c>true</c>.</item>
    ///     <item>Value of <c>C:my-dir</c> will return <c>true</c>.</item>
    ///     <item>Value of <c>C/my-dir/my-file.txt</c> will return <c>false</c>.</item>
    ///     <item>Value of <c>//my-dir</c> will return <c>false</c>.</item>
    ///     <item>Value of <c>./my-dir</c> will return <c>false</c>.</item>
    ///     <item>Value of <c>../my-dir</c> will return <c>false</c>.</item>
    /// </list>
    /// </example>
    /// <remarks>
    ///     This does not check if the drive exists.  It is for valid path syntax checks only.
    /// </remarks>
    public static bool HasValidDriveSyntax(this string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (path.Length < 2)
        {
            return false;
        }

        if (path.Contains(':') is false)
        {
            return false;
        }

        if (path.StartsWith(':'))
        {
            return false;
        }

        return !path[0].IsNotLetter() && path[1].IsNotLetter();
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="dirPath"/> is valid.
    /// </summary>
    /// <param name="dirPath">The directory path to check.</param>
    /// <returns>True if the <paramref name="dirPath"/> is valid.</returns>
    /// <example>
    /// <list type="number">
    ///     <item>Value of <c>C:/my-dir</c> will return <c>true</c>.</item>
    ///     <item>Value of <c>C:/my-dir/</c> will return <c>true</c>.</item>
    ///     <item>Value of <c>C:/my-dir/my-file.txt</c> will return <c>false</c>.</item>
    /// </list>
    /// </example>
    /// <remarks>
    ///     This does not check if the directory path exists.  It is for valid path syntax checks only.
    /// </remarks>
    public static bool HasValidFullDirPathSyntax(this string dirPath)
    {
        if (string.IsNullOrEmpty(dirPath))
        {
            return false;
        }

        if (dirPath.HasValidDriveSyntax() is false)
        {
            return false;
        }

        dirPath = dirPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (dirPath.Any(c => c == Path.AltDirectorySeparatorChar) is false)
        {
            return false;
        }

        return !Path.HasExtension(dirPath);
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="path"/> is valid.
    /// </summary>
    /// <param name="path">Processes directory and file paths.</param>
    /// <returns>True if the <paramref name="path"/> is valid.</returns>
    /// <example>
    /// <list type="number">
    ///     <item>Value of <c>C:/Windows</c> will return <c>false</c>.</item>
    ///     <item>Value of <c>//my-share</c> will return <c>true</c>.</item>
    /// </list>
    /// </example>
    /// <remarks>
    ///     This does not check if the UNC directory path exists.  It is for valid path syntax checks only.
    /// </remarks>
    public static bool HasValidUNCPathSyntax(this string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (path.Length < 3)
        {
            return false;
        }

        path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (path.DoesNotStartWith($@"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}"))
        {
            return false;
        }

        return path.Contains(':') is false;
    }

    /// <summary>
    /// Trims any newline characters from the end of the <c>string</c>.
    /// </summary>
    /// <param name="value">The string to trim.</param>
    /// <returns>Returns the string with all new line characters removed from the end.</returns>
    public static string TrimNewLineFromEnd(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        const char newLine = '\n';
        const char carriageReturn = '\r';

        while (value.EndsWith(newLine) || value.EndsWith(carriageReturn))
        {
            value = value.TrimEnd(newLine);
            value = value.TrimEnd(carriageReturn);
        }

        return value;
    }

    /// <summary>
    /// Trims any directory separator characters from the end of the <c>string</c>.
    /// </summary>
    /// <param name="value">The string to trim.</param>
    /// <returns>Returns the string with all directory separator characters removed from the end.</returns>
    public static string TrimDirSeparatorFromEnd(this string value)
    {
        const char backSlash = '\\';
        const char forwardSlash = '/';

        while (value.EndsWith(backSlash) || value.EndsWith(forwardSlash))
        {
            value = value.TrimEnd(backSlash);
            value = value.TrimEnd(forwardSlash);
        }

        return value;
    }

    /// <summary>
    /// Converts the given <paramref name="path"/> to a cross platform path.
    /// </summary>
    /// <param name="path">Processes directory and file paths.</param>
    /// <returns>The cross platform version of the <paramref name="path"/>.</returns>
    /// <returns>
    ///     This changes all '\' characters to '/' characters.
    ///     The '/' directory separator is valid on Windows and Linux systems.
    /// </returns>
    public static string ToCrossPlatPath(this string path) => path.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);

    /// <summary>
    /// Removes all instances of the given <paramref name="str"/> parameter from the <c>string</c>.
    /// </summary>
    /// <param name="value">The string to change.</param>
    /// <param name="str">The string to remove.</param>
    /// <returns>The original string with the <paramref name="str"/> values removed.</returns>
    public static string RemoveAll(this string value, string str) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Replace(str, string.Empty);

    /// <summary>
    /// Builds a name that represents a location of where an execution took place.
    /// </summary>
    /// <param name="unused">The object to enable this extension method to be executed anywhere.</param>
    /// <param name="postFixValue">The value to add to the end of the name.</param>
    /// <param name="memberName">The name of the member invoked this method.</param>
    /// <returns>The formatted member name of where this was invoked.</returns>
    /// <exception cref="Exception">Occurs if the stack frame is null.</exception>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Helper method.")]
    [ExcludeFromCodeCoverage]
    public static string GetExecutionMemberName(
        this object unused,
        string postFixValue = "",
        [CallerMemberName] string memberName = "")
    {
        string callerLocation;
        Type? declaringType;
        var skipFrames = 2;

        do
        {
            var method = new StackFrame(skipFrames, false).GetMethod();

            if (method is null)
            {
                throw new Exception("There was an issue getting the method for stack frame 2.");
            }

            declaringType = method.DeclaringType;

            if (declaringType == null)
            {
                return method.Name;
            }

            skipFrames++;
            callerLocation = declaringType.FullName ?? string.Empty;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        var containsPostFixValue = string.IsNullOrEmpty(postFixValue);

        callerLocation = callerLocation.Contains(".")
            ? callerLocation.Split('.')[^1]
            : callerLocation;

        memberName = memberName == ".ctor"
            ? "Ctor"
            : memberName;

        return $"{callerLocation}.{memberName}{(containsPostFixValue ? string.Empty : " - ")}{postFixValue}";
    }

    /// <summary>
    /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
    /// inherit from <see cref="IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type to suppress against.</typeparam>
    /// <param name="container">The container that the suppression applies to.</param>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
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
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <remarks>
    ///     This method uses the container's LifestyleSelectionBehavior to select the exact
    ///     lifestyle for the specified type. By default this will be transient.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
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
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <remarks>
    ///     This method uses the container's LifestyleSelectionBehavior to select the exact
    ///     lifestyle for the specified type. By default this will be Transient.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
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
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown when this container instance is locked and cannot be altered, or when the
    /// <typeparamref name="TService" /> has already been registered.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown when <paramref name="instanceCreator" /> is a null reference.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
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
    /// Converts the given <paramref name="image"/> of type <see cref="ImageData"/>
    /// to the type of <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="image">The image data to convert.</param>
    /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
    public static Image<Rgba32> ToSixLaborImage(this ImageData image)
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
                    var pixel = image.Pixels[x, row];

                    pixelRowSpan[x] = new Rgba32(
                        pixel.R,
                        pixel.G,
                        pixel.B,
                        pixel.A);
                }
            });
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

    /// <summary>
    /// Removes the given <paramref name="trimChar"/> from the end of all the given string <paramref name="items"/>.
    /// </summary>
    /// <param name="items">The string items to trim.</param>
    /// <param name="trimChar">The character to trim.</param>
    /// <returns>
    /// The string that remains for each item after all characters are removed from the end of each string.
    /// If no characters can be trimmed from an item, the item is unchanged.
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
    /// Returns the normalized versions of the given <paramref name="paths"/>.
    /// </summary>
    /// <param name="paths">The list of paths to normalize.</param>
    /// <returns>Normalized paths.</returns>
    /// <remarks>
    ///     A normalized path is a path that has all of its directory separators all the same to the value of <c>'/'</c>.
    /// </remarks>
    public static IReadOnlyCollection<string> NormalizePaths(this IEnumerable<string> paths) =>
        paths.Select(p => p.Contains(WinDirSeparatorChar)
                ? p.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar)
                : p).ToArray().AsReadOnly();

    /// <summary>
    /// Updates the <see cref="RectVertexData.VertexPos"/> using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="pos">The position to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetVertexPos(this RectGPUData gpuData, Vector2 pos, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            pos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Rectangle"/> of a vertex using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="rect">The rectangle to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetRectangle(this RectGPUData gpuData, Vector4 rect, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            rect,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Rectangle"/> for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="rectangle">The rectangle to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetRectangle(this RectGPUData gpuData, Vector4 rectangle)
    {
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.One);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Two);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Three);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.IsFilled"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="isFilled">The is filled setting to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetIsFilled(this RectGPUData gpuData, bool isFilled, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            isFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.IsFilled"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="isFilled">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetIsFilled(this RectGPUData gpuData, bool isFilled)
    {
        gpuData = gpuData.SetIsFilled(isFilled, VertexNumber.One);
        gpuData = gpuData.SetIsFilled(isFilled, VertexNumber.Two);
        gpuData = gpuData.SetIsFilled(isFilled, VertexNumber.Three);
        gpuData = gpuData.SetIsFilled(isFilled, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BorderThickness"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="borderThickness">The is filled setting to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBorderThickness(this RectGPUData gpuData, float borderThickness, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            borderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BorderThickness"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="borderThickness">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBorderThickness(this RectGPUData gpuData, float borderThickness)
    {
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.One);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Two);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Three);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopLeftCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topLeftCornerRadius">The top left corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopLeftCornerRadius(this RectGPUData gpuData, float topLeftCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            topLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopLeftCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topLeftCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopLeftCornerRadius(this RectGPUData gpuData, float topLeftCornerRadius)
    {
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomLeftCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomLeftCornerRadius">The bottom left corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomLeftCornerRadius(this RectGPUData gpuData, float bottomLeftCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            bottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomLeftCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomLeftCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomLeftCornerRadius(this RectGPUData gpuData, float bottomLeftCornerRadius)
    {
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomRightCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomRightCornerRadius">The bottom right corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomRightCornerRadius(this RectGPUData gpuData, float bottomRightCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            bottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomRightCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomRightCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomRightCornerRadius(this RectGPUData gpuData, float bottomRightCornerRadius)
    {
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopRightCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topRightCornerRadius">The top right corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopRightCornerRadius(this RectGPUData gpuData, float topRightCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            topRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopRightCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topRightCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopRightCornerRadius(this RectGPUData gpuData, float topRightCornerRadius)
    {
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Color"/> of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="color">The color to set the vertex to.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetColor(this RectGPUData gpuData, NETColor color, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            color,
            oldVertex.IsFilled,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Color"/> for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="color">The color to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetColor(this RectGPUData gpuData, NETColor color)
    {
        gpuData = gpuData.SetColor(color, VertexNumber.One);
        gpuData = gpuData.SetColor(color, VertexNumber.Two);
        gpuData = gpuData.SetColor(color, VertexNumber.Three);
        gpuData = gpuData.SetColor(color, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.VertexPos"/> using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="pos">The position to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static LineGPUData SetVertexPos(this LineGPUData gpuData, Vector2 pos, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new LineVertexData(
            pos,
            oldVertex.Color);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new LineGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new LineGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new LineGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new LineGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Sets the color of the <see cref="LineGPUData"/> to the given <paramref name="color"/>.
    /// </summary>
    /// <param name="gpuData">The line GPU data.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The <see cref="LineGPUData"/> with the new color applied.</returns>
    public static LineGPUData SetColor(this LineGPUData gpuData, NETColor color)
    {
        var newVertex1 = new LineVertexData(gpuData.Vertex1.VertexPos, color);
        var newVertex2 = new LineVertexData(gpuData.Vertex2.VertexPos, color);
        var newVertex3 = new LineVertexData(gpuData.Vertex3.VertexPos, color);
        var newVertex4 = new LineVertexData(gpuData.Vertex4.VertexPos, color);

        return new LineGPUData(newVertex1, newVertex2, newVertex3, newVertex4);
    }

    /// <summary>
    /// Scales the length of the given <paramref name="line"/> by the given <paramref name="scale"/> amount.
    /// </summary>
    /// <param name="line">The line to to scale.</param>
    /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
    /// <returns>The scaled line.</returns>
    public static LineBatchItem Scale(this LineBatchItem line, float scale)
    {
        var translatedVector = line.P2 - line.P1;

        var scaledVector = new Vector2(translatedVector.X * scale, translatedVector.Y * scale);

        // Translate the vector back to its original position
        line = line.SetP2(scaledVector + line.P1);

        return line;
    }

    /// <summary>
    /// Flips the end of the <paramref name="line"/> 180 degrees from its current position.
    /// </summary>
    /// <param name="line">The line to flip.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the end flipped.</returns>
    public static LineBatchItem FlipEnd(this LineBatchItem line)
    {
        var translatedStop = line.P2 - line.P1;

        translatedStop *= -1;

        // Translates the end of the line back
        translatedStop += line.P1;

        var result = line.SetP2(translatedStop);

        return result;
    }

    /// <summary>
    /// Calculates the length of the line.
    /// </summary>
    /// <param name="line">The line to calculate the length from.</param>
    /// <returns>The length of the line.</returns>
    public static float Length(this LineBatchItem line)
        => (float)Math.Sqrt(Math.Pow(line.P2.X - line.P1.X, 2) + Math.Pow(line.P2.Y - line.P1.Y, 2));

    /// <summary>
    /// Creates a rectangle from the given line.  This rectangle is takes the thickness of the line into account with
    /// its length to construct the rectangle.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    ///     The four corners of the rectangle as vectors.
    /// </returns>
    public static IEnumerable<Vector2> CreateRectFromLine(this LineBatchItem line)
    {
        var halfThickness = line.Thickness / 2f;

        // Green
        var lineA = line;
        lineA = lineA.SetP2(lineA.P2.RotateAround(lineA.P1, 90));

        var scale = halfThickness / lineA.Length();
        lineA = lineA.Scale(scale);

        // Blue
        var lineB = lineA;
        lineB = lineB.FlipEnd();

        // Yellow
        var lineC = line;

        lineC = lineC.SetP1(lineC.P1.RotateAround(lineC.P2, 90));

        lineC = lineC.SwapEnds();
        lineC = lineC.Scale(scale);

        // Red
        var lineD = lineC;
        lineD = lineD.FlipEnd();

        var rectPoints = new[]
        {
            lineB.P2,
            lineC.P2,
            lineD.P2,
            lineA.P2,
        };

        return rectPoints;
    }

    /// <summary>
    /// Sets the <see cref="LineBatchItem.P1"/> vector component of the batch item to the given <paramref name="p1"/> vector.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <param name="p1">The line end vector component.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the new line end vector component.</returns>
    public static LineBatchItem SetP1(this LineBatchItem item, Vector2 p1) =>
        new (p1,
            item.P2,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Sets the <see cref="LineBatchItem.P2"/> vector component of the batch item to the given <paramref name="p2"/> vector.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <param name="p2">The line end vector component.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the new line end vector component.</returns>
    public static LineBatchItem SetP2(this LineBatchItem item, Vector2 p2) =>
        new (item.P1,
            p2,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Swaps the <see cref="LineBatchItem"/>.<see cref="LineBatchItem.P1"/> and <see cref="LineBatchItem"/>.<see cref="LineBatchItem.P2"/>
    /// components of the batch item.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the components swapped.</returns>
    public static LineBatchItem SwapEnds(this LineBatchItem item) =>
        new (item.P2,
            item.P1,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Converts the given <paramref name="value"/> from the type <see cref="Point"/> to the type <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Vector2"/> result.</returns>
    public static Vector2 ToVector2(this NETPoint value) => new (value.X, value.Y);

    /// <summary>
    /// Converts the given <paramref name="value"/> from the type <see cref="Vector2"/> to the type <see cref="NETPoint"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Point"/> result.</returns>
    /// <remarks>
    ///     Converting from floating point components of a <see cref="Vector2"/> to
    ///     integer components of a <see cref="Point"/> could result in a loss of information.
    ///     Regular casting rules apply.
    /// </remarks>
    public static NETPoint ToPoint(this Vector2 value) => new ((int)value.X, (int)value.Y);

    /// <summary>
    /// Dequeues the given <paramref name="queue"/> of items until the <paramref name="untilPredicate"/> returns true.
    /// </summary>
    /// <param name="queue">The <see cref="Queue{T}"/> to process.</param>
    /// <param name="untilPredicate">Stops dequeuing the items if the <see cref="Predicate{T}"/> returns true.</param>
    /// <typeparam name="T">The type of items in the <paramref name="queue"/>.</typeparam>
    public static void DequeueWhile<T>(this Queue<T> queue, Predicate<T> untilPredicate)
    {
        var maxIterations = queue.Count + 1;
        var currentIteration = 0;

        while (currentIteration < maxIterations)
        {
            if (queue.Count <= 0)
            {
                break;
            }

            var peekedItem = queue.Peek();

            if (untilPredicate(peekedItem))
            {
                queue.Dequeue();
            }

            currentIteration += 1;
        }
    }

    /// <summary>
    /// Gets the index of the first item when the given <paramref name="predicate"/> returns true.
    /// </summary>
    /// <param name="items">The items to search.</param>
    /// <param name="predicate">The predicate to execute for each item.</param>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <returns>
    /// The positive index location of the item or <c>-1</c> if the item is not found.
    /// </returns>
    public static int IndexOf<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
        var index = -1;

        foreach (T item in items)
        {
            index++;

            if (predicate(item))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first occurence of an item that matches the given <paramref name="predicate"/> result.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="predicate">Indicates whether or not if the item index should be returned.</param>
    /// <typeparam name="T">The type of item in the list.</typeparam>
    /// <returns>The index of the item that the <paramref name="predicate"/> returned <c>true</c>.</returns>
    /// <remarks>
    ///     The iterating over the items will stop once the <paramref name="predicate"/> returns true.
    /// </remarks>
    public static int FirstItemIndex<T>(this Memory<T> items, Predicate<T> predicate)
    {
        var index = -1;

        foreach (var item in items.Span)
        {
            index++;

            if (predicate(item))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first item that matches the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="layer">The layer to check.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>.</typeparam>
    /// <returns>
    ///     The index of the item in the list of <paramref name="items"/>.
    ///     <br/>
    ///     The value of -1 will be returned if the layer cannot be found.
    /// </returns>
    public static int FirstLayerIndex<T>(this Memory<RenderItem<T>> items, int layer)
    {
        for (var i = 0; i < items.Span.Length; i++)
        {
            if (items.Span[i].Layer == layer)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Gets the total number of items that are on the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="layer">The layer to check.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>s.</typeparam>
    /// <returns>
    ///     The total number of items on the <paramref name="layer"/>.
    /// </returns>
    /// <remarks>
    ///     This method assumes that the items are sorted by layer in ascending order to work properly.
    /// </remarks>
    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Left for performance reasons.")]
    public static int TotalOnLayer<T>(this Memory<RenderItem<T>> items, int layer)
    {
        var result = 0;

        for (var i = 0; i < items.Span.Length; i++)
        {
            // If there is no point in checking the rest of the items
            if (items.Span[i].Layer > layer)
            {
                return result;
            }

            if (items.Span[i].Layer == layer)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the index of the first item that returns <c>true</c> with the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="predicate">The predicate to execute against each item.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>s.</typeparam>
    /// <returns>The index of the item.</returns>
    public static int IndexOf<T>(this Memory<RenderItem<T>> items, Predicate<T> predicate)
    {
        for (var i = 0; i < items.Span.Length; i++)
        {
            if (predicate(items.Span[i].Item))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Increases the total amount of the given <paramref name="items"/> by the given <paramref name="amount"/>.
    /// </summary>
    /// <param name="items">The items to increase its total by the given <paramref name="amount"/>.</param>
    /// <param name="amount">The amount to add to the given <paramref name="items"/>.</param>
    /// <typeparam name="T">The arbitrary data referenced by the <paramref name="items"/> of type <see cref="Memory{T}"/>.</typeparam>
    public static void IncreaseBy<T>(ref this Memory<T> items, uint amount)
    {
        var dataBackup = new Span<T>(new T[items.Length]);

        // Backup the data to not lose it
        items.Span.CopyTo(dataBackup);
        items = new T[items.Length + amount];

        // Copy the backup back to where it came from
        dataBackup.CopyTo(items.Span);
    }
}
