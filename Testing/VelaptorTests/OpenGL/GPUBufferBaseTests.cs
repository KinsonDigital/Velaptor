// <copyright file="GPUBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using VelaptorTests.Fakes;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of <see cref="GPUBufferBaseTests"/>.
    /// </summary>
    public class GPUBufferBaseTests : IDisposable
    {
        private const string BufferName = "UNKNOWN BUFFER";
        private const uint VertexArrayId = 1256;
        private const uint VertexBufferId = 1234;
        private const uint IndexBufferId = 5678;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly OpenGLInitObservable glInitObservable;
        private bool vertexBufferCreated;
        private bool indexBufferCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferBaseTests"/> class.
        /// </summary>
        public GPUBufferBaseTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.GenBuffer()).Returns(() =>
            {
                if (!this.vertexBufferCreated)
                {
                    this.vertexBufferCreated = true;
                    return VertexBufferId;
                }

                if (this.indexBufferCreated)
                {
                    return 0;
                }

                this.indexBufferCreated = true;
                return IndexBufferId;
            });

            this.mockGL.Setup(m => m.GenVertexArray()).Returns(VertexArrayId);

            this.glInitObservable = new OpenGLInitObservable();
        }

        #region Props Tests
        [Fact]
        public void BatchSize_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var buffer = CreateBuffer();

            // Act
            var actual = buffer.BatchSize;

            // Assert
            Assert.Equal(100u, actual);
        }

        [Fact]
        public void IsInitialized_AfterGLInitializes_ReturnsTrue()
        {
            // Arrange
            var buffer = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            Assert.True(buffer.IsInitialized);
        }
        #endregion

        #region Method Tests

        [Fact]
        public void OpenGLInit_WhenInvoked_CreatesVertexArrayObject()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGL.Verify(m => m.GenVertexArray(), Times.Once);
            this.mockGL.Verify(m => m.BindVertexArray(VertexArrayId), Times.Once);
            this.mockGL.Verify(m => m.BindVertexArray(0), Times.Once);
            this.mockGL.Verify(m => m.LabelVertexArray(VertexArrayId, BufferName));
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_CreatesVertexBufferObject()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            // These are all invoked once per quad
            this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
            this.mockGL.Verify(m => m.BindBuffer(GLBufferTarget.ArrayBuffer, VertexBufferId), Times.Once);
            this.mockGL.Verify(m => m.BindBuffer(GLBufferTarget.ArrayBuffer, 0), Times.Once);
            this.mockGL.Verify(m => m.LabelBuffer(VertexBufferId, BufferName, BufferType.VertexBufferObject));
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_CreatesElementBufferObject()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            // First invoke is done creating the Vertex Buffer, the second is the index buffer
            this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
            this.mockGL.Verify(m => m.BindBuffer(GLBufferTarget.ElementArrayBuffer, IndexBufferId), Times.Once);
            this.mockGL.Verify(m => m.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0), Times.Once);
            this.mockGL.Verify(m => m.LabelBuffer(IndexBufferId, BufferName, BufferType.IndexArrayObject));
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_GeneratesVertexData()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            Assert.True(unused.GenerateDataInvoked, $"The method '{nameof(GPUBufferBase<SpriteBatchItem>.GenerateData)}'() has not been invoked.");
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_GeneratesIndicesData()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            Assert.True(unused.GenerateIndicesInvoked, $"The method '{nameof(GPUBufferBase<SpriteBatchItem>.GenerateData)}'() has not been invoked.");
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_UploadsVertexData()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ArrayBuffer,
                new[] { 1f, 2f, 3f, 4f },
                GLBufferUsageHint.DynamicDraw),
            Times.Once);
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_UploadsIndicesData()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ElementArrayBuffer,
                    new uint[] { 11, 22, 33, 44 },
                    GLBufferUsageHint.StaticDraw),
                Times.Once);
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_SetsUpVertexArrayObject()
        {
            // Arrange
            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            Assert.True(unused.SetupVAOInvoked, $"The method '{nameof(GPUBufferBase<SpriteBatchItem>.SetupVAO)}'() has not been invoked.");
        }

        [Fact]
        public void OpenGLInit_WhenInvoked_SetsUpProperGLGrouping()
        {
            // Arrange
            var totalInvokes = 0;
            var setupDataGroupName = $"Setup {BufferName} Data";
            var setupDataGroupSequence = 0;
            var uploadVertexDataGroupName = $"Upload {BufferName} Vertex Data";
            var uploadVertexDataGroupSequence = 0;
            var uploadIndicesDataGroupName = $"Upload {BufferName} Indices Data";
            var uploadIndicesDataGroupSequence = 0;
            this.mockGL.Setup(m => m.BeginGroup(setupDataGroupName))
                .Callback(() =>
                {
                    totalInvokes += 1;
                    setupDataGroupSequence = totalInvokes;
                });
            this.mockGL.Setup(m => m.BeginGroup(uploadVertexDataGroupName))
                .Callback(() =>
                {
                    totalInvokes += 1;
                    uploadVertexDataGroupSequence = totalInvokes;
                });
            this.mockGL.Setup(m => m.BeginGroup(uploadIndicesDataGroupName))
                .Callback(() =>
                {
                    totalInvokes += 1;
                    uploadIndicesDataGroupSequence = totalInvokes;
                });

            var unused = CreateBuffer();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGL.Verify(m => m.BeginGroup(It.IsAny<string>()), Times.Exactly(3));
            this.mockGL.Verify(m => m.BeginGroup(setupDataGroupName), Times.Once);
            this.mockGL.Verify(m => m.BeginGroup(uploadVertexDataGroupName), Times.Once);
            this.mockGL.Verify(m => m.BeginGroup(uploadIndicesDataGroupName), Times.Once);
            this.mockGL.Verify(m => m.EndGroup(), Times.Exactly(3));

            // Check that the setup data group was called first
            Assert.Equal(1, setupDataGroupSequence);
            Assert.Equal(2, uploadVertexDataGroupSequence);
            Assert.Equal(3, uploadIndicesDataGroupSequence);
        }

        [Fact]
        public void UploadData_WhenInvoked_PreparesGPUForDataUpload()
        {
            // Arrange
            var buffer = CreateBuffer();
            var batchItem = default(SpriteBatchItem);

            // Act
            buffer.UploadData(batchItem, 0u);

            // Assert
            Assert.True(buffer.PrepareForUseInvoked, $"The method '{nameof(GPUBufferBase<SpriteBatchItem>.PrepareForUpload)}'() has not been invoked.");
        }

        [Fact]
        public void UploadData_WhenInvoked_UpdatesGPUData()
        {
            // Arrange
            var buffer = CreateBuffer();
            var batchItem = default(SpriteBatchItem);

            // Act
            buffer.UploadData(batchItem, 0u);

            // Assert
            Assert.True(buffer.UpdateVertexDataInvoked, $"The method '{nameof(GPUBufferBase<SpriteBatchItem>.UploadVertexData)}'() has not been invoked.");
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DisposesOfUnmanagedResources()
        {
            // Arrange
            var buffer = CreateBuffer();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            buffer.Dispose();
            buffer.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteVertexArray(VertexArrayId), Times.Once());
            this.mockGL.Verify(m => m.DeleteBuffer(VertexBufferId), Times.Once());
            this.mockGL.Verify(m => m.DeleteBuffer(IndexBufferId), Times.Once());
        }
        #endregion

        /// <inheritdoc />
        public void Dispose() => this.glInitObservable.Dispose();

        /// <summary>
        /// Creates an instance of the type <see cref="GPUBufferFake"/> for the purpose of
        /// testing the abstract class <see cref="GPUBufferBase{TData}"/>.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private GPUBufferFake CreateBuffer() => new (this.mockGL.Object, this.glInitObservable);
    }
}
