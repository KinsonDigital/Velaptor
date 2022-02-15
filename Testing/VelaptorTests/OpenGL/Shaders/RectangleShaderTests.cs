// <copyright file="RectangleShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders
{
    using Moq;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Services;
    using Velaptor.OpenGL.Shaders;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Xunit;

    public class RectangleShaderTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsUpCorrectShaderName()
        {
            // Arrange
            var shader = new RectangleShader(
                new Mock<IGLInvoker>().Object,
                new Mock<IOpenGLService>().Object,
                new Mock<IShaderLoaderService<uint>>().Object,
                new Mock<IReactable<GLInitData>>().Object,
                new Mock<IReactable<ShutDownData>>().Object);

            // Act
            var actual = shader.Name;

            // Assert
            Assert.Equal("Rectangle", actual);
        }
        #endregion
    }
}
