// <copyright file="LoadAtlasException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue loading atlas data content.
/// </summary>
public class LoadAtlasException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAtlasException"/> class.
    /// </summary>
    public LoadAtlasException()
        : base($"There was an issue loading the atlas data.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAtlasException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoadAtlasException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadAtlasException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public LoadAtlasException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
