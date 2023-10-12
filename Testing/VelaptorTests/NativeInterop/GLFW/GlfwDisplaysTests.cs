// <copyright file="GlfwDisplaysTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Moq;
using Silk.NET.GLFW;
using Velaptor;
using Velaptor.Hardware;
using Velaptor.NativeInterop.GLFW;
using Xunit;
using FluentAssertions;


/// <summary>
/// Tests the <see cref="GlfwDisplays"/> class.
/// </summary>
public unsafe class GlfwDisplaysTests
{
    private readonly Mock<IGlfwInvoker> mockGlfwInvoker;
    private readonly Mock<IPlatform> mockPlatform;
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    private readonly Monitor monitorA;
    private readonly Monitor monitorB;
    private readonly nint monitorHandleA;
    private readonly nint monitorHandleB;
    private readonly GlfwVideoMode videoModeA;
    private readonly GlfwVideoMode videoModeB;
    // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwDisplaysTests"/> class.
    /// </summary>
    public GlfwDisplaysTests()
    {
        this.mockPlatform = new Mock<IPlatform>();
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

        this.videoModeA = new GlfwVideoMode
        {
            Width = 1,
            Height = 2,
            RedBits = 3,
            GreenBits = 4,
            BlueBits = 5,
            RefreshRate = 6,
        };

        this.videoModeB = new GlfwVideoMode
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

        this.mockGlfwInvoker = new Mock<IGlfwInvoker>();
        this.mockGlfwInvoker.Setup(m => m.GetMonitors()).Returns(() =>
        {
            return new[] { this.monitorHandleA, this.monitorHandleB };
        });

        this.mockGlfwInvoker.Setup(m => m.GetVideoMode(this.monitorHandleA)).Returns(this.videoModeA);
        this.mockGlfwInvoker.Setup(m => m.GetVideoMode(this.monitorHandleB)).Returns(this.videoModeB);

        this.mockGlfwInvoker.Setup(m => m.GetMonitorContentScale(this.monitorHandleA))
            .Returns(new Vector2(7, 8));

        this.mockGlfwInvoker.Setup(m => m.GetMonitorContentScale(this.monitorHandleB))
            .Returns(new Vector2(77, 88));
    }

    #region Constructor Test
    [Fact]
    public void Ctor_WithNullGLFWInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwDisplays(null, this.mockPlatform.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("The parameter must not be null. (Parameter 'glfwInvoker')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwDisplays(this.mockGlfwInvoker.Object, null);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("The parameter must not be null. (Parameter 'platform')");
    }

    [Fact]
    public void Ctor_WhenInvoked_InitializesGLFW()
    {
        // Act
        _ = new GlfwDisplays(this.mockGlfwInvoker.Object, this.mockPlatform.Object);

        // Assert
        this.mockGlfwInvoker.Verify(m => m.Init(), Times.Once());
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsDisplayCallback()
    {
        // Act
        CreateDisplays();

        // Assert
        this.mockGlfwInvoker.VerifyAdd(m => m.OnDisplayChanged += It.IsAny<EventHandler<GlfwDisplayChangedEventArgs>>(), Times.Once());
    }

    [Fact]
    public void Ctor_WhenInvoked_SystemDisplaysRefreshed()
    {
        // Arrange
        var expectedDisplayA = new SystemDisplay(this.mockPlatform.Object)
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

        var expectedDisplayB = new SystemDisplay(this.mockPlatform.Object)
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
        var displays = CreateDisplays();
        var actual = displays.SystemDisplays;

        // Assert
        actual.Should().HaveCount(2).And.ContainInOrder(expectedDisplayA, expectedDisplayB);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void WhenDisplaySetupChanges_RefreshesDisplayData()
    {
        // Arrange
        var refreshInvoked = false;
        CreateDisplays();

        this.mockGlfwInvoker.Setup(m => m.GetMonitors())
            .Callback(() => refreshInvoked = true);

        // Act
        this.mockGlfwInvoker.Raise(e
            => e.OnDisplayChanged += null, new GlfwDisplayChangedEventArgs(true));

        // Assert
        refreshInvoked.Should().BeTrue();
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesProperly()
    {
        // Arrange
        var displays = CreateDisplays();

        // Act
        displays.Dispose();
        displays.Dispose();

        // Assert
        this.mockGlfwInvoker.VerifyRemove(e
            => e.OnDisplayChanged -= It.IsAny<EventHandler<GlfwDisplayChangedEventArgs>>(), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GlfwDisplays"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GlfwDisplays CreateDisplays() => new (this.mockGlfwInvoker.Object, this.mockPlatform.Object);
}
