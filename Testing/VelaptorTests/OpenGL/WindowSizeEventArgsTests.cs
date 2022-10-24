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
            (var heightArg, var widthArg) = (123, 456);
            var eventArgs = new WindowSizeEventArgs(widthArg, heightArg);

            // Act
            var actualWidth = eventArgs.Width;
            var actualHeight = eventArgs.Height;

            // Assert
            actualHeight.Should().Be(heightArg);
            actualWidth.Should().Be(widthArg);
        }
        #endregion
    }
}
