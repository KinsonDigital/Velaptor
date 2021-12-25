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
        public ShaderProgramFake(IGLInvoker gl, IShaderLoaderService<uint> shaderLoaderService, IObservable<bool> glInitObservable)
            : base(gl, shaderLoaderService, glInitObservable)
        {
        }
    }
}
