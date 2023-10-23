// <copyright file="MouseStateTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System.ComponentModel;
using System.Drawing;
using FluentAssertions;
using Velaptor.Input;
using Xunit;

/// <summary>
/// Tests the <see cref="MouseState"/> struct.
/// </summary>
public class MouseStateTests
{
    #region Method Tests
    [Fact]
    public void SetPosition_WhenInvoked_SetsPosition()
    {
        // Arrange
        var expected = new Point(11, 22);
        var state = default(MouseState);

        // Act
        state.SetPosition(11, 22);
        var actual = state.GetPosition();

        // Arrange
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetX_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);
        state.SetPosition(123, 0);

        // Act
        var actual = state.GetX();

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void GetY_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);
        state.SetPosition(0, 123);

        // Act
        var actual = state.GetY();

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void IsButtonDown_WithInvalidParamValue_ThrowsException()
    {
        // Arrange
        const int invalidMouseButton = 1234;
        var expected = $"The value of argument 'button' ({invalidMouseButton}) is invalid for Enum type " +
                       $"'{nameof(MouseButton)}'. (Parameter 'button')";

        var state = default(MouseState);

        // Act
        var act = () => state.IsButtonDown((MouseButton)1234);

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void IsButtonDown_WithLeftButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, true);
        state.SetButtonState(MouseButton.MiddleButton, false);
        state.SetButtonState(MouseButton.RightButton, false);

        // Act
        var actualLeft = state.IsButtonDown(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonDown(MouseButton.MiddleButton);
        var actualRight = state.IsButtonDown(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeTrue();
        actualMiddle.Should().BeFalse();
        actualRight.Should().BeFalse();
    }

    [Fact]
    public void IsButtonDown_WithMiddleButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, false);
        state.SetButtonState(MouseButton.MiddleButton, true);
        state.SetButtonState(MouseButton.RightButton, false);

        // Act
        var actualLeft = state.IsButtonDown(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonDown(MouseButton.MiddleButton);
        var actualRight = state.IsButtonDown(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeFalse();
        actualMiddle.Should().BeTrue();
        actualRight.Should().BeFalse();
    }

    [Fact]
    public void IsButtonDown_WithRightButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, false);
        state.SetButtonState(MouseButton.MiddleButton, false);
        state.SetButtonState(MouseButton.RightButton, true);

        // Act
        var actualLeft = state.IsButtonDown(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonDown(MouseButton.MiddleButton);
        var actualRight = state.IsButtonDown(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeFalse();
        actualMiddle.Should().BeFalse();
        actualRight.Should().BeTrue();
    }

    [Fact]
    public void IsButtonUp_WithInvalidParamValue_ThrowsException()
    {
        // Arrange
        const int invalidMouseButton = 1234;
        var expected = $"The value of argument 'button' ({invalidMouseButton}) is invalid for Enum type " +
                       $"'{nameof(MouseButton)}'. (Parameter 'button')";

        var state = default(MouseState);

        // Act
        var act = () => state.IsButtonUp((MouseButton)invalidMouseButton);

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void IsButtonUp_WithLeftButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, false);
        state.SetButtonState(MouseButton.MiddleButton, true);
        state.SetButtonState(MouseButton.RightButton, true);

        // Act
        var actualLeft = state.IsButtonUp(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonUp(MouseButton.MiddleButton);
        var actualRight = state.IsButtonUp(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeTrue();
        actualMiddle.Should().BeFalse();
        actualRight.Should().BeFalse();
    }

    [Fact]
    public void IsButtonUp_WithMiddleButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, true);
        state.SetButtonState(MouseButton.MiddleButton, false);
        state.SetButtonState(MouseButton.RightButton, true);

        // Act
        var actualLeft = state.IsButtonUp(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonUp(MouseButton.MiddleButton);
        var actualRight = state.IsButtonUp(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeFalse();
        actualMiddle.Should().BeTrue();
        actualRight.Should().BeFalse();
    }

    [Fact]
    public void IsButtonUp_WithRightButtonDown_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(MouseButton.LeftButton, true);
        state.SetButtonState(MouseButton.MiddleButton, true);
        state.SetButtonState(MouseButton.RightButton, false);

        // Act
        var actualLeft = state.IsButtonDown(MouseButton.LeftButton);
        var actualMiddle = state.IsButtonDown(MouseButton.MiddleButton);
        var actualRight = state.IsButtonDown(MouseButton.RightButton);

        // Assert
        actualLeft.Should().BeTrue();
        actualMiddle.Should().BeTrue();
        actualRight.Should().BeFalse();
    }

    [Fact]
    public void SetScrollWheelValue_WhenInvoked_SetsValue()
    {
        // Arrange
        var state = default(MouseState);

        // Act
        state.SetScrollWheelValue(123);
        var actual = state.GetScrollWheelValue();

        // Assert
        actual.Should().Be(123);
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true, false, false)]
    [InlineData(MouseButton.MiddleButton, false, true, false)]
    [InlineData(MouseButton.RightButton, false, false, true)]
    public void SetButtonState_WhenInvoked_SetsState(MouseButton mouseButton, bool expectedLeft, bool expectedMiddle, bool expectedRight)
    {
        // Arrange
        var state = default(MouseState);

        // Act
        state.SetButtonState(mouseButton, true);
        var actualLeft = state.GetButtonState(MouseButton.LeftButton);
        var actualMiddle = state.GetButtonState(MouseButton.MiddleButton);
        var actualRight = state.GetButtonState(MouseButton.RightButton);

        // Assert
        actualLeft.Should().Be(expectedLeft);
        actualMiddle.Should().Be(expectedMiddle);
        actualRight.Should().Be(expectedRight);
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true)]
    [InlineData(MouseButton.MiddleButton, true)]
    [InlineData(MouseButton.RightButton, true)]
    public void AnyButtonsDown_WhenInvoked_ReturnsTrue(MouseButton button, bool expected)
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(button, expected);

        // Act
        var actual = state.AnyButtonsDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void AnyButtonsDown_WithNoButtonsDown_ReturnsFalse()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.LeftButton, false);
        state.SetButtonState(MouseButton.MiddleButton, false);
        state.SetButtonState(MouseButton.RightButton, false);

        // Act
        var actual = state.AnyButtonsDown();

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsLeftButtonDown_WhenLeftButtonIsDown_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.LeftButton, true);

        // Act
        var actual = state.IsLeftButtonDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsMiddleButtonDown_WhenLeftButtonIsDown_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.MiddleButton, true);

        // Act
        var actual = state.IsMiddleButtonDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsRightButtonDown_WhenLeftButtonIsDown_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.RightButton, true);

        // Act
        var actual = state.IsRightButtonDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsLeftButtonUp_WhenLeftButtonIsUp_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.LeftButton, false);

        // Act
        var actual = state.IsLeftButtonUp();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsMiddleButtonUp_WhenLeftButtonIsUp_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.MiddleButton, false);

        // Act
        var actual = state.IsMiddleButtonUp();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsRightButtonUp_WhenLeftButtonIsUp_ReturnsTrue()
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(MouseButton.RightButton, false);

        // Act
        var actual = state.IsRightButtonUp();

        // Assert
        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true)]
    [InlineData(MouseButton.MiddleButton, true)]
    [InlineData(MouseButton.RightButton, true)]
    public void GetButtonState_WhenInvoked_ReturnsCorrectResult(MouseButton mouseButton, bool expected)
    {
        // Arrange
        var state = default(MouseState);
        state.SetButtonState(mouseButton, expected);

        // Act
        var actual = state.GetButtonState(mouseButton);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true)]
    [InlineData(MouseButton.MiddleButton, true)]
    [InlineData(MouseButton.RightButton, true)]
    public void GetButtonState_WhenButtonIsDown_ReturnsTrue(MouseButton downButton, bool expected)
    {
        // Arrange
        var state = default(MouseState);

        state.SetButtonState(downButton, true);

        // Act
        var actual = state.GetButtonState(downButton);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetButtonState_WithInvalidMouseButton_ReturnsFalse()
    {
        // Arrange
        var state = default(MouseState);

        // Act
        var actual = state.GetButtonState((MouseButton)1234);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenUsingSameTypeParamWhenEqual_ReturnsTrue()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = default(MouseState);

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = new object();

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOfSameTypeWhenEqual_ReturnsTrue()
    {
        // Arrange
        var stateA = default(MouseState);
        object stateB = default(MouseState);

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WhenBothOperandsAreEqual_ReturnsTrue()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = default(MouseState);

        // Act
        var actual = stateA == stateB;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WhenBothOperandsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = default(MouseState);
        stateB.SetPosition(11, 22);

        // Act
        var actual = stateA == stateB;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void NotEqualsOperator_WhenBothOperandsAreEqual_ReturnsFalse()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = default(MouseState);

        // Act
        var actual = stateA != stateB;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void NotEqualsOperator_WhenBothOperandsAreNotEqual_ReturnsTrue()
    {
        // Arrange
        var stateA = default(MouseState);
        var stateB = default(MouseState);
        stateB.SetPosition(11, 22);

        // Act
        var actual = stateA != stateB;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion
}
