// <copyright file="ShaderProgramFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;

    /// <summary>
    /// Used to test the abstract class <see cref="ShaderProgram"/>.
    /// </summary>
    internal class ShaderProgramFake : ShaderProgram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramFake"/> class for the purpose of testing.
        /// </summary>
        /// <param name="gl">Mocked <see cref="IGLInvoker"/> for calling OpenGL functions.</param>
        /// <param name="glExtensions">Mocked <see cref="IGLInvokerExtensions"/> for calling OpenGL functions.</param>
        /// <param name="shaderLoaderService">Mocked <see cref="IShaderLoaderService{TValue}"/> for loading shader code.</param>
        /// <param name="glInitReactable">Mocked <see cref="IReactable{T}"/> for OpenGL initialization..</param>
        /// <param name="shutDownReactable">Mocked <see cref="IReactable{T}"/> for shutdown notifications.</param>
        public ShaderProgramFake(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
            : base(gl, glExtensions, shaderLoaderService, glInitReactable, shutDownReactable)
        {
        }
    }
}
