// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of <see cref="ShaderProgramTests"/>.
    /// </summary>
    public class ShaderProgramTests
    {
        private const string BatchSizeVarName = "BATCH_SIZE";
        private const string VertShaderSrc = "vert-shader-src";
        private const string FragShaderSrc = "frag-shader-src";
        private const string ShaderName = "texture-shader";
        private const uint VertexShaderId = 1234u;
        private const uint FragShaderId = 5678u;
        private const uint ShaderProgramId = 1928u;
        private const uint DefaultBatchSize = 10u;
        private readonly Mock<IShaderLoaderService<uint>> mockLoader;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly Mock<IGLInvokerExtensions> mockGLInvokerExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramTests"/> class.
        /// </summary>
        public ShaderProgramTests()
        {
            this.mockLoader = new Mock<IShaderLoaderService<uint>>();

            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, DefaultBatchSize),
            };

            // Sets up the vertex shader file mock.
            this.mockLoader.Setup(m
                    => m.LoadVertSource(ShaderName, vertTemplateVars))
                        .Returns(() => VertShaderSrc);

            // Sets up the fragment shader file mock.
            this.mockLoader.Setup(m
                    => m.LoadFragSource(ShaderName, null))
                        .Returns(() => FragShaderSrc);

            this.mockGLInvoker = new Mock<IGLInvoker>();

            var getShaderStatusCode = 1;
            var getProgramStatusCode = 1;
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(VertexShaderId);
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(FragShaderId);
            this.mockGLInvoker.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGLInvoker.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGLInvoker.Setup(m => m.CreateProgram()).Returns(ShaderProgramId);

            this.mockGLInvokerExtensions = new Mock<IGLInvokerExtensions>();
            this.mockGLInvokerExtensions.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvokerExtensions.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
        }

        #region Prop Tests
        [Fact]
        public void BatchSize_WhenCreatingNewProgram_DefaultValueIsCorrect()
        {
            // Arrange
            var program = CreateProgram();

            // Act
            var actual = program.BatchSize;

            // Assert
            Assert.Equal(10u, actual);
        }

        [Fact]
        public void BatchSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var program = CreateProgram();

            // Act
            program.BatchSize = 1234u;
            var actual = program.BatchSize;

            // Assert
            Assert.Equal(1234u, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Init_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, 10u),
            };

            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockLoader.Verify(m
                => m.LoadVertSource(ShaderName, vertTemplateVars), Times.Once);
            this.mockLoader.Verify(m
                => m.LoadFragSource(ShaderName, null), Times.Once);
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
            this.mockLoader.Verify(m
                => m.LoadVertSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
            this.mockLoader.Verify(m
                => m.LoadFragSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
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
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            // Verify the creation of the vertex shader
            this.mockGLInvoker.Verify(m => m.CreateShader(GLShaderType.VertexShader), Times.Once());
            this.mockGLInvoker.Verify(m => m.ShaderSource(VertexShaderId, VertShaderSrc), Times.Once());
            this.mockGLInvoker.Verify(m => m.CompileShader(VertexShaderId), Times.Once());

            // Verify the creation of the fragment shader
            this.mockGLInvoker.Verify(m => m.CreateShader(GLShaderType.FragmentShader), Times.Once());
            this.mockGLInvoker.Verify(m => m.ShaderSource(FragShaderId, FragShaderSrc), Times.Once());
            this.mockGLInvoker.Verify(m => m.CompileShader(FragShaderId), Times.Once());
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
            this.mockGLInvoker.Verify(m => m.AttachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.LinkProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void Init_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            var program = CreateProgram();

            // Act
            program.Init();

            // Assert
            this.mockGLInvoker.Verify(m => m.DetachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DetachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(FragShaderId), Times.Once());
        }

        [Fact]
        public void Init_WithShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetShader(VertexShaderId, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetShaderInfoLog(VertexShaderId)).Returns("Vertex Shader Compile Error");
            this.mockGLInvokerExtensions.Setup(m => m.ShaderCompileSuccess(VertexShaderId)).Returns(false);
            var program = CreateProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                program.Init();
            }, $"Error occurred while compiling shader with ID '{VertexShaderId}'\nVertex Shader Compile Error");
        }

        [Fact]
        public void Init_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetProgramInfoLog(ShaderProgramId)).Returns("Program Linking Error");
            this.mockGLInvokerExtensions.Setup(m => m.LinkProgramSuccess(ShaderProgramId)).Returns(false);
            var program = CreateProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                program.Init();
            }, $"Error occurred while linking program with ID '{ShaderProgramId}'\nProgram Linking Error");
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
            this.mockGLInvoker.Verify(m => m.UseProgram(ShaderProgramId), Times.Once());
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
            this.mockGLInvoker.Verify(m => m.DeleteProgram(ShaderProgramId), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="ShaderProgram"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private ShaderProgram CreateProgram() => new (this.mockGLInvoker.Object, this.mockGLInvokerExtensions.Object, this.mockLoader.Object);
    }
}
