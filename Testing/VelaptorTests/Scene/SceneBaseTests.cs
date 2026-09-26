// <copyright file="SceneBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Drawing;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Fakes;
using NSubstitute;
using Shouldly;
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
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription<WindowSizeData>? winSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBaseTests"/> class.
    /// </summary>
    public SceneBaseTests()
    {
        var mockPushWinSizeReactable = Substitute.For<IPushReactable<WindowSizeData>>();
        mockPushWinSizeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<WindowSizeData>>()))
            .Do(callInfo =>
            {
                this.winSizeReactor = callInfo.Arg<IReceiveSubscription<WindowSizeData>>();
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreatePushWindowSizeReactable().Returns(mockPushWinSizeReactable);
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_PropAreCorrectDefaultValues()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Name.ShouldBeEmpty();
        sut.Id.ShouldNotBe(Guid.Empty);
        sut.IsLoaded.ShouldBeFalse();
        sut.WindowSize.ShouldBeEquivalentTo(default(SizeU));
    }

    [Fact]
    public void Ctor_WhenInvoked_RequestsWindowSize()
    {
        // Act & Arrange
        var mockPullWinSizeReactable = Substitute.For<IPullReactable<WindowSizeData>>();
        mockPullWinSizeReactable.Pull(Arg.Any<Guid>()).Returns(new WindowSizeData { Width = 100, Height = 200 });

        this.mockReactableFactory.CreatePullWindowSizeReactable().Returns(mockPullWinSizeReactable);

        var sut = CreateSystemUnderTest();

        // Assert
        mockPullWinSizeReactable.Received(1).Pull(PullNotifications.GetWindowSizeId);
        sut.WindowSize.ShouldBe(new SizeU(100, 200));
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act
        var sut = new SceneBaseFake(this.mockReactableFactory)
        {
            Name = "test-value",
        };

        // Assert
        sut.Name.ShouldBe("test-value");
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
        sut.IsLoaded.ShouldBeTrue();
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
        sut.IsLoaded.ShouldBeFalse();
    }

    [Fact]
    public void UnloadContent_WhenContentHasNotBeenLoaded_DoesNotUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();

        // Assert
        sut.IsLoaded.ShouldBeFalse();
    }

    [Fact]
    public void Resize_WhenInvoked_UpdatesWindowSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Resize(new SizeU(10u, 10u));

        // Assert
        sut.WindowSize.ShouldBe(new SizeU(10u, 10u));
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void PushWinSizeReactable_WithWindowSizeNotification_SetsWindowSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.winSizeReactor.OnReceive(new WindowSizeData { Width = 100, Height = 200 });

        // Assert
        sut.WindowSize.ShouldBeEquivalentTo(new SizeU { Width = 100, Height = 200 });
        sut.WindowCenter.ShouldBeEquivalentTo(new Point { X = 50, Y = 100 });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SceneBaseFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SceneBaseFake CreateSystemUnderTest() => new (this.mockReactableFactory);
}
