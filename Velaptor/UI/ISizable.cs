// <copyright file="ISizable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

/// <summary>
/// Represents the size of an object by its width and height.
/// </summary>
public interface ISizable
{
    /// <summary>
    /// Gets the width of the <see cref="IControl"/>.
    /// </summary>
    uint Width { get; }

    /// <summary>
    /// Gets the half width of the <see cref="IControl"/>.
    /// </summary>
    uint HalfWidth { get; }

    /// <summary>
    /// Gets the height of the <see cref="IControl"/>.
    /// </summary>
    uint Height { get; }

    /// <summary>
    /// Gets the half height of the <see cref="IControl"/>.
    /// </summary>
    uint HalfHeight { get; }
}
