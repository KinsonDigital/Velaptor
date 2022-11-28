// <copyright file="EnumOutOfRangeException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;
using Graphics;

/// <summary>
/// Thrown when an invalid <see cref="RenderEffects"/> value is used.
/// </summary>
public class EnumOutOfRangeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumOutOfRangeException"/> class.
    /// </summary>
    public EnumOutOfRangeException()
        : base("The enum value is invalid because it is out of range.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumOutOfRangeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EnumOutOfRangeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumOutOfRangeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public EnumOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
