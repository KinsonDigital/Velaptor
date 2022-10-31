// <copyright file="WindowSizeEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using FluentAssertions;
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
            var eventArgs = new WindowSizeEventArgs(123u, 456u);

            // Act
            var actualWidth = eventArgs.Width;
            var actualHeight = eventArgs.Height;

            // Assert
            Assert.Equal(123u, actualWidth);
            Assert.Equal(456u, actualHeight);
        }
        #endregion
    }
}
