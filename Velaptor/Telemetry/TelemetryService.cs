// <copyright file="TelemetryService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

#if ENABLE_TELEMETRY

using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using System.IO.Abstractions;
using Hardware.Services;
using System.Linq;
using System.Text.Json.Serialization;

#else

using System;

#endif

using System.Diagnostics.CodeAnalysis;
using Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal class TelemetryService : ITelemetryService
{
#if ENABLE_TELEMETRY
    private const string HardwareDataFileName = "hardware-specs.json";
    private readonly ITelemetryClient telemetryClient;
    private readonly ICpuService cpuService;
    private readonly IGpuService gpuService;
    private readonly JsonSerializerOptions serializeOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter() },
    };
    private readonly IFile file;
#endif
    private readonly IAppService appService;
#if ENABLE_TELEMETRY
    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryService"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    /// <param name="telemetryClient">Communicates with the telemetry server.</param>
    /// <param name="cpuService">Provides CPU services.</param>
    /// <param name="gpuService">Provides GPU services.</param>
    /// <param name="file">Performs operations with files.</param>
    public TelemetryService(IAppService appService, ITelemetryClient telemetryClient, ICpuService cpuService, IGpuService gpuService, IFile file)
    {
        ArgumentNullException.ThrowIfNull(appService);
        ArgumentNullException.ThrowIfNull(telemetryClient);
        ArgumentNullException.ThrowIfNull(cpuService);
        ArgumentNullException.ThrowIfNull(gpuService);
        ArgumentNullException.ThrowIfNull(file);
        this.appService = appService;
        this.telemetryClient = telemetryClient;
        this.cpuService = cpuService;
        this.gpuService = gpuService;
        this.file = file;

        // Has the user opted into telemetry?
        // Set by anyone (maintainer or game developer) to suppress telemetry.
        // Game developers who do not want telemetry tracked should set this env var to 1 or true.
        if (IsNotOptedIn())
        {
            ShowDisabledTelemetryMsg();
        }
        else
        {
            ShowEnabledTelemetryMsg();
        }
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryService"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    public TelemetryService(IAppService appService)
    {
        ArgumentNullException.ThrowIfNull(appService);

        this.appService = appService;

        // Has the user opted into telemetry?
        // Set by anyone (maintainer or game developer) to suppress telemetry.
        // Game developers who do not want telemetry tracked should set this env var to 1 or true.
        if (IsNotOptedIn())
        {
            ShowDisabledTelemetryMsg();
        }
        else
        {
            ShowEnabledTelemetryMsg();
        }
    }
#endif

    /// <inheritdoc/>
    public void TrackAppStart()
    {
#if ENABLE_TELEMETRY
        if (IsNotOptedIn())
        {
            return;
        }

        if (!this.appService.ConsumerIsDebug)
        {
            return;
        }

        // Only send telemetry when running in a developer environment.
        // A published game distributed to players will not be running in
        // a game development environment.
        if (!this.appService.InDevelopmentEnvironment)
        {
            return;
        }

        var version = this.appService.Version;
        var osName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown";
        var payload = new UsageData
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
#endif
    }

    /// <inheritdoc/>
    public void TrackHardware()
    {
#if ENABLE_TELEMETRY
        // Only send telemetry when running in a developer environment.
        // A published game distributed to players will not be running in
        // a game development environment.
        if (!this.appService.InDevelopmentEnvironment)
        {
            return;
        }

        var jsonData = GetHardwareData();

        Task.Run(() => this.telemetryClient.TrackHardware(jsonData));
#endif
    }

    /// <summary>
    /// Returns a value indicating whether the user has opted out of telemetry.
    /// </summary>
    /// <returns>True if the user has opted out.</returns>
    private static bool IsNotOptedIn()
    {
        var optIn = (Environment.GetEnvironmentVariable("VELAPTOR_OPT_IN_TELEMETRY") ?? string.Empty).ToLower();
        return string.IsNullOrEmpty(optIn) || optIn is "0" or "false";
    }

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

                            Learn more: https://docs.velaptor.io/telemetry

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

                            Learn more: https://docs.velaptor.io/telemetry
                          ────────────────────────────────────────────────────────────────────────────────────────────────────────

                          """);
    }

#if ENABLE_TELEMETRY
    /// <summary>
    /// Gets the hardware data.
    /// </summary>
    /// <returns>The hardware JSON data.</returns>
    private string GetHardwareData()
    {
        var hardwareSpecsFilepath = $"{this.appService.AppDirectory}/{HardwareDataFileName}";
        string jsonData;

        if (this.file.Exists(hardwareSpecsFilepath))
        {
            jsonData = this.file.ReadAllText(hardwareSpecsFilepath);
        }
        else
        {
            var cpu = this.cpuService.GetCpuInfo();
            var gpus = this.gpuService.GetGpuInfo();
            var hardware = new HardwareData { Cpu = cpu, Gpus = gpus.ToArray(), };
            jsonData = JsonSerializer.Serialize(hardware, this.serializeOptions);

            this.file.WriteAllText(hardwareSpecsFilepath, jsonData);
        }

        return jsonData;
    }
#endif
}
