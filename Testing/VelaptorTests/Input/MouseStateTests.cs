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
    public void GetPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(10, 20);

        // Act
        var actual = sut.GetPosition();

        // Assert
        actual.Should().Be(new Point(10, 20));
    }

    [Fact]
    public void GetX_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(x: 123, y: 0);

        // Act
        var actual = sut.GetX();

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void GetY_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(x: 0, y: 123);

        // Act
        var actual = sut.GetY();

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

        var sut = default(MouseState);

        // Act
        var act = () => sut.IsButtonDown((MouseButton)1234);

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void IsButtonUp_WithInvalidParamValue_ThrowsException()
    {
        // Arrange
        const int invalidMouseButton = 1234;
        var expected = $"The value of argument 'button' ({invalidMouseButton}) is invalid for Enum type " +
                       $"'{nameof(MouseButton)}'. (Parameter 'button')";

        var sut = default(MouseState);

        // Act
        var act = () => sut.IsButtonUp((MouseButton)invalidMouseButton);

        // Assert
        act.Should()
            .Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true)]
    [InlineData(MouseButton.MiddleButton, true)]
    [InlineData(MouseButton.RightButton, true)]
    public void IsButtonUp_WhenInvoked_ReturnsCorrectResult(MouseButton button, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(
            isLeftButtonDown: button != MouseButton.LeftButton,
            isRightButtonDown: button != MouseButton.RightButton,
            isMiddleButtonDown: button != MouseButton.MiddleButton);

        // Act
        var actual = sut.IsButtonUp(button);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(MouseButton.LeftButton, true)]
    [InlineData(MouseButton.MiddleButton, true)]
    [InlineData(MouseButton.RightButton, true)]
    public void IsButtonDown_WhenInvoked_ReturnsCorrectResult(MouseButton button, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(
            isLeftButtonDown: button == MouseButton.LeftButton,
            isRightButtonDown: button == MouseButton.RightButton,
            isMiddleButtonDown: button == MouseButton.MiddleButton);

        // Act
        var actual = sut.IsButtonDown(button);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, false, true)]
    [InlineData(false, false, true, true)]
    [InlineData(false, false, false, false)]
    public void AnyButtonsDown_WhenInvoked_ReturnsCorrectResult(
        bool leftButtonState,
        bool rightButtonState,
        bool middleButtonState,
        bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(
            isLeftButtonDown: leftButtonState,
            isRightButtonDown: rightButtonState,
            isMiddleButtonDown: middleButtonState);

        // Act
        var actual = sut.AnyButtonsDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetScrollWheelValue_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(scrollWheelValue: 123);

        // Act
        var actual = sut.GetScrollWheelValue();

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void GetScrollDirection_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(scrollDirection: MouseScrollDirection.ScrollDown);

        // Act
        var actual = sut.GetScrollDirection();

        // Assert
        actual.Should().Be(MouseScrollDirection.ScrollDown);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void IsLeftButtonDown_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isLeftButtonDown: isButtonDown);

        // Act
        var actual = sut.IsLeftButtonDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void IsMiddleButtonDown_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isMiddleButtonDown: isButtonDown);

        // Act
        var actual = sut.IsMiddleButtonDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void IsRightButtonDown_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isRightButtonDown: isButtonDown);

        // Act
        var actual = sut.IsRightButtonDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void IsLeftButtonUp_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isLeftButtonDown: isButtonDown);

        // Act
        var actual = sut.IsLeftButtonUp();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void IsMiddleButtonUp_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isMiddleButtonDown: isButtonDown);

        // Act
        var actual = sut.IsMiddleButtonUp();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void IsRightButtonUp_WhenInvoked_ReturnsCorrectResult(bool isButtonDown, bool expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest(isRightButtonDown: isButtonDown);

        // Act
        var actual = sut.IsRightButtonUp();

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
        var sut = CreateSystemUnderTest(
            isLeftButtonDown: downButton == MouseButton.LeftButton,
            isMiddleButtonDown: downButton == MouseButton.MiddleButton,
            isRightButtonDown: downButton == MouseButton.RightButton);

        // Act
        var actual = sut.GetButtonState(downButton);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetButtonState_WithInvalidMouseButton_ReturnsFalse()
    {
        // Arrange
        var sut = default(MouseState);

        // Act
        var actual = sut.GetButtonState((MouseButton)1234);

        // Assert
        actual.Should().BeFalse();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="MouseState"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private static MouseState CreateSystemUnderTest(
        int x = 0,
        int y = 0,
        bool isLeftButtonDown = false,
        bool isRightButtonDown = false,
        bool isMiddleButtonDown = false,
        MouseScrollDirection scrollDirection = MouseScrollDirection.None,
        int scrollWheelValue = 0)
    {
        return new MouseState(
            new Point(x, y),
            isLeftButtonDown,
            isRightButtonDown,
            isMiddleButtonDown,
            scrollDirection,
            scrollWheelValue);
    }
}
