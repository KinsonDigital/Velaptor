// <copyright file="FontAtlasMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides metric data for a single font atlas texture.
/// </summary>
internal readonly record struct FontAtlasMetrics
{
    /// <summary>
    /// Gets the number of rows in the atlas.
    /// </summary>
    public uint Rows { get; init; }

    /// <summary>
    /// Gets the number of columns in the atlas.
    /// </summary>
    public uint Columns { get; init; }

    /// <summary>
    /// Gets the width of the atlas.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// Gets the height of the atlas.
    /// </summary>
    public uint Height { get; init; }
}
