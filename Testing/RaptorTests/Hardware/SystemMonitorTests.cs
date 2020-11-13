// <copyright file="SystemMonitorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Hardware
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Moq;
    using Raptor;
    using Raptor.Hardware;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SystemMonitor"/> struct.
    /// </summary>
    public class SystemMonitorTests
    {
        /// <summary>
        /// Gets horizontal DPI data for testing.
        /// </summary>
        /// <returns>The horizontal DPI data and expected results.</returns>
        public static IEnumerable<object[]> GetHorizontalDPIData()
        {
            yield return new object[] { OSPlatform.Windows, 672 };
            yield return new object[] { OSPlatform.Linux, 672 };
            yield return new object[] { OSPlatform.FreeBSD, 672 };
            yield return new object[] { OSPlatform.OSX, 504 };
        }

        /// <summary>
        /// Gets vertical DPI data for testing.
        /// </summary>
        /// <returns>The vertical DPI data and expected results.</returns>
        public static IEnumerable<object[]> GetVerticalDPIData()
        {
            yield return new object[] { OSPlatform.Windows, 768 };
            yield return new object[] { OSPlatform.Linux, 768 };
            yield return new object[] { OSPlatform.FreeBSD, 768 };
            yield return new object[] { OSPlatform.OSX, 576 };
        }

        #region Prop Tests
        [Theory]
        [MemberData(nameof(GetHorizontalDPIData))]
        public void HorizontalDPI_WhenGettingValueOnAnyPlatformExceptOSX_ReturnsCorrectResult(OSPlatform platform, int expectedDPI)
        {
            // Arrange
            var mockPlatform = new Mock<IPlatform>();
            mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(platform);

            var monitor = new SystemMonitor(mockPlatform.Object)
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
            var actual = monitor.HorizontalDPI;

            // Assert
            Assert.Equal(expectedDPI, actual);
        }

        [Theory]
        [MemberData(nameof(GetVerticalDPIData))]
        public void VerticalDPI_WhenGettingValueOnAnyPlatformExceptOSX_ReturnsCorrectResult(OSPlatform platform, int expectedDPI)
        {
            // Arrange
            var mockPlatform = new Mock<IPlatform>();
            mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(platform);

            var monitor = new SystemMonitor(mockPlatform.Object)
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
            var actual = monitor.VerticalDPI;

            // Assert
            Assert.Equal(expectedDPI, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Equals_WhenInvokingOverloadWithSameTypeParamButNull_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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

            SystemMonitor? monitorB = null;

            // Act
            var actual = monitorA.Equals(monitorB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenInvokingOverloadWithSameTypeParam_ReturnsTrue()
        {
            // Arrange
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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

            var monitorB = new SystemMonitor(new Mock<IPlatform>().Object)
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
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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

            object monitorB = new SystemMonitor(new Mock<IPlatform>().Object)
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
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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

            var monitorB = new SystemMonitor(new Mock<IPlatform>().Object)
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
            var monitorA = new SystemMonitor(new Mock<IPlatform>().Object)
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

            var monitorB = new SystemMonitor(new Mock<IPlatform>().Object)
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

        [Fact]
        public void EqualsOverloadedOperator_WithLeftOperandAsNullAndRightOperandNotNull_ReturnsTrue()
        {
            // Arrange
            SystemMonitor? monitorA = null;

            var monitorB = new SystemMonitor(new Mock<IPlatform>().Object)
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
