// <copyright file="TextureShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate;
using Guards;
using NativeInterop.OpenGL;
using ReactableData;
using Services;
using Velaptor.Exceptions;

/// <summary>
/// A texture shader used to render 2D textures.
/// </summary>
[ShaderName("Texture")]
internal sealed class TextureShader : ShaderProgram
{
    private readonly IDisposable unsubscriber;
    private int mainTextureUniformLocation = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public TextureShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IPushReactable reactable)
            : base(gl, openGLService, shaderLoaderService, reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        var batchSizeName = this.GetExecutionMemberName(nameof(NotificationIds.BatchSizeSetId));
        this.unsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.BatchSizeSetId,
            name: batchSizeName,
            onReceiveMsg: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException($"{nameof(TextureShader)}.Constructor()", NotificationIds.BatchSizeSetId);
                }

                BatchSize = batchSize.Value;
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }

    /// <inheritdoc/>
    public override void Use()
    {
        base.Use();

        if (this.mainTextureUniformLocation < 0)
        {
            this.mainTextureUniformLocation = GL.GetUniformLocation(ShaderId, "mainTexture");
        }

        GL.ActiveTexture(GLTextureUnit.Texture0);
        GL.Uniform1(this.mainTextureUniformLocation, 0);
    }
}
