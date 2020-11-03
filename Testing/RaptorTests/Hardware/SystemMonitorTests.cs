// <copyright file="SystemMonitorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Hardware
{
    using Raptor.Hardware;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SystemMonitor"/> struct.
    /// </summary>
    public class SystemMonitorTests
    {
        #region Method Tests
        [Fact]
        public void Equals_WhenInvokingOverloadWithSameTypeParam_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            var monitorB = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            // Act
            var actual = monitorA.Equals(monitorB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenInvokingOverloadWithObjectParamAndIsNotSameType_ReturnsFalse()
        {
            // Arrange
            var monitorA = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            var monitorB = new object();

            // Act
            var actual = monitorA.Equals(monitorB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenInvokingOverloadWithObjectParamAndIsEqual_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            object monitorB = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            // Act
            var actual = monitorA.Equals(monitorB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOverloadedOperator_WhenInvokedWithEqualObjects_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            var monitorB = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            // Act
            var actual = monitorA == monitorB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOverloadedOperator_WhenInvokedWithNonEqualObjects_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor()
            {
                IsMain = true,
                RedBitDepth = 1,
                GreenBitDepth = 2,
                BlueBitDepth = 3,
                Width = 4,
                Height = 5,
                RefreshRate = 6,
                HorizontalScale = 7,
                VerticalScale = 8,
            };

            var monitorB = new SystemMonitor()
            {
                IsMain = false,
                RedBitDepth = 11,
                GreenBitDepth = 22,
                BlueBitDepth = 33,
                Width = 44,
                Height = 55,
                RefreshRate = 66,
                HorizontalScale = 77,
                VerticalScale = 88,
            };

            // Act
            var actual = monitorA != monitorB;

            // Assert
            Assert.True(actual);
        }
        #endregion
    }
}
