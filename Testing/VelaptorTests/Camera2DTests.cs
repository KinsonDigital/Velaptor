// <copyright file="Camera2DTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using System;
using System.Drawing;
using System.Numerics;
using Carbonate.OneWay;
using NSubstitute;
using Shouldly;
using Velaptor;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Camera2D"/>
/// </summary>
public class Camera2DTests
{
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable<WindowSizeData> mockPushWinSizeReactable;
    private readonly IPullReactable<WindowSizeData> mockPullWinSizeReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera2DTests"/> class.
    /// </summary>
    public Camera2DTests()
    {
        this.mockPushWinSizeReactable = Substitute.For<IPushReactable<WindowSizeData>>();
        this.mockPullWinSizeReactable = Substitute.For<IPullReactable<WindowSizeData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreatePushWindowSizeReactable().Returns(this.mockPushWinSizeReactable);
        this.mockReactableFactory.CreatePullWindowSizeReactable().Returns(this.mockPullWinSizeReactable);
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Camera2D(null);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_GetsAndSetsWindowSize()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 123, Height = 456 });
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.WindowSize;

        // Assert
        actual.Width.ShouldBe(123u);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Position_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.Position;

        // Assert
        actual.ShouldBe(Vector2.Zero);
    }

    [Fact]
    public void Zoom_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.Zoom;

        // Assert
        actual.ShouldBe(0.1f);
    }

    [Theory]
    [InlineData(5f, 10f)]
    [InlineData(200f, 100f)]
    [InlineData(50f, 50f)]
    public void Zoom_WhenSettingValue_ReturnsCorrectResult(float zoom, float expectedResult)
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);
        sut.ZoomMin = 10;
        sut.ZoomMax = 100;
        sut.Zoom = zoom;

        // Act
        var actual = sut.Zoom;

        // Assert
        actual.ShouldBe(expectedResult);
    }

    [Fact]
    public void ZoomMin_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.ZoomMin;

        // Assert
        actual.ShouldBe(0.1f);
    }

    [Fact]
    public void ZoomMax_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.ZoomMax;

        // Assert
        actual.ShouldBe(4);
    }

    [Fact]
    public void Rotation_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.Rotation;

        // Assert
        actual.ShouldBe(0f);
    }

    [Fact]
    public void WindowSize_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.WindowSize;

        // Assert
        actual.ShouldBe(new SizeU(0, 0));
    }

    [Fact]
    public void WorldBounds_WithNoTransform_ReturnsWindowSizeAsWorldBounds()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.WorldBounds;

        // Assert
        actual.ShouldBe(new RectangleF(0, 0, 800, 600));
    }

    [Fact]
    public void WorldBounds_WithPositionOffset_ReturnsOffsetBounds()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;
        sut.Position = new Vector2(100, 50);

        // Act
        var actual = sut.WorldBounds;

        // Assert
        actual.ShouldBe(new RectangleF(100, 50, 800, 600));
    }

    [Fact]
    public void WorldBounds_WithZoom_ReturnsScaledBounds()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 2f;

        // Act
        var actual = sut.WorldBounds;

        // Assert
        // With 2x zoom, visible world area is half the screen size, centered on screen center
        actual.ShouldBe(new RectangleF(200, 150, 400, 300));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void TransformPosition_whenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.TransformPosition(new Vector2(100, 200));

        // Assert
        actual.ShouldBe(new Vector2(10, 20));
    }

    [Fact]
    public void TransformPosition_WithRotation_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;
        sut.Rotation = 90f;

        // Act
        var actual = sut.TransformPosition(new Vector2(100, 0));

        // Assert
        actual.X.ShouldBe(0f, 0.0001f);
        actual.Y.ShouldBe(100f, 0.0001f);
    }

    [Fact]
    public void TransformSize_whenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 2;

        // Act
        var actual = sut.TransformSize(0.2f);

        // Assert
        actual.ShouldBe(0.4f);
    }

    [Fact]
    public void TransformRotation_WithNoCameraRotation_ReturnsWorldRotation()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.TransformRotation(45f);

        // Assert
        actual.ShouldBe(45f);
    }

    [Fact]
    public void TransformRotation_WithCameraRotation_ReturnsWorldRotationPlusCameraRotation()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Rotation = 90f;

        // Act
        var actual = sut.TransformRotation(45f);

        // Assert
        actual.ShouldBe(135f);
    }

    [Fact]
    public void IsVisible_WithPointOnScreen_ReturnsTrue()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.IsVisible(new Vector2(400, 300));

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void IsVisible_WithPointOffScreenRight_ReturnsFalse()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.IsVisible(new Vector2(1000, 300));

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void IsVisible_WithPointOffScreenLeft_ReturnsFalse()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.IsVisible(new Vector2(-100, 300));

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void IsVisible_WithRectFullyOnScreen_ReturnsTrue()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.IsVisible(new Vector2(100, 100), new Vector2(50, 50));

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void IsVisible_WithRectPartiallyOnScreen_ReturnsTrue()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        // Rect starts at x=750 with width=100, so it extends from 750 to 850 (partially off right edge)
        var actual = sut.IsVisible(new Vector2(750, 300), new Vector2(100, 100));

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void IsVisible_WithRectFullyOffScreen_ReturnsFalse()
    {
        // Arrange
        this.mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 800, Height = 600 });
        var sut = new Camera2D(this.mockReactableFactory);
        sut.Zoom = 1f;

        // Act
        var actual = sut.IsVisible(new Vector2(1000, 300), new Vector2(50, 50));

        // Assert
        actual.ShouldBeFalse();
    }
    #endregion
}
