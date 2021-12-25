// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.OpenGL.Exceptions;

namespace VelaptorTests.OpenGL
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using VelaptorTests.Fakes;
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
        private const string ShaderName = "UNKNOWN";
        private const uint VertexShaderId = 1234u;
        private const uint FragShaderId = 5678u;
        private const uint ShaderProgramId = 1928u;
        private const uint DefaultBatchSize = 0u;
        private readonly Mock<IShaderLoaderService<uint>> mockLoader;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly OpenGLInitObservable glInitObservable;

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
                    => m.LoadFragSource(ShaderName, vertTemplateVars))
                        .Returns(() => FragShaderSrc);

            this.mockGLInvoker = new Mock<IGLInvoker>();

            var getShaderStatusCode = 1;
            var getProgramStatusCode = 1;
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(VertexShaderId);
            this.mockGLInvoker.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(FragShaderId);
            this.mockGLInvoker.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGLInvoker.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGLInvoker.Setup(m => m.CreateProgram()).Returns(ShaderProgramId);

            this.glInitObservable = new OpenGLInitObservable();
        }

        #region Prop Tests
        [Fact]
        public void Name_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var shader = CreateShaderProgram();

            // Act
            var actual = shader.Name;

            // Assert
            Assert.Equal("UNKNOWN", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ObservableInit_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, 0u),
            };

            var unused = CreateShaderProgram();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockLoader.Verify(m
                => m.LoadVertSource(ShaderName, vertTemplateVars), Times.Once);
            this.mockLoader.Verify(m
                => m.LoadFragSource(ShaderName, vertTemplateVars), Times.Once);
        }

        [Fact]
        public void ObservableInit_WhenInvokedSecondTime_DoesNotCreateShaderProgram()
        {
            // Arrange
            var program = CreateShaderProgram();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockLoader.Verify(m
                => m.LoadVertSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
            this.mockLoader.Verify(m
                => m.LoadFragSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
            this.mockGLInvoker.Verify(m => m.CreateShader(It.IsAny<GLShaderType>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.LinkProgram(It.IsAny<uint>()), Times.Once());
            this.mockGLInvoker.Verify(m => m.DetachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGLInvoker.Verify(m => m.DeleteShader(It.IsAny<uint>()), Times.Exactly(2));
        }

        [Fact]
        public void ObservableInit_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            var program = CreateShaderProgram();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

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
        public void ObservableInit_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Arrange
            var program = CreateShaderProgram();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGLInvoker.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.AttachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.LinkProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void ObservableInit_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            var program = CreateShaderProgram();

            // Act
            this.glInitObservable.OnOpenGLInitialized();

            // Assert
            this.mockGLInvoker.Verify(m => m.DetachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(VertexShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DetachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGLInvoker.Verify(m => m.DeleteShader(FragShaderId), Times.Once());
        }

        [Fact]
        public void ObservableInit_WithVertexShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetShader(VertexShaderId, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetShaderInfoLog(VertexShaderId)).Returns("Vertex Shader Compile Error");

            var unused = CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObservable.OnOpenGLInitialized();
            }, $"Error compiling vertex shader '{ShaderName}' with shader ID '{VertexShaderId}'.\nVertex Shader Compile Error");
        }

        [Fact]
        public void ObservableInit_WithFragmentShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetShader(FragShaderId, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetShaderInfoLog(FragShaderId)).Returns("Fragment Shader Compile Error");

            var unused = CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObservable.OnOpenGLInitialized();
            }, $"Error compiling fragment shader '{ShaderName}' with shader ID '{FragShaderId}'.\nFragment Shader Compile Error");
        }

        [Fact]
        public void ObservableInit_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGLInvoker.Setup(m => m.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus, out statusCode));
            this.mockGLInvoker.Setup(m => m.GetProgramInfoLog(ShaderProgramId)).Returns("Program Linking Error");
            var program = CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObservable.OnOpenGLInitialized();
            }, $"Error linking shader with ID '{ShaderProgramId}'\nProgram Linking Error");
        }

        [Fact]
        public void Use_WhenNotInitialized_ThrowsException()
        {
            // Arrange
            var program = CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ShaderNotInitializedException>(() =>
            {
                program.Use();
            }, "The shader has not been initialized.");
        }

        [Fact]
        public void Use_WhenInvoked_SetsProgramForUse()
        {
            // Arrange
            var program = CreateShaderProgram();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            program.Use();

            // Assert
            this.mockGLInvoker.Verify(m => m.UseProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DeletesProgram()
        {
            // Arrange
            var program = CreateShaderProgram();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            program.Dispose();
            program.Dispose();

            // Assert
            this.mockGLInvoker.Verify(m => m.DeleteProgram(ShaderProgramId), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="ShaderProgramFake"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private ShaderProgramFake CreateShaderProgram() => new (this.mockGLInvoker.Object, this.mockLoader.Object, this.glInitObservable);
    }
}
