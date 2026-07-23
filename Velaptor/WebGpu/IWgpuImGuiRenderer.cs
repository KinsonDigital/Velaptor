// <copyright file="IWgpuImGuiRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;

/// <summary>
/// Renders ImGui draw data through the WebGPU backend.
/// </summary>
internal interface IWgpuImGuiRenderer : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the renderer has been initialized and is ready to render.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes the GPU resources (pipeline, buffers, bind group layout).
    /// Must be called after the WebGPU device is ready.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Uploads the ImGui font atlas pixel data to a GPU texture and creates
    /// the associated bind group. Must be called once after the font atlas
    /// is built and the renderer is initialized.
    /// </summary>
    /// <param name="pixels">RGBA32 pixel data from ImGui.GetTexDataAsRGBA32().</param>
    /// <param name="width">Atlas width in pixels.</param>
    /// <param name="height">Atlas height in pixels.</param>
    void CreateFontAtlasTexture(byte[] pixels, int width, int height);

    /// <summary>
    /// Renders one frame of ImGui draw data into the current render pass.
    /// </summary>
    /// <param name="drawDataPtr">
    ///     A pointer to the native <c>ImDrawData</c> structure returned by
    ///     <c>ImGui.GetDrawData()</c>.
    /// </param>
    void Render(nint drawDataPtr);
}
