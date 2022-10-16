// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Exceptions;
    using Velaptor.OpenGL.Services;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
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
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IOpenGLService> mockGLService;
        private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
        private readonly Mock<IDisposable> mockGLInitReactorUnsubscriber;
        private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;
        private readonly Mock<IDisposable> mockShutDownUnsubscriber;
        private IReactor<GLInitData>? glInitReactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramTests"/> class.
        /// </summary>
        public ShaderProgramTests()
        {
            this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();

            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, DefaultBatchSize),
            };

            // Sets up the vertex shader file mock.
            this.mockShaderLoader.Setup(m
                    => m.LoadVertSource(ShaderName, vertTemplateVars))
                        .Returns(() => VertShaderSrc);

            // Sets up the fragment shader file mock.
            this.mockShaderLoader.Setup(m
                    => m.LoadFragSource(ShaderName, vertTemplateVars))
                        .Returns(() => FragShaderSrc);

            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLService = new Mock<IOpenGLService>();

            const int getShaderStatusCode = 1;
            const int getProgramStatusCode = 1;
            this.mockGL.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(VertexShaderId);
            this.mockGL.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(FragShaderId);
            this.mockGL.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus))
                .Returns(getShaderStatusCode);
            this.mockGL.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus))
                .Returns(getProgramStatusCode);
            this.mockGL.Setup(m => m.CreateProgram()).Returns(ShaderProgramId);

            this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
            this.mockGLInitReactorUnsubscriber = new Mock<IDisposable>();
            this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
                .Returns(this.mockGLInitReactorUnsubscriber.Object)
                .Callback<IReactor<GLInitData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "Shutdown reactable subscription failed.  Reactor is null.");
                    }

                    this.glInitReactor = reactor;
                });

            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullGLParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    null,
                    this.mockGLService.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    null,
                    this.mockShaderLoader.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'openGLService')");
        }

        [Fact]
        public void Ctor_WithNullShaderLoaderServiceParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    null,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'shaderLoaderService')");
        }

        [Fact]
        public void Ctor_WithNullInitReactorParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderLoader.Object,
                    null,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'glInitReactable')");
        }

        [Fact]
        public void Ctor_WithNullShutDownReactorParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
        }
        #endregion

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
        public void ReactorInit_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, 0u),
            };

            CreateShaderProgram();

            // Act
            this.glInitReactor.OnNext(default);

            // Assert
            this.mockShaderLoader.Verify(m
                => m.LoadVertSource(ShaderName, vertTemplateVars), Times.Once);
            this.mockShaderLoader.Verify(m
                => m.LoadFragSource(ShaderName, vertTemplateVars), Times.Once);
        }

        [Fact]
        public void ReactorInit_WhenInvokedSecondTime_DoesNotCreateShaderProgram()
        {
            // Arrange
            CreateShaderProgram();
            this.glInitReactor.OnNext(default);

            // Act
            this.glInitReactor.OnNext(default);

            // Assert
            this.mockShaderLoader.Verify(m
                => m.LoadVertSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
            this.mockShaderLoader.Verify(m
                => m.LoadFragSource(It.IsAny<string>(), It.IsAny<(string, uint)[]>()), Times.Once);
            this.mockGL.Verify(m => m.CreateShader(It.IsAny<GLShaderType>()), Times.Exactly(2));
            this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGL.Verify(m => m.LinkProgram(It.IsAny<uint>()), Times.Once());
            this.mockGL.Verify(m => m.DetachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            this.mockGL.Verify(m => m.DeleteShader(It.IsAny<uint>()), Times.Exactly(2));
        }

        [Fact]
        public void ReactorInit_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitReactor.OnNext(default);

            // Assert
            // Verify the creation of the vertex shader
            this.mockGL.Verify(m => m.CreateShader(GLShaderType.VertexShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(VertexShaderId, VertShaderSrc), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(VertexShaderId), Times.Once());

            // Verify the creation of the fragment shader
            this.mockGL.Verify(m => m.CreateShader(GLShaderType.FragmentShader), Times.Once());
            this.mockGL.Verify(m => m.ShaderSource(FragShaderId, FragShaderSrc), Times.Once());
            this.mockGL.Verify(m => m.CompileShader(FragShaderId), Times.Once());
        }

        [Fact]
        public void ReactorInit_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitReactor.OnNext(default);

            // Assert
            this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGL.Verify(m => m.LinkProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void ReactorInit_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitReactor.OnNext(default);

            // Assert
            this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(FragShaderId), Times.Once());
        }

        [Fact]
        public void ReactorInit_WithVertexShaderCompileIssue_ThrowsException()
        {
            // Arrange
            const int statusCode = 0;
            this.mockGL.Setup(m => m.GetShader(VertexShaderId, GLShaderParameter.CompileStatus))
                .Returns(statusCode);
            this.mockGL.Setup(m => m.GetShaderInfoLog(VertexShaderId)).Returns("Vertex Shader Compile Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitReactor.OnNext(default);
            }, $"Error compiling vertex shader '{ShaderName}' with shader ID '{VertexShaderId}'.{Environment.NewLine}Vertex Shader Compile Error");
        }

        [Fact]
        public void ReactorInit_WithFragmentShaderCompileIssue_ThrowsException()
        {
            // Arrange
            const int statusCode = 0;
            this.mockGL.Setup(m => m.GetShader(FragShaderId, GLShaderParameter.CompileStatus))
                .Returns(statusCode);
            this.mockGL.Setup(m => m.GetShaderInfoLog(FragShaderId)).Returns("Fragment Shader Compile Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitReactor.OnNext(default);
            }, $"Error compiling fragment shader '{ShaderName}' with shader ID '{FragShaderId}'.{Environment.NewLine}Fragment Shader Compile Error");
        }

        [Fact]
        public void ReactorInit_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            const int statusCode = 0;
            this.mockGL.Setup(m => m.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus))
                .Returns(statusCode);
            this.mockGL.Setup(m => m.GetProgramInfoLog(ShaderProgramId)).Returns("Program Linking Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitReactor.OnNext(default);
            }, $"Error linking shader with ID '{ShaderProgramId}'{Environment.NewLine}Program Linking Error");
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
            this.glInitReactor.OnNext(default);

            // Act
            program.Use();

            // Assert
            this.mockGL.Verify(m => m.UseProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfShaderProgram()
        {
            // Arrange
            IReactor<ShutDownData>? shutDownReactor = null;

            this.mockShutDownReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<ShutDownData>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IReactor<ShutDownData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "Shutdown reactable subscription failed.  Reactor is null.");
                    }

                    shutDownReactor = reactor;
                });

            CreateShaderProgram();
            this.glInitReactor.OnNext(default);

            // Act
            shutDownReactor?.OnNext(default);
            shutDownReactor?.OnNext(default);

            // Assert
            this.mockGL.Verify(m => m.DeleteProgram(ShaderProgramId), Times.Once);
            this.mockGLInitReactorUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="ShaderProgramFake"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private ShaderProgramFake CreateShaderProgram()
            => new (
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitReactable.Object,
                this.mockShutDownReactable.Object);
    }
}
