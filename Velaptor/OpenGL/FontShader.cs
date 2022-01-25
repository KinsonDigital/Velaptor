// <copyright file="FontShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Services;
    using VelObservable = Velaptor.Observables.Core.IObservable<bool>;

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
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
        /// <param name="glInitObservable">Receives a notification when OpenGL has been initialized.</param>
        /// <param name="shutDownObservable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        public FontShader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            VelObservable glInitObservable,
            VelObservable shutDownObservable)
            : base(gl, glExtensions, shaderLoaderService, glInitObservable, shutDownObservable)
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
