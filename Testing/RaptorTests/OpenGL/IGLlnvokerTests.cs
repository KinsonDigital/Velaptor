// <copyright file="IGLlnvokerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using Raptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="IGLInvoker"/> interface.
    /// </summary>
    public class IGLlnvokerTests
    {
        [Fact]
        public void IsOpenGLInitialized_WhenInitialized_ReturnsTrue()
        {
            // Act
            IGLInvoker.SetOpenGLAsInitialized();

            // Assert
            Assert.True(IGLInvoker.IsOpenGLInitialized());
        }
    }
}
