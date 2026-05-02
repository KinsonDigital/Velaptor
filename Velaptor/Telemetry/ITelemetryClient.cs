// <copyright file="ITelemetryClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

using System.Threading.Tasks;

/// <summary>
/// Communicates with the telemetry server to send usage data.
/// </summary>
internal interface ITelemetryClient
{
    /// <summary>
    /// Tracks an event.
    /// </summary>
    /// <param name="jsonPayload">The JSON data to send.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation of sending the telemetry data.
    /// </returns>
    Task TrackEvent(string jsonPayload);

    /// <summary>
    /// Tracks hardware information.
    /// </summary>
    /// <param name="jsonPayload">The JSON data to send.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation of sending the telemetry data.
    /// </returns>
    Task TrackHardware(string jsonPayload);
}
