// <copyright file="AppSettingsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when there is an issue loading the application settings.
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private AppSettingsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
