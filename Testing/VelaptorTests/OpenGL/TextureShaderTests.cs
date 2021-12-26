// <copyright file="TextureShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
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
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoaderService;
        private readonly OpenGLInitObservable glInitObservable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureShaderTests"/> class.
        /// </summary>
        public TextureShaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
            this.mockShaderLoaderService = new Mock<IShaderLoaderService<uint>>();
            this.glInitObservable = new OpenGLInitObservable();
        }

        #region Method Tests
        [Fact]
        public void Use_WhenInvoked_SetsShaderAsUsed()
        {
            // Arrange
            const uint shaderId = 78;
            const int uniformLocation = 1234;
            this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
            this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "mainTexture"))
                .Returns(uniformLocation);
            var status = 1;
            this.mockGL.Setup(m
                => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus, out status));

            var shader = new TextureShader(
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockShaderLoaderService.Object,
                this.glInitObservable);

            this.glInitObservable.OnOpenGLInitialized();

            // Act
            shader.Use();

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Once);
            this.mockGL.Verify(m => m.Uniform1(uniformLocation, 0), Times.Once);
        }
        #endregion
    }
}
