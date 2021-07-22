// <copyright file="OpenTKGLFWInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// OPENTK

namespace Raptor.NativeInterop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using Raptor.OpenGL;

    /// <summary>
    /// Invokes GLFW calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class OpenTKGLFWInvoker : IGLFWInvoker
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTKGLFWInvoker"/> class.
        /// </summary>
        public OpenTKGLFWInvoker()
        {
            GLFW.SetErrorCallback(ErrorCallback);

            unsafe
            {
                GLFW.SetMonitorCallback(MonitorCallback);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="OpenTKGLFWInvoker"/> class.
        /// </summary>
        ~OpenTKGLFWInvoker()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public event EventHandler<GLFWErrorEventArgs>? OnError;

        /// <inheritdoc/>
        public event EventHandler<GLFWMonitorChangedEventArgs>? OnMonitorChanged;

        /// <inheritdoc/>
        public bool Init() => GLFW.Init();

        /// <inheritdoc/>
        public IntPtr[] GetMonitors()
        {
            var pointers = new List<IntPtr>();

            unsafe
            {
                var unsafeMonitorPointers = GLFW.GetMonitors();

                foreach (var unsafePtr in unsafeMonitorPointers)
                {
                    pointers.Add((IntPtr)unsafePtr);
                }
            }

            return pointers.ToArray();
        }

        /// <inheritdoc/>
        public Vector2 GetMonitorContentScale(IntPtr monitor)
        {
            unsafe
            {
                GLFW.GetMonitorContentScale((Monitor*)monitor, out var scaleX, out var scaleY);

                return new Vector2(scaleX, scaleY);
            }
        }

        /// <inheritdoc/>
        public GLFWVideoMode GetVideoMode(IntPtr monitor)
        {
            GLFWVideoMode result = default;

            unsafe
            {
                var pVideoMode = GLFW.GetVideoMode((Monitor*)monitor);

                result.RedBits = pVideoMode->RedBits;
                result.GreenBits = pVideoMode->RedBits;
                result.BlueBits = pVideoMode->RedBits;
                result.Width = pVideoMode->Width;
                result.Height = pVideoMode->Height;
                result.RefreshRate = pVideoMode->RefreshRate;
            }

            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                GLFW.SetErrorCallback(null);

                unsafe
                {
                    GLFW.SetMonitorCallback(null);
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Invoked when GLFW has an error and if so, invokes the <see cref="OnError"/> event.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="description">The error description.</param>
        private void ErrorCallback(ErrorCode errorCode, string description)
            => OnError?.Invoke(this, new GLFWErrorEventArgs((GLFWErrorCode)errorCode, description));

        /// <summary>
        /// Invoked when GLFW detects that something has changed with the monitors,
        /// and then invokes the <see cref="OnMonitorChanged"/> event.
        /// </summary>
        /// <param name="monitor">The monitor that has changed.</param>
        /// <param name="connectedState">The connected state of the monitor.</param>
        private unsafe void MonitorCallback(Monitor* monitor, ConnectedState connectedState)
            => OnMonitorChanged?.Invoke(this, new GLFWMonitorChangedEventArgs(connectedState == ConnectedState.Connected));
    }
}
