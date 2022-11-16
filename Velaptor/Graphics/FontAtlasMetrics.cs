// <copyright file="FontAtlasMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Velaptor.Graphics;

/// <summary>
/// Provides metric data for a single font atlas texture.
/// </summary>
internal struct FontAtlasMetrics
{
    /// <summary>
    /// The number of rows in the atlas.
    /// </summary>
    public uint Rows;

    /// <summary>
    /// The number of columns in the atlas.
    /// </summary>
    public uint Columns;

    /// <summary>
    /// The width of the atlas.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The height of the atlas.
    /// </summary>
    public uint Height;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FontAtlasMetrics other &&
                                                this.Rows == other.Rows &&
                                                this.Columns == other.Columns &&
                                                this.Width == other.Width &&
                                                this.Height == other.Height;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => HashCode.Combine(this.Rows, this.Columns, this.Width, this.Height);
}
