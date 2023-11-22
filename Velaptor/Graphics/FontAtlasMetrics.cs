// <copyright file="FontAtlasMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

/// <summary>
/// Provides metric data for a single font atlas texture.
/// <param name="Rows">Number of rows in the atlas.</param>
/// <param name="Columns">Number of columns in the atlas.</param>
/// <param name="Width">Width of the atlas.</param>
/// <param name="Height">Height of the atlas.</param>
/// </summary>
internal readonly record struct FontAtlasMetrics(uint Rows, uint Columns, uint Width, uint Height);
