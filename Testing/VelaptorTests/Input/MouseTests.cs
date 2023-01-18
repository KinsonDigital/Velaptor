// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using Carbonate.Core;
using Carbonate.Core.UniDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Mouse"/> class.
/// </summary>
public class MouseTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<MouseStateData>> mockMouseReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseTests"/> class.
    /// </summary>
    public MouseTests()
    {
        this.mockMouseReactable = new Mock<IPushReactable<MouseStateData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateMouseReactable()).Returns(this.mockMouseReactable.Object);
    }

    #region Method Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Mouse(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactables()
    {
        // Act & Arrange
        _ = CreateSystemUnderTest();

        // Assert
        this.mockMouseReactable.VerifyOnce(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()));
    }

    [Fact]
    public void GetState_WhenGettingMousePosition_ReturnsCorrectResult()
    {
        // Arrange
        IReceiveReactor<MouseStateData>? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()))
            .Callback<IReceiveReactor<MouseStateData>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { X = 11, Y = 22 };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        Assert.Equal(11, actual.GetPosition().X);
        Assert.Equal(22, actual.GetPosition().Y);
    }

    [Theory]
    [InlineData(0)] // Left
    [InlineData(1)] // Right
    [InlineData(2)] // Middle
    public void GetState_WhenGettingMouseButton_ReturnsCorrectResult(int mouseButtonValue)
    {
        // Arrange
        var mouseButton = (MouseButton)mouseButtonValue;
        IReceiveReactor<MouseStateData>? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()))
            .Callback<IReceiveReactor<MouseStateData>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { Button = mouseButton, ButtonIsDown = true };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        Assert.True(actual.GetButtonState(mouseButton));
    }

    [Fact]
    public void GetState_WhenGettingMouseWheelData_ReturnsCorrectResult()
    {
        // Arrange
        IReceiveReactor<MouseStateData>? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()))
            .Callback<IReceiveReactor<MouseStateData>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { ScrollDirection = MouseScrollDirection.ScrollDown, ScrollWheelValue = 33 };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        Assert.Equal(MouseScrollDirection.ScrollDown, actual.GetScrollDirection());
        Assert.Equal(33, actual.GetScrollWheelValue());
    }

    [Fact]
    public void PushReactable_WithOnNextMessageActionAndInvalidMouseButton_ThrowsException()
    {
        // Arrange
        const string expected = $"The enum '{nameof(MouseButton)}' is out of range.";

        IReceiveReactor<MouseStateData>? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()))
            .Callback<IReceiveReactor<MouseStateData>>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        var mouseStateData = new MouseStateData { Button = (MouseButton)1234 };

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnReceive(mouseStateData);

        // Assert
        act.Should().Throw<EnumOutOfRangeException>()
            .WithMessage(expected);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WhenReactorCompletes_DisposesOfSubscriptions()
    {
        // Arrange
        IReactor? posReactor = null;
        var unsubscriber = new Mock<IDisposable>();

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<MouseStateData>>()))
            .Returns(unsubscriber.Object)
            .Callback<IReceiveReactor<MouseStateData>>(reactorObj =>
            {
                posReactor = reactorObj;
            });

        _ = CreateSystemUnderTest();

        // Act
        posReactor.OnUnsubscribe();

        // Assert
        unsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Mouse"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Mouse CreateSystemUnderTest()
        => new (this.mockReactableFactory.Object);
}
