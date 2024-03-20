// <copyright file="DotnetService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

/// <inheritdoc />
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the dotnet {nameof(System)}.{nameof(GC)} and {nameof(System)}.{nameof(System.Runtime)}.{nameof(System.Runtime.InteropServices)}.{nameof(Marshal)} API.")]
internal sealed class DotnetService : IDotnetService
{
    /// <inheritdoc />
    public void GCKeepAlive(object? obj) => GC.KeepAlive(obj);

    /// <inheritdoc />
    public nint MarshalStringToHGlobalAnsi(string? s) => Marshal.StringToHGlobalAnsi(s);

    /// <inheritdoc />
    public string? MarshalPtrToStringAnsi(nint ptr) => Marshal.PtrToStringAnsi(ptr);
}
