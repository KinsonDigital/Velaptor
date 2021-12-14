// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Drawing;
using Velaptor.Graphics;

namespace Velaptor.Content
{
    using System.Linq;

    /// <summary>
    /// Provides various helper extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns <see langword="true"/> if the character is a letter.
        /// </summary>
        /// <param name="item">The character to check.</param>
        /// <returns><see langword="true"/> if the character is a letter.</returns>
        public static bool IsLetter(this char item) => (item >= 65 && item <= 90) || (item >= 97 && item <= 122);

        /// <summary>
        /// Returns <see langword="true"/> if the character is a number.
        /// </summary>
        /// <param name="item">The character to check.</param>
        /// <returns><see langword="true"/> if the character is a number.</returns>
        public static bool IsNumber(this char item) => item >= 48 && item <= 57;

        /// <summary>
        /// Returns <see langword="true"/> if the string has any single digit numbers in them.
        /// </summary>
        /// <param name="item">The string to check.</param>
        /// <returns><see langword="true"/> if the string has numbers.</returns>
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

            var parseSuccess = int.TryParse(number, out var result);

            if (parseSuccess)
            {
                return result;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the string contains only letters and numbers.
        /// </summary>
        /// <param name="item">The string to check.</param>
        /// <returns><see langword="true"/> if only letters and numbers.</returns>
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

        public static float ApplySize(this uint value, float size)
        {
            return value + (value * size);
        }

        // TODO: Code docs
        public static float ApplySize(this float value, float size) => value + (value * size);

        public static SizeF ApplySize(this SizeF value, float size)
        {
            value.Width = value.Width.ApplySize(size);
            value.Height = value.Height.ApplySize(size);

            return value;
        }

        public static RectangleF ApplySize(this RectangleF value, float size)
        {
            value.X = value.X.ApplySize(size);
            value.Y = value.Y.ApplySize(size);
            value.Width = value.Width.ApplySize(size);
            value.Height = value.Height.ApplySize(size);

            return value;
        }

        public static GlyphMetrics ApplySize(this GlyphMetrics value, float size)
        {
            value.GlyphBounds = value.GlyphBounds.ApplySize(size);
            value.Ascender = value.Ascender.ApplySize(size);
            value.Descender = value.Descender.ApplySize(size);
            value.HoriBearingX = value.HoriBearingX.ApplySize(size);
            value.HoriBearingY = value.HoriBearingY.ApplySize(size);
            value.GlyphWidth = value.GlyphWidth.ApplySize(size);
            value.GlyphHeight = value.GlyphHeight.ApplySize(size);
            value.XMin = value.XMin.ApplySize(size);
            value.XMax = value.XMax.ApplySize(size);
            value.YMin = value.YMin.ApplySize(size);
            value.YMax = value.YMax.ApplySize(size);

            return value;
        }
    }
}
