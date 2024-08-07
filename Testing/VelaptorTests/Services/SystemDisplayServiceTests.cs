 // <copyright file="SystemDisplayServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Linq;
using FluentAssertions;
using Moq;
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
        var mockDisplays = new Mock<IDisplays>();
        mockDisplays.SetupGet(m => m.SystemDisplays)
            .Returns([]);

        var service = new SystemDisplayService(mockDisplays.Object);

        // Act
        var actual = service.Displays;

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Displays_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockPlatform = new Mock<IPlatform>();
        var mockDisplays = new Mock<IDisplays>();
        var display = new SystemDisplay(mockPlatform.Object);
        mockDisplays.SetupGet(m => m.SystemDisplays)
            .Returns(new[] { display });

        var service = new SystemDisplayService(mockDisplays.Object);

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
        var mockPlatform = new Mock<IPlatform>();
        var mockDisplays = new Mock<IDisplays>();
        var display = new SystemDisplay(mockPlatform.Object)
        {
            IsMain = true,
        };
        mockDisplays.SetupGet(m => m.SystemDisplays)
            .Returns(new[] { display });

        var service = new SystemDisplayService(mockDisplays.Object);

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
        var mockPlatform = new Mock<IPlatform>();
        var mockDisplays = new Mock<IDisplays>();
        var display = new SystemDisplay(mockPlatform.Object);
        mockDisplays.SetupGet(m => m.SystemDisplays)
            .Returns(new[] { display });

        var service = new SystemDisplayService(mockDisplays.Object);

        // Act
        service.Refresh();

        // Assert
        mockDisplays.Verify(m => m.Refresh(), Times.Once);
    }
    #endregion
}
