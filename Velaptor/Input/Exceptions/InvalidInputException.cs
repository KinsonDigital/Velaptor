﻿// <copyright file="InvalidInputException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions;

using System;

/// <summary>
/// Occurs when invalid input has occured.
/// </summary>
public sealed class InvalidInputException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
    /// </summary>
    public InvalidInputException()
        : base("Invalid Input")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidInputException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public InvalidInputException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
