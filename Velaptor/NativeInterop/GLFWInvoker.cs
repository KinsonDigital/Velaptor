// <copyright file="GLFWInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// TODO: Move GLFW types to a GLFW folder and namespace

namespace Velaptor.NativeInterop
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using Silk.NET.GLFW;
    using Velaptor.OpenGL;

    /// <summary>
    /// Invokes GLFW calls.
    /// </summary>
    internal class GLFWInvoker : IGLFWInvoker
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
        ~GLFWInvoker()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public event EventHandler<GLFWErrorEventArgs>? OnError;

        /// <inheritdoc/>
        public event EventHandler<GLFWMonitorChangedEventArgs>? OnMonitorChanged;

        /// <inheritdoc/>
        public bool Init() => this.glfw.Init();

        /// <inheritdoc/>
        public IntPtr[] GetMonitors()
        {
            var result = new List<IntPtr>();

            unsafe
            {
                var unsafeMonitorPointers = this.glfw.GetMonitors(out var count);

                for (var i = 0; i < count; i++)
                {
                    result.Add((IntPtr)unsafeMonitorPointers[i]);
                }

            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        public Vector2 GetMonitorContentScale(IntPtr monitor)
        {
            unsafe
            {
                this.glfw.GetMonitorContentScale((Monitor*)monitor, out var scaleX, out var scaleY);

                return new Vector2(scaleX, scaleY);
            }
        }

        /// <inheritdoc/>
        public GLFWVideoMode GetVideoMode(IntPtr monitor)
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
                this.glfw.Dispose();
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
