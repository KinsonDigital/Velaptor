// <copyright file="IGlfwInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using System.Numerics;
using Silk.NET.GLFW;

/// <summary>
/// Invokes GLFW calls.
/// </summary>
internal interface IGlfwInvoker : IDisposable
{
    /// <summary>
    /// Occurs when a GLFW related error occurs.
    /// </summary>
    event EventHandler<GlfwErrorEventArgs>? OnError;

    /// <summary>
    /// Occurs when something with the monitors has changed.
    /// <para>
    ///     Example: A monitor has been connected or disconnected.
    /// </para>
    /// </summary>
    event EventHandler<GlfwDisplayChangedEventArgs>? OnDisplayChanged;

    /// <summary>
    /// <para>
    /// This function initializes the GLFW library. Before most GLFW functions can be used,
    /// GLFW must be initialized, and before an application terminates GLFW should be terminated in order to
    /// free any resources allocated during or after initialization.
    /// </para>
    /// <para>
    /// If this function fails, it calls Terminate before returning.
    /// </para>
    /// <para>
    /// If it succeeds, you should call Terminate before the application exits.
    /// </para>
    /// <para>
    /// Additional calls to this function after successful initialization
    /// but before termination will return <c>true</c> immediately.
    /// </para>
    /// </summary>
    /// <returns><c>true</c> if successful, or <c>false</c> if an error occurred.</returns>
    /// <remarks>
    /// <para>
    /// OS X: This function will change the current directory of the application
    /// to the Contents/Resources subdirectory of the application's bundle, if present.
    /// </para>
    /// <para>
    /// This function must only be called from the main thread.
    /// </para>
    /// <para>
    /// Possible errors include <see cref="ErrorCode.PlatformError"/>.
    /// </para>
    /// </remarks>
    bool Init();

    /// <summary>
    /// <para>
    ///     This function retrieves the content scale for the specified monitor.
    /// </para>
    /// <para>
    ///     The content scale is the ratio between the current DPI and the platform's default DPI.
    /// </para>
    /// <para>
    ///     If you scale all pixel dimensions by this scale then your content should appear at an appropriate size.
    ///     This is especially important for text and any UI elements.
    /// </para>
    ///
    /// <para>
    ///     The content scale may depend on both the monitor resolution and pixel density and on user settings.
    ///     It may be very different from the raw DPI calculated from the physical size and current resolution.
    /// </para>
    /// </summary>
    /// <param name="monitor">The monitor to query.</param>
    /// <returns>The monitor scale in the X and Y axis.</returns>
    Vector2 GetMonitorContentScale(nint monitor);

    /// <summary>
    /// <para>
    ///     This function returns an array of handles for all currently connected monitors.
    ///     The primary monitor is always first in the returned array.
    /// </para>
    /// <para>
    ///     If no monitors were found, this function returns <c>null</c>.
    /// </para>
    /// </summary>
    /// <returns>
    ///     An array of monitor pointers, or <c>null</c> if no monitors were found or if an error occurred.
    /// </returns>
    /// <remarks>
    /// <para>
    ///     The returned array is allocated and freed by GLFW. You should not free it yourself.
    ///     It is only guaranteed to be valid until the monitor configuration changes or the library is terminated.
    /// </para>
    /// <para>
    ///     This function must only be called from the main thread.
    /// </para>
    /// <para>
    ///     Possible errors include <see cref="ErrorCode.NotInitialized"/>.
    /// </para>
    /// </remarks>
    nint[] GetMonitors();

    /// <summary>
    /// <para>
    ///     This function returns the current video mode of the specified monitor.
    /// </para>
    /// <para>
    ///     If you have created a full screen window for that monitor,
    ///     the return value will depend on whether that window is iconified.
    /// </para>
    /// </summary>
    /// <param name="monitor">The monitor to query. </param>
    /// <returns>A pointer to the current video mode of the monitor, or <c>null</c> if an error occurred.</returns>
    /// <remarks>
    /// <para>
    ///     The returned array is allocated and freed by GLFW.
    ///     You should not free it yourself.
    ///     It is valid until the specified monitor is disconnected or the library is terminated.
    /// </para>
    /// <para>
    ///     This function must only be called from the main thread.
    /// </para>
    /// <para>
    ///     Possible errors include <see cref="ErrorCode.NotInitialized"/> and <see cref="ErrorCode.PlatformError"/>.
    /// </para>
    /// </remarks>
    GlfwVideoMode GetVideoMode(nint monitor);
}
