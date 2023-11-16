// <copyright file="FontAtlasMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides metric data for a single font atlas texture.
/// <param name="Rows">Number of rows in the atlas.</param>
/// <param name="Columns">Number of columns in the atlas.</param>
/// <param name="Width">Width of the atlas.</param>
/// <param name="Height">Height of the atlas.</param>
/// </summary>
internal readonly record struct FontAtlasMetrics(uint Rows, uint Columns, uint Width, uint Height)
{
    /// <summary>
    /// Gets the number of rows in the atlas.
    /// </summary>
    public uint Rows { get; init; } = Rows;

    /// <summary>
    /// Gets the number of columns in the atlas.
    /// </summary>
    public uint Columns { get; init; } = Columns;

    /// <summary>
    /// Gets the width of the atlas.
    /// </summary>
    public uint Width { get; init; } = Width;

    /// <summary>
    /// Gets the height of the atlas.
    /// </summary>
    public uint Height { get; init; } = Height;
}
