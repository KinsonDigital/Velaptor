// <copyright file="GLContextMessage.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.Core;
using Silk.NET.OpenGL;

/// <summary>
/// A message that contains the OpenGL instance.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the SILK library.")]
public class GLContextMessage : IMessage<GL>
{
    private readonly GL gl;

    /// <summary>
    /// Initializes a new instance of the <see cref="GLContextMessage"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    public GLContextMessage(GL gl) => this.gl = gl;

    /// <summary>
    /// Gets the OpenGL context instance.
    /// </summary>
    /// <param name="onError">The action to invoke if an exception occurs.</param>
    /// <returns>The deserialized message data.</returns>
    public GL GetData(Action<Exception>? onError = null) => this.gl;
}
