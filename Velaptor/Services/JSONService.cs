// <copyright file="JSONService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Performs JSON services.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the '{nameof(JsonConvert)}' API")]
internal sealed class JSONService : IJsonService
{
    /// <inheritdoc/>
    public string Serialize(object? value) => JsonConvert.SerializeObject(value, Formatting.Indented);

    /// <inheritdoc/>
    public T? Deserialize<T>(string value, bool useCamelCase = true)
    {
        var settings = useCamelCase
            ? new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), }
            : null;

        return JsonConvert.DeserializeObject<T>(value, settings);
    }
}
