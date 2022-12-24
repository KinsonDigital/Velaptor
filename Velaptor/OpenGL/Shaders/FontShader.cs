// <copyright file="FontShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate;
using Guards;
using Velaptor.NativeInterop.OpenGL;
using Services;
using Reactables.Core;
using Reactables.ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// A shader used to render text of a particular font.
/// </summary>
[ShaderName("Font")]
internal sealed class FontShader : ShaderProgram
{
    private readonly IDisposable unsubscriber;
    private int fontTextureUniformLocation = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public FontShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IReactable reactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, shaderLoaderService, reactable, shutDownReactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.unsubscriber = reactable.Subscribe(new Reactor(
            eventId: NotificationIds.BatchSizeId,
            onNextMsg: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException($"{nameof(FontShader)}.Constructor()", NotificationIds.BatchSizeId);
                }

                BatchSize = batchSize.Value;
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }

    /// <inheritdoc/>
    public override void Use()
    {
        base.Use();

        if (this.fontTextureUniformLocation < 0)
        {
            this.fontTextureUniformLocation = GL.GetUniformLocation(ShaderId, "fontTexture");
        }

        GL.ActiveTexture(GLTextureUnit.Texture1);
        GL.Uniform1(this.fontTextureUniformLocation, 1);
    }
}
