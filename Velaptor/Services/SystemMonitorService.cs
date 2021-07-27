// <copyright file="SystemMonitorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Velaptor.Hardware;
    using Velaptor.OpenGL;

    /// <summary>
    /// Gets information about the monitors in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SystemMonitorService : ISystemMonitorService
    {
        private readonly GLFWMonitors? monitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitorService"/> class.
        /// </summary>
        public SystemMonitorService() => this.monitors = IoC.Container.GetInstance<GLFWMonitors>();

        /// <inheritdoc/>
        public ReadOnlyCollection<SystemMonitor> Monitors
        {
            get
            {
                if (this.monitors is null)
                {
                    return new ReadOnlyCollection<SystemMonitor>(Array.Empty<SystemMonitor>());
                }

                return new ReadOnlyCollection<SystemMonitor>(this.monitors.SystemMonitors);
            }
        }

        /// <inheritdoc/>
        public SystemMonitor? MainMonitor
        {
            get
            {
                if (this.monitors is null)
                {
                    return null;
                }

                return this.monitors.SystemMonitors.Where(m => m.IsMain).FirstOrDefault();
            }
        }

        /// <inheritdoc/>
        public void Refresh() => this.monitors?.Refresh();
    }
}
