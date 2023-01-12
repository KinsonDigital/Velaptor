// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using System.Collections.Generic;
using Carbonate;
using Carbonate.Core;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Keyboard"/> class.
/// </summary>
public class KeyboardTests
{
    private readonly Mock<IPushReactable> mockReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardTests"/> class.
    /// </summary>
    public KeyboardTests() => this.mockReactable = new Mock<IPushReactable>();

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
        this.mockReactable.VerifyOnce(m => m.Subscribe(It.IsAny<IReceiveReactor>()));
    }

    [Fact]
    public void Ctor_WithNullMessageData_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue with the '{nameof(Keyboard)}.Constructor()' subscription source";
        expected += $" for subscription ID '{NotificationIds.KeyboardStateChangedId}'.";

        IReceiveReactor? reactor = null;

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<KeyboardKeyStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => null);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");
                reactor = reactorObj;
            });

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnReceive(mockMessage.Object);

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
        IReceiveReactor? reactor = null;

        var expected = new KeyValuePair<KeyCode, bool>(KeyCode.K, true);

        var keyState = new KeyboardKeyStateData
        {
            Key = KeyCode.K,
            IsDown = true,
        };

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<KeyboardKeyStateData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => keyState);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var sut = CreateSystemUnderTest();
        reactor.OnReceive(mockMessage.Object);

        // Act
        var actual = sut.GetState().GetKeyStates();

        // Assert
        actual.Should().Contain(expected);
    }

    [Fact]
    public void PushReactable_WhenReactorCompletes_DisposeOfSubscription()
    {
        // Arrange
        var mockUnsubscriber = new Mock<IDisposable>();
        IReactor? reactor = null;
        mockUnsubscriber.Name = nameof(mockUnsubscriber);

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns(mockUnsubscriber.Object)
            .Callback<IReceiveReactor>(reactorObj =>
            {
                reactor = reactorObj;
            });
        var unused = new Keyboard(this.mockReactable.Object);

        // Act
        reactor.OnUnsubscribe();

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
