// <copyright file="GLFWMonitors.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using Velaptor.Hardware;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Gets all of the monitors in the system.
    /// </summary>
    internal sealed class GLFWMonitors : IDisposable
    {
        private static bool glfwInitialized;
        private readonly IGLFWInvoker glfwInvoker;
        private readonly IPlatform platform;
        private readonly List<SystemMonitor> monitors = new ();
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLFWMonitors"/> class.
        /// </summary>
        /// <param name="glfwInvoker">Used to make calls to GLFW.</param>
        /// <param name="platform">The current platform.</param>
        public GLFWMonitors(IGLFWInvoker glfwInvoker, IPlatform platform)
        {
            this.glfwInvoker = glfwInvoker;
            this.platform = platform;

            if (!glfwInitialized)
            {
                this.glfwInvoker.Init();
                glfwInitialized = true;
            }

            Refresh();

            this.glfwInvoker.OnMonitorChanged += GLFWInvoker_OnMonitorChanged;
        }

        /// <summary>
        /// Gets a list of all the monitors currently in the system.
        /// </summary>
        public SystemMonitor[] SystemMonitors => this.monitors.ToArray();

        /// <summary>
        /// Refreshes the monitor information.
        /// </summary>
        public void Refresh()
        {
            Vector2 GetMonitorScale(IntPtr monitorHandle)
            {
                var scale = this.glfwInvoker.GetMonitorContentScale(monitorHandle);

                return new Vector2(scale.X, scale.Y);
            }

            var monitorHandles = this.glfwInvoker.GetMonitors();

            foreach (var monitorHandle in monitorHandles)
            {
                var monitorVideoMode = this.glfwInvoker.GetVideoMode(monitorHandle);

                var monitorScale = GetMonitorScale(monitorHandle);

                var newMonitor = new SystemMonitor(this.platform)
                {
                    IsMain = this.monitors.Count <= 0,
                    RedBitDepth = monitorVideoMode.RedBits,
                    BlueBitDepth = monitorVideoMode.BlueBits,
                    GreenBitDepth = monitorVideoMode.GreenBits,
                    Height = monitorVideoMode.Height,
                    Width = monitorVideoMode.Width,
                    RefreshRate = monitorVideoMode.RefreshRate,
                    HorizontalScale = monitorScale.X,
                    VerticalScale = monitorScale.Y,
                };

                this.monitors.Add(newMonitor);
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/></param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.glfwInvoker.OnMonitorChanged -= GLFWInvoker_OnMonitorChanged;
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Occurs when a monitor is connected or disconnected.
        /// </summary>
        private void GLFWInvoker_OnMonitorChanged(object? sender, GLFWMonitorChangedEventArgs e)
            => Refresh();
    }
}
