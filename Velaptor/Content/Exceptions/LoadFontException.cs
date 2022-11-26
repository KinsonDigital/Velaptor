// <copyright file="LoadFontException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue loading fonts.
/// </summary>
public class LoadFontException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadFontException"/> class.
    /// </summary>
    public LoadFontException()
        : base($"There was an issue loading the font.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadFontException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoadFontException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadFontException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public LoadFontException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
