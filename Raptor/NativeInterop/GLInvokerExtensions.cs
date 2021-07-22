// <copyright file="GLInvokerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.NativeInterop
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;
    using Raptor.OpenGL;

    /// <summary>
    /// Provides OpenTK extensions/helper methods to improve OpenGL related functionality.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class GLInvokerExtensions : IGLInvokerExtensions
    {
        private readonly IGLInvoker glInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLInvokerExtensions"/> class.
        /// </summary>
        /// <param name="glInvoker">Invokes OpenGL functions.</param>
        public GLInvokerExtensions(IGLInvoker glInvoker) => this.glInvoker = glInvoker;

        /// <inheritdoc/>
        public Size GetViewPortSize()
        {
            /*
             * [0] = X
             * [1] = Y
             * [3] = Width
             * [4] = Height
             */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            return new Size(data[2], data[3]);
        }

        /// <inheritdoc/>
        public void SetViewPortSize(Size size)
        {
            /*
             * [0] = X
             * [1] = Y
             * [3] = Width
             * [4] = Height
             */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            this.glInvoker.Viewport(data[0], data[1], (uint)size.Width, (uint)size.Height);
        }

        /// <inheritdoc/>
        public Vector2 GetViewPortPosition()
        {
            /*
           * [0] = X
           * [1] = Y
           * [3] = Width
           * [4] = Height
           */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            return new Vector2(data[0], data[1]);
        }

        /// <inheritdoc/>
        public bool LinkProgramSuccess(uint program)
        {
            this.glInvoker.GetProgram(program, GLProgramParameterName.LinkStatus, out var programParams);

            return programParams >= 1;
        }

        /// <inheritdoc/>
        public bool ShaderCompileSuccess(uint shaderID)
        {
            this.glInvoker.GetShader(shaderID, GLShaderParameter.CompileStatus, out var shaderParams);

            return shaderParams >= 1;
        }
    }
}
