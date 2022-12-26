// <copyright file="RectangleShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate;
using Guards;
using Velaptor.NativeInterop.OpenGL;
using Services;
using Reactables.ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// A rectangle shader used to render 2D rectangles.
/// </summary>
[ShaderName("Rectangle")]
internal sealed class RectangleShader : ShaderProgram
{
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public RectangleShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IReactable reactable)
            : base(gl, openGLService, shaderLoaderService, reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.unsubscriber = reactable.Subscribe(new Reactor(
            eventId: NotificationIds.BatchSizeId,
            onNextMsg: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException($"{nameof(RectangleShader)}.Constructor()", NotificationIds.BatchSizeId);
                }

                BatchSize = batchSize.Value;
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }
}
