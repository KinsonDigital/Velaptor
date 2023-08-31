// <copyright file="SystemDisplayService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Guards;
using Hardware;
using NativeInterop.GLFW;

/// <summary>
/// Gets information about all of the monitors in the system.
/// </summary>
internal sealed class SystemDisplayService : ISystemDisplayService
{
    private readonly IDisplays? displays;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayService"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public SystemDisplayService() => this.displays = IoC.Container.GetInstance<GlfwDisplays>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayService"/> class.
    /// </summary>
    /// <param name="displays">Holds the list of monitors in the system.</param>
    internal SystemDisplayService(IDisplays displays)
    {
        EnsureThat.ParamIsNotNull(displays);
        this.displays = displays;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<SystemDisplay> Displays =>
        this.displays is null ?
            new ReadOnlyCollection<SystemDisplay>(Array.Empty<SystemDisplay>()) :
            new ReadOnlyCollection<SystemDisplay>(this.displays.SystemDisplays);

    /// <inheritdoc/>
    public SystemDisplay MainDisplay =>
        Array.Find(this.displays?.SystemDisplays ?? Array.Empty<SystemDisplay>(), m => m.IsMain);

    /// <inheritdoc/>
    public void Refresh() => this.displays?.Refresh();
}
