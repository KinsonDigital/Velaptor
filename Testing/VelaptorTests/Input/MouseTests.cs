﻿// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using System.ComponentModel;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;
using ReceiveMouseDataReactor = Carbonate.Core.OneWay.IReceiveSubscription<
    Velaptor.ReactableData.MouseStateData
>;

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
        this.mockMouseReactable.VerifyOnce(m => m.Subscribe(It.IsAny<ReceiveMouseDataReactor>()));
    }

    [Fact]
    public void GetState_WhenGettingMousePosition_ReturnsCorrectResult()
    {
        // Arrange
        ReceiveMouseDataReactor? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<ReceiveMouseDataReactor>()))
            .Callback<ReceiveMouseDataReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { X = 11, Y = 22 };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        actual.GetPosition().X.Should().Be(11);
        actual.GetPosition().Y.Should().Be(22);
    }

    [Theory]
    [InlineData(0)] // Left
    [InlineData(1)] // Right
    [InlineData(2)] // Middle
    public void GetState_WhenGettingMouseButton_ReturnsCorrectResult(int mouseButtonValue)
    {
        // Arrange
        var mouseButton = (MouseButton)mouseButtonValue;
        ReceiveMouseDataReactor? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<ReceiveMouseDataReactor>()))
            .Callback<ReceiveMouseDataReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { Button = mouseButton, ButtonIsDown = true };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        actual.GetButtonState(mouseButton).Should().BeTrue();
    }

    [Fact]
    public void GetState_WhenGettingMouseWheelData_ReturnsCorrectResult()
    {
        // Arrange
        ReceiveMouseDataReactor? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<ReceiveMouseDataReactor>()))
            .Callback<ReceiveMouseDataReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();

        var mouseStateData = new MouseStateData { ScrollDirection = MouseScrollDirection.ScrollDown, ScrollWheelValue = 33 };

        reactor?.OnReceive(mouseStateData);

        // Act
        var actual = sut.GetState();

        // Assert
        actual.GetScrollDirection().Should().Be(MouseScrollDirection.ScrollDown);
        actual.GetScrollWheelValue().Should().Be(33);
    }

    [Fact]
    public void PushReactable_WithOnNextMessageActionAndInvalidMouseButton_ThrowsException()
    {
        // Arrange
        const int invalidMouseButton = 1234;
        var expected = $"The value of argument 'data.{nameof(MouseStateData.Button)}' ({invalidMouseButton}) is invalid for Enum type " +
                       $"'{nameof(MouseButton)}'. (Parameter 'data.{nameof(MouseStateData.Button)}')";

        ReceiveMouseDataReactor? reactor = null;

        this.mockMouseReactable.Setup(m => m.Subscribe(It.IsAny<ReceiveMouseDataReactor>()))
            .Callback<ReceiveMouseDataReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        var mouseStateData = new MouseStateData { Button = (MouseButton)invalidMouseButton };

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnReceive(mouseStateData);

        // Assert
        act.Should().Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Mouse"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Mouse CreateSystemUnderTest()
        => new (this.mockReactableFactory.Object);
}
