// <copyright file="SystemDisplayTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Hardware;

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Hardware;
using Xunit;

/// <summary>
/// Tests the <see cref="SystemDisplay"/> struct.
/// </summary>
public class SystemDisplayTests
{
    /// <summary>
    /// Gets horizontal DPI data for testing.
    /// </summary>
    /// <returns>The horizontal DPI data and expected results.</returns>
    public static TheoryData<OSPlatform, int> GetHorizontalDPIData =>
        new ()
        {
            { OSPlatform.Windows, 672 },
            { OSPlatform.Linux, 672 },
            { OSPlatform.FreeBSD, 672 },
            { OSPlatform.OSX, 504 },
        };

    /// <summary>
    /// Gets vertical DPI data for testing.
    /// </summary>
    /// <returns>The vertical DPI data and expected results.</returns>
    public static TheoryData<OSPlatform, int> GetVerticalDPIData =>
        new ()
        {
            { OSPlatform.Windows, 768 },
            { OSPlatform.Linux, 768 },
            { OSPlatform.FreeBSD, 768 },
            { OSPlatform.OSX, 576 },
        };

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new SystemDisplay(null);
        }, "Value cannot be null. (Parameter 'platform')");
    }
    #endregion

    #region Prop Tests
    [Theory]
    [MemberData(nameof(GetHorizontalDPIData))]
    public void HorizontalDPI_WhenGettingValueOnAnyPlatformExceptOSX_ReturnsCorrectResult(OSPlatform platform, int expectedDPI)
    {
        // Arrange
        var mockPlatform = new Mock<IPlatform>();
        mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(platform);

        var sut = new SystemDisplay(mockPlatform.Object)
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
        var actual = sut.HorizontalDPI;

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

        var sut = new SystemDisplay(mockPlatform.Object)
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
        var actual = sut.VerticalDPI;

        // Assert
        Assert.Equal(expectedDPI, actual);
    }

    [Fact]
    public void Center_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new SystemDisplay(new Mock<IPlatform>().Object)
        {
            Width = 100,
            Height = 200,
        };

        // Act
        var actual = sut.Center;

        // Assert
        Assert.Equal(new Vector2(50, 100), actual);
    }
    #endregion
}
