// <copyright file="WindowSizeEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using Shouldly;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="WindowSizeEventArgs"/> class.
/// </summary>
public class WindowSizeEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperties()
    {
        // Arrange
        var eventArgs = new WindowSizeEventArgs(123u, 456u);

        // Act
        var actualWidth = eventArgs.Width;
        var actualHeight = eventArgs.Height;

        // Assert
        actualWidth.ShouldBe(123u);
        actualHeight.ShouldBe(456u);
    }
    #endregion
}
