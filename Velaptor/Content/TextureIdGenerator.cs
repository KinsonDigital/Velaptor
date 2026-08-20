// <copyright file="TextureIdGenerator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System.Diagnostics.CodeAnalysis;
using System.Threading;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No worth testing.")]
public class TextureIdGenerator : ITextureIdGenerator
{
    private static uint nextId = 1;

    /// <inheritdoc/>
    public uint GenerateNextId() => Interlocked.Increment(ref nextId) - 1;
}
