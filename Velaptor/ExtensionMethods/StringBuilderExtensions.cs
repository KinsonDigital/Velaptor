// <copyright file="StringBuilderExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System.Text;

/// <summary>
/// Provides extension methods for the <see cref="StringBuilder"/> type.
/// </summary>
internal static class StringBuilderExtensions
{
    /// <summary>
    /// Removes the last character from the string.
    /// </summary>
    /// <param name="value">The string builder to mutate.</param>
    public static void RemoveLastChar(this StringBuilder value)
    {
        if (value.Length <= 0)
        {
            return;
        }

        value.Replace(value[^1].ToString(), string.Empty, value.Length - 1, 1);
    }

    /// <summary>
    /// Gets a substring of the text in the <see cref="StringBuilder"/> starting at the given <paramref name="start"/>
    /// position and with the given <paramref name="length"/>.
    /// </summary>
    /// <param name="value">The string builder.</param>
    /// <param name="start">The starting character of the string to get.</param>
    /// <param name="length">The length of the substring to get.</param>
    /// <returns>The substring from the entire string.</returns>
    public static string Substring(this StringBuilder value, uint start, uint length)
    {
        if (start >= value.Length)
        {
            return string.Empty;
        }

        length = start + length > value.Length ? 0 : length;

        return value.ToString((int)start, (int)length);
    }

    /// <summary>
    /// Removes a character at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="value">The string builder.</param>
    /// <param name="index">The index of the character to remove.</param>
    public static void RemoveChar(this StringBuilder value, uint index)
    {
        if (index >= value.Length)
        {
            return;
        }

        value.Remove((int)index, 1);
    }

    /// <summary>
    /// Gets the index of the last character in the string.
    /// </summary>
    /// <param name="value">The string builder.</param>
    /// <returns>The index of the last character.</returns>
    /// <remarks>Returns -1 if the string has no characters.</remarks>
    public static int LastCharIndex(this StringBuilder value) => value.Length <= 0 ? -1 : value.Length - 1;

    /// <summary>
    /// Returns a value indicating whether or not the string is empty.
    /// </summary>
    /// <param name="value">The string builder.</param>
    /// <returns><c>true</c> if the string is empty.</returns>
    public static bool IsEmpty(this StringBuilder value) => value.Length <= 0;
}
