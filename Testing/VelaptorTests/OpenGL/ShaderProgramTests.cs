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
    using Velaptor.OpenGL.Exceptions;
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
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IObservable<bool>> mockGLInitObservable;
        private readonly Mock<IDisposable> mockGLInitObservableUnsubscriber;
        private readonly Mock<IObservable<bool>> mockShutDownObservable;
        private readonly Mock<IDisposable> mockShutDownObservableUnsubscriber;
        private IObserver<bool>? glInitObserver;

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
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();

            var getShaderStatusCode = 1;
            var getProgramStatusCode = 1;
            this.mockGL.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(VertexShaderId);
            this.mockGL.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(FragShaderId);
            this.mockGL.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus, out getShaderStatusCode));
            this.mockGL.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus, out getProgramStatusCode));
            this.mockGL.Setup(m => m.CreateProgram()).Returns(ShaderProgramId);

            this.mockGLInitObservable = new Mock<IObservable<bool>>();
            this.mockGLInitObservableUnsubscriber = new Mock<IDisposable>();
            this.mockGLInitObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(this.mockGLInitObservableUnsubscriber.Object)
                .Callback<IObserver<bool>>(observer =>
                {
                    if (observer is null)
                    {
                        Assert.True(false, "Shutdown observable subscription failed.  Observer is null.");
                    }

                    this.glInitObserver = observer;
                });

            this.mockShutDownObservable = new Mock<IObservable<bool>>();
            this.mockShutDownObservableUnsubscriber = new Mock<IDisposable>();
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
                    this.mockGLExtensions.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitObservable.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WithNullGLExtensionsParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    null,
                    this.mockShaderLoader.Object,
                    this.mockGLInitObservable.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'glExtensions')");
        }

        [Fact]
        public void Ctor_WithNullShaderLoaderServiceParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    null,
                    this.mockGLInitObservable.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'shaderLoaderService')");
        }

        [Fact]
        public void Ctor_WithNullInitObservableParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockShaderLoader.Object,
                    null,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'glInitObservable')");
        }

        [Fact]
        public void Ctor_WithNullShutDownObservableParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new ShaderProgramFake(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitObservable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownObservable')");
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
        public void ObservableInit_WhenInvoked_LoadsShaderSourceCode()
        {
            // Arrange
            IEnumerable<(string, uint)> vertTemplateVars = new[]
            {
                (BatchSizeVarName, 0u),
            };

            CreateShaderProgram();

            // Act
            this.glInitObserver.OnNext(true);

            // Assert
            this.mockShaderLoader.Verify(m
                => m.LoadVertSource(ShaderName, vertTemplateVars), Times.Once);
            this.mockShaderLoader.Verify(m
                => m.LoadFragSource(ShaderName, vertTemplateVars), Times.Once);
        }

        [Fact]
        public void ObservableInit_WhenInvokedSecondTime_DoesNotCreateShaderProgram()
        {
            // Arrange
            CreateShaderProgram();
            this.glInitObserver.OnNext(true);

            // Act
            this.glInitObserver.OnNext(true);

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
        public void ObservableInit_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitObserver.OnNext(true);

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
        public void ObservableInit_WhenInvoked_SuccessfullyCreatesProgram()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitObserver.OnNext(true);

            // Assert
            this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGL.Verify(m => m.LinkProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void ObservableInit_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            // Arrange
            CreateShaderProgram();

            // Act
            this.glInitObserver.OnNext(true);

            // Assert
            this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(VertexShaderId), Times.Once());
            this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, FragShaderId), Times.Once());
            this.mockGL.Verify(m => m.DeleteShader(FragShaderId), Times.Once());
        }

        [Fact]
        public void ObservableInit_WithVertexShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGL.Setup(m => m.GetShader(VertexShaderId, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGL.Setup(m => m.GetShaderInfoLog(VertexShaderId)).Returns("Vertex Shader Compile Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObserver.OnNext(true);
            }, $"Error compiling vertex shader '{ShaderName}' with shader ID '{VertexShaderId}'.\nVertex Shader Compile Error");
        }

        [Fact]
        public void ObservableInit_WithFragmentShaderCompileIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGL.Setup(m => m.GetShader(FragShaderId, GLShaderParameter.CompileStatus, out statusCode));
            this.mockGL.Setup(m => m.GetShaderInfoLog(FragShaderId)).Returns("Fragment Shader Compile Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObserver.OnNext(true);
            }, $"Error compiling fragment shader '{ShaderName}' with shader ID '{FragShaderId}'.\nFragment Shader Compile Error");
        }

        [Fact]
        public void ObservableInit_WithProgramLinkingIssue_ThrowsException()
        {
            // Arrange
            var statusCode = 0;
            this.mockGL.Setup(m => m.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus, out statusCode));
            this.mockGL.Setup(m => m.GetProgramInfoLog(ShaderProgramId)).Returns("Program Linking Error");

            CreateShaderProgram();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                this.glInitObserver.OnNext(true);
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
            this.glInitObserver.OnNext(true);

            // Act
            program.Use();

            // Assert
            this.mockGL.Verify(m => m.UseProgram(ShaderProgramId), Times.Once());
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfShaderProgram()
        {
            // Arrange
            IObserver<bool>? shutDownObserver = null;

            this.mockShutDownObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(this.mockShutDownObservableUnsubscriber.Object)
                .Callback<IObserver<bool>>(observer =>
                {
                    if (observer is null)
                    {
                        Assert.True(false, "Shutdown observable subscription failed.  Observer is null.");
                    }

                    shutDownObserver = observer;
                });

            CreateShaderProgram();
            this.glInitObserver.OnNext(true);

            // Act
            shutDownObserver?.OnNext(true);
            shutDownObserver?.OnNext(true);

            // Assert
            this.mockGL.Verify(m => m.DeleteProgram(ShaderProgramId), Times.Once);
            this.mockGLInitObservableUnsubscriber.Verify(m => m.Dispose(), Times.Once);
            this.mockShutDownObservableUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="ShaderProgramFake"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private ShaderProgramFake CreateShaderProgram()
            => new (
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitObservable.Object,
                this.mockShutDownObservable.Object);
    }
}
