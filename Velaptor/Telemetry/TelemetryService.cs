// <copyright file="TelemetryService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

#if DEBUG && ENABLE_TELEMETRY
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
#endif

using System;
using System.Diagnostics.CodeAnalysis;
using Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal class TelemetryService : ITelemetryService
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ITelemetryClient telemetryClient;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IAppService appService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryService"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    /// <param name="telemetryClient">Communicates with the telemetry server.</param>
    public TelemetryService(IAppService appService, ITelemetryClient telemetryClient)
    {
        this.appService = appService;
        this.telemetryClient = telemetryClient;

        if (this.appService.TelemetryEnabled)
        {
            ShowEnabledTelemetryMsg();
        }
        else
        {
            ShowDisabledTelemetryMsg();
        }
    }

    /// <inheritdoc/>
    public void TrackAppStart()
    {
#if DEBUG && ENABLE_TELEMETRY
        // VELAPTOR_OPT_OUT_TELEMETRY: Set by anyone (maintainer or game developer) to suppress telemetry.
        // Game developers who do not want telemetry tracked should set this env var to 1 or true.
        var optOut = (Environment.GetEnvironmentVariable("VELAPTOR_OPT_OUT_TELEMETRY") ?? string.Empty).ToLower();

        if (optOut is "1" or "true")
        {
            return;
        }

        var version = this.appService.Version;

        var payload = new
        {
            version,
            dotnetVersion = Environment.Version.ToString(),
            locale = CultureInfo.CurrentUICulture.Name,
            rid = RuntimeInformation.RuntimeIdentifier,
            osArchitecture = RuntimeInformation.OSArchitecture.ToString().ToLower(),
            timestamp = DateTime.UtcNow.ToString("o"),
        };

        var json = JsonSerializer.Serialize(payload);

        Task.Run(() => this.telemetryClient.TrackEvent(json));
#endif
    }

    // TODO: update url to the learn more about telemetry docs

    /// <summary>
    /// Shows the message that explains the telemetry feature.
    /// </summary>
    private static void ShowEnabledTelemetryMsg() =>
        Console.WriteLine("""

                          📊 Velaptor Telemetry: Enabled
                          ─────────────────────────────────────────────────────────────────
                            Velaptor collects anonymous usage data to help us understand
                            how the framework is being used and improve future versions.

                            What is collected:
                              • Velaptor version
                              • .NET runtime version
                              • OS architecture and runtime identifier
                              • Locale
                              • Timestamp of app start

                            No personal information, project names, or game content is ever collected.
                            Data is only collected in debug builds —
                            telemetry is completely disabled in release/production builds.

                            📖 Learn more: https://github.com/KinsonDigital/Velaptor/blob/main/docs/telemetry.md

                            🚫 To opt out, set this environment variable:
                                 VELAPTOR_OPT_OUT_TELEMETRY=1
                          ─────────────────────────────────────────────────────────────────

                          """);

    /// <summary>
    /// Shows the message that explains the telemetry feature is disabled.
    /// </summary>
    private static void ShowDisabledTelemetryMsg() =>
        Console.WriteLine("""
                          
                          📊 Velaptor Telemetry: Disabled
                          ─────────────────────────────────────────────────────────────────
                            Telemetry helps improve Velaptor. To opt in, remove or unset the
                            VELAPTOR_OPT_OUT_TELEMETRY environment variable.
                            📖 Learn more: https://github.com/KinsonDigital/Velaptor/blob/main/docs/telemetry.md
                          ─────────────────────────────────────────────────────────────────
                          
                          """);
}
