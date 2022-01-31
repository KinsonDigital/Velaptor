// <copyright file="TextureShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Services;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// A texture shader used to render 2D textures.
    /// </summary>
    [ShaderName("Texture")]
    internal class TextureShader : ShaderProgram
    {
        // private readonly IDisposable shutDownReactorUnsubscriber;
        private int mainTextureUniformLocation = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureShader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
        /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
        /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        public TextureShader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
            : base(gl, glExtensions, shaderLoaderService, glInitReactable, shutDownReactable)
        {
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
}
