// <copyright file="GlfwErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds GLFW related error information.
/// </summary>
internal sealed class GlfwErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwErrorEventArgs"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorMessage">The error message.</param>
    public GlfwErrorEventArgs(GlfwErrorCode errorCode, string errorMessage)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorMessage);
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the error code of the error.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for users.")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
    public GlfwErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for users.")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
    public string ErrorMessage { get; }
}
