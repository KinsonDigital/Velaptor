// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System.Linq;

    /// <summary>
    /// Provides various helper extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns true if the character is a letter.
        /// </summary>
        /// <param name="item">The character to check.</param>
        /// <returns>True if the character is a letter.</returns>
        public static bool IsLetter(this char item) => (item >= 65 && item <= 90) || (item >= 97 && item <= 122);

        /// <summary>
        /// Returns true if the character is a number.
        /// </summary>
        /// <param name="item">The character to check.</param>
        /// <returns>True if the character is a number.</returns>
        public static bool IsNumber(this char item) => item >= 48 && item <= 57;

        /// <summary>
        /// Returns true if the string has any single digit numbers in them.
        /// </summary>
        /// <param name="item">The string to check.</param>
        /// <returns>True if the string has numbers.</returns>
        public static bool HasNumbers(this string item) => item.Any(IsNumber);

        /// <summary>
        /// Gets the first occurance of any number in the string.
        /// </summary>
        /// <param name="item">The string to check.</param>
        /// <returns>
        ///     The first occuring number.
        /// <para>
        ///     Example: With the string 'abcd7fghi', the first occuring number would be '7'.
        /// </para>
        /// </returns>
        public static int GetFirstOccurenceOfNumber(this string item)
        {
            var number = new string(item.Where(IsNumber).ToArray());

            if (string.IsNullOrEmpty(number))
            {
                return -1;
            }

            var parseSuccess = int.TryParse(number, out var result);

            return parseSuccess ? result : -1;
        }

        /// <summary>
        /// Returns true if the string contains only letters and numbers.
        /// </summary>
        /// <param name="item">The string to check.</param>
        /// <returns>True if only letters and numbers.</returns>
        public static bool IsAlphaNumeric(this string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return false;
            }

            for (var i = 0; i < item.Length; i++)
            {
                // If any of the characters are not a letter or number
                if (!item[i].IsNumber() && !item[i].IsLetter())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
