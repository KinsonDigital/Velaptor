// <copyright file="GlfwInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Silk.NET.GLFW;

/// <summary>
/// Invokes GLFW calls.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the GLFW library.")]
internal sealed class GlfwInvoker : IGlfwInvoker
{
    private readonly Glfw glfw;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwInvoker"/> class.
    /// </summary>
    public GlfwInvoker()
    {
        this.glfw = Glfw.GetApi();

        this.glfw.SetErrorCallback(ErrorCallback);

        unsafe
        {
            this.glfw.SetMonitorCallback(MonitorCallback);
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GlfwInvoker"/> class.
    /// </summary>
    ~GlfwInvoker() => Dispose();

    /// <inheritdoc/>
    public event EventHandler<GlfwErrorEventArgs>? OnError;

    /// <inheritdoc/>
    public event EventHandler<GlfwDisplayChangedEventArgs>? OnDisplayChanged;

    /// <inheritdoc/>
    public bool Init() => this.glfw.Init();

    /// <inheritdoc/>
    public nint[] GetMonitors()
    {
        var result = new List<nint>();

        unsafe
        {
            var unsafeMonitorPointers = this.glfw.GetMonitors(out var count);

            for (var i = 0; i < count; i++)
            {
                result.Add((nint)unsafeMonitorPointers[i]);
            }
        }

        return result.ToArray();
    }

    /// <inheritdoc/>
    public Vector2 GetMonitorContentScale(nint monitor)
    {
        unsafe
        {
            this.glfw.GetMonitorContentScale((Monitor*)monitor, out var scaleX, out var scaleY);

            return new Vector2(scaleX, scaleY);
        }
    }

    /// <inheritdoc/>
    public GlfwVideoMode GetVideoMode(nint monitor)
    {
        GlfwVideoMode result = default;

        unsafe
        {
            var pVideoMode = this.glfw.GetVideoMode((Monitor*)monitor);

            result = result with
            {
                RedBits = pVideoMode->RedBits,
                GreenBits = pVideoMode->GreenBits,
                BlueBits = pVideoMode->BlueBits,
                Width = pVideoMode->Width,
                Height = pVideoMode->Height,
                RefreshRate = pVideoMode->RefreshRate,
            };
        }

        return result;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (!this.isDisposed)
        {
            this.glfw.Dispose();
            this.isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Invoked when GLFW has an error and if so, invokes the <see cref="OnError"/> event.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="description">The error description.</param>
    private void ErrorCallback(ErrorCode errorCode, string description)
        => this.OnError?.Invoke(this, new GlfwErrorEventArgs((GlfwErrorCode)errorCode, description));

    /// <summary>
    /// Invoked when GLFW detects that something has changed with the monitors,
    /// and then invokes the <see cref="OnDisplayChanged"/> event.
    /// </summary>
    /// <param name="monitor">The monitor that has changed.</param>
    /// <param name="connectedState">The connected state of the monitor.</param>
    private unsafe void MonitorCallback(Monitor* monitor, ConnectedState connectedState)
        => this.OnDisplayChanged?.Invoke(this, new GlfwDisplayChangedEventArgs(connectedState == ConnectedState.Connected));
}
