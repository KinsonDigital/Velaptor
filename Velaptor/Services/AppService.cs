// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No implementation to test")]
internal class AppService : IAppService
{
    private bool alreadyInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppService"/> class.
    /// </summary>
    public AppService() => AppDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    /// <inheritdoc/>
    public string AppDirectory { get; }

    /// <inheritdoc/>
    public bool IsDebug { get; private set; }

    /// <inheritdoc/>
    public bool TelemetryEnabled { get; private set; }

    /// <inheritdoc/>
    public string Version { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public void Init()
    {
        if (this.alreadyInitialized)
        {
            return;
        }

        var assembly = typeof(AppService).Assembly;
        var debuggable = assembly.GetCustomAttribute<DebuggableAttribute>();

        IsDebug = debuggable?.IsJITTrackingEnabled == true;

#if ENABLE_TELEMETRY
        TelemetryEnabled = true;
#else
        TelemetryEnabled = false;
#endif

        Version = assembly.
            GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "unknown";

        this.alreadyInitialized = true;
    }
}
