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

            // Assert
            Assert.Equal(11, IMouseInput<MouseButton, MouseState>.XPos);
        }

        [Fact]
        public void SetYPos_WhenInvoked_SetsYPosition()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetYPos(22);

            // Assert
            Assert.Equal(22, IMouseInput<MouseButton, MouseState>.YPos);
        }

        [Fact]
        public void SetScrollWheelValue_WhenInvoked_SetsScrollWheelValue()
        {
            // Arrange
            var mouse = new Mouse();

            // Act
            mouse.SetScrollWheelValue(33);

            // Assert
            Assert.Equal(33, IMouseInput<MouseButton, MouseState>.ScrollWheelValue);
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
                Assert.False(state.Value);
            });
        }

        [Fact]
        public void Reset_WhenInvoked_CorrectlyResetsAllState()
        {
            // Arrange
            var mouse = new Mouse();

            var keyCodes = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>().ToArray();

            for (var i = 0; i < keyCodes.Length; i++)
            {
                IMouseInput<MouseButton, MouseState>.InputStates.Add(keyCodes[i], true);
            }

            IMouseInput<MouseButton, MouseState>.XPos = 111;
            IMouseInput<MouseButton, MouseState>.YPos = 222;
            IMouseInput<MouseButton, MouseState>.ScrollWheelValue = 333;

            // Act
            mouse.Reset();

            // Assert
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton]);
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.RightButton]);
            Assert.False(IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.MiddleButton]);
            Assert.Equal(0, IMouseInput<MouseButton, MouseState>.XPos);
            Assert.Equal(0, IMouseInput<MouseButton, MouseState>.YPos);
            Assert.Equal(0, IMouseInput<MouseButton, MouseState>.ScrollWheelValue);
        }

        /// <inheritdoc/>
        public void Dispose() => ClearMouseState();

        /// <summary>
        /// Clears the state for the mouse for testing purposes.
        /// </summary>
        private void ClearMouseState()
        {
            IMouseInput<MouseButton, MouseState>.InputStates.Clear();
            IMouseInput<MouseButton, MouseState>.XPos = 0;
            IMouseInput<MouseButton, MouseState>.YPos = 0;
            IMouseInput<MouseButton, MouseState>.ScrollWheelValue = 0;
        }
    }
}
