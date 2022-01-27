// <copyright file="TextureShaderTests.cs" company="KinsonDigital">
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
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureShader"/> class.
    /// </summary>
    public class TextureShaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
        private readonly Mock<IReactor<GLInitData>> mockGLInitReactor;
        private readonly Mock<IDisposable> mockGLInitUnsubscriber;
        private readonly Mock<IReactor<ShutDownData>> mockShutDownReactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureShaderTests"/> class.
        /// </summary>
        public TextureShaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
            this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();
            this.mockGLInitReactor = new Mock<IReactor<GLInitData>>();
            this.mockGLInitUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownReactor = new Mock<IReactor<ShutDownData>>();
        }

        #region Method Tests
        [Fact]
        public void Use_WhenInvoked_SetsShaderAsUsed()
        {
            // Arrange
            IObserver<GLInitData>? glInitObserver = null;
            const uint shaderId = 78;
            const int uniformLocation = 1234;
            this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
            this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "mainTexture"))
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

            var shader = new TextureShader(
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitReactor.Object,
                this.mockShutDownReactor.Object);

            glInitObserver.OnNext(default);

            // Act
            shader.Use();

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Once);
            this.mockGL.Verify(m => m.Uniform1(uniformLocation, 0), Times.Once);
        }
        #endregion
    }
}
