// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Services
{
    using System.Text;

    /// <summary>
    /// Provides helper methods to be used within the <see cref="Velaptor.OpenGL.Services"/> namespace.
    /// </summary>
    public static class ExtensionMethods
    {
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

            while (result.Contains($"{trimChar}{value}"))
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

            while (result.Contains($"{value}{trimChar}"))
            {
                result.Replace($"{value}{trimChar}", value);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns a value indicating if the <see cref="StringBuilder"/> contains the given <paramref name="value"/>.
        /// </summary>
        /// <param name="builder">The string builder to check.</param>
        /// <param name="value">The value to check for.</param>
        /// <returns>
        ///     True if the <paramref name="value"/> is contained in the <see cref="StringBuilder"/>.
        /// </returns>
        private static bool Contains(this StringBuilder builder, string value) => builder.ToString().Contains(value);
    }
}
