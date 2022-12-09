// <copyright file="GLFWMonitorsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Moq;
using Silk.NET.GLFW;
using Velaptor;
using Velaptor.Hardware;
using Velaptor.NativeInterop.GLFW;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="GLFWMonitors"/> class.
/// </summary>
public unsafe class GLFWMonitorsTests
{
    private readonly Mock<IGLFWInvoker> mockGLFWInvoker;
    private readonly Mock<IPlatform> mockPlatform;
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    private readonly Monitor monitorA;
    private readonly Monitor monitorB;
    private readonly nint monitorHandleA;
    private readonly nint monitorHandleB;
    private readonly GLFWVideoMode videoModeA;
    private readonly GLFWVideoMode videoModeB;
    // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

    /// <summary>
    /// Initializes a new instance of the <see cref="GLFWMonitorsTests"/> class.
    /// </summary>
    public GLFWMonitorsTests()
    {
        this.mockPlatform = new Mock<IPlatform>();
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

        this.videoModeA = new GLFWVideoMode()
        {
            Width = 1,
            Height = 2,
            RedBits = 3,
            GreenBits = 4,
            BlueBits = 5,
            RefreshRate = 6,
        };

        this.videoModeB = new GLFWVideoMode()
        {
            Width = 11,
            Height = 22,
            RedBits = 33,
            GreenBits = 44,
            BlueBits = 55,
            RefreshRate = 66,
        };

        this.monitorA = default;
        this.monitorB = default;

        fixed (Monitor* pMonitorA = &this.monitorA)
        {
            this.monitorHandleA = (nint)pMonitorA;
        }

        fixed (Monitor* pMonitorB = &this.monitorB)
        {
            this.monitorHandleB = (nint)pMonitorB;
        }

        this.mockGLFWInvoker = new Mock<IGLFWInvoker>();
        this.mockGLFWInvoker.Setup(m => m.GetMonitors()).Returns(() =>
        {
            return new[] { this.monitorHandleA, this.monitorHandleB };
        });

        this.mockGLFWInvoker.Setup(m => m.GetVideoMode(this.monitorHandleA)).Returns(this.videoModeA);
        this.mockGLFWInvoker.Setup(m => m.GetVideoMode(this.monitorHandleB)).Returns(this.videoModeB);

        this.mockGLFWInvoker.Setup(m => m.GetMonitorContentScale(this.monitorHandleA))
            .Returns(new Vector2(7, 8));

        this.mockGLFWInvoker.Setup(m => m.GetMonitorContentScale(this.monitorHandleB))
            .Returns(new Vector2(77, 88));
    }

    #region Constructor Test
    [Fact]
    public void Ctor_WithNullGLFWInvokerParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GLFWMonitors(null, this.mockPlatform.Object);
        }, "The parameter must not be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GLFWMonitors(this.mockGLFWInvoker.Object, null);
        }, "The parameter must not be null. (Parameter 'platform')");
    }

    [Fact]
    public void Ctor_WhenInvoked_InitializesGLFW()
    {
        // Act
        var unused = new GLFWMonitors(this.mockGLFWInvoker.Object, this.mockPlatform.Object);

        // Assert
        this.mockGLFWInvoker.Verify(m => m.Init(), Times.Once());
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsMonitorCallback()
    {
        // Act
        CreateMonitors();

        // Assert
        this.mockGLFWInvoker.VerifyAdd(m => m.OnMonitorChanged += It.IsAny<EventHandler<GLFWMonitorChangedEventArgs>>(), Times.Once());
    }

    [Fact]
    public void Ctor_WhenInvoked_SystemMonitorsRefreshed()
    {
        // Arrange
        var expectedMonitorA = new SystemMonitor(this.mockPlatform.Object)
        {
            IsMain = true,
            Width = 1,
            Height = 2,
            RedBitDepth = 3,
            GreenBitDepth = 4,
            BlueBitDepth = 5,
            RefreshRate = 6,
            HorizontalScale = 7,
            VerticalScale = 8,
        };

        var expectedMonitorB = new SystemMonitor(this.mockPlatform.Object)
        {
            IsMain = false,
            Width = 11,
            Height = 22,
            RedBitDepth = 33,
            GreenBitDepth = 44,
            BlueBitDepth = 55,
            RefreshRate = 66,
            HorizontalScale = 77,
            VerticalScale = 88,
        };

        // Act
        var monitors = CreateMonitors();
        var actual = monitors.SystemMonitors;

        // Assert
        Assert.Equal(expectedMonitorA, actual[0]);
        Assert.Equal(expectedMonitorB, actual[1]);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void WhenMonitorSetupChanges_RefreshesMonitorData()
    {
        // Arrange
        var refreshInvoked = false;
        CreateMonitors();

        this.mockGLFWInvoker.Setup(m => m.GetMonitors())
            .Callback(() => refreshInvoked = true);

        // Act
        this.mockGLFWInvoker.Raise(e
            => e.OnMonitorChanged += null, new GLFWMonitorChangedEventArgs(true));

        // Assert
        Assert.True(refreshInvoked);
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesProperly()
    {
        // Arrange
        var monitors = CreateMonitors();

        // Act
        monitors.Dispose();
        monitors.Dispose();

        // Assert
        this.mockGLFWInvoker.VerifyRemove(e
            => e.OnMonitorChanged -= It.IsAny<EventHandler<GLFWMonitorChangedEventArgs>>(), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GLFWMonitors"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GLFWMonitors CreateMonitors() => new (this.mockGLFWInvoker.Object, this.mockPlatform.Object);
}
