﻿// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using System.Collections.Generic;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor.Input;
using Helpers;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Reactables.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Keyboard"/> class.
/// </summary>
public class KeyboardTests
{
    private readonly Mock<IReactable> mockReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardTests"/> class.
    /// </summary>
    public KeyboardTests() => this.mockReactable = new Mock<IReactable>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Keyboard(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactable()
    {
        // Arrange & Act
        var unused = CreateSystemUnderTest();

        // Assert
        this.mockReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor>()));
    }

    [Fact]
    public void Ctor_WithNullMessageData_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue with the '{nameof(Keyboard)}.Constructor()' subscription source";
        expected += $" for subscription ID '{NotificationIds.KeyboardId}'.";

        IReactor? reactor = null;

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<KeyboardKeyStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => null);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetState_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        IReactor? reactor = null;

        var expected = new KeyValuePair<KeyCode, bool>(KeyCode.K, true);

        var keyState = new KeyboardKeyStateData
        {
            Key = KeyCode.K,
            IsDown = true,
        };

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<KeyboardKeyStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => keyState);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();
        reactor.OnNext(mockMessage.Object);

        // Act
        var actual = sut.GetState().GetKeyStates();

        // Assert
        actual.Should().Contain(expected);
    }

    [Fact]
    public void Reactable_WhenReactorCompletes_DisposeOfSubscription()
    {
        // Arrange
        var mockUnsubscriber = new Mock<IDisposable>();
        IReactor? reactor = null;
        mockUnsubscriber.Name = nameof(mockUnsubscriber);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Returns(mockUnsubscriber.Object)
            .Callback<IReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });
        var unused = new Keyboard(this.mockReactable.Object);

        // Act
        reactor.OnComplete();

        // Assert
        mockUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Keyboard"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Keyboard CreateSystemUnderTest() => new (this.mockReactable.Object);
}
