// <copyright file="MouseStateDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="MouseStateData"/> struct.
/// </summary>
public class MouseStateDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var sut = new MouseStateData
        {
            X = 1,
            Y = 2,
            Button = MouseButton.MiddleButton,
            ButtonIsDown = true,
            ScrollDirection = MouseScrollDirection.ScrollDown,
            ScrollWheelValue = 3,
        };

        // Assert
        sut.X.Should().Be(1);
        sut.Y.Should().Be(2);
        sut.Button.Should().Be(MouseButton.MiddleButton);
        sut.ButtonIsDown.Should().BeTrue();
        sut.ScrollDirection.Should().Be(MouseScrollDirection.ScrollDown);
        sut.ScrollWheelValue.Should().Be(3);
    }
    #endregion
}
