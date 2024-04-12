// <copyright file="LineShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate;
using Factories;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using Services;

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
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public LineShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService shaderLoaderService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, shaderLoaderService, reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(reactableFactory);

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        // Subscribe to batch size changes
        this.unsubscriber = batchSizeReactable.CreateOneWayReceive(
            PushNotifications.BatchSizeChangedId,
            (data) =>
            {
                if (data.TypeOfBatch == BatchType.Line)
                {
                    BatchSize = data.BatchSize;
                }
            },
            () => this.unsubscriber?.Dispose());
    }
}
