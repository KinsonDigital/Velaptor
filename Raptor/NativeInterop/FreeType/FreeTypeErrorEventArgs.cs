// <copyright file="FreeTypeErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.NativeInterop.FreeType
{
    using System;

    /// <summary>
    /// Occurs when there is an error message related to the FreeType font library.
    /// </summary>
    internal class FreeTypeErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTypeErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public FreeTypeErrorEventArgs(string errorMessage) => ErrorMessage = errorMessage;

        /// <summary>
        /// Gets the error message that occured.
        /// </summary>
        public string ErrorMessage { get; private set; } = "A FreeType error has occured";
    }
}
