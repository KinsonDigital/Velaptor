// <copyright file="RectGPUDataGenerator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Velaptor.OpenGL.GPUData;

/// <summary>
/// Helps generate <see cref="RectVertexData"/> easily for the purpose of testing.
/// </summary>
[SuppressMessage("csharpsquid", "S101", Justification = "Class naming is fine.")]
internal static class RectGPUDataGenerator
{
    /// <summary>
    /// Generates a <see cref="RectGPUData"/> instance with the component values beginning at <paramref name="start"/> value
    /// with each component getting larger by 1.
    /// </summary>
    /// <param name="start">The start value of the component values.</param>
    /// <returns>The instance used for testing.</returns>
    public static RectGPUData GenerateGPUData(float start)
    {
        var vertex1 = GenerateVertexData(start, out var next);
        var vertex2 = GenerateVertexData(next, out next);
        var vertex3 = GenerateVertexData(next, out next);
        var vertex4 = GenerateVertexData(next, out next);

        return new RectGPUData(vertex1, vertex2, vertex3, vertex4);
    }

    /// <summary>
    /// Generates a <see cref="RectVertexData"/> instance with the component values beginning at the given <paramref name="start"/> value
    /// with each component getting larger by 1.
    /// </summary>
    /// <param name="start">The start value of the component values.</param>
    /// <param name="next">The next value available after all values have been used relative to the given <paramref name="start"/> value.</param>
    /// <returns>The instance used for testing.</returns>
    public static RectVertexData GenerateVertexData(float start, out float next)
    {
        var pos = GenerateVector2(start, out next);
        var rect = GenerateVector4(next, out next);
        var color = GenerateColor((byte)next, out var nextByte);
        next = nextByte;

        const bool isFilled = true;

        var borderThicknessRadius = next;

        next += 1;
        var topLeftRadius = next;
        next += 1;
        var bottomLeftRadius = next;
        next += 1;
        var bottomRightRadius = next;
        next += 1;
        var topRightRadius = next;

        next += 1;
        return new RectVertexData(
            pos,
            rect,
            color,
            isFilled,
            borderThicknessRadius,
            topLeftRadius,
            bottomLeftRadius,
            bottomRightRadius,
            topRightRadius);
    }

    /// <summary>
    /// Generates a <see cref="Vector2"/> instance with the component values beginning at the given <paramref name="start"/> value
    /// with each component getting larger by 1.
    /// </summary>
    /// <param name="start">The start value of the component values.</param>
    /// <param name="next">The next value available after all values have been used relative to the given <paramref name="start"/> value.</param>
    /// <returns>A <see cref="Vector2"/> instance for testing.</returns>
    /// <remarks>
    ///     Example:
    ///     <para>
    ///         If the given <paramref name="start"/> value is four, then the the vector components would be the values below:
    ///         <list type="number">
    ///             <item>
    ///                 <see cref="Vector2"/>.<see cref="Vector2.X"/> = 4
    ///             </item>
    ///             <item>
    ///                 <see cref="Vector2"/>.<see cref="Vector2.Y"/> = 5
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    private static Vector2 GenerateVector2(float start, out float next)
    {
        next = start + 2;
        return new Vector2(start, start + 1);
    }

    /// <summary>
    /// Generates a <see cref="Vector4"/> instance with the component values beginning at the
    /// <paramref name="start"/> value with each component getting larger by 1.
    /// </summary>
    /// <param name="start">The start value of the component values.</param>
    /// <param name="next">The next value available after all values have been used relative to the given <paramref name="start"/> value.</param>
    /// <returns>A <see cref="Vector4"/> instance for testing.</returns>
    /// <remarks>
    ///     Example:
    ///     <para>
    ///         If the given <paramref name="start"/> value is four, then the the vector components would be the values below:
    ///         <list type="number">
    ///             <item>
    ///                 <see cref="Vector4"/>.<see cref="Vector4.X"/> = 4
    ///             </item>
    ///             <item>
    ///                 <see cref="Vector4"/>.<see cref="Vector4.Y"/> = 5
    ///             </item>
    ///             <item>
    ///                 <see cref="Vector4"/>.<see cref="Vector4.Z"/> = 6
    ///             </item>
    ///             <item>
    ///                 <see cref="Vector4"/>.<see cref="Vector4.W"/> = 7
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    private static Vector4 GenerateVector4(float start, out float next)
    {
        next = start + 4;
        return new Vector4(start, start + 1, start + 2, start + 3);
    }

    /// <summary>
    /// Generates a color instance with the component values beginning at the given <paramref name="start"/> value
    /// with each component getting larger by 1.
    /// </summary>
    /// <param name="start">The start value of the component values.</param>
    /// <param name="next">The next value available after all values have been used relative to the given <paramref name="start"/> value.</param>
    /// <returns>A <see cref="Color"/> instance for testing.</returns>
    /// <remarks>
    ///     Example:
    ///     <para>
    ///         If the given <paramref name="start"/> value is four, than the the color components would be the values below:
    ///         <list type="number">
    ///             <item>
    ///                 <see cref="Color"/>.<see cref="Color.A"/> = 4
    ///             </item>
    ///             <item>
    ///                 <see cref="Color"/>.<see cref="Color.R"/> = 5
    ///             </item>
    ///             <item>
    ///                 <see cref="Color"/>.<see cref="Color.G"/> = 6
    ///             </item>
    ///             <item>
    ///                 <see cref="Color"/>.<see cref="Color.B"/> = 7
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    private static Color GenerateColor(byte start, out byte next)
    {
        next = (byte)(start + 4);
        return Color.FromArgb(start, start + 1, start + 2, start + 3);
    }
}
