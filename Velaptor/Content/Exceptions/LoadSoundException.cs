// <copyright file="LoadSoundException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when there is an issue loading sounds.
/// </summary>
[Serializable]
public sealed class LoadSoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadSoundException"/> class.
    /// </summary>
    public LoadSoundException()
        : base("There was an issue loading the sound.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadSoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoadSoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadSoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public LoadSoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadSoundException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private LoadSoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
