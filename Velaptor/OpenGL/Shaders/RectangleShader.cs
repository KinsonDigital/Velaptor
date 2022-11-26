// <copyright file="RectangleShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Velaptor.NativeInterop.OpenGL;
using Services;
using Reactables.Core;
using Reactables.ReactableData;

/// <summary>
/// A rectangle shader used to render 2D rectangles.
/// </summary>
[ShaderName("Rectangle")]
internal sealed class RectangleShader : ShaderProgram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
    /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public RectangleShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IReactable<GLInitData> glInitReactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, shaderLoaderService, glInitReactable, shutDownReactable)
    {
    }
}
