// <copyright file="ServiceEnums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Represents the different types of batching services.
/// </summary>
internal enum BatchServiceType
{
    /// <summary>
    /// A texture batching service.
    /// </summary>
    Texture = 1,

    /// <summary>
    /// A font glyph batching service.
    /// </summary>
    FontGlyph = 2,

    /// <summary>
    /// A rectangle batching service.
    /// </summary>
    Rectangle = 3,
}
