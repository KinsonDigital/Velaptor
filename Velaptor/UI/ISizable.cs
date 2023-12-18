// <copyright file="ISizable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;

/// <summary>
/// Represents the size of an object by its width and height.
/// </summary>
[Obsolete("This interface is deprecated and will be removed in a future release.")]
public interface ISizable
{
    /// <summary>
    /// Gets the width of the <see cref="IControl"/>.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    uint Width { get; }

    /// <summary>
    /// Gets the half width of the <see cref="IControl"/>.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    uint HalfWidth { get; }

    /// <summary>
    /// Gets the height of the <see cref="IControl"/>.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    uint Height { get; }

    /// <summary>
    /// Gets the half height of the <see cref="IControl"/>.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    uint HalfHeight { get; }
}
