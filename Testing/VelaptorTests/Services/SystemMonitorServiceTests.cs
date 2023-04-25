 // <copyright file="SystemMonitorServiceTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SystemMonitorService"/> class.
/// </summary>
public class SystemMonitorServiceTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullMonitorsParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SystemMonitorService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'monitors')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Monitors_WithNoMonitorsInSystem_ReturnsEmptyResult()
    {
        // Arrange
        var mockMonitors = new Mock<IMonitors>();
        mockMonitors.SetupGet(m => m.SystemMonitors)
            .Returns(Array.Empty<SystemMonitor>());

        var service = new SystemMonitorService(mockMonitors.Object);

        // Act
        var actual = service.Monitors;

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Monitors_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockPlatform = new Mock<IPlatform>();
        var mockMonitors = new Mock<IMonitors>();
        var monitor = new SystemMonitor(mockPlatform.Object);
        mockMonitors.SetupGet(m => m.SystemMonitors)
            .Returns(new[] { monitor });

        var service = new SystemMonitorService(mockMonitors.Object);

        // Act
        var actual = service.Monitors.ToArray();

        // Assert
        actual.Should().HaveCount(1);
        actual[0].Should().Be(monitor);
    }

    [Fact]
    public void MainMonitor_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockPlatform = new Mock<IPlatform>();
        var mockMonitors = new Mock<IMonitors>();
        var monitor = new SystemMonitor(mockPlatform.Object)
        {
            IsMain = true,
        };
        mockMonitors.SetupGet(m => m.SystemMonitors)
            .Returns(new[] { monitor });

        var service = new SystemMonitorService(mockMonitors.Object);

        // Act
        var actual = service.MainMonitor;

        // Assert
        actual.Should().NotBeNull();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Refresh_WhenInvoked_RefreshesMonitor()
    {
        // Arrange
        var mockPlatform = new Mock<IPlatform>();
        var mockMonitors = new Mock<IMonitors>();
        var monitor = new SystemMonitor(mockPlatform.Object);
        mockMonitors.SetupGet(m => m.SystemMonitors)
            .Returns(new[] { monitor });

        var service = new SystemMonitorService(mockMonitors.Object);

        // Act
        service.Refresh();

        // Assert
        mockMonitors.Verify(m => m.Refresh(), Times.Once);
    }
    #endregion
}
