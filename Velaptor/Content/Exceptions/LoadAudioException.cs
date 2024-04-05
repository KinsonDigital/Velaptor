// <copyright file="LoadAudioException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue loading audio.
/// </summary>
public sealed class LoadAudioException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAudioException"/> class.
    /// </summary>
    public LoadAudioException()
        : base("There was an issue loading the audio.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAudioException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoadAudioException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAudioException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public LoadAudioException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
