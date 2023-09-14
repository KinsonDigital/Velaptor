// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using System.Collections.Generic;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Keyboard"/> class.
/// </summary>
public class KeyboardTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<KeyboardKeyStateData>> mockKeyboardReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardTests"/> class.
    /// </summary>
    public KeyboardTests()
    {
        this.mockKeyboardReactable = new Mock<IPushReactable<KeyboardKeyStateData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateKeyboardReactable()).Returns(this.mockKeyboardReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Keyboard(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactable()
    {
        // Arrange & Act
        _ = CreateSystemUnderTest();

        // Assert
        this.mockKeyboardReactable.VerifyOnce(m =>
            m.Subscribe(It.IsAny<IReceiveSubscription<KeyboardKeyStateData>>()));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetState_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        IReceiveSubscription<KeyboardKeyStateData>? reactor = null;

        var expected = new KeyValuePair<KeyCode, bool>(KeyCode.K, true);

        var keyState = new KeyboardKeyStateData
        {
            Key = KeyCode.K,
            IsDown = true,
        };

        this.mockKeyboardReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<KeyboardKeyStateData>>()))
            .Callback<IReceiveSubscription<KeyboardKeyStateData>>(reactorObj =>
            {
                reactor = reactorObj;
             });

        var sut = CreateSystemUnderTest();
        reactor.OnReceive(keyState);

        // Act
        var actual = sut.GetState().GetKeyStates();

        // Assert
        actual.Should().Contain(expected);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WhenReactorCompletes_DisposeOfSubscription()
    {
        // Arrange
        var mockUnsubscriber = new Mock<IDisposable>();
        IReceiveSubscription<KeyboardKeyStateData>? reactor = null;
        mockUnsubscriber.Name = nameof(mockUnsubscriber);

        this.mockKeyboardReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<KeyboardKeyStateData>>()))
            .Returns(mockUnsubscriber.Object)
            .Callback<IReceiveSubscription<KeyboardKeyStateData>>(reactorObj =>
            {
                reactor = reactorObj;
            });
        _ = CreateSystemUnderTest();

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
    private Keyboard CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}
