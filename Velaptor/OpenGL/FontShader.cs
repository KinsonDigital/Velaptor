// <copyright file="FontShader.cs" company="KinsonDigital">
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
    /// A shader used to render text of a particular font.
    /// </summary>
    [ShaderName("Font")]
    internal class FontShader : ShaderProgram
    {
        private int fontTextureUniformLocation = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontShader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="openGLService">Provides OpenGL related helper methods.</param>
        /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
        /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
        /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        public FontShader(
            IGLInvoker gl,
            IOpenGLService openGLService,
            IShaderLoaderService<uint> shaderLoaderService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
            : base(gl, openGLService, shaderLoaderService, glInitReactable, shutDownReactable)
        {
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
}
