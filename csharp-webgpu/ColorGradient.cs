// <copyright file="ColorGradient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

/// <summary>
/// Defines the type of color gradient applied to a shape.
/// </summary>
public enum ColorGradient
{
    /// <summary>
    /// No gradient — the shape is rendered with a single solid color.
    /// </summary>
    None = 0,

    /// <summary>
    /// Horizontal gradient: transitions from <c>GradientStart</c> (left) to <c>GradientStop</c> (right).
    /// </summary>
    Horizontal = 1,

    /// <summary>
    /// Vertical gradient: transitions from <c>GradientStart</c> (top) to <c>GradientStop</c> (bottom).
    /// </summary>
    Vertical = 2,
}
