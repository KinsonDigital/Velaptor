// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FileIO.Core;
    using Moq;
    using OpenToolkit.Graphics.OpenGL4;
    using Raptor.OpenGL;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of <see cref="ShaderProgramTests"/>.
    /// </summary>
    public class ShaderProgramTests
    {
        private readonly Mock<ITextFile> mockTextFile;
        private readonly Mock<IGLInvoker> mockGL;
        //TODO: This might have to be used somewhere else to make it work
        private readonly string vertexShaderPath = $@"shader.vert";
        private readonly string fragShaderPath = $@"shader.frag";
        private readonly uint vertextShaderID = 1234;
        private readonly uint fragShaderID = 5678;
        private readonly uint shaderProgramID = 1928;

        // TODO: This might have to be used somewhere else to make it work
        private readonly string vertexShaderPath = $@"shader.vert";
        private readonly string fragShaderPath = $@"shader.frag";
        private readonly int vertextShaderID = 1234;
        private readonly int fragShaderID = 5678;
        private readonly int shaderProgramID = 1928;

        public ShaderProgramTests()
        {
            this.mockTextFile = new Mock<ITextFile>();

            this.mockGL = new Mock<IGLInvoker>();

            int getShaderStatusCode = 1;
            int getProgramStatusCode = 1;
            this.mockGL.Setup(m => m.CreateShader(ShaderType.VertexShader)).Returns(this.vertextShaderID);
            this.mockGL.Setup(m => m.CreateShader(ShaderType.FragmentShader)).Returns(this.fragShaderID);
            this.mockGL.Setup(m => m.GetShader(It.IsAny<uint>(), ShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGL.Setup(m => m.GetProgram(It.IsAny<uint>(), GetProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGL.Setup(m => m.CreateProgram()).Returns(this.shaderProgramID);

            this.mockGL.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGL.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
        }

        [Fact]
        public void Ctor_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            this.mockTextFile.Setup(m => m.LoadAsLines(It.IsAny<string>())).Returns(() => new[] { "line-1", null });

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Assert
            this.mockTextFile.Verify(m => m.LoadAsLines(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            SetupVertexShaderFileMock();
            var expected = "layout(location = 3) in float aTransformIndex;\r\nuniform mat4 uTransform[10];//MODIFIED_DURING_COMPILE_TIME\r\n";

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateShader(ShaderType.VertexShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(this.vertextShaderID, expected), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(this.vertextShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesFragmentShader()
        {
            // Arrange
            SetupFragmentShaderFileMock();
            var expected = "in vec2 v_TexCoord;\r\nin vec4 v_TintClr;\r\n";

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateShader(ShaderType.FragmentShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(this.fragShaderID, expected), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(this.fragShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Arrange
            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(this.shaderProgramID, this.vertextShaderID), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(this.shaderProgramID, this.fragShaderID), Times.Once());
            this.mockGL.Verify(m => m.LinkProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Assert
            this.mockGL.Verify(m => m.DetachShader(this.shaderProgramID, this.vertextShaderID), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(this.vertextShaderID), Times.Once());
            this.mockGL.Verify(m => m.DetachShader(this.shaderProgramID, this.fragShaderID), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(this.fragShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WithShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGL.Setup(m => m.GetShader(this.vertextShaderID, ShaderParameter.CompileStatus, out statusCode));
            this.mockGL.Setup(m => m.GetShaderInfoLog(this.vertextShaderID)).Returns("Vertex Shader Compile Error");
            this.mockGL.Setup(m => m.ShaderCompileSuccess(this.vertextShaderID)).Returns(false);

            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);
            }, $"Error occurred while compiling shader with ID '{this.vertextShaderID}'\nVertex Shader Compile Error");
        }

        [Fact]
        public void Ctor_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGL.Setup(m => m.GetProgram(this.shaderProgramID, GetProgramParameterName.LinkStatus, out statusCode));
            this.mockGL.Setup(m => m.GetProgramInfoLog(this.shaderProgramID)).Returns("Program Linking Error");
            this.mockGL.Setup(m => m.LinkProgramSuccess(this.shaderProgramID)).Returns(false);

            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);
            }, $"Error occurred while linking program with ID '{this.shaderProgramID}'\nProgram Linking Error");
        }

        [Fact]
        public void UseProgram_WhenInvoked_SetsProgramForUse()
        {
            // Arrange
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Act
            program.UseProgram();

            // Assert
            this.mockGL.Verify(m => m.UseProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DeletesProgram()
        {
            // Arrange
            var program = new ShaderProgram(this.mockGL.Object, this.mockTextFile.Object);

            // Act
            program.Dispose();
            program.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteProgram(this.shaderProgramID), Times.Once());
        }

        /// <summary>
        /// Sets up the vertex shader file mock.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupVertexShaderFileMock()
        {
            this.mockTextFile.Setup(m => m.LoadAsLines(this.vertexShaderPath)).Returns(() =>
            {
                return new[] { "layout(location = 3) in float aTransformIndex;", "uniform mat4 uTransform[1];//$REPLACE_INDEX" };
            });
        }

        /// <summary>
        /// Sets up the fragment shader file mock.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupFragmentShaderFileMock()
        {
            this.mockTextFile.Setup(m => m.LoadAsLines(this.fragShaderPath)).Returns(() =>
            {
                return new[] { "in vec2 v_TexCoord;", "in vec4 v_TintClr;" };
            });
        }
    }
}
