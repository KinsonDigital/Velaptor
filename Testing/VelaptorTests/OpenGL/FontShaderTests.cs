// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontShader"/> class.
    /// </summary>
    public class FontShaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
        private readonly Mock<IReactor<GLInitData>> mockGLInitReactor;
        private readonly Mock<IDisposable> mockGLInitUnsubscriber;
        private readonly Mock<IReactor<ShutDownData>> mockShutDownReactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
        /// </summary>
        public FontShaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
            this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();
            this.mockShutDownReactor = new Mock<IReactor<ShutDownData>>();
            this.mockGLInitReactor = new Mock<IReactor<GLInitData>>();
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
                    this.mockGLExtensions.Object,
                    this.mockShaderLoader.Object,
                    null,
                    this.mockShutDownReactor.Object);
            }, "The parameter must not be null. (Parameter 'glInitReactor')");
        }

        [Fact]
        public void Ctor_WithNullShutDownReactorParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontShader(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockShaderLoader.Object,
                    this.mockGLInitReactor.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactor')");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Use_WhenInvoked_SetsShaderAsUsed()
        {
            // Arrange
            IObserver<GLInitData>? glInitObserver = null;

            const uint shaderId = 78;
            const int uniformLocation = 1234;
            this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
            this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture"))
                .Returns(uniformLocation);
            var status = 1;
            this.mockGL.Setup(m
                => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus, out status));
            this.mockGLInitReactor.Setup(m => m.Subscribe(It.IsAny<IObserver<GLInitData>>()))
                .Returns(this.mockGLInitUnsubscriber.Object)
                .Callback<IObserver<GLInitData>>(observer =>
                {
                    if (observer is null)
                    {
                        Assert.True(false, "GL initialization observable subscription failed.  Observer is null.");
                    }

                    glInitObserver = observer;
                });

            var shader = new FontShader(
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitReactor.Object,
                this.mockShutDownReactor.Object);

            glInitObserver?.OnNext(default);

            // Act
            shader.Use();

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture1), Times.Once);
            this.mockGL.Verify(m => m.Uniform1(uniformLocation, 1), Times.Once);
        }
        #endregion
    }
}
