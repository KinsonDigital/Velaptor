// <copyright file="SystemMonitorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Hardware;
    using Raptor.OpenGL;

    /// <summary>
    /// Gets information about the monitors in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SystemMonitorService : ISystemMonitorService
    {
        private readonly GLFWMonitors monitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitorService"/> class.
        /// </summary>
        public SystemMonitorService() => this.monitors = IoC.Container.GetInstance<GLFWMonitors>();

        /// <inheritdoc/>
        public ReadOnlyCollection<SystemMonitor> Monitors => new ReadOnlyCollection<SystemMonitor>(this.monitors.SystemMonitors);

        /// <inheritdoc/>
        public void Refresh() => this.monitors.Refresh();
    }
}
