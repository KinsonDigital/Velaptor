// <copyright file="ShaderProgramFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using System;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using VelObservable = Velaptor.Observables.Core.IObservable<bool>;

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
        /// <param name="glInitObservable">Mocked <see cref="IObservable{T}"/> for OpenGL initialization..</param>
        /// <param name="shutDownObservable">Mocks <see cref="IObservable{T}"/> for shutting down the application.</param>
        public ShaderProgramFake(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            VelObservable glInitObservable,
            VelObservable shutDownObservable)
            : base(gl, glExtensions, shaderLoaderService, glInitObservable, shutDownObservable)
        {
        }
    }
}
