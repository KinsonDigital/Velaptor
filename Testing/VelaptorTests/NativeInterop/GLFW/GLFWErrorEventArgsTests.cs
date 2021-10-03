// <copyright file="GLFWErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW
{
    using Velaptor.NativeInterop.GLFW;
    using Velaptor.OpenGL;
    using Xunit;

    public class GLFWErrorEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsPropertyValues()
        {
            // Arrange & Act
            var eventArgs = new GLFWErrorEventArgs(GLFWErrorCode.ApiUnavailable, "test-message");

            // Assert
            Assert.Equal(GLFWErrorCode.ApiUnavailable, eventArgs.ErrorCode);
            Assert.Equal("test-message", eventArgs.ErrorMessage);
        }
        #endregion
    }
}
