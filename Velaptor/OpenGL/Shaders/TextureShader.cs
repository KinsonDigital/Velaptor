// <copyright file="TextureShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate.UniDirectional;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using ReactableData;
using Services;

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
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public TextureShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService shaderLoaderService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, shaderLoaderService, reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId));
        this.unsubscriber = batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: PushNotifications.BatchSizeChangedId,
            name: batchSizeName,
            onReceiveData: data =>
            {
                if (data.TypeOfBatch == BatchType.Texture)
                {
                    BatchSize = data.BatchSize;
                }
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
