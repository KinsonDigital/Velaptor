﻿// <copyright file="LineShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Guards;
using Velaptor.NativeInterop.OpenGL;
using Services;
using Reactables.Core;
using Reactables.ReactableData;

/// <summary>
/// A line shader used to render 2D lines.
/// </summary>
[ShaderName("Line")]
internal sealed class LineShader : ShaderProgram
{
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
    /// <param name="batchSizeReactable">Receives a push notification about the batch size.</param>
    /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public LineShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IReactable<GLInitData> glInitReactable,
        IReactable<BatchSizeData> batchSizeReactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, shaderLoaderService, glInitReactable, shutDownReactable)
    {
        EnsureThat.ParamIsNotNull(batchSizeReactable);

        this.unsubscriber = batchSizeReactable.Subscribe(new Reactor<BatchSizeData>(
            onNext: data =>
            {
                BatchSize = data.BatchSize;
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }
}