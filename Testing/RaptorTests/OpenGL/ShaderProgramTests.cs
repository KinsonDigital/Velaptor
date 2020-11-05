// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using System;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using Raptor.OpenGL;
    using Raptor.Services;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of <see cref="ShaderProgramTests"/>.
    /// </summary>
    public class ShaderProgramTests
    {
        private readonly Mock<IEmbeddedResourceLoaderService> mockLoader;
        private readonly Mock<IGLInvoker> mockGL;

        // TODO: This might have to be used somewhere else to make it work
        private readonly string vertextShaderFileName = $@"shader.vert";
        private readonly string fragShaderFileName = $@"shader.frag";
        private readonly uint vertextShaderID = 1234;
        private readonly uint fragShaderID = 5678;
        private readonly uint shaderProgramID = 1928;

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

            this.mockGL = new Mock<IGLInvoker>();

            var getShaderStatusCode = 1;
            var getProgramStatusCode = 1;
            this.mockGL.Setup(m => m.CreateShader(ShaderType.VertexShader)).Returns(this.vertextShaderID);
            this.mockGL.Setup(m => m.CreateShader(ShaderType.FragmentShader)).Returns(this.fragShaderID);
            this.mockGL.Setup(m => m.GetShader(It.IsAny<uint>(), ShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGL.Setup(m => m.GetProgram(It.IsAny<uint>(), GetProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGL.Setup(m => m.CreateProgram()).Returns(this.shaderProgramID);

            this.mockGL.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGL.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            this.mockLoader.Setup(m => m.LoadResource(It.IsAny<string>())).Returns("line-1");

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Assert
            this.mockLoader.Verify(m => m.LoadResource(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            var expected = "layout(location = 3) in float aTransformIndex;\r\n\r\nuniform mat4 uTransform[10];//MODIFIED_DURING_COMPILE_TIME\r\n";

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateShader(ShaderType.VertexShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(this.vertextShaderID, expected), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(this.vertextShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesFragmentShader()
        {
            // Arrange
            var expected = "in vec2 v_TexCoord;\r\n\r\nin vec4 v_TintClr;\r\n";

            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateShader(ShaderType.FragmentShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(this.fragShaderID, expected), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(this.fragShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Assert
            this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(this.shaderProgramID, this.vertextShaderID), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(this.shaderProgramID, this.fragShaderID), Times.Once());
            this.mockGL.Verify(m => m.LinkProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Act
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

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

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);
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

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);
            }, $"Error occurred while linking program with ID '{this.shaderProgramID}'\nProgram Linking Error");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void UseProgram_WhenInvoked_SetsProgramForUse()
        {
            // Arrange
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Act
            program.UseProgram();

            // Assert
            this.mockGL.Verify(m => m.UseProgram(this.shaderProgramID), Times.Once());
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DeletesProgram()
        {
            // Arrange
            var program = new ShaderProgram(this.mockGL.Object, this.mockLoader.Object);

            // Act
            program.Dispose();
            program.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteProgram(this.shaderProgramID), Times.Once());
        }
        #endregion
    }
}
