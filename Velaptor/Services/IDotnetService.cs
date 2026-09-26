// <copyright file="IDotnetService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Invokes Dotnet functions.
/// </summary>
internal interface IDotnetService
{
    /// <inheritdoc cref="GC.KeepAlive"/>
    void GcKeepAlive(object? obj);

    /// <inheritdoc cref="Marshal.StringToHGlobalAnsi"/>
    nint MarshalStringToHGlobalAnsi(string? s);

    /// <inheritdoc cref="Marshal.PtrToStringAnsi(System.IntPtr)"/>
    string? MarshalPtrToStringAnsi(nint ptr);
}
