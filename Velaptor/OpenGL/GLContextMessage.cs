// <copyright file="GLContextMessage.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Silk.NET.OpenGL;

/// <summary>
/// A message that contains the OpenGL instance.
/// </summary>
[ExcludeFromCodeCoverage]
public class GLContextMessage : IMessage
{
    private readonly GL gl;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLContextMessage"/> class.
    /// </summary>
    /// <param name="gl">An OpenGL instance.</param>
    public GLContextMessage(GL gl) => this.gl = gl;

    /// <summary>
    /// Gets the OpenGL context instance.
    /// </summary>
    /// <param name="onError">The action to invoke if an exception occurs.</param>
    /// <typeparam name="T">The type to deserialize the message into.</typeparam>
    /// <returns>The deserialized message data.</returns>
    public T? GetData<T>(Action<Exception>? onError = null)
        where T : class =>
        this.gl as T;
}
