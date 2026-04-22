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
    /// <summary>
    /// Initializes a new instance of the <see cref="AppService"/> class.
    /// </summary>
    public AppService()
    {
        AppDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        IsDebug = IsInDebugMode();
        ConsumerIsDebug = IsLibraryConsumerInDebugMode();
        Version = GetVelaptorVersion();
    }

    /// <inheritdoc/>
    public string AppDirectory { get; }

    /// <inheritdoc/>
    public bool IsDebug { get; }

    /// <inheritdoc/>
    public bool ConsumerIsDebug { get; }

    /// <inheritdoc/>
    public string Version { get; }

    /// <summary>
    /// Returns a value indicating whether the consumer of the velaptor library is in debug mode.
    /// </summary>
    /// <returns>True if the consumer is in debug mode.</returns>
    private static bool IsLibraryConsumerInDebugMode()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var isDebuggableAttribute = entryAssembly?.GetCustomAttribute<DebuggableAttribute>();

        return isDebuggableAttribute?.IsJITOptimizerDisabled ?? false;
    }

    /// <summary>
    /// Returns a value indicating whether the application is running in debug mode.
    /// </summary>
    /// <returns>True if in debug mode.</returns>
    private static bool IsInDebugMode()
    {
        var assembly = typeof(AppService).Assembly;
        var debuggableAttribute = assembly.GetCustomAttribute<DebuggableAttribute>();

        return debuggableAttribute?.IsJITTrackingEnabled == true;
    }

    /// <summary>
    /// Gets the version of the Velaptor library.
    /// </summary>
    /// <returns>The version.</returns>
    private static string GetVelaptorVersion()
    {
        var assembly = typeof(AppService).Assembly;

        return assembly.
            GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "unknown";
    }
}
