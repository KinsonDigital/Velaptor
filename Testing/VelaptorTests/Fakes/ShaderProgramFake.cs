// <copyright file="ShaderProgramFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using System;
    using Velaptor.NativeInterop.OpenGL;
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
        /// <param name="gl">Mocked <see cref="IGLInvoker"/>.</param>
        /// <param name="glExtensions">Mocked <see cref="IGLInvokerExtensions"/>.</param>
        /// <param name="shaderLoaderService">Mocked <see cref="IShaderLoaderService{TValue}"/>.</param>
        /// <param name="glInitObservable">Mocked <see cref="IObservable{T}"/>.</param>
        public ShaderProgramFake(IGLInvoker gl, IGLInvokerExtensions glExtensions, IShaderLoaderService<uint> shaderLoaderService, IObservable<bool> glInitObservable)
            : base(gl, glExtensions, shaderLoaderService, glInitObservable)
        {
        }
    }
}
