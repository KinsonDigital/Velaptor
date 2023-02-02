// <copyright file="FontShader.cs" company="KinsonDigital">
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
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public FontShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService shaderLoaderService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, shaderLoaderService, reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var reactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId));
        this.unsubscriber = reactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: PushNotifications.BatchSizeChangedId,
            name: batchSizeName,
            onReceiveData: data =>
            {
                if (data.TypeOfBatch == BatchType.Font)
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

        if (this.fontTextureUniformLocation < 0)
        {
            this.fontTextureUniformLocation = GL.GetUniformLocation(ShaderId, "fontTexture");
        }

        GL.ActiveTexture(GLTextureUnit.Texture1);
        GL.Uniform1(this.fontTextureUniformLocation, 1);
    }
}
