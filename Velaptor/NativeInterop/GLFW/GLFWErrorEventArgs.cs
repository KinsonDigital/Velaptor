// <copyright file="GLFWErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds GLFW related error information.
    /// </summary>
    internal class GLFWErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GLFWErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public GLFWErrorEventArgs(GLFWErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the error code of the error.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
        public GLFWErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Used by library users")]
        public string ErrorMessage { get; }
    }
}
