// <copyright file="ITelemetryService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

/// <summary>
/// Tracks usage data for the Velaptor framework.
/// </summary>
internal interface ITelemetryService
{
    /// <summary>
    /// Tracks the start of the application.
    /// </summary>
    void TrackAppStart();

    /// <summary>
    /// Tracks what hardware the user has.
    /// </summary>
    void TrackHardware();
}
