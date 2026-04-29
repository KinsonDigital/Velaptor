// <copyright file="TelemetryService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable once GrammarMistakeInComment
namespace Velaptor.Telemetry;

#if ENABLE_TELEMETRY
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using Services;
#endif

using System.Diagnostics.CodeAnalysis;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal class TelemetryService : ITelemetryService
{
#if ENABLE_TELEMETRY
    private readonly ITelemetryClient telemetryClient;
    private readonly IAppService appService;
    private readonly JsonSerializerOptions serializeOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
#endif

#if ENABLE_TELEMETRY
    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryService"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    /// <param name="telemetryClient">Communicates with the telemetry server.</param>
    public TelemetryService(IAppService appService, ITelemetryClient telemetryClient)
    {
        this.appService = appService;
        this.telemetryClient = telemetryClient;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryService"/> class.
    /// </summary>
    public TelemetryService()
    {
    }
#endif

    /// <inheritdoc/>
    public void TrackAppStart()
    {
#if ENABLE_TELEMETRY
        // Set by anyone (maintainer or game developer) to suppress telemetry.
        // Game developers who do not want telemetry tracked should set this env var to 1 or true.
        var optIn = (Environment.GetEnvironmentVariable("VELAPTOR_OPT_IN_TELEMETRY") ?? string.Empty).ToLower();

        Console.WriteLine($"Opt In State: {optIn}");
        Console.WriteLine($"Consumer Is Debug: {this.appService.ConsumerIsDebug}");
        Console.WriteLine($"In Dev Environment: {this.appService.InDevelopmentEnvironment}");

        // Has the user opted into telemetry?
        if (string.IsNullOrEmpty(optIn) || optIn is "0" or "false")
        {
            ShowDisabledTelemetryMsg();

            return;
        }

        if (!this.appService.ConsumerIsDebug)
        {
            return;
        }

        // Only send telemetry when running in a developer environment.
        // A published game distributed to players will not be running in
        // a game development environment.
        if (this.appService.InDevelopmentEnvironment)
        {
            ShowEnabledTelemetryMsg();

            var version = this.appService.Version;
            var osName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows"
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS"
                : "Unknown";

            var payload = new TelemetryData
            {
                LifeCycleEvent = "app-start",
                FeatureArea = null,
                FeatureName = null,
                Version = version,
                DotnetVersion = Environment.Version.ToString(),
                Language = CultureInfo.CurrentUICulture.Parent.EnglishName,
                OsName = osName,
                OsArchitecture = RuntimeInformation.OSArchitecture.ToString().ToLower(),
            };

            var json = JsonSerializer.Serialize(payload, this.serializeOptions);

            Task.Run(() => this.telemetryClient.TrackEvent(json));
        }
#endif
    }

#if ENABLE_TELEMETRY
    /// <summary>
    /// Shows the message that explains the telemetry feature.
    /// </summary>
    private void ShowEnabledTelemetryMsg()
    {
        // Do not show console messages about telemetry in release builds
        if (!this.appService.ConsumerIsDebug)
        {
            return;
        }

        Console.WriteLine("""

                          Velaptor Telemetry: Enabled
                          ───────────────────────────────────────────────────────────────────────────────────
                            You have opted in to telemetry, and Velaptor is collecting
                            anonymous usage data to help us understand how the framework
                            is being used to improve future versions.

                            What is being collected:
                              • Various game lifecycle events
                              • Velaptor features
                              • Velaptor version
                              • .NET runtime version
                              • Language
                              • Operating system
                              • Operating system architecture
                              • Hardware specs (CPU, GPU, RAM, etc.)

                            No personal information, project names, or game content is ever collected.
                            Data is only collected from the developers that use Velaptor.
                            Data is NOT collected in built/compiled games!

                            Learn more: https://docs.velaptor.io/telemetry.md

                            To opt out, set this environment variable:
                                 VELAPTOR_OPT_IN_TELEMETRY=0 or VELAPTOR_OPT_IN_TELEMETRY=false 
                          ───────────────────────────────────────────────────────────────────────────────────

                          """);
    }

    /// <summary>
    /// Shows the message that explains the telemetry feature is disabled.
    /// </summary>
    private void ShowDisabledTelemetryMsg()
    {
        // Do not show console messages about telemetry in release builds
        if (!this.appService.ConsumerIsDebug)
        {
            return;
        }

        Console.WriteLine("""

                          Velaptor Telemetry: Disabled
                          ────────────────────────────────────────────────────────────────────────────────────────────────────────
                            Help us improve and evolve Velaptor!

                            Velaptor is an open-source community-driven and independent project.
                            Would you be willing to share anonymous technical telemetry and usage data?

                            What would be collected:
                              • Various game lifecycle events
                              • Velaptor features
                              • Velaptor version
                              • .NET runtime version
                              • Language
                              • Operating system
                              • Operating system architecture
                              • Hardware specs (CPU, GPU, RAM, etc.)

                            Why? It helps us improve Velaptor by understanding what features are used, prioritize optimizations
                            and catch TDR/lag issues before they hit your players.

                            Privacy: No PII, no project names, no tracking. Just usage and hardware spec data.
                            Transparency: You can audit the telemetry code in the Velaptor.Telemetry namespace.
                            Ownership: This is 100% Opt-In.

                            To opt in, add the environment variable VELAPTOR_OPT_IN_TELEMETRY=1 or VELAPTOR_OPT_IN_TELEMETRY=true.

                            Learn more: https://docs.velaptor.io/telemetry.md
                          ────────────────────────────────────────────────────────────────────────────────────────────────────────

                          """);
    }
#endif
}
