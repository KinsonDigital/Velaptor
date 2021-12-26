// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input
{
    using System;
    using System.Linq;
    using Velaptor.Input;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Mouse"/> class.
    /// </summary>
    public class MouseTests : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseTests"/> class.
        /// </summary>
        public MouseTests() => ClearMouseState();

        [Fact]
        public void GetState_WhenInvokedWithNoDefaultState_SetsUpDefaultState()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            var actual = mouse.GetState();

            var keyCodes = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>().ToArray();

            // Assert
            Assert.All(keyCodes, (mouseInput) =>
            {
                Assert.False(actual.GetButtonState(mouseInput));
            });
        }

        [Fact]
        public void GetState_WhenInvoked_ReturnsCorrectState()
        {
            // Arrange
            var mouse = new Mouse();
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = true;

            // Act
            var actual = mouse.GetState();

            // Assert
            Assert.True(actual.IsLeftButtonDown());
        }

        [Theory]
        [InlineData(MouseButton.LeftButton)]
        [InlineData(MouseButton.RightButton)]
        [InlineData(MouseButton.MiddleButton)]
        public void SetState_WithButton_CorrectlySetsState(MouseButton button)
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetState(button, true);

            // Assert
            Assert.True(IMouseInput<MouseButton, MouseState>.InputStates[button]);
        }

        [Fact]
        public void SetXPos_WhenInvoked_SetsXPosition()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetXPos(11);
            var actual = mouse.GetState().GetX();

            // Assert
            Assert.Equal(11, actual);
        }

        [Fact]
        public void SetYPos_WhenInvoked_SetsYPosition()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetYPos(22);
            var actual = mouse.GetState().GetY();

            // Assert
            Assert.Equal(22, actual);
        }

        [Fact]
        public void SetScrollWheelValue_WhenInvoked_SetsScrollWheelValue()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetScrollWheelValue(33);
            var actual = mouse.GetState().GetScrollWheelValue();

            // Assert
            Assert.Equal(33, actual);
        }

        [Fact]
        public void Reset_WhenInvoked_InitializesButtonState()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.Reset();

            // Assert
            Assert.Equal(Enum.GetValues(typeof(MouseButton)).Length, IMouseInput<MouseButton, MouseState>.InputStates.Count);
            Assert.All(IMouseInput<MouseButton, MouseState>.InputStates, (state) =>
            {
                var (_, value) = state;
                Assert.False(value);
            });
        }

        [Fact]
        public void Reset_WhenInvoked_CorrectlyResetsAllState()
        {
            // Arrange
            var mouse = new Mouse();

            var keyCodes = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>().ToArray();

            foreach (var key in keyCodes)
            {
                IMouseInput<MouseButton, MouseState>.InputStates.Add(key, true);
            }

            mouse.SetXPos(111);
            mouse.SetYPos(222);
            mouse.SetScrollWheelValue(333);

            // Act
            mouse.Reset();
            var actual = mouse.GetState();

            // Assert
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton]);
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.RightButton]);
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.MiddleButton]);
            Assert.Equal(0, actual.GetX());
            Assert.Equal(0, actual.GetY());
            Assert.Equal(0, actual.GetScrollWheelValue());
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => ClearMouseState();

        /// <summary>
        /// Clears the state for the mouse for testing purposes.
        /// </summary>
        private static void ClearMouseState()
        {
            new Mouse().SetXPos(0);
            new Mouse().SetYPos(0);
            IMouseInput<MouseButton, MouseState>.InputStates.Clear();
        }
    }
}
