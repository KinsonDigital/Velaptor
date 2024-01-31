// <copyright file="SceneBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Drawing;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Fakes;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Xunit;

/// <summary>
/// Tests the <see cref="SceneBase"/> class.
/// </summary>
public class SceneBaseTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription<WindowSizeData>? winSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBaseTests"/> class.
    /// </summary>
    public SceneBaseTests()
    {
        var mockWinSizeReactable = new Mock<IPushReactable<WindowSizeData>>();
        mockWinSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<WindowSizeData>>()))
            .Callback<IReceiveSubscription<WindowSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit tests.");
                reactor.Name.Should().Be($"SceneBase.Init - {nameof(PushNotifications.WindowSizeChangedId)}");

                this.winSizeReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreatePushWindowSizeReactable())
            .Returns(mockWinSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SceneBaseFake(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_PropAreCorrectDefaultValues()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Name.Should().BeEmpty();
        sut.Id.Should().NotBe(Guid.Empty);
        sut.IsLoaded.Should().BeFalse();
        sut.WindowSize.Should().BeEquivalentTo(default(SizeU));
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act
        var sut = new SceneBaseFake(this.mockReactableFactory.Object)
        {
            Name = "test-value",
        };

        // Assert
        sut.Name.Should().Be("test-value");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadContent_WhenInvoked_LoadsContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.LoadContent();

        // Assert
        sut.IsLoaded.Should().BeTrue();
    }

    [Fact]
    public void UnloadContent_WhenContentHasBeenLoaded_UnloadsContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        sut.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void UnloadContent_WhenContentHasNotBeenLoaded_DoesNotUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();

        // Assert
        sut.IsLoaded.Should().BeFalse();
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void WinSizeReactable_WithWindowSizeNotification_SetsWindowSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.winSizeReactor.OnReceive(new WindowSizeData { Width = 100, Height = 200 });

        // Assert
        sut.WindowSize.Should().BeEquivalentTo(new SizeU { Width = 100, Height = 200 });
        sut.WindowCenter.Should().BeEquivalentTo(new Point { X = 50, Y = 100 });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SceneBaseFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SceneBaseFake CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}
