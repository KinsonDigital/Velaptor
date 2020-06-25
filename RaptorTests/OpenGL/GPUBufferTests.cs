using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL4;
using Moq;
using Raptor.OpenGL;
using Xunit;
using System.Linq;
using Xunit.Sdk;
using System.Drawing;

namespace RaptorTests.OpenGL
{
    /// <summary>
    /// Initializes a new instance of <see cref="GPUBufferTests"/>.
    /// </summary>
    public class GPUBufferTests
    {
        private readonly Mock<IGLInvoker> _mockGL;
        private readonly int _vertexArrayID = 1256;
        private readonly int _vertexBufferID = 1234;
        private readonly int _indexBufferID = 5678;
        private bool _vertexBufferCreated;
        private bool _indexBufferCreated;

        public GPUBufferTests()
        {
            _mockGL = new Mock<IGLInvoker>();
            _mockGL.Setup(m => m.GenBuffer()).Returns(() =>
            {
                if (!_vertexBufferCreated)
                {
                    _vertexBufferCreated = true;
                    return _vertexBufferID;
                }

                if (!_indexBufferCreated)
                {
                    _indexBufferCreated = true;
                    return _indexBufferID;
                }

                return -1;
            });
            _mockGL.Setup(m => m.GenVertexArray()).Returns(_vertexArrayID);
        }

        [Fact]
        public void Ctor_WhenInvoked_CreatesVertexBuffer()
        {
            //Arrange
            var totalQuadBytes = 320;//160 bytes per quad * 2 quads = 320 bytes

            //Act
            var buffer = new GPUBuffer<VertexData>(_mockGL.Object)
            {
                TotalQuads = 2
            };

            //Assert
            //These are all invoked once per quad.  The number of invokes is 4 because
            //the internal Init() method that invokes these is called once in the constructor
            //as well as when the setter of the TotalQuads property is invoked.
            _mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce());

            _mockGL.Verify(m => m.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferID), Times.AtLeast(2));
            _mockGL.Verify(m => m.BufferData(BufferTarget.ArrayBuffer, totalQuadBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw), Times.AtLeastOnce());
            _mockGL.Verify(m => m.BindBuffer(BufferTarget.ArrayBuffer, 0), Times.AtLeast(2));
        }
        
        [Fact]
        public void Ctor_WhenInvoked_CreatesIndexBuffer()
        {
            //Act
            var buffer = new GPUBuffer<VertexData>(_mockGL.Object);

            //Assert
            //First invoke is done creating the Vertex Buffer, the second is the index buffer
            _mockGL.Verify(m => m.GenBuffer(), Times.Exactly(2));
            _mockGL.Verify(m => m.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferID), Times.Exactly(2));
            _mockGL.Verify(m => m.BindBuffer(BufferTarget.ElementArrayBuffer, 0), Times.Exactly(1));
        }

        [Fact]
        public void Ctor_WhenInvoking_SetsUpAttributes()
        {
            //Arrange
            var invokeCount = typeof(VertexData).GetFields().Count();
            var actualIndices = new List<int>();
            var actualSizes = new List<int>();
            var actualOffsets = new List<int>();

            //Collect the various parameter values for assert comparisons later
            _mockGL.Setup(m => m.VertexAttribPointer(It.IsAny<int>(), It.IsAny<int>(), VertexAttribPointerType.Float, false, It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int, VertexAttribPointerType, bool, int, int>((index, size, type, normalized, stride, offset) =>
                {
                    actualIndices.Add(index);
                    actualSizes.Add(size);
                    actualOffsets.Add(offset);
                });

            //Act
            var buffer = new GPUBuffer<VertexData>(_mockGL.Object);

            //Assert
            _mockGL.Verify(m => m.EnableVertexArrayAttrib(_vertexArrayID, It.IsAny<int>()), Times.Exactly(invokeCount));
            _mockGL.Verify(m => m.VertexAttribPointer(
                It.IsAny<int>(),
                It.IsAny<int>(),
                VertexAttribPointerType.Float,
                false,
                40,
                It.IsAny<int>()),
            Times.Exactly(invokeCount));

            Assert.Equal(new[] { 0, 1, 2, 3 }, actualIndices.ToArray());
            Assert.Equal(new[] { 3, 2, 4, 1 }, actualSizes.ToArray());
            Assert.Equal(new[] { 0, 12, 20, 36 }, actualOffsets.ToArray());
        }

        [Fact]
        public void UpdateQuad_WhenInvoked_UpdatesGPUVertexBuffer()
        {
            //Arrange
            var buffer = new GPUBuffer<VertexData>(_mockGL.Object);
            var srcRect = new Rectangle();

            //Act
            buffer.UpdateQuad(0, srcRect, 50, 50, Color.White);
            
            //Assert
            _mockGL.Verify(m => m.BufferSubData(BufferTarget.ArrayBuffer, It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<QuadData>()));
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DisposesOfUnmanagedResources()
        {
            //Arrange
            var buffer = new GPUBuffer<VertexData>(_mockGL.Object);

            //Act
            buffer.Dispose();
            buffer.Dispose();

            //Assert
            _mockGL.Verify(m => m.DeleteVertexArray(_vertexArrayID), Times.Once());
            _mockGL.Verify(m => m.DeleteBuffer(_vertexBufferID), Times.Once());
            _mockGL.Verify(m => m.DeleteBuffer(_indexBufferID), Times.Once());
        }
    }
}
