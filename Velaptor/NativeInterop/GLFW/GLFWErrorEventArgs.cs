// <copyright file="GLFWErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW
{
    using System;
    using Velaptor.OpenGL;

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
        public GLFWErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; private set; }
    }
}
