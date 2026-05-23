// <copyright file="TelemetryClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Services;

/// <inheritdoc cref="ITelemetryClient"/>
[ExcludeFromCodeCoverage(Justification = "Telemetry code is challenging to test and provides minimal value to cover with unit tests.")]
internal sealed class TelemetryClient : ITelemetryClient
{
#if TELEMETRY_DEBUG
    private const string Protocol = "http";
    private const string ServerHost = "localhost:8500";
#else
    private const string Protocol = "https";
    private const string ServerHost = "kinson-digital.kinsondigital.deno.net";
#endif
    private readonly HttpClient httpClient;
    private readonly string? telemetryKey;
    private readonly IConsoleService consoleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryClient"/> class.
    /// </summary>
    /// <param name="client">Makes HTTP requests.</param>
    /// <param name="consoleService">Provides console services.</param>
    public TelemetryClient(HttpClient client, IConsoleService consoleService)
    {
        this.httpClient = client;
        this.consoleService = consoleService;
        this.telemetryKey = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "TelemetryKey")?.Value;

        if (string.IsNullOrEmpty(this.telemetryKey))
        {
            this.consoleService.WriteLine("Telemetry key is missing. Telemetry data will not be sent.");
        }
    }

    /// <inheritdoc/>
    public async Task TrackEvent(string jsonPayload)
    {
        if (string.IsNullOrEmpty(this.telemetryKey))
        {
            return;
        }

        try
        {
            const string endpoint = "usage";
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Protocol}://{ServerHost}/{endpoint}");
            SetHeadersAndContent(request, jsonPayload);

            using var response = await this.httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseText = await response.Content.ReadAsStringAsync();

                this.consoleService.WriteLine($"Failed to send telemetry data. Status code: {response.StatusCode}, Response: {responseText}");
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
        if (string.IsNullOrEmpty(this.telemetryKey))
        {
            return;
        }

        try
        {
            const string endpoint = "hardware";
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Protocol}://{ServerHost}/{endpoint}");
            SetHeadersAndContent(request, jsonPayload);

            using var response = await this.httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseText = await response.Content.ReadAsStringAsync();

                this.consoleService.WriteLine($"Failed to send telemetry data. Status code: {response.StatusCode}, Response: {responseText}");
            }
        }
        catch
        {
            // Silently swallow - telemetry must never crash the user's workflow
        }
    }

    /// <summary>
    /// Sets the headers and content of the given <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="jsonPayload">The JSON payload to send with the request.</param>
    private void SetHeadersAndContent(HttpRequestMessage request, string jsonPayload)
    {
        request.Headers.Add("Api-Key", this.telemetryKey);
        request.Headers.Add("Origin", $"{Protocol}://{ServerHost}");
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}
