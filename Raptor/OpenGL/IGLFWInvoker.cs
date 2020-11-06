// <copyright file="IGLFWInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Numerics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// Invokes GLFW calls.
    /// </summary>
    internal interface IGLFWInvoker
    {
        /// <summary>
        /// <para>
        /// This function initializes the GLFW library. Before most GLFW functions can be used,
        /// GLFW must be initialized, and before an application terminates GLFW should be terminated in order to
        /// free any resources allocated during or after initialization.
        /// </para>
        /// <para>
        /// If this function fails, it calls <see cref="Terminate"/> before returning.
        /// </para>
        /// <para>
        /// If it succeeds, you should call <see cref="Terminate"/> before the application exits.
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
        /// This function sets the error callback, which is called with an error code
        /// and a human-readable description each time a GLFW error occurs.
        /// </para>
        /// <para>
        /// The error callback is called on the thread where the error occurred.
        /// If you are using GLFW from multiple threads, your error callback needs to be written accordingly.
        /// </para>
        /// <para>
        /// Because the description string may have been generated specifically for that error,
        /// it is not guaranteed to be valid after the callback has returned.
        /// If you wish to use it after the callback returns, you need to make a deep copy.
        /// </para>
        /// <para>
        /// Once set, the error callback remains set even after the library has been terminated.
        /// </para>
        /// </summary>
        /// <param name="callback">The new callback, or <c>null</c> to remove the currently set callback.</param>
        /// <returns>The previously set callback, or <c>null</c> if no callback was set.</returns>
        /// <remarks>
        /// <para>
        /// This function may be called before <see cref="Init"/>.
        /// </para>
        /// <para>
        /// This function must only be called from the main thread.
        /// </para>
        /// </remarks>
        IntPtr SetErrorCallback(GLFWCallbacks.ErrorCallback callback);

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
        unsafe Vector2 GetMonitorContentScale(IntPtr monitor);

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
        /// <seealso cref="GetPrimaryMonitor"/>
        unsafe IntPtr[] GetMonitors();

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
        ///     The returned array is allocated and freed by GLFW
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
        /// <seealso cref="GetVideoModes"/>
        unsafe VideoMode GetVideoMode(IntPtr monitor);

        /// <summary>
        /// <para>
        ///     This function sets the monitor configuration callback, or removes the currently set callback.
        ///     This is called when a monitor is connected to or disconnected from the system.
        /// </para>
        /// </summary>
        /// <param name="callback">The new callback, or <c>null</c> to remove the currently set callback.</param>
        /// <returns>
        ///     The previously set callback, or <c>null</c> if no callback was set or the library had not been initialized.
        /// </returns>
        /// <remarks>
        /// <para>
        ///     This function must only be called from the main thread.
        /// </para>
        /// <para>
        ///     Possible errors include <see cref="ErrorCode.NotInitialized"/>.
        /// </para>
        /// </remarks>
        IntPtr SetMonitorCallback(GLFWCallbacks.MonitorCallback callback);
    }
}
