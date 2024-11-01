// <copyright file="SystemDisplayServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Linq;
using FluentAssertions;
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
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'displays')");
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
        actual.Should().BeEmpty();
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
        actual.Should().HaveCount(1);
        actual[0].Should().Be(display);
    }

    [Fact]
    public void MainDisplay_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockPlatform = Substitute.For<IPlatform>();
        var mockDisplays = Substitute.For<IDisplays>();
        var display = new SystemDisplay(mockPlatform) { IsMain = true, };
        mockDisplays.SystemDisplays.Returns([display]);

        var service = new SystemDisplayService(mockDisplays);

        // Act
        var actual = service.MainDisplay;

        // Assert
        actual.Should().NotBeNull();
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
