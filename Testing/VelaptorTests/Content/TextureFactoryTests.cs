// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using Moq;
    using Velaptor.Content.Factories;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureFactory"/> class.
    /// </summary>
    public class TextureFactoryTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
        /// </summary>
        public TextureFactoryTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureFactory(null, this.mockGLExtensions.Object);
            }, "The parameter must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WithNullGLInvokerExtensions_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureFactory(this.mockGL.Object, null);
            }, "The parameter must not be null. (Parameter 'glExtensions')");
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Create_WhenUsingNullOrEmptyName_ThrowsException(string name)
        {
            // Arrange
            var factory = CreateFactory();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                factory.Create(name, "test-path", new ImageData(null, 1, 2), true);
            }, "The parameter must not be null or empty. (Parameter 'name')");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Create_WhenUsingNullOrEmptyFilePath_ThrowsException(string filePath)
        {
            // Arrange
            var factory = CreateFactory();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                factory.Create("test-name", filePath, new ImageData(null, 1, 2), true);
            }, "The parameter must not be null or empty. (Parameter 'filePath')");
        }

        [Fact]
        public void Create_WhenInvoked_WorksCorrectly()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            factory.Create("test-name", "test-path", new ImageData(null, 1, 2), true);

            // Assert
            // NOTE: These are only here to prove that the same injected objects are the ones being used.
            this.mockGL.Verify(m => m.GenTexture(), Times.Once);
            this.mockGLExtensions.Verify(m => m.LabelTexture(It.IsAny<uint>(), It.IsAny<string>()), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureFactory"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureFactory CreateFactory() => new (this.mockGL.Object, this.mockGLExtensions.Object);
    }
}
