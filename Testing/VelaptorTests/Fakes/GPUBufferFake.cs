// <copyright file="GPUBufferFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;

    /// <summary>
    /// Used to test the abstract class <see cref="GPUBufferBase{TData}"/>.
    /// </summary>
    internal class GPUBufferFake : GPUBufferBase<SpriteBatchItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferFake"/> class for the purpose of testing.
        /// </summary>
        /// <param name="gl">Mocked <see cref="IGLInvoker"/>.</param>
        /// <param name="glExtensions">Mocked <see cref="IGLInvokerExtensions"/>.</param>
        /// <param name="glInitObservable">Invokes initialization.</param>
        public GPUBufferFake(IGLInvoker gl, IGLInvokerExtensions glExtensions, OpenGLInitObservable glInitObservable)
            : base(gl, glExtensions, glInitObservable)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SetupVAO"/>() method has been invoked.
        /// </summary>
        /// <remarks>Used for unit testing.</remarks>
        public bool SetupVAOInvoked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GenerateData"/>() method has been invoked.
        /// </summary>
        /// <remarks>Used for unit testing.</remarks>
        public bool GenerateDataInvoked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PrepareForUpload"/>() method has been invoked.
        /// </summary>
        /// <remarks>Used for unit testing.</remarks>
        public bool PrepareForUseInvoked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GenerateIndices"/>() method has been invoked.
        /// </summary>
        /// <remarks>Used for unit testing.</remarks>
        public bool GenerateIndicesInvoked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="UploadVertexData"/>() method has been invoked.
        /// </summary>
        /// <remarks>Used for unit testing.</remarks>
        public bool UpdateVertexDataInvoked { get; private set; }

        protected internal override void SetupVAO() => SetupVAOInvoked = true;

        protected internal override void UploadVertexData(SpriteBatchItem data, uint batchIndex)
            => UpdateVertexDataInvoked = true;

        protected internal override void PrepareForUpload() => PrepareForUseInvoked = true;

        protected internal override float[] GenerateData()
        {
            GenerateDataInvoked = true;
            return new[] { 1f, 2f, 3f, 4f };
        }

        protected internal override uint[] GenerateIndices()
        {
            GenerateIndicesInvoked = true;
            return new uint[] { 11, 22, 33, 44 };
        }
    }
}
