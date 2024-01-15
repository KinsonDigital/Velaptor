// <copyright file="IImGuiFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;

/// <summary>
/// Provides extensions to the ImGui functionality.
/// </summary>
internal interface IImGuiFacade : IDisposable
{
    /// <summary>
    /// Updates the <see cref="ImGui"/> system.
    /// </summary>
    /// <param name="timeSeconds">The current frame time in seconds.</param>
    void Update(double timeSeconds);

    /// <summary>
    /// Renders the <see cref="ImGui"/> UI controls.
    /// </summary>
    void Render();
}
