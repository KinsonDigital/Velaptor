// <copyright file="AppSettingsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions
{
    // ReSharper disable RedundantNameQualifier
    using System;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Thrown when there is an issue loading the application settings.
    /// </summary>
    public sealed class AppSettingsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsException"/> class.
        /// </summary>
        public AppSettingsException()
            : base("There was an issue loading the application settings.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AppSettingsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The <see cref="Exception"/> instance that caused the current exception.
        /// </param>
        public AppSettingsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
