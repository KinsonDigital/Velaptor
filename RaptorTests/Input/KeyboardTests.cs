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
        public void GetState_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            Keyboard.SetKeyState(KeyCode.T, true);

            // Act
            var actual = Keyboard.GetState();

            // Assert
            AssertHelpers.AllItemsAre(actual.GetKeyStates(), state =>
            {
                if (state.Key == KeyCode.T)
                    return state.Value;
                else
                    return true;
            });
        }

        [Fact]
        public void SetKeyState_WhenInvoked_SetsProperKey()
        {
            // Act
            Keyboard.SetKeyState(KeyCode.F, true);
            var actual = Keyboard.GetState();

            // Assert
            AssertHelpers.AllItemsAre(actual.GetKeyStates(), state =>
            {
                if (state.Key == KeyCode.F)
                    return state.Value;
                else
                    return true;
            });
        }
        #endregion
    }
}
