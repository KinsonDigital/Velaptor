// <copyright file="IRenderContext.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;

/// <summary>
/// Represents where and how rendering will occur.
/// </summary>
public interface IRenderContext
{
    /// <summary>
    /// Gets the current size of the rendering area.
    /// </summary>
    /// <returns>The render area size.</returns>
    /// <remarks>This is synonymous with the OpenGL viewport.</remarks>
    Size GetRenderAreaSize();

    /// <summary>
    /// Sets the size of the rendering area.
    /// </summary>
    /// <param name="width">The width in pixels of the area.</param>
    /// <param name="height">The height in pixels of the area.</param>
    /// <remarks>This is synonymous with the OpenGL viewport.</remarks>
    void SetRenderAreaSize(int width, int height);
}
