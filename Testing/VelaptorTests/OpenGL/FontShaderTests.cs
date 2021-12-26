// <copyright file="FontShaderTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="FontShader"/> class.
    /// </summary>
    public class FontShaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IShaderLoaderService<uint>> mockShaderLoaderService;
        private readonly OpenGLInitObservable glInitObservable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
        /// </summary>
        public FontShaderTests()
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
            this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture"))
                .Returns(uniformLocation);
            var status = 1;
            this.mockGL.Setup(m
                => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus, out status));

            var shader = new FontShader(
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockShaderLoaderService.Object,
                this.glInitObservable);

            this.glInitObservable.OnOpenGLInitialized();

            // Act
            shader.Use();

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture1), Times.Once);
            this.mockGL.Verify(m => m.Uniform1(uniformLocation, 1), Times.Once);
        }
        #endregion
    }
}
