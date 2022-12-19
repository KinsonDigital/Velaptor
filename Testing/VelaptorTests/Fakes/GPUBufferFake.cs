// <copyright file="GPUBufferFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Used to test the abstract class <see cref="GPUBufferBase{TData}"/>.
/// </summary>
internal sealed class GPUBufferFake : GPUBufferBase<TextureBatchItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GPUBufferFake"/> class for the purpose of testing.
    /// </summary>
    /// <param name="gl">Mocked <see cref="IGLInvoker"/> for OpenGL function calls.</param>
    /// <param name="openGLService">Mocked <see cref="IOpenGLService"/> for OpenGL function calls.</param>
    /// <param name="glInitReactable">Mocked <see cref="IReactable{T}"/> for OpenGL initialization.</param>
    /// <param name="shutDownReactable">Mocked <see cref="IReactable{T}"/> for application shutdown..</param>
    public GPUBufferFake(IGLInvoker gl,
        IOpenGLService openGLService,
        IReactable<GLInitData> glInitReactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, glInitReactable, shutDownReactable)
    {
    }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="SetupVAO"/>() method has been invoked.
    /// </summary>
    /// <remarks>Used for unit testing.</remarks>
    public bool SetupVAOInvoked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="GenerateData"/>() method has been invoked.
    /// </summary>
    /// <remarks>Used for unit testing.</remarks>
    public bool GenerateDataInvoked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="PrepareForUpload"/>() method has been invoked.
    /// </summary>
    /// <remarks>Used for unit testing.</remarks>
    public bool PrepareForUseInvoked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="GenerateIndices"/>() method has been invoked.
    /// </summary>
    /// <remarks>Used for unit testing.</remarks>
    public bool GenerateIndicesInvoked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="UploadVertexData"/>() method has been invoked.
    /// </summary>
    /// <remarks>Used for unit testing.</remarks>
    public bool UpdateVertexDataInvoked { get; private set; }

    /// <summary>
    /// Set the <see cref="SetupVAOInvoked"/> to true to simulate that the VAO has been setup.
    /// </summary>
    protected internal override void SetupVAO() => SetupVAOInvoked = true;

    /// <summary>
    /// Set the <see cref="UpdateVertexDataInvoked"/> to true to simulate that the vertex data has been updated.
    /// </summary>
    /// <param name="data">The fake data to use for the test.</param>
    /// <param name="batchIndex">The fake batch index to use for the text.</param>
    protected internal override void UploadVertexData(TextureBatchItem data, uint batchIndex)
        => UpdateVertexDataInvoked = true;

    /// <summary>
    /// Sets the <see cref="PrepareForUseInvoked"/> to true to simulate that the method has been invoked.
    /// </summary>
    protected internal override void PrepareForUpload() => PrepareForUseInvoked = true;

    /// <summary>
    /// Sets the <see cref="GenerateDataInvoked"/> to true to simulate that the method has
    /// been invoked and returns fake data.
    /// </summary>
    /// <returns>The data to use for testing.</returns>
    protected internal override float[] GenerateData()
    {
        GenerateDataInvoked = true;
        return new[] { 1f, 2f, 3f, 4f };
    }

    /// <summary>
    /// Sets the <see cref="GenerateIndicesInvoked"/> to true to simulate that the method has
    /// been invoked and returns fake data.
    /// </summary>
    /// <returns>The data to use for testing.</returns>
    protected internal override uint[] GenerateIndices()
    {
        GenerateIndicesInvoked = true;
        return new uint[] { 11, 22, 33, 44 };
    }
}
