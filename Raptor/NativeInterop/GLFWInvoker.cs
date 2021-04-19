// <copyright file="GLFWInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.NativeInterop
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
    internal class GLFWInvoker : IGLFWInvoker
    {
        /// <inheritdoc/>
        public bool Init() => GLFW.Init();

        /// <inheritdoc/>
        public IntPtr SetErrorCallback(GLFWCallbacks.ErrorCallback callback)
            => GLFW.SetErrorCallback(callback);

        /// <inheritdoc/>
        public unsafe IntPtr[] GetMonitors()
        {
            var pointers = new List<IntPtr>();

            var rawPointers = GLFW.GetMonitors();

            foreach (var rawPointer in rawPointers)
            {
                pointers.Add((IntPtr)rawPointer);
            }

            return pointers.ToArray();
        }

        /// <inheritdoc/>
        public unsafe Vector2 GetMonitorContentScale(IntPtr monitor)
        {
            GLFW.GetMonitorContentScale((Monitor*)monitor, out var xScale, out var yScale);

            return new Vector2(xScale, yScale);
        }

        /// <inheritdoc/>
        public unsafe VideoMode GetVideoMode(IntPtr monitor)
        {
            var pVideoMode = GLFW.GetVideoMode((Monitor*)monitor);

            return new VideoMode()
            {
                RedBits = pVideoMode->RedBits,
                GreenBits = pVideoMode->RedBits,
                BlueBits = pVideoMode->RedBits,
                Width = pVideoMode->Width,
                Height = pVideoMode->Height,
                RefreshRate = pVideoMode->RefreshRate,
            };
        }

        /// <inheritdoc/>
        public IntPtr SetMonitorCallback(GLFWCallbacks.MonitorCallback callback) => GLFW.SetMonitorCallback(callback);
    }
}
