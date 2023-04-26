// <copyright file="BatchItemFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;

/// <summary>
/// Creates batch items instances to ease the process of creating and maintaining unit tests.
/// </summary>
[SuppressMessage("csharpsquid", "S107", Justification = "Not required for unit testing.")]
internal static class BatchItemFactory
{
    public static TextureBatchItem CreateTextureItem(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        float size = 0f,
        float angle = 0f,
        Color clr = default,
        RenderEffects effects = RenderEffects.None,
        uint textureId = 0) =>
        new (srcRect,
            destRect,
            size,
            angle,
            clr,
            effects,
            textureId);

    /// <summary>
    /// Creates a new instance of a <see cref="TextureBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static TextureBatchItem CreateTextureItemWithOrderedValues(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        float size = 0f,
        float angle = 0f,
        Color clr = default,
        RenderEffects effects = RenderEffects.None,
        int textureId = 0) =>
            new (srcRect == default ? new RectangleF(1, 2, 3, 4) : srcRect,
                destRect == default ? new RectangleF(5, 6, 7, 8) : destRect,
                size == 0 ? 9 : size,
                angle == 0 ? 10 : angle,
                clr == default ? Color.FromArgb(11, 12, 13, 14) : clr,
                effects == RenderEffects.None ? RenderEffects.FlipHorizontally : effects,
                (uint)textureId == 0 ? 15u : (uint)textureId);

    /// <summary>
    /// Creates new instances of a <see cref="TextureBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static TextureBatchItem[] CreateTextureItemsWithOrderedValues(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        float size = 0f,
        float angle = 0f,
        Color clr = default,
        RenderEffects effects = RenderEffects.None,
        int textureId = 0,
        int totalItems = 1)
    {
        var result = new List<TextureBatchItem>();

        for (var i = 0; i < totalItems; i++)
        {
            result.Add(CreateTextureItemWithOrderedValues(
                srcRect,
                destRect,
                size,
                angle,
                clr,
                effects,
                textureId));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a new instance of a <see cref="FontGlyphBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static FontGlyphBatchItem CreateFontItemWithOrderedValues(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        char glyph = '\0',
        float size = 0f,
        float angle = 0f,
        Color tintColor = default,
        RenderEffects effects = RenderEffects.None,
        int textureId = 0) =>
            new (srcRect == default ? new RectangleF(1, 2, 3, 4) : srcRect,
                destRect == default ? new RectangleF(5, 6, 7, 8) : destRect,
                glyph == '\0' ? 'V' : glyph,
                size == 0 ? 9 : size,
                angle == 0 ? 10 : angle,
                tintColor == default ? Color.FromArgb(11, 12, 13, 14) : tintColor,
                effects == RenderEffects.None ? RenderEffects.FlipHorizontally : effects,
                (uint)textureId == 0 ? 15u : (uint)textureId);

    /// <summary>
    /// Creates new instances of a <see cref="FontGlyphBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static FontGlyphBatchItem[] CreateFontItemsWithOrderedValues(
        RectangleF srcRect = default,
        RectangleF destRect = default,
        char glyph = '\0',
        float size = 0f,
        float angle = 0f,
        Color tintColor = default,
        RenderEffects effects = RenderEffects.None,
        int textureId = 0,
        int totalItems = 1)
    {
        var result = new List<FontGlyphBatchItem>();

        for (var i = 0; i < totalItems; i++)
        {
            result.Add(CreateFontItemWithOrderedValues(
                srcRect,
                destRect,
                glyph,
                size,
                angle,
                tintColor,
                effects,
                textureId));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a new instance of a <see cref="ShapeBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static ShapeBatchItem CreateRectItemWithOrderedValues(
        Vector2 position = default,
        float width = 0f,
        float height = 0f,
        Color color = default,
        bool isSolid = false,
        float borderThickness = 0f,
        CornerRadius cornerRadius = default,
        ColorGradient gradientType = ColorGradient.None,
        Color gradientStart = default,
        Color gradientStop = default) =>
            new (position == default ? new Vector2(1, 2) : position,
                width == 0 ? 3 : width,
                height == 0 ? 4 : height,
                color == default ? Color.FromArgb(5, 6, 7, 8) : color,
                isSolid,
                borderThickness == 0 ? 9 : borderThickness,
                cornerRadius == default ? new CornerRadius(10, 11, 12, 13) : cornerRadius,
                gradientType == ColorGradient.None ? ColorGradient.Vertical : gradientType,
                gradientStart == default ? Color.FromArgb(14, 15, 16, 17) : gradientStart,
                gradientStop == default ? Color.FromArgb(18, 19, 20, 21) : gradientStop);

    /// <summary>
    /// Creates new instances of a <see cref="ShapeBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static ShapeBatchItem[] CreateRectItemsWithOrderedValues(
        Vector2 position = default,
        float width = 0f,
        float height = 0f,
        Color color = default,
        bool isSolid = false,
        float borderThickness = 0f,
        CornerRadius cornerRadius = default,
        ColorGradient gradientType = ColorGradient.None,
        Color gradientStart = default,
        Color gradientStop = default,
        int totalItems = 1)
    {
        var result = new List<ShapeBatchItem>();

        for (var i = 0; i < totalItems; i++)
        {
            result.Add(CreateRectItemWithOrderedValues(
                position,
                width,
                height,
                color,
                isSolid,
                borderThickness,
                cornerRadius,
                gradientType,
                gradientStart,
                gradientStop));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a new instance of <see cref="LineBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static LineBatchItem CreateLineItemWithOrderedValues(
        Vector2 p1 = default,
        Vector2 p2 = default,
        Color color = default,
        float thickness = 0) =>
            new (p1 == default ? new Vector2(1, 2) : p1,
                p2 == default ? new Vector2(3, 4) : p2,
                color == default ? Color.FromArgb(5, 6, 7, 8) : color,
                thickness == 0 ? 9 : thickness);

    /// <summary>
    /// Creates new instances of a <see cref="LineBatchItem"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1611:Element parameters should be documented",
        Justification = "Not required for unit testing.")]
    public static LineBatchItem[] CreateLineItemsWithOrderedValues(
        Vector2 p1 = default,
        Vector2 p2 = default,
        Color color = default,
        float thickness = 0,
        int totalItems = 1)
    {
        var result = new List<LineBatchItem>();

        for (var i = 0; i < totalItems; i++)
        {
            result.Add(CreateLineItemWithOrderedValues(
                p1,
                p2,
                color,
                thickness));
        }

        return result.ToArray();
    }
}
