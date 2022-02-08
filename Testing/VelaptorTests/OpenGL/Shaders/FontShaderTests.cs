// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders
{
    using System;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using Velaptor.OpenGL.Shaders;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontShader"/> class.
    /// </summary>
    public class FontShaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IOpenGLService> mockGLService;
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
        private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
        private readonly Mock<IDisposable> mockGLInitUnsubscriber;
        private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
        /// </summary>
        public FontShaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLService = new Mock<IOpenGLService>();
            this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();
            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
            this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
            this.mockGLInitUnsubscriber = new Mock<IDisposable>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullInitReactorParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontShader(
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
                var unused = new FontShader(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Use_WhenInvoked_SetsShaderAsUsed()
        {
            // Arrange
            IReactor<GLInitData>? glInitReactor = null;

            const uint shaderId = 78;
            const int uniformLocation = 1234;
            this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
            this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture"))
                .Returns(uniformLocation);
            const int status = 1;
            this.mockGL.Setup(m
                    => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus))
                .Returns(status);
            this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
                .Returns(this.mockGLInitUnsubscriber.Object)
                .Callback<IReactor<GLInitData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "GL initialization reactable subscription failed.  Reactor is null.");
                    }

                    glInitReactor = reactor;
                });

            var shader = new FontShader(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitReactable.Object,
                this.mockShutDownReactable.Object);

            glInitReactor?.OnNext(default);

            // Act
            shader.Use();

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture1), Times.Once);
            this.mockGL.Verify(m => m.Uniform1(uniformLocation, 1), Times.Once);
        }
        #endregion
    }
}
