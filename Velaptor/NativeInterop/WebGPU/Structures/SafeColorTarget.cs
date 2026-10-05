// <copyright file="SafeColorTarget.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Structures;

using Silk.NET.WebGPU;

/// <summary>
/// A safe version of <see cref="ColorTargetState"/>. The optional blend
/// state is held by value instead of by pointer.
/// </summary>
internal struct SafeColorTarget
{
    /// <summary>
    /// The swap-chain or attachment pixel format.
    /// </summary>
    public TextureFormat Format;

    /// <summary>
    /// Which color channels are written by the pipeline.
    /// </summary>
    public ColorWriteMask WriteMask;

    /// <summary>
    /// The alpha-blending configuration, or <see langword="null"/> to disable blending.
    /// </summary>
    public BlendState? Blend;
}
