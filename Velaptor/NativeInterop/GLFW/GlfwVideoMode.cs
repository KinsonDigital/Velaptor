// <copyright file="GlfwVideoMode.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System.Runtime.InteropServices;

/// <summary>
/// The GLFW video mode.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly record struct GlfwVideoMode
{
    /// <summary>
    /// Gets the width, in screen coordinates, of the GLFWVideoMode.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// Gets the height, in screen coordinates, of the GLFWVideoMode.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// Gets the bit depth of the red channel of the GLFWVideoMode.
    /// </summary>
    public int RedBits { get; init; }

    /// <summary>
    /// Gets the bit depth of the green channel of the GLFWVideoMode.
    /// </summary>
    public int GreenBits { get; init; }

    /// <summary>
    /// Gets the bit depth of the blue channel of the GLFWVideoMode.
    /// </summary>
    public int BlueBits { get; init; }

    /// <summary>
    /// Gets the refresh rate, in Hz, of the GLFWVideoMode.
    /// </summary>
    public int RefreshRate { get; init; }
}
