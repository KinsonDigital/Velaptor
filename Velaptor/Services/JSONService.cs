// <copyright file="JSONService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

/// <summary>
/// Performs JSON services.
/// </summary>
[ExcludeFromCodeCoverage(
    Justification =
        $"Cannot test due to direct interaction with the '{nameof(Newtonsoft)}.{nameof(Newtonsoft.Json)}.{nameof(JsonConvert)}' API")]
internal sealed class JSONService : IJsonService
{
    /// <inheritdoc/>
    public string Serialize(object? value) => JsonConvert.SerializeObject(value, Formatting.Indented);

    /// <inheritdoc/>
    public T? Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);
}
