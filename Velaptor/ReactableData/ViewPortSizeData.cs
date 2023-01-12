// <copyright file="ViewPortSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds the size of the viewport.
/// </summary>
internal sealed record ViewPortSizeData
{
    /// <summary>
    /// Gets or sets the width of the viewport.
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the viewport.
    /// </summary>
    public uint Height { get; set; }
}
