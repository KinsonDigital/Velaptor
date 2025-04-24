// <copyright file="MouseStateDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using Shouldly;
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
        sut.X.ShouldBe(1);
        sut.Y.ShouldBe(2);
        sut.Button.ShouldBe(MouseButton.MiddleButton);
        sut.ButtonIsDown.ShouldBeTrue();
        sut.ScrollDirection.ShouldBe(MouseScrollDirection.ScrollDown);
        sut.ScrollWheelValue.ShouldBe(3);
    }
    #endregion
}
