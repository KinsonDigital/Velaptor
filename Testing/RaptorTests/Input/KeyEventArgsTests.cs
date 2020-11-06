// <copyright file="KeyEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Input
{
    using Raptor.Input;
    using Xunit;

    // TODO: Remove this class/file

    /// <summary>
    /// Tests the <see cref="KeyEventArgs"/> class.
    /// </summary>
    public class KeyEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoking_SetsKeysProp()
        {
            // Arrange
            var expected = new KeyCode[]
            {
                KeyCode.Left,
                KeyCode.Right,
            };

            // Act
            var eventArgs = new KeyEventArgs(new KeyCode[] { KeyCode.Left, KeyCode.Right });
            var actual = eventArgs.Keys;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Keys_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var expected = new KeyCode[]
            {
                KeyCode.Up,
                KeyCode.Down,
            };

            // Act
            var eventArgs = new KeyEventArgs(new KeyCode[] { KeyCode.Up, KeyCode.Down });
            var actual = eventArgs.Keys;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
