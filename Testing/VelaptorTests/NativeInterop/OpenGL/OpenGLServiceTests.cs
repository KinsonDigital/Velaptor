// <copyright file="OpenGLServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.OpenGL
{
    using System;
    using System.Drawing;
    using System.Numerics;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="OpenGLService"/> class.
    /// </summary>
    public class OpenGLServiceTests
    {
        private readonly Mock<IGLInvoker> mockGLInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLServiceTests"/> class.
        /// </summary>
        public OpenGLServiceTests()
        {
            this.mockGLInvoker = new Mock<IGLInvoker>();

            unsafe
            {
                this.mockGLInvoker.Setup(m => m.GetInteger(GLGetPName.Viewport, It.IsAny<int[]>()))
                    .Callback<GLGetPName, int[]>((_, data) =>
                    {
                        fixed (int* dataPtr = data)
                        {
                            dataPtr[0] = 11;
                            dataPtr[1] = 22;
                            dataPtr[2] = 33;
                            dataPtr[3] = 44;
                        }
                    });
            }
        }

        #region Method Tests
        [Fact]
        public void GetViewPortSize_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var service = CreateService();

            // Act
            var actual = service.GetViewPortSize();

            // Assert
            Assert.Equal(new Size(33, 44), actual);
        }

        [Fact]
        public void SetViewPortSize_WhenInvoked_SetsViewPort()
        {
            // Arrange
            var service = CreateService();

            // Act
            service.SetViewPortSize(new Size(55, 66));

            // Assert
            this.mockGLInvoker.Verify(m => m.Viewport(11, 22, 55, 66));
        }

        [Fact]
        public void GetViewPortPosition_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var service = CreateService();

            // Act
            var actual = service.GetViewPortPosition();

            // Assert
            Assert.Equal(new Vector2(11, 22), actual);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(0, false)]
        public void ProgramLinkedSuccessfully_WhenSuccessful_ReturnsCorrectResult(int linkStatus, bool expected)
        {
            // Arrange
            this.mockGLInvoker.Setup(m =>
                    m.GetProgram(123, GLProgramParameterName.LinkStatus))
                .Returns(linkStatus);
            var service = CreateService();

            // Act
            var actual = service.ProgramLinkedSuccessfully(123);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(0, false)]
        public void ShaderCompiledSuccessfully_WhenInvoked_ReturnsCorrectResult(int compileStatus, bool expected)
        {
            // Arrange
            this.mockGLInvoker.Setup(m =>
                    m.GetShader(123, GLShaderParameter.CompileStatus))
                .Returns(compileStatus);
            var service = CreateService();

            // Act
            var actual = service.ShaderCompiledSuccessfully(123);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BeginGroup_WhenInvoked_CreatesDebugGroup()
        {
            // Arrange
            const string label = "test-label";
            var service = CreateService();

            // Act
            service.BeginGroup(label);

            // Assert
            this.mockGLInvoker.Verify(m =>
                m.PushDebugGroup(GLDebugSource.DebugSourceApplication, 100, (uint)label.Length, label), Times.Once);
        }

        [Fact]
        public void EndGroup_WhenInvoked_EndsDebugGroup()
        {
            // Arrange
            var service = CreateService();

            // Act
            service.EndGroup();

            // Assert
            this.mockGLInvoker.Verify(m =>
                m.PopDebugGroup(), Times.Once);
        }

        [Fact]
        public void LabelShader_WhenInvoked_LabelsShader()
        {
            // Arrange
            const string label = "test-label";
            var service = CreateService();

            // Act
            service.LabelShader(123, label);

            // Assert
            this.mockGLInvoker.Verify(m => m.ObjectLabel(GLObjectIdentifier.Shader, 123, (uint)label.Length, label));
        }

        [Fact]
        public void LabelShaderProgram_WhenInvoked_LabelsShaderProgram()
        {
            // Arrange
            const string label = "test-label";
            var service = CreateService();

            // Act
            service.LabelShaderProgram(123, label);

            // Assert
            this.mockGLInvoker.Verify(m => m.ObjectLabel(GLObjectIdentifier.Program, 123, (uint)label.Length, label));
        }

        [Theory]
        [InlineData("", "NOT SET VAO")]
        [InlineData(null, "NOT SET VAO")]
        [InlineData("test-label", "test-label VAO")]
        public void LabelVertexArray_WhenInvoked_LabelsVertexArray(string label, string expected)
        {
            // Arrange
            var service = CreateService();

            // Act
            service.LabelVertexArray(123, label);

            // Assert
            this.mockGLInvoker.Verify(m =>
                m.ObjectLabel(GLObjectIdentifier.VertexArray, 123, (uint)expected.Length, expected));
        }

        [Fact]
        public void LabelBuffer_WithInvalidBufferType_ThrowsException()
        {
            // Arrange
            const string exceptionMsg =
                "Exception of type 'System.ArgumentOutOfRangeException' was thrown. (Parameter 'bufferType')\r\nActual value was 123.";
            var service = CreateService();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                service.LabelBuffer(It.IsAny<uint>(), It.IsAny<string>(), (BufferType)123);
            }, exceptionMsg);
        }

        [Theory]
        [InlineData("", BufferType.VertexBufferObject, "NOT SET VBO")]
        [InlineData(null, BufferType.VertexBufferObject, "NOT SET VBO")]
        [InlineData("test-label", BufferType.VertexBufferObject, "test-label VBO")]
        [InlineData("", BufferType.IndexArrayObject, "NOT SET EBO")]
        [InlineData(null, BufferType.IndexArrayObject, "NOT SET EBO")]
        [InlineData("test-label", BufferType.IndexArrayObject, "test-label EBO")]
        public void LabelBuffer_WhenInvoked_LabelsVertexArray(string label, BufferType bufferType, string expected)
        {
            // Arrange
            var service = CreateService();

            // Act
            service.LabelBuffer(123, label, bufferType);

            // Assert
            this.mockGLInvoker.Verify(m =>
                m.ObjectLabel(GLObjectIdentifier.Buffer, 123, (uint)expected.Length, expected));
        }

        [Theory]
        [InlineData("", "NOT SET")]
        [InlineData(null, "NOT SET")]
        [InlineData("test-label", "test-label")]
        public void LabelTexture_WhenInvoked_LabelsTexture(string label, string expected)
        {
            // Arrange
            var service = CreateService();

            // Act
            service.LabelTexture(123, label);

            // Assert
            this.mockGLInvoker.Verify(m =>
                m.ObjectLabel(GLObjectIdentifier.Texture, 123, (uint)expected.Length, expected));
        }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="OpenGLService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private OpenGLService CreateService() => new (this.mockGLInvoker.Object);
    }
}
