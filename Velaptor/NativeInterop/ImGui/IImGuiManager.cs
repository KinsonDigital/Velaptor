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
