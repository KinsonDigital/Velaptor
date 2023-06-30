// <copyright file="RenderingExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable MemberCanBePrivate.Global
namespace Velaptor.ExtensionMethods;

using System;
using System.Collections.Generic;
using System.Numerics;
using Graphics;
using OpenGL.Batching;

/// <summary>
/// Provides extension methods to rendering related types.
/// </summary>
internal static class RenderingExtensions
{
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
    /// Clamps all the radius values between the given <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to clamp.</param>
    /// <param name="min">The clamp minimum.</param>
    /// <param name="max">The clamp maximum.</param>
    /// <returns>The result after clamping has been applied.</returns>
    public static CornerRadius Clamp(this CornerRadius cornerRadius, float min, float max)
    {
        var topLeft = Math.Clamp(cornerRadius.TopLeft, min, max);
        var bottomLeft = Math.Clamp(cornerRadius.BottomLeft, min, max);
        var bottomRight = Math.Clamp(cornerRadius.BottomRight, min, max);
        var topRight = Math.Clamp(cornerRadius.TopRight, min, max);

        return new CornerRadius(topLeft, bottomLeft, bottomRight, topRight);
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
}
