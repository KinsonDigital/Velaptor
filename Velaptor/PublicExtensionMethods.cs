// <copyright file="PublicExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Drawing;
    using System.IO;
    using System.Numerics;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides extension helper methods for common game related operations.
    /// </summary>
    public static class PublicExtensionMethods
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
        // ReSharper disable once MemberCanBePrivate.Global
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
            => toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));

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
        public static Vector4 ToVector4(this Color clr) => new (clr.R, clr.G, clr.B, clr.A);

        /// <summary>
        /// Returns the position in the given <paramref name="rect"/> as a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="rect">The rect that contains the position.</param>
        /// <returns>The <see cref="RectangleF"/> position as a <see cref="Vector2"/>.</returns>
        public static Vector2 GetPosition(this RectangleF rect) => new (rect.X, rect.Y);

        /// <summary>
        /// Returns the given <paramref name="value"/> with the given <paramref name="size"/> applied.
        /// </summary>
        /// <param name="value">The value to apply the size to.</param>
        /// <param name="size">The size to apply.</param>
        /// <returns>The value after the size has been applied.</returns>
        /// <remarks>
        ///     A <paramref name="size"/> value of 1 represents 100% or the unchanged normal size of the value.
        ///     If the value of <paramref name="size"/> is 2, then the result would be the given
        ///     <paramref name="value"/> that is doubled.
        /// </remarks>
        /// <example>
        /// If the value was 3 and the size was 2, then the result would be 6.
        /// </example>
        public static float ApplySize(this uint value, float size) => value == 0 ? value : value + (value * size);

        /// <inheritdoc cref="ApplySize(uint,float)"/>
        public static float ApplySize(this float value, float size) => value == 0 ? value : value + (value * size);

        /// <summary>
        /// Returns the given <paramref name="value"/> with the given <paramref name="size"/> applied.
        /// </summary>
        /// <param name="value">The <see cref="SizeF"/> to apply the size to.</param>
        /// <param name="size">The size to apply.</param>
        /// <returns>The result after the size has been applied.</returns>
        /// <remarks>
        ///     The size will be applied to the following:
        ///     <list type="bullet">
        ///         <item><see cref="SizeF.Width"/></item>
        ///         <item><see cref="SizeF.Height"/></item>
        ///     </list>
        /// </remarks>
        public static SizeF ApplySize(this SizeF value, float size)
        {
            value.Width = value.Width.ApplySize(size);
            value.Height = value.Height.ApplySize(size);

            return value;
        }

        /// <summary>
        /// Returns the given <paramref name="value"/> with the given <paramref name="size"/> applied.
        /// </summary>
        /// <param name="value">The <see cref="RectangleF"/> to apply the size to.</param>
        /// <param name="size">The size to apply.</param>
        /// <returns>The result after the size has been applied.</returns>
        /// <remarks>
        ///     The size will be applied to the following:
        ///     <list type="bullet">
        ///         <item><see cref="RectangleF.X"/></item>
        ///         <item><see cref="RectangleF.Y"/></item>
        ///         <item><see cref="RectangleF.Width"/></item>
        ///         <item><see cref="RectangleF.Height"/></item>
        ///     </list>
        /// </remarks>
        public static RectangleF ApplySize(this RectangleF value, float size)
        {
            value.X = value.X.ApplySize(size);
            value.Y = value.Y.ApplySize(size);
            value.Width = value.Width.ApplySize(size);
            value.Height = value.Height.ApplySize(size);

            return value;
        }

        /// <summary>
        /// Returns the given <paramref name="value"/> with the given <paramref name="size"/> applied.
        /// </summary>
        /// <param name="value">The <see cref="GlyphMetrics"/> to apply the size to.</param>
        /// <param name="size">The size to apply.</param>
        /// <returns>The result after the size has been applied.</returns>
        /// <remarks>
        ///     The size will be applied to the following:
        ///     <list type="bullet">
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.GlyphBounds"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.Ascender"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.Descender"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.HorizontalAdvance"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.HoriBearingX"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.HoriBearingY"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.GlyphWidth"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.GlyphHeight"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.XMin"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.XMax"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.YMin"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.YMax"/></item>
        ///     </list>
        ///
        ///     The size will NOT be applied to the following:
        ///     <list type="bullet">
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.Glyph"/></item>
        ///         <item><see cref="GlyphMetrics"/>.<see cref="GlyphMetrics.CharIndex"/></item>
        ///     </list>
        /// </remarks>
        public static GlyphMetrics ApplySize(this GlyphMetrics value, float size)
        {
            value.GlyphBounds = value.GlyphBounds.ApplySize(size);
            value.Ascender = value.Ascender.ApplySize(size);
            value.Descender = value.Descender.ApplySize(size);
            value.HorizontalAdvance = value.HorizontalAdvance.ApplySize(size);
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

        /// <summary>
        /// Returns a value indicating if the given <see langword="string"/> <paramref name="path"/>
        /// is a fully qualified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if valid.</returns>
        /// <example>
        /// <list type="number">
        ///     <item>Value of <c>null</c> will return <c>false</c>.</item>
        ///     <item><c>null</c> or <c>empty</c> will returns <c>false</c>.</item>
        ///     <item>
        ///         Start with a drive letter
        ///         <para>Value of <c>C:\</c> will return <c>true</c>.</para>
        ///     </item>
        ///     <item>
        ///         Contain at least 1 directory
        ///         <para>will return <c>true</c>: C:\my-directory</para>
        ///     </item>
        ///     <item>
        ///         Must contain a file name with file extension
        ///         <list type="bullet">
        ///             <item>Value of <c>C:\my-directory\my-file.txt</c> will return <c>true</c>.</item>
        ///             <item>Value of <c>C:\my-directory\my-file</c> will return <c>false</c></item>
        ///             <item>Value of <c>C:\my-directory\.txt</c> will return <c>false</c></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// </example>
        public static bool IsValidFilePath(this string path)
        {
            // Must not be null or empty
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            // Must have a drive letter
            if (path.Contains(':') is false && path.NotStartsWith(':'))
            {
                return false;
            }

            // Must have a directory path
            if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                return false;
            }

            // Must have a file name with an extension
            return !string.IsNullOrEmpty(Path.GetFileName(path)) && !string.IsNullOrEmpty(Path.GetExtension(path));
        }

        /// <summary>
        /// Returns a value indicating if the given <see langword="string"/> <paramref name="path"/>
        /// is an invalid fully qualified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if invalid.</returns>
        /// <example>
        /// <list type="number">
        ///     <item>Value of <c>null</c> will return <c>true</c>.</item>
        ///     <item><c>null</c> or <c>empty</c> will returns <c>true</c>.</item>
        ///     <item>
        ///         Start with a drive letter
        ///         <para>Value of <c>C:\</c> will return <c>false</c>.</para>
        ///     </item>
        ///     <item>
        ///         Contain at least 1 directory
        ///         <para>will return <c>false</c>: C:\my-directory</para>
        ///     </item>
        ///     <item>
        ///         Must contain a file name with file extension
        ///         <list type="bullet">
        ///             <item>Value of <c>C:\my-directory\my-file.txt</c> will return <c>false</c>.</item>
        ///             <item>Value of <c>C:\my-directory\my-file</c> will return <c>true</c></item>
        ///             <item>Value of <c>C:\my-directory\.txt</c> will return <c>true</c></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// </example>
        public static bool IsInvalidFilePath(this string path) => !IsValidFilePath(path);
    }
}
