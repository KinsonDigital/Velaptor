// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Input
{
    using Raptor.Input;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Keyboard"/> class.
    /// </summary>
    public class KeyboardTests
    {
        #region Method Tests
        [Fact]
        public void GetState_WhenInvokedWithNoKeyStates_SetsUpKeyStates()
        {
            // Act
            var keyboard = new Keyboard();
            keyboard.GetState();

            // Assert
            Assert.Equal(119, IGameInput<KeyCode, KeyboardState>.InputStates.Count);
        }

        [Fact]
        public void GetState_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var keyboard = new Keyboard();
            Keyboard.SetKeyState(KeyCode.T, true);

            // Act
            var actual = keyboard.GetState();

            // Assert
            AssertHelpers.AllItemsAre(actual.GetKeyStates(), state =>
            {
                if (state.Key == KeyCode.T)
                {
                    return state.Value;
                }
                else
                {
                    return true;
                }
            });
        }

        [Fact]
        public void SetKeyState_WhenInvoked_SetsProperKey()
        {
            // Act
            var keyboard = new Keyboard();
            Keyboard.SetKeyState(KeyCode.F, true);
            var actual = keyboard.GetState();

            // Assert
            AssertHelpers.AllItemsAre(actual.GetKeyStates(), state =>
            {
                if (state.Key == KeyCode.F)
                {
                    return state.Value;
                }
                else
                {
                    return true;
                }
            });
        }
        #endregion
    }
}
