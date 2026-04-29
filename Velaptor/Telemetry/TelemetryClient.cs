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
    }

    /// <inheritdoc/>
    public async Task TrackEvent(string jsonPayload)
    {
        try
        {
#if TELEMETRY_DEBUG || TELEMETRY_RELEASE
            const string protocol = "http";
            const string serverHost = "localhost:8500";
#else
            const string protocol = "https";
            const string serverHost = "kinson-digital.kinsondigital.deno.net";
#endif
            const string endpoint = "velaptor-template-telemetry";

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.PostAsync($"{protocol}://{serverHost}/{endpoint}", content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Telemetry event failed to send. Status code: {response.StatusCode}");
                Console.WriteLine($"Response body: {responseBody}");
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
