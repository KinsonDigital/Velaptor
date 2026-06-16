// <copyright file="IImGuiManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;

/// <summary>
/// Provides ImGui functionality.
/// </summary>
internal interface IImGuiManager : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether ImGui is backed by an OpenGL rendering context.
    /// When <c>false</c>, the engine is running in WebGPU mode: ImGui state is maintained
    /// (so UI libraries such as KdGui can call ImGui freely) but draw data is never uploaded
    /// to the GPU.
    /// </summary>
    bool IsOpenGlMode { get; }

    /// <summary>
    /// Updates ImGui input and IO configuration state.
    /// </summary>
    /// <param name="timeSeconds">The current frame time in seconds.</param>
    void Update(double timeSeconds);

    /// <summary>
    /// Renders the ImGui draw list data.
    /// This method requires a <see cref="!:GraphicsDevice" /> because it may create new DeviceBuffers if the size of vertex
    /// or index data has increased beyond the capacity of the existing buffers.
    /// A <see cref="!:CommandList" /> is needed to submit drawing and resource update commands.
    /// </summary>
    void Render();
}
