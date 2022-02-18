// <copyright file="WindowSizeEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using Velaptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="WindowSizeEventArgs"/> class.
    /// </summary>
    public class WindowSizeEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsProperties()
        {
            // Arrange
            var eventArgs = new WindowSizeEventArgs(123, 456);

            // Act
            var actualWidth = eventArgs.Width;
            var actualHeight = eventArgs.Height;

            // Assert
            Assert.Equal(123, actualWidth);
            Assert.Equal(456, actualHeight);
        }
        #endregion
    }
}
