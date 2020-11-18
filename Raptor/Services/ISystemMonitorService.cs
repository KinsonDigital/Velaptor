// <copyright file="ISystemMonitorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using System.Collections.ObjectModel;
    using Raptor.Hardware;

    /// <summary>
    /// Gets information about the monitors in the system.
    /// </summary>
    public interface ISystemMonitorService
    {
        /// <summary>
        /// Gets a list of the monitors in the system.
        /// </summary>
        ReadOnlyCollection<SystemMonitor> Monitors { get; }

        /// <summary>
        /// Gets the main monitor in the system.
        /// </summary>
        /// <remarks>Will return null if no monitors are hooked up to the system.</remarks>
        public SystemMonitor? MainMonitor { get; }

        /// <summary>
        /// Refreshes the monitor information.
        /// </summary>
        void Refresh();
    }
}
