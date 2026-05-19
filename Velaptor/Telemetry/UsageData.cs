// <copyright file="UsageData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Velaptor.Telemetry;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Contains the data to send to the telemetry service.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Very minimal properties with no logic.")]
public record UsageData
{
    /// <summary>
    /// Gets or sets the name of the life cycle event.
    /// </summary>
    public string? LifeCycleEvent { get; set; }

    /// <summary>
    /// Gets or sets the feature area.
    /// </summary>
    public string? FeatureArea { get; set; }

    /// <summary>
    /// Gets or sets the name of the feature.
    /// </summary>
    public string? FeatureName { get; set; }

    /// <summary>
    /// Gets or sets the version of the application.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the .NET runtime.
    /// </summary>
    public string DotnetVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operating system for which the runtime was built (or on which an app is running).
    /// </summary>
    public string OsName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operating system architecture.
    /// </summary>
    public string OsArchitecture { get; set; } = string.Empty;
}
