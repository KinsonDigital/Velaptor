// <copyright file="SceneDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene.Exceptions;
using System;

/// <summary>
/// Thrown when a scene does not exist.
/// </summary>
public sealed class SceneDoesNotExistException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneDoesNotExistException"/> class.
    /// </summary>
    public SceneDoesNotExistException()
        : base("The scene does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneDoesNotExistException"/> class.
    /// </summary>
    /// <param name="id">The scene ID.</param>
    public SceneDoesNotExistException(Guid id)
        : base(CreateMessage(id))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SceneDoesNotExistException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public SceneDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates the exception message using the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id of the scene.</param>
    /// <returns>The exception message.</returns>
    private static string CreateMessage(Guid id) => $"The scene with the ID '{id}' does not exist.";
}
