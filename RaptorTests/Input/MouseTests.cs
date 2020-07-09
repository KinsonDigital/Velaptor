// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Input
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Input;
    using Xunit;

    public class MouseTests
    {
        [ExcludeFromCodeCoverage]
        public MouseTests()
        {
            // Reset the static values of the mouse
            Mouse.SetPosition(0, 0);
            Mouse.SetButtonState(MouseButton.LeftButton, false);
            Mouse.SetButtonState(MouseButton.MiddleButton, false);
            Mouse.SetButtonState(MouseButton.RightButton, false);
            Mouse.SetScrollWheelValue(0);
        }

        [Fact]
        public void SetPosition_WhenInvoked_SetsPosition()
        {
            // Arrange
            var expected = default(MouseState);
            expected.SetPosition(11, 22);

            // Act
            Mouse.SetPosition(11, 22);
            var actual = Mouse.GetMouseState();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetScrollWheelValue_WhenInvoked_SetsWheelValue()
        {
            // Arrange
            var expected = default(MouseState);
            expected.SetScrollWheelValue(123);

            // Act
            Mouse.SetScrollWheelValue(123);
            var actual = Mouse.GetMouseState();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MouseButton.LeftButton, true)]
        [InlineData(MouseButton.MiddleButton, true)]
        [InlineData(MouseButton.RightButton, true)]
        public void SetButtonState_WhenSettingButton_ReturnsCorrectButtonState(MouseButton mouseButton, bool expected)
        {
            // Arrange
            var state = default(MouseState);
            state.SetButtonState(mouseButton, expected);

            // Act
            state.SetButtonState(mouseButton, expected);
            var actual = state.GetButtonState(mouseButton);

            // Assert
            Assert.True(actual);
        }
    }
}
