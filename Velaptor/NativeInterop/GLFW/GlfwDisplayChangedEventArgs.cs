// <copyright file="GlfwDisplayChangedEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds GLFW display changed event information.
/// </summary>
internal sealed class GlfwDisplayChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwDisplayChangedEventArgs"/> class.
    /// </summary>
    /// <param name="isConnected">True if a display has been connected.</param>
    public GlfwDisplayChangedEventArgs(bool isConnected) => IsConnected = isConnected;

    /// <summary>
    /// Gets a value indicating whether or not a display has been connected.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
    public bool IsConnected { get; }
}
