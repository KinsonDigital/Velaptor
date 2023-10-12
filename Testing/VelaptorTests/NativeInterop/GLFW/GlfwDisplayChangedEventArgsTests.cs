// <copyright file="GlfwDisplayChangedEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using Velaptor.NativeInterop.GLFW;
using Xunit;
using FluentAssertions;

public class GlfwDisplayChangedEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValue()
    {
        // Arrange & Act
        var eventArgs = new GlfwDisplayChangedEventArgs(true);

        // Assert
        eventArgs.IsConnected.Should().BeTrue();
    }
    #endregion
}
