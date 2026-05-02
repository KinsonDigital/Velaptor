// <copyright file="TelemetryClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <inheritdoc cref="ITelemetryClient"/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal class TelemetryClient : ITelemetryClient, IDisposable
{
#if TELEMETRY_DEBUG || TELEMETRY_RELEASE
    private const string Protocol = "http";
    private const string ServerHost = "localhost:8500";
#else
    private const string Protocol = "https";
    private const string ServerHost = "kinson-digital.kinsondigital.deno.net";
#endif
    private static readonly HttpClient HttpClient = new () { Timeout = TimeSpan.FromSeconds(5) };
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryClient"/> class.
    /// </summary>
    public TelemetryClient()
    {
        var telemetryKey = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "TelemetryKey")?.Value;

        HttpClient.DefaultRequestHeaders.Add("Api-Key", telemetryKey);
        HttpClient.DefaultRequestHeaders.Add("Origin", $"{Protocol}://{ServerHost}");
    }

    /// <inheritdoc/>
    public async Task TrackEvent(string jsonPayload)
    {
        try
        {
            const string endpoint = "usage";

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.PostAsync($"{Protocol}://{ServerHost}/{endpoint}", content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await response.Content.ReadAsStringAsync();
            }
        }
        catch
        {
            // Silently swallow - telemetry must never crash the user's workflow
        }
    }

    /// <inheritdoc/>
    public async Task TrackHardware(string jsonPayload)
    {
        try
        {
            const string endpoint = "hardware";

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.PostAsync($"{Protocol}://{ServerHost}/{endpoint}", content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await response.Content.ReadAsStringAsync();
            }
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
