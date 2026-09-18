﻿// <copyright file="IGraphicsSurface.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;
using Silk.NET.WebGPU;

/// <summary>
/// Connects an OS window to the WebGPU rendering system and manages the swap chain
/// that controls how rendered frames reach the display.
/// </summary>
internal interface IGraphicsSurface : IDisposable
{
    /// <summary>
    /// Gets the raw WebGPU surface handle — the platform-specific binding between the
    /// OS window and the WebGPU instance. Must be initialized via <see cref="Initialize"/>
    /// before use.
    /// </summary>
    SafeSurfaceHandle Handle { get; }

    /// <summary>
    /// Gets the pixel format the swap chain textures are allocated in.
    /// Populated by <see cref="InitializeFormat"/> by querying the adapter for its preference.
    /// </summary>
    TextureFormat Format { get; }

    /// <summary>
    /// Gets the physical framebuffer size in pixels.
    /// On macOS HiDPI displays, this is 2× the logical window size.
    /// On Windows, this typically matches the logical window size (1:1).
    /// </summary>
    (int X, int Y) FramebufferSize { get; }

    /// <summary>
    /// Initializes the WebGPU surface by creating the platform-specific surface handle.
    /// This must be called after the window is fully created and shown.
    /// After calling this, <see cref="Handle"/> will be available.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Queries the adapter's preferred pixel format. Call after <see cref="Initialize"/>
    /// and after the device exists, but before configuring the swap chain — this lets
    /// pipelines be built with the correct color-target format before the window size
    /// has been finalized.
    /// </summary>
    void InitializeFormat();

    /// <summary>
    /// (Re)configures the swap chain to match the current framebuffer size.
    /// Must be called once after the window reaches its final initial size and
    /// again after every window resize.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the surface was successfully configured; <c>false</c> if the
    /// framebuffer size is zero (e.g. the window is minimized) and configuration
    /// was skipped.
    /// </returns>
    bool Configure();

    /// <summary>
    /// Gets the current surface texture from the swap chain for rendering this frame.
    /// </summary>
    /// <returns>The surface texture handle for this frame.</returns>
    SafeSurfaceTextureHandle GetSurfaceTexture();
}
