// <copyright file="SystemMonitorService.cs" company="KinsonDigital">
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
internal sealed class SystemMonitorService : ISystemMonitorService
{
    private readonly IMonitors? monitors;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorService"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public SystemMonitorService() => this.monitors = IoC.Container.GetInstance<GLFWMonitors>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorService"/> class.
    /// </summary>
    /// <param name="monitors">Holds the list of monitors in the system.</param>
    internal SystemMonitorService(IMonitors monitors)
    {
        EnsureThat.ParamIsNotNull(monitors);
        this.monitors = monitors;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<SystemMonitor> Monitors =>
        this.monitors is null ?
            new ReadOnlyCollection<SystemMonitor>(Array.Empty<SystemMonitor>()) :
            new ReadOnlyCollection<SystemMonitor>(this.monitors.SystemMonitors);

    /// <inheritdoc/>
    public SystemMonitor MainMonitor =>
        Array.Find(this.monitors?.SystemMonitors ?? Array.Empty<SystemMonitor>(), m => m.IsMain);

    /// <inheritdoc/>
    public void Refresh() => this.monitors?.Refresh();
}
