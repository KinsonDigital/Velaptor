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
    }
}
