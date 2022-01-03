// <copyright file="GLFWMonitorChangedEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds GLFW monitor changed event information.
    /// </summary>
    internal class GLFWMonitorChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GLFWMonitorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isConnected">True if a monitor has been connected.</param>
        public GLFWMonitorChangedEventArgs(bool isConnected) => IsConnected = isConnected;

        /// <summary>
        /// Gets a value indicating whether or not a monitor has been connected.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
        public bool IsConnected { get; }
    }
}
