// <copyright file="GLFWInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// Invokes GLFW calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GLFWInvoker : IGLFWInvoker
    {
        /// <inheritdoc/>
        public bool Init() => GLFW.Init();

        /// <inheritdoc/>
        public IntPtr SetErrorCallback(GLFWCallbacks.ErrorCallback callback)
            => GLFW.SetErrorCallback(callback);

        /// <inheritdoc/>
        public unsafe IntPtr[] GetMonitors()
        {
            var monitors = GLFW.GetMonitors();

            var result = new List<IntPtr>();

            foreach (var monitor in monitors)
            {
                result.Add((IntPtr)monitor);
            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        public unsafe Vector2 GetMonitorContentScale(IntPtr monitor)
        {
            GLFW.GetMonitorContentScale((Monitor*)monitor, out var xScale, out var yScale);

            return new Vector2(xScale, yScale);
        }

        /// <inheritdoc/>
        public unsafe IntPtr GetVideoMode(IntPtr monitor) => (IntPtr)GLFW.GetVideoMode((Monitor*)monitor);

        /// <inheritdoc/>
        public IntPtr SetMonitorCallback(GLFWCallbacks.MonitorCallback callback) => GLFW.SetMonitorCallback(callback);
    }
}
