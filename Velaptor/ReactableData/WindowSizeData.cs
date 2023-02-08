// <copyright file="WindowSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds the size of the window.
/// </summary>
internal readonly record struct WindowSizeData
{
    /// <summary>
    /// Gets the width of the window.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// Gets the height of the window.
    /// </summary>
    public uint Height { get; init; }
}
