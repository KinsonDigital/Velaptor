// <copyright file="LineShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate.OneWay;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using ReactableData;
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
        EnsureThat.ParamIsNotNull(reactableFactory);

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId));
        this.unsubscriber = batchSizeReactable.Subscribe(new ReceiveSubscription<BatchSizeData>(
            id: PushNotifications.BatchSizeChangedId,
            name: batchSizeName,
            onReceive: data =>
            {
                if (data.TypeOfBatch == BatchType.Line)
                {
                    BatchSize = data.BatchSize;
                }
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }
}
