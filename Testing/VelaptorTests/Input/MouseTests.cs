// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor.Input;
using Helpers;
using Velaptor.Reactables.ReactableData;
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
