// <copyright file="GPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using Raptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of <see cref="GPUBufferTests"/>.
    /// </summary>
    public class GPUBufferTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly uint vertexArrayID = 1256;
        private readonly uint vertexBufferID = 1234;
        private readonly uint indexBufferID = 5678;
        private bool vertexBufferCreated;
        private bool indexBufferCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferTests"/> class.
        /// </summary>
        public GPUBufferTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.GenBuffer()).Returns(() =>
            {
                if (!this.vertexBufferCreated)
                {
                    this.vertexBufferCreated = true;
                    return this.vertexBufferID;
                }

                if (!this.indexBufferCreated)
                {
                    this.indexBufferCreated = true;
                    return this.indexBufferID;
                }

                return 0;
            });
            this.mockGL.Setup(m => m.GenVertexArray()).Returns(this.vertexArrayID);
        }

        #region Props Tests
        [Fact]
        public void TotalQuads_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
#pragma warning disable IDE0017 // Simplify object initialization
            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object);
#pragma warning restore IDE0017 // Simplify object initialization

            // Act
            buffer.TotalQuads = 5;

            // Assert
            Assert.Equal(5u, buffer.TotalQuads);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Init_WhenInvoked_CreatesIndexBuffer()
        {
            // Arrange
            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object);

            // Act
            buffer.Init();

            // Assert
            // First invoke is done creating the Vertex Buffer, the second is the index buffer
            this.mockGL.Verify(m => m.GenBuffer(), Times.Exactly(2));
            this.mockGL.Verify(m => m.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferID), Times.Exactly(2));
            this.mockGL.Verify(m => m.BindBuffer(BufferTarget.ElementArrayBuffer, 0), Times.Exactly(1));
        }

        [Fact]
        public void Init_WhenInvoked_CreatesVertexBuffer()
        {
            // Arrange
            var totalQuadBytes = 320u; // 160 bytes per quad * 2 quads = 320 bytes

            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object)
            {
                TotalQuads = 2,
            };

            // Act
            buffer.Init();

            // Assert
            // These are all invoked once per quad
            this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce());

            this.mockGL.Verify(m => m.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferID), Times.AtLeast(2));
            this.mockGL.Verify(m => m.BufferData(BufferTarget.ArrayBuffer, totalQuadBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw), Times.AtLeastOnce());
            this.mockGL.Verify(m => m.BindBuffer(BufferTarget.ArrayBuffer, 0), Times.AtLeast(2));
        }

        [Fact]
        public void Init_WhenInvoking_SetsUpAttributes()
        {
            // Arrange
            var invokeCount = typeof(VertexData).GetFields().Length;
            var actualIndices = new List<uint>();
            var actualSizes = new List<uint>();
            var actualOffsets = new List<uint>();

            // Collect the various parameter values for assert comparisons later
            this.mockGL.Setup(m => m.VertexAttribPointer(It.IsAny<uint>(), It.IsAny<uint>(), VertexAttribPointerType.Float, false, It.IsAny<uint>(), It.IsAny<uint>()))
                .Callback<uint, uint, VertexAttribPointerType, bool, uint, uint>((index, size, type, normalized, stride, offset) =>
                {
                    actualIndices.Add(index);
                    actualSizes.Add(size);
                    actualOffsets.Add(offset);
                });

            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object);

            // Act
            buffer.Init();

            // Assert
            this.mockGL.Verify(m => m.EnableVertexArrayAttrib(this.vertexArrayID, It.IsAny<uint>()), Times.Exactly(invokeCount));
            this.mockGL.Verify(m => m.VertexAttribPointer(
                It.IsAny<uint>(),
                It.IsAny<uint>(),
                VertexAttribPointerType.Float,
                false,
                40,
                It.IsAny<uint>()),
            Times.Exactly(invokeCount));

            Assert.Equal(new uint[] { 0, 1, 2, 3 }, actualIndices.ToArray());
            Assert.Equal(new uint[] { 3, 2, 4, 1 }, actualSizes.ToArray());
            Assert.Equal(new uint[] { 0, 12, 20, 36 }, actualOffsets.ToArray());
        }

        [Fact]
        public void UpdateQuad_WhenInvoked_UpdatesGPUVertexBuffer()
        {
            // Arrange
            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object);
            var srcRect = default(Rectangle);

            // Act
            buffer.UpdateQuad(0, srcRect, 50, 50, Color.White);

            // Assert
            this.mockGL.Verify(m => m.BufferSubData(BufferTarget.ArrayBuffer, It.IsAny<IntPtr>(), It.IsAny<uint>(), ref It.Ref<QuadData>.IsAny));
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DisposesOfUnmanagedResources()
        {
            // Arrange
            var buffer = new GPUBuffer<VertexData>(this.mockGL.Object);
            buffer.Init();

            // Act
            buffer.Dispose();
            buffer.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteVertexArray(this.vertexArrayID), Times.Once());
            this.mockGL.Verify(m => m.DeleteBuffer(this.vertexBufferID), Times.Once());
            this.mockGL.Verify(m => m.DeleteBuffer(this.indexBufferID), Times.Once());
        }
        #endregion
    }
}
