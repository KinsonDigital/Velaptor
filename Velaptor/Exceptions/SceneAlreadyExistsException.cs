// <copyright file="SceneAlreadyExistsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when a scene already exists.
/// </summary>
[Serializable]
public sealed class SceneAlreadyExistsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneAlreadyExistsException"/> class.
    /// </summary>
    public SceneAlreadyExistsException()
        : base("The scene already exists.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="name">The name of the scene.</param>
    /// <param name="id">The scene ID.</param>
    public SceneAlreadyExistsException(string name, Guid id)
        : base(CreateMessage(name, id))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SceneAlreadyExistsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public SceneAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private SceneAlreadyExistsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Creates the exception message from the given scene <paramref name="name"/> and <paramref name="id"/>.
    /// </summary>
    /// <param name="name">The name of the scene.</param>
    /// <param name="id">The id of the scene.</param>
    /// <returns>The exception message.</returns>
    private static string CreateMessage(string name, Guid id)
    {
        var midSection = string.IsNullOrEmpty(name)
            ? $"with the ID '{id}'"
            : $"'{name}' with the ID '{id}'";

        return $"The scene {midSection} already exists.";
    }
}
