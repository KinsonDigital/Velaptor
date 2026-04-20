// <copyright file="TelemetryClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Services;

/// <inheritdoc cref="ITelemetryClient"/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal class TelemetryClient : ITelemetryClient, IDisposable
{
    private static readonly HttpClient HttpClient = new () { Timeout = TimeSpan.FromSeconds(5) };
    private readonly IAppService appService;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryClient"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    public TelemetryClient(IAppService appService) => this.appService = appService;

    /// <inheritdoc/>
    public async Task TrackEvent(string jsonPayload)
    {
        try
        {
            var isDebug = this.appService.IsDebug;
            var protocol = isDebug ? "http" : "https";
            var serverHost = isDebug ? "localhost:8000" : "velaptor-telemetry.kinsondigital.deno.net";
            const string endpoint = "velaptor-template-telemetry";

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            await HttpClient.PostAsync($"{protocol}://{serverHost}/{endpoint}", content);
        }
        catch
        {
            // Silently swallow - telemetry must never crash the user's workflow
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            HttpClient.Dispose();
        }

        this.isDisposed = true;
    }
}
