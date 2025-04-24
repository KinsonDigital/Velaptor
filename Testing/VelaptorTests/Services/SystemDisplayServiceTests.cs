// <copyright file="SystemDisplayServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Linq;
using System.Numerics;
using Shouldly;
using NSubstitute;
using Velaptor;
using Velaptor.Hardware;
using Velaptor.NativeInterop.GLFW;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="SystemDisplayService"/> class.
/// </summary>
public class SystemDisplayServiceTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDisplaysParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SystemDisplayService(null);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'displays')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Displays_WithNoDisplaysInSystem_ReturnsEmptyResult()
    {
        // Arrange
        var mockDisplays = Substitute.For<IDisplays>();
        mockDisplays.SystemDisplays.Returns([]);

        var service = new SystemDisplayService(mockDisplays);

        // Act
        var actual = service.Displays;

        // Assert
        actual.ShouldBeEmpty();
    }

    [Fact]
    public void Displays_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockPlatform = Substitute.For<IPlatform>();
        var mockDisplays = Substitute.For<IDisplays>();
        var display = new SystemDisplay(mockPlatform);
        mockDisplays.SystemDisplays.Returns([display]);

        var service = new SystemDisplayService(mockDisplays);

        // Act
        var actual = service.Displays.ToArray();

        // Assert
        actual.Length.ShouldBe(1);
        actual[0].ShouldBe(display);
    }

    [Fact]
    public void MainDisplay_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = default(SystemDisplay);
        expected = expected with { IsMain = true };
        var mockPlatform = Substitute.For<IPlatform>();
        var mockDisplays = Substitute.For<IDisplays>();
        var display = new SystemDisplay(mockPlatform) { IsMain = true, };
        mockDisplays.SystemDisplays.Returns([display]);

        var service = new SystemDisplayService(mockDisplays);

        // Act
        var actual = service.MainDisplay;

        // Assert
        actual.IsMain.ShouldBeTrue();
        actual.BlueBitDepth.ShouldBe(0);
        actual.GreenBitDepth.ShouldBe(0);
        actual.RedBitDepth.ShouldBe(0);
        actual.RefreshRate.ShouldBe(0);
        actual.Width.ShouldBe(0);
        actual.Height.ShouldBe(0);
        actual.RefreshRate.ShouldBe(0);
        actual.VerticalDPI.ShouldBe(0);
        actual.VerticalScale.ShouldBe(0);
        actual.HorizontalDPI.ShouldBe(0);
        actual.HorizontalScale.ShouldBe(0);
        actual.Center.ShouldBe(Vector2.Zero);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Refresh_WhenInvoked_RefreshesDisplay()
    {
        // Arrange
        var mockPlatform = Substitute.For<IPlatform>();
        var mockDisplays = Substitute.For<IDisplays>();
        var display = new SystemDisplay(mockPlatform);
        mockDisplays.SystemDisplays.Returns([display]);

        var service = new SystemDisplayService(mockDisplays);

        // Act
        service.Refresh();

        // Assert
        mockDisplays.Received(1).Refresh();
    }
    #endregion
}
