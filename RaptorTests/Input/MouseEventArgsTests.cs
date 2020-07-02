// <copyright file="MouseEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Input
{
    using System.Numerics;
    using Raptor.Input;
    using Xunit;

    /// <summary>
    /// Unit tests to test the <see cref="MouseEventArgs"/> class.
    /// </summary>
    public class MouseEventArgsTests
    {
        [Fact]
        public void Ctor_WhenInvoking_SetsMouseInputStatePropValue()
        {
            // Arrange
            var mouseEventArgs = new MouseEventArgs(new MouseInputState()
            {
                LeftButtonDown = true,
                RightButtonDown = true,
                MiddleButtonDown = true,
                Position = new Vector2(11, 22),
                ScrollWheelValue = 4,
            });

            var expected = new MouseInputState()
            {
                LeftButtonDown = true,
                RightButtonDown = true,
                MiddleButtonDown = true,
                Position = new Vector2(11, 22),
                ScrollWheelValue = 4,
            };

            // Act
            var actual = mouseEventArgs.State;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
