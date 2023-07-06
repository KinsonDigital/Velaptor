// <copyright file="ViewPortSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds the size of the viewport.
/// </summary>
internal readonly record struct ViewPortSizeData
{
    /// <summary>
    /// Gets the width of the viewport.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// Gets the height of the viewport.
    /// </summary>
    public uint Height { get; init; }
}
