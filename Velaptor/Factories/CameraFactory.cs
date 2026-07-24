// <copyright file="CameraFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Creates camera instances for applying pan and zoom transforms to rendered objects.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
public static class CameraFactory
{
    /// <summary>
    /// Creates a 2-D camera instance that applies pan and zoom transforms on the CPU.
    /// </summary>
    /// <returns>The camera instance.</returns>
    /// <remarks>
    /// <para>
    /// The camera transforms world pixel coordinates to screen pixel coordinates.
    /// Call <see cref="ICamera2D.TransformPosition"/> and <see cref="ICamera2D.TransformSize"/>
    /// each frame when building render data.
    /// </para>
    /// <para>
    /// The returned instance is a singleton — repeated calls return the same camera.
    /// </para>
    /// </remarks>
    public static ICamera2D CreateCamera() => IoC.Container.GetInstance<ICamera2D>();
}
