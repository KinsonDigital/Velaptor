// <copyright file="GlfwVideoMode.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System.Runtime.InteropServices;

/// <summary>
/// The GLFW video mode.
/// </summary>
/// <param name="Width">The width, in screen coordinates, of the GLFWVideoMode.</param>
/// <param name="Height">The height, in screen coordinates, of the GLFWVideoMode.</param>
/// <param name="RedBits">The bit depth of the red channel of the GLFWVideoMode.</param>
/// <param name="GreenBits">The bit depth of the green channel of the GLFWVideoMode.</param>
/// <param name="BlueBits">The bit depth of the blue channel of the GLFWVideoMode.</param>
/// <param name="RefreshRate">The refresh rate, in Hz, of the GLFWVideoMode.</param>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly record struct GlfwVideoMode(int Width,
                                              int Height,
                                              int RedBits,
                                              int GreenBits,
                                              int BlueBits,
                                              int RefreshRate)
{/*
    /// <summary>
    /// Gets the width, in screen coordinates, of the GLFWVideoMode.
    /// </summary>
    public int Width { get; init; } = Width;

    /// <summary>
    /// Gets the height, in screen coordinates, of the GLFWVideoMode.
    /// </summary>
    public int Height { get; init; } = Height;

    /// <summary>
    /// Gets the bit depth of the red channel of the GLFWVideoMode.
    /// </summary>
    public int RedBits { get; init; } = RedBits;

    /// <summary>
    /// Gets the bit depth of the green channel of the GLFWVideoMode.
    /// </summary>
    public int GreenBits { get; init; } = GreenBits;

    /// <summary>
    /// Gets the bit depth of the blue channel of the GLFWVideoMode.
    /// </summary>
    public int BlueBits { get; init; } = BlueBits;

    /// <summary>
    /// Gets the refresh rate, in Hz, of the GLFWVideoMode.
    /// </summary>
    public int RefreshRate { get; init; } = RefreshRate; */
}
