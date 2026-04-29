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
        InDevelopmentEnvironment = IsInDeveloperEnvironment();
    }

    /// <inheritdoc/>
    public string AppDirectory { get; }

    /// <inheritdoc/>
    public bool IsDebug { get; }

    /// <inheritdoc/>
    public bool ConsumerIsDebug { get; }

    /// <inheritdoc/>
    public string Version { get; }

    /// <inheritdoc/>
    public bool InDevelopmentEnvironment { get; }

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

    /// <summary>
    /// Returns <c>true</c> if the process appears to be running inside a developer's project.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Primary signal: walk ancestor directories of the running executable looking for
    /// <c>*.csproj</c> or <c>*.sln</c> files.  These only exist in a development
    /// environment — a game that has been published and distributed to players will never
    /// have project files anywhere in its directory tree.
    /// </para>
    /// <para>
    /// Fallback signal: check whether the .NET SDK is installed on this machine.
    /// Developers must have the SDK installed to build games; players typically only have
    /// the .NET Runtime or run a self-contained game bundle.
    /// </para>
    /// </remarks>
    private static bool IsInDeveloperEnvironment()
    {
#if ENABLE_TELEMETRY
        try
        {
            var processPath = Environment.ProcessPath ?? string.Empty;
            var sep = Path.DirectorySeparatorChar;

            // Fast exit path: .NET always builds to bin/Debug/ or bin/Release/.
            // A distributed game will never have these in its path.
            if (processPath.Contains($"{sep}bin{sep}Debug{sep}", StringComparison.OrdinalIgnoreCase) ||
                processPath.Contains($"{sep}bin{sep}Release{sep}", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var executableDir = Path.GetDirectoryName(processPath) ?? string.Empty;
            var root = Path.GetPathRoot(executableDir) ?? string.Empty;
            var dir = executableDir;

            while (!string.IsNullOrEmpty(dir) && !string.Equals(dir, root, StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.GetFiles(dir, "*.csproj", SearchOption.TopDirectoryOnly).Length > 0 ||
                    Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).Length > 0)
                {
                    return true;
                }

                var parent = Path.GetDirectoryName(dir);
                if (parent is null || string.Equals(parent, dir, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                dir = parent;
            }

            // No project files found anywhere in the directory tree — this is a distributed game, not a development run.
            return false;
        }
        catch
        {
            return true;
        }
#else
        return false;
#endif
    }
}
