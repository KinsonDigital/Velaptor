// <copyright file="JSONService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Performs JSON services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class JSONService : IJSONService
    {
        /// <inheritdoc/>
        public string Serialize(object? value) => JsonConvert.SerializeObject(value, Formatting.Indented);

        /// <inheritdoc/>
        public T? Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);
    }
}
