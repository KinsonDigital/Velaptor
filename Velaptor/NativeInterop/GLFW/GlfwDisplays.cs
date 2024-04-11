// <copyright file="GlfwDisplays.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using System.Collections.Generic;
using System.Numerics;
using Hardware;

/// <summary>
/// Gets all the displays in the system.
/// </summary>
internal sealed class GlfwDisplays : IDisplays
{
    private readonly bool glfwInitialized;
    private readonly IGlfwInvoker glfwInvoker;
    private readonly IPlatform platform;
    private readonly List<SystemDisplay> displays = new ();
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwDisplays"/> class.
    /// </summary>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public GlfwDisplays(IGlfwInvoker glfwInvoker, IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(platform);

        this.glfwInvoker = glfwInvoker;
        this.platform = platform;

        if (!this.glfwInitialized)
        {
            this.glfwInvoker.Init();
            this.glfwInitialized = true;
        }

        Refresh();

        this.glfwInvoker.OnDisplayChanged += GlfwInvoker_OnDisplayChanged;
    }

    /// <inheritdoc/>
    public SystemDisplay[] SystemDisplays => this.displays.ToArray();

    /// <inheritdoc/>
    public void Refresh()
    {
        Vector2 GetMonitorScale(nint monitorHandle)
        {
            var scale = this.glfwInvoker.GetMonitorContentScale(monitorHandle);

            return new Vector2(scale.X, scale.Y);
        }

        var monitorHandles = this.glfwInvoker.GetMonitors();

        foreach (var monitorHandle in monitorHandles)
        {
            var monitorVideoMode = this.glfwInvoker.GetVideoMode(monitorHandle);

            var monitorScale = GetMonitorScale(monitorHandle);

            var newDisplay = new SystemDisplay(this.platform)
            {
                IsMain = this.displays.Count <= 0,
                RedBitDepth = monitorVideoMode.RedBits,
                BlueBitDepth = monitorVideoMode.BlueBits,
                GreenBitDepth = monitorVideoMode.GreenBits,
                Height = monitorVideoMode.Height,
                Width = monitorVideoMode.Width,
                RefreshRate = monitorVideoMode.RefreshRate,
                HorizontalScale = monitorScale.X,
                VerticalScale = monitorScale.Y,
            };

            this.displays.Add(newDisplay);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.glfwInvoker.OnDisplayChanged -= GlfwInvoker_OnDisplayChanged;
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Occurs when a display is connected or disconnected.
    /// </summary>
    private void GlfwInvoker_OnDisplayChanged(object? sender, GlfwDisplayChangedEventArgs e)
        => Refresh();
}
