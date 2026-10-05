// <copyright file="Delegates.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu;

using Silk.NET.WebGPU;

/// <summary>
/// A safe version of the <see cref="PfnRequestAdapterCallback"/>. Uses <see cref="nint"/> instead of raw pointers
/// so callers do not need <c>unsafe</c> context.
/// </summary>
/// <param name="status">The request outcome.</param>
/// <param name="adapter">A pointer to the adapter, or <see cref="nint.Zero"/> on failure.</param>
/// <param name="message">A null-terminated UTF-8 error message, or <see cref="nint.Zero"/>.</param>
/// <param name="userdata">Opaque user-data pointer passed through from the request.</param>
internal delegate void SafeRequestAdapterCallback(
    RequestAdapterStatus status,
    nint adapter,
    nint message,
    nint userdata);

/// <summary>
/// A safe version of the <see cref="PfnRequestDeviceCallback"/>. Uses <see cref="nint"/> instead of raw pointers
/// so callers do not need <c>unsafe</c> context.
/// </summary>
/// <param name="status">The request outcome.</param>
/// <param name="device">A pointer to the device, or <see cref="nint.Zero"/> on failure.</param>
/// <param name="message">A null-terminated UTF-8 error message, or <see cref="nint.Zero"/>.</param>
/// <param name="userdata">Opaque user-data pointer passed through from the request.</param>
internal delegate void SafeRequestDeviceCallback(
    RequestDeviceStatus status,
    nint device,
    nint message,
    nint userdata);

/// <summary>
/// A safe version of the <see cref="PfnErrorCallback"/> error callback. Uses <see cref="nint"/> instead of raw pointers
/// so callers do not need <c>unsafe</c> context.
/// </summary>
/// <param name="type">The severity of the error.</param>
/// <param name="message">A null-terminated UTF-8 error message.</param>
/// <param name="userdata">Opaque user-data pointer passed through from the registration.</param>
internal delegate void SafeErrorCallback(
    ErrorType type,
    nint message,
    nint userdata);
