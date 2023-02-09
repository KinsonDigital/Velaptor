// <copyright file="SizeU.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

/// <summary>
/// Stores an ordered pair of <see langword="unsigned"/> integers, which specify a <see cref="Width"/> and <see cref="Height"/>.
/// </summary>
public readonly record struct SizeU
{
#pragma warning disable SA1642
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeU"/> struct.
    /// </summary>
    /// <param name="width">The width of the size.</param>
    /// <param name="height">The height of the size.</param>
#pragma warning restore SA1642
    public SizeU(uint width, uint height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the width of the size.
    /// </summary>
    public uint Width { get; init; }

    /// <summary>
    /// Gets the height of the size.
    /// </summary>
    public uint Height { get; init; }
}
