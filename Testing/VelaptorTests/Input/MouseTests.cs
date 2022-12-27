// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using Carbonate;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Mouse"/> class.
/// </summary>
public class MouseTests
{
    private readonly Mock<IReactable> mockReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseTests"/> class.
    /// </summary>
    public MouseTests() => this.mockReactable = new Mock<IReactable>();

    #region Method Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Mouse(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactables()
    {
        // Act & Arrange
        var unused = CreateSystemUnderTest();

        // Assert
        this.mockReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor>()));
    }

    [Fact]
    public void GetState_WhenGettingMousePosition_ReturnsCorrectResult()
    {
        // Arrange
        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<MouseStateData>(It.IsAny<Action<Exception>?>()))
            .Returns(new MouseStateData { X = 11, Y = 22 });

        reactor?.OnNext(mockMessage.Object);

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
        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<MouseStateData>(It.IsAny<Action<Exception>?>()))
            .Returns(new MouseStateData { Button = mouseButton, ButtonIsDown = true });

        reactor?.OnNext(mockMessage.Object);

        // Act
        var actual = sut.GetState();

        // Assert
        Assert.True(actual.GetButtonState(mouseButton));
    }

    [Fact]
    public void GetState_WhenGettingMouseWheelData_ReturnsCorrectResult()
    {
        // Arrange
        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<MouseStateData>(It.IsAny<Action<Exception>?>()))
            .Returns(new MouseStateData { ScrollDirection = MouseScrollDirection.ScrollDown, ScrollWheelValue = 33 });

        reactor?.OnNext(mockMessage.Object);

        // Act
        var actual = sut.GetState();

        // Assert
        Assert.Equal(MouseScrollDirection.ScrollDown, actual.GetScrollDirection());
        Assert.Equal(33, actual.GetScrollWheelValue());
    }

    [Fact]
    public void Reactable_WhenOnNextMessageIsNull_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue with the '{nameof(Mouse)}.Constructor()' subscription source";
        expected += $" for subscription ID '{NotificationIds.MouseStateChangedId}'.";

        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<MouseStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Reactable_WithOnNextMessageActionAndInvalidMouseButton_ThrowsException()
    {
        // Arrange
        const string expected = $"The enum '{nameof(MouseButton)}' is out of range.";

        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        var mouseStateData = new MouseStateData();
        mouseStateData.Button = (MouseButton)1234;

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<MouseStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => mouseStateData);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<EnumOutOfRangeException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Reactable_WhenReactorCompletes_DisposesOfSubscriptions()
    {
        // Arrange
        IReactor? posReactor = null;
        var unsubscriber = new Mock<IDisposable>();

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Returns(unsubscriber.Object)
            .Callback<IReactor>(reactorObj =>
            {
                posReactor = reactorObj;
            });

        var unused = CreateSystemUnderTest();

        // Act
        posReactor.OnComplete();

        // Assert
        unsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Mouse"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Mouse CreateSystemUnderTest()
        => new (this.mockReactable.Object);
}
