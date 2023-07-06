// <copyright file="StringExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Provides extension methods for the <see cref="string"/> type.
/// </summary>
internal static class StringExtensions
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
}
