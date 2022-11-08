// <copyright file="GLFWMonitorChangedEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.NativeInterop.GLFW;
using Xunit;

namespace VelaptorTests.NativeInterop.GLFW;

public class GLFWMonitorChangedEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValue()
    {
        // Arrange & Act
        var eventArgs = new GLFWMonitorChangedEventArgs(true);

        // Assert
        Assert.True(eventArgs.IsConnected);
    }
    #endregion
}
