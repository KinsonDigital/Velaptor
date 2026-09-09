// <copyright file="Camera2DTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using System;
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
    public void WindowSize_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Camera2D(this.mockReactableFactory);

        // Act
        var actual = sut.WindowSize;

        // Assert
        actual.ShouldBe(new SizeU(0, 0));
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
    #endregion
}
