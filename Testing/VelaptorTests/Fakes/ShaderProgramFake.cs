// <copyright file="ShaderProgramFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;

/// <summary>
/// Used to test the abstract class <see cref="ShaderProgram"/>.
/// </summary>
internal sealed class ShaderProgramFake : ShaderProgram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderProgramFake"/> class for the purpose of testing.
    /// </summary>
    /// <param name="gl">Mocked <see cref="IGLInvoker"/> for calling OpenGL functions.</param>
    /// <param name="openGLService">Mocked <see cref="IOpenGLService"/> for calling OpenGL functions.</param>
    /// <param name="shaderLoaderService">Mocked <see cref="IShaderLoaderService{TValue}"/> for loading shader code.</param>
    /// <param name="reactableFactory">Mocked <see cref="IReactableFactory"/> for creating reactables.</param>
    public ShaderProgramFake(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService shaderLoaderService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, shaderLoaderService, reactableFactory)
    {
    }
}
