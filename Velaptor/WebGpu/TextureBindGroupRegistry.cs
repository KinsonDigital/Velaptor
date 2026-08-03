// <copyright file="TextureBindGroupRegistry.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Central registry mapping texture IDs to their WebGPU bind groups, so renderers
/// can resolve <c>textureId → bindGroup</c> without holding references to
/// <see cref="Content.ITexture"/> objects.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Nothing worth testing.")]
internal sealed class TextureBindGroupRegistry
{
    private readonly Dictionary<uint, SafeBindGroupHandle> bindGroups = new ();

    /// <summary>
    /// Associates <paramref name="bindGroup"/> with <paramref name="textureId"/>.
    /// </summary>
    /// <param name="textureId">The unique texture identifier.</param>
    /// <param name="bindGroup">The bind group containing the texture view and sampler.</param>
    public void Register(uint textureId, SafeBindGroupHandle bindGroup) => this.bindGroups[textureId] = bindGroup;

    /// <summary>
    /// Removes the bind group registration for <paramref name="textureId"/>.
    /// </summary>
    /// <param name="textureId">The unique texture identifier.</param>
    public void Unregister(uint textureId) => this.bindGroups.Remove(textureId);

    /// <summary>
    /// Returns the bind group associated with <paramref name="textureId"/>, or
    /// <c>null</c> if no registration exists.
    /// </summary>
    /// <param name="textureId">The unique texture identifier.</param>
    /// <returns>The bind group, or <c>null</c>.</returns>
    public SafeBindGroupHandle? GetBindGroup(uint textureId) =>
        this.bindGroups.TryGetValue(textureId, out var handle) ? handle : null;
}
