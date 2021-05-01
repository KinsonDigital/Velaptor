// <copyright file="GLFWMonitors.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using Raptor.Hardware;
    using Raptor.NativeInterop;

    /// <summary>
    /// Gets all of the monitors in the system.
    /// </summary>
    internal class GLFWMonitors
    {
        private static bool glfwInitialzed;
        private readonly IGLFWInvoker glfwInvoker;
        private readonly IPlatform platform;
        private readonly List<SystemMonitor> monitors = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="GLFWMonitors"/> class.
        /// </summary>
        /// <param name="glfwInvoker">Used to make calls to GLFW.</param>
        /// <param name="platform">The current platform.</param>
        public unsafe GLFWMonitors(IGLFWInvoker glfwInvoker, IPlatform platform)
        {
            this.glfwInvoker = glfwInvoker;
            this.platform = platform;

            if (!glfwInitialzed)
            {
                this.glfwInvoker.Init();
                glfwInitialzed = true;
            }

            Refresh();

            this.glfwInvoker.SetMonitorCallback(MonitorCallback);
        }

        /// <summary>
        /// Gets a list of all the monitors currently in the system.
        /// </summary>
        public SystemMonitor[] SystemMonitors => this.monitors.ToArray();

        /// <summary>
        /// Refreshes the monitor information.
        /// </summary>
        public unsafe void Refresh()
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

                var newMonitor = new SystemMonitor(this.platform)
                {
                    IsMain = this.monitors.Count <= 0,
                    RedBitDepth = monitorVideoMode.RedBits,
                    BlueBitDepth = monitorVideoMode.BlueBits,
                    GreenBitDepth = monitorVideoMode.GreenBits,
                    Height = monitorVideoMode.Height,
                    Width = monitorVideoMode.Width,
                    RefreshRate = monitorVideoMode.RefreshRate,
                };

                var monitorScale = GetMonitorScale(monitorHandle);

                newMonitor.HorizontalScale = monitorScale.X;
                newMonitor.VerticalScale = monitorScale.Y;

                this.monitors.Add(newMonitor);
            }
        }

        /// <summary>
        /// Invoked when a monitor is connected or disconnected.
        /// </summary>
        /// <param name="monitor">The handle/pointer to the monitor that just go connected.</param>
        /// <param name="state">The state of the monitor.</param>
        [ExcludeFromCodeCoverage]
        private unsafe void MonitorCallback(Monitor* monitor, ConnectedState state) => Refresh();
    }
}
