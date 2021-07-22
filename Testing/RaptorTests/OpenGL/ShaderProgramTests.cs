// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using System;
    using Moq;
    using Raptor.NativeInterop;
    using Raptor.OpenGL;
    using Raptor.Services;
    using Xunit;
    using Assert = Helpers.AssertExtensions;

    /// <summary>
    /// Initializes a new instance of <see cref="ShaderProgramTests"/>.
    /// </summary>
    public class ShaderProgramTests
    {
        private readonly Mock<IEmbeddedResourceLoaderService> mockLoader;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly string vertextShaderFileName = $@"shader.vert";
        private readonly string fragShaderFileName = $@"shader.frag";
        private readonly uint vertextShaderID = 1234;
        private readonly uint fragShaderID = 5678;
        private readonly uint shaderProgramID = 1928;
        private readonly Mock<IGLInvokerExtensions> mockGLInvokerExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramTests"/> class.
        /// </summary>
        public ShaderProgramTests()
        {
            this.mockLoader = new Mock<IEmbeddedResourceLoaderService>();

            // Sets up the vertex shader file mock.
            this.mockLoader.Setup(m => m.LoadResource(this.vertextShaderFileName)).Returns(() =>
            {
                return "layout(location = 3) in float aTransformIndex;\r\nuniform mat4 uTransform[1];//$REPLACE_INDEX";
            });

            // Sets up the fragment shader file mock.
            this.mockLoader.Setup(m => m.LoadResource(this.fragShaderFileName)).Returns(() =>
            {
                return "in vec2 v_TexCoord;\r\nin vec4 v_TintClr;";
            });

            this.mockGLInvoker = new Mock<IGLInvoker>();

            var getShaderStatusCode = 1;
            var getProgramStatusCode = 1;
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(this.vertextShaderID);
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(this.fragShaderID);
            this.mockGLInvoker.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGLInvoker.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGLInvoker.Setup(m => m.CreateProgram()).Returns(this.shaderProgramID);


            this.mockGLInvokerExtensions = new Mock<IGLInvokerExtensions>();
            this.mockGLInvokerExtensions.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvokerExtensions.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
        }

        #region Method Tests
        [Fact]
        public void Init_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            this.mockLoader.Setup(m => m.LoadResource(It.IsAny<string>())).Returns("line-1");
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockLoader.Verify(m => m.LoadResource(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void Init_WhenInvokedSecondTime_DoesNotCreateShaderProgram()
        {
            // Arrange
            var program = CreateProgram();
            program.Init();

            // Act
            program.Init();

            // Assert
            this.mockLoader.Verify(m => m.LoadResource(It.IsAny<string>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.CreateShader(It.IsAny<GLShaderType>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.LinkProgram(It.IsAny<uint>()), Times.Once());
            this.mockGLInvokerExtensions.Verify(m => m.LinkProgramSuccess(It.IsAny<uint>()), Times.Once());
            this.mockGLInvoker.Verify(m => m.DetachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.DeleteShader(It.IsAny<uint>()), Times.Exactly(2));
        }

        [Fact]
        public void Init_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            var expected = "layout(location = 3) in float aTransformIndex;\r\n\r\nuniform mat4 uTransform[10];//MODIFIED_DURING_COMPILE_TIME\r\n";
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockGLInvoker.Verify(m => m.CreateShader(GLShaderType.VertexShader), Times.Once());
            this.mockGLInvoker.Verify(m => m.ShaderSource(this.vertextShaderID, expected), Times.Once());
            this.mockGLInvoker.Verify(m => m.CompileShader(this.vertextShaderID), Times.Once());
        }

        [Fact]
        public void Init_WhenInvoked_SuccessfullyCreatesFragmentShader()
        {
            // Arrange
            var expected = "in vec2 v_TexCoord;\r\n\r\nin vec4 v_TintClr;\r\n";
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockGLInvoker.Verify(m => m.CreateShader(GLShaderType.FragmentShader), Times.Once());
            this.mockGLInvoker.Verify(m => m.ShaderSource(this.fragShaderID, expected), Times.Once());
            this.mockGLInvoker.Verify(m => m.CompileShader(this.fragShaderID), Times.Once());
        }

        [Fact]
        public void Init_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Arrange
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockGLInvoker.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(this.shaderProgramID, this.vertextShaderID), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(this.shaderProgramID, this.fragShaderID), Times.Once());
            this.mockGLInvoker.Verify(m => m.LinkProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Init_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockGLInvoker.Verify(m => m.DetachShader(this.shaderProgramID, this.vertextShaderID), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(this.vertextShaderID), Times.Once());
            this.mockGLInvoker.Verify(m => m.DetachShader(this.shaderProgramID, this.fragShaderID), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(this.fragShaderID), Times.Once());
        }

        [Fact]
        public void Init_WithShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetShader(this.vertextShaderID, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetShaderInfoLog(this.vertextShaderID)).Returns("Vertex Shader Compile Error");
            this.mockGLInvokerExtensions.Setup(m => m.ShaderCompileSuccess(this.vertextShaderID)).Returns(false);
            var program = CreateProgram();

            // Act & Assert
            Assert.ThrowsWithMessage<Exception>(() =>
            {
                program.Init();
            }, $"Error occurred while compiling shader with ID '{this.vertextShaderID}'\nVertex Shader Compile Error");
        }

        [Fact]
        public void Init_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetProgram(this.shaderProgramID, GLProgramParameterName.LinkStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetProgramInfoLog(this.shaderProgramID)).Returns("Program Linking Error");
            this.mockGLInvokerExtensions.Setup(m => m.LinkProgramSuccess(this.shaderProgramID)).Returns(false);
            var program = CreateProgram();

            // Act & Assert
            Assert.ThrowsWithMessage<Exception>(() =>
            {
                program.Init();
            }, $"Error occurred while linking program with ID '{this.shaderProgramID}'\nProgram Linking Error");
        }

        [Fact]
        public void UseProgram_WhenInvoked_SetsProgramForUse()
        {
            // Arrange
            var program = CreateProgram();
            program.Init();

            // Act
            program.UseProgram();

            // Assert
            this.mockGLInvoker.Verify(m => m.UseProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DeletesProgram()
        {
            // Arrange
            var program = CreateProgram();
            program.Init();

            // Act
            program.Dispose();
            program.Dispose();

            // Assert
            this.mockGLInvoker.Verify(m => m.DeleteProgram(this.shaderProgramID), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="ShaderProgram"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private ShaderProgram CreateProgram() => new (this.mockGLInvoker.Object, this.mockGLInvokerExtensions.Object, this.mockLoader.Object);
    }
}
