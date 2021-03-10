// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;

    /// <summary>
    /// Adds basic effects to a texture when rendered.
    /// </summary>
    [Flags]
    public enum RenderEffects
    {
        /// <summary>
        /// No effects are applied.
        /// </summary>
        None = 1,

        /// <summary>
        /// The texture is flipped horizontally.
        /// </summary>
        FlipHorizontally = 2,

        /// <summary>
        /// The texture is flipped vertically.
        /// </summary>
        FlipVertically = 4,

        /// <summary>
        /// The texture is flipped horizontally and vertically.
        /// </summary>
        FlipBothDirections = FlipHorizontally | FlipVertically,
    }
}
