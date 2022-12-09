// <copyright file="GLFWInvoker.cs" company="KinsonDigital">
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
[ExcludeFromCodeCoverage]
internal sealed class GLFWInvoker : IGLFWInvoker
{
    private readonly Glfw glfw;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLFWInvoker"/> class.
    /// </summary>
    public GLFWInvoker()
    {
        this.glfw = Glfw.GetApi();

        this.glfw.SetErrorCallback(ErrorCallback);

        unsafe
        {
            this.glfw.SetMonitorCallback(MonitorCallback);
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GLFWInvoker"/> class.
    /// </summary>
    ~GLFWInvoker() => Dispose();

    /// <inheritdoc/>
    public event EventHandler<GLFWErrorEventArgs>? OnError;

    /// <inheritdoc/>
    public event EventHandler<GLFWMonitorChangedEventArgs>? OnMonitorChanged;

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
    public GLFWVideoMode GetVideoMode(nint monitor)
    {
        GLFWVideoMode result = default;

        unsafe
        {
            var pVideoMode = this.glfw.GetVideoMode((Monitor*)monitor);

            result.RedBits = pVideoMode->RedBits;
            result.GreenBits = pVideoMode->GreenBits;
            result.BlueBits = pVideoMode->BlueBits;
            result.Width = pVideoMode->Width;
            result.Height = pVideoMode->Height;
            result.RefreshRate = pVideoMode->RefreshRate;
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
        => this.OnError?.Invoke(this, new GLFWErrorEventArgs((GLFWErrorCode)errorCode, description));

    /// <summary>
    /// Invoked when GLFW detects that something has changed with the monitors,
    /// and then invokes the <see cref="OnMonitorChanged"/> event.
    /// </summary>
    /// <param name="monitor">The monitor that has changed.</param>
    /// <param name="connectedState">The connected state of the monitor.</param>
    private unsafe void MonitorCallback(Monitor* monitor, ConnectedState connectedState)
        => this.OnMonitorChanged?.Invoke(this, new GLFWMonitorChangedEventArgs(connectedState == ConnectedState.Connected));
}
