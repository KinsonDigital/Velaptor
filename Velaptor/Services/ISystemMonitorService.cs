// <copyright file="ISystemMonitorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.ObjectModel;
    using Velaptor.Hardware;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Gets information about all of the monitors in the system.
    /// </summary>
    public interface ISystemMonitorService
    {
        /// <summary>
        /// Gets a list of all of the monitors in the system.
        /// </summary>
        ReadOnlyCollection<SystemMonitor> Monitors { get; }

        /// <summary>
        /// Gets the primary monitor in the system.
        /// </summary>
        /// <remarks>Will return null if no monitors are hooked up to the system.</remarks>
        public SystemMonitor? MainMonitor { get; }

        /// <summary>
        /// Refreshes the monitor information.
        /// </summary>
        void Refresh();
    }
}
