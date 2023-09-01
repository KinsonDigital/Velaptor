// <copyright file="GlfwDisplayChangedEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using Velaptor.NativeInterop.GLFW;
using Xunit;

public class GlfwDisplayChangedEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValue()
    {
        // Arrange & Act
        var eventArgs = new GlfwDisplayChangedEventArgs(true);

        // Assert
        Assert.True(eventArgs.IsConnected);
    }
    #endregion
}
