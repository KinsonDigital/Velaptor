// <copyright file="SizeU.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Stores an ordered pair of <see langword="unsigned"/> integers, which specify a <see cref="Width"/> and <see cref="Height"/>.
/// </summary>
public struct SizeU : IEquatable<SizeU>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeU"/> struct.
    /// </summary>
    /// <param name="width">The width of the size.</param>
    /// <param name="height">The height of the size.</param>
    public SizeU(uint width, uint height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets or sets the width of the size.
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the size.
    /// </summary>
    public uint Height { get; set; }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns>True if both operators are equal.</returns>
    public static bool operator ==(SizeU left, SizeU right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns>True if both operators are not equal.</returns>
    public static bool operator !=(SizeU left, SizeU right) => !(left == right);

    /// <inheritdoc/>
    public bool Equals(SizeU other) => Width == other.Width && Height == other.Height;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SizeU size && Equals(size);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => HashCode.Combine(Width, Height);
}
