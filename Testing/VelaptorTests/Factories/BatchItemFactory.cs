// <copyright file="BatchItemFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Factories;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;

/// <summary>
/// Creates batch items instances to ease the process of creating and maintaining unit tests.
/// </summary>
internal static class BatchItemFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="TextureBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static TextureBatchItem CreateTextureBatchItem(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        float size = 0f,
        float angle = 0f,
        Color clr = default,
        RenderEffects effects = RenderEffects.None,
        int textureId = 0)
    {
        var result = new TextureBatchItem(
            srcRect,
            destRect,
            size,
            angle,
            clr,
            effects,
            (uint)textureId);

        return result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="RectBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static RectBatchItem CreateRectBatchItem(
        Vector2 position = default,
        float width = 0f,
        float height = 0f,
        Color color = default,
        bool isFilled = false,
        float borderThickness = 0f,
        CornerRadius cornerRadius = default,
        ColorGradient gradientType = ColorGradient.None,
        Color gradientStart = default,
        Color gradientStop = default)
    {
        return new RectBatchItem(
            position == default ? new Vector2(1, 2) : position,
            width == 0 ? 3 : width,
            height == 0 ? 4 : height,
            color == default ? Color.FromArgb(5, 6, 7, 8) : color,
            isFilled,
            borderThickness == 0 ? 9 : borderThickness,
            cornerRadius == default ? new CornerRadius(10, 11, 12, 13) : cornerRadius,
            gradientType == ColorGradient.None ? ColorGradient.Vertical : gradientType,
            gradientStart == default ? Color.FromArgb(14, 15, 16, 17) : gradientStart,
            gradientStop == default ? Color.FromArgb(18, 19, 20, 21) : gradientStop);
    }
}
