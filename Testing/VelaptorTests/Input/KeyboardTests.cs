// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Moq;
using Velaptor.Input;
using Velaptor.Reactables.Core;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.Input;

/// <summary>
/// Tests the <see cref="Keyboard"/> class.
/// </summary>
public class KeyboardTests
{
    private readonly Mock<IReactable<(KeyCode key, bool isDown)>> mockKeyboardReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardTests"/> class.
    /// </summary>
    public KeyboardTests() => this.mockKeyboardReactable = new Mock<IReactable<(KeyCode key, bool isDown)>>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactable()
    {
        // Arrange & Act
        var unused = CreateKeyboard();

        // Assert
        this.mockKeyboardReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor<(KeyCode, bool)>>()));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetState_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        IReactor<(KeyCode key, bool isDown)>? reactor = null;

        var keyState = (KeyCode.K, true);
        this.mockKeyboardReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(KeyCode key, bool isDown)>>()))
            .Callback<IReactor<(KeyCode key, bool isDown)>>(reactorObj =>
            {
                reactor = reactorObj;
            });
        this.mockKeyboardReactable.Setup(m
                => m.PushNotification(It.IsAny<(KeyCode key, bool isDown)>()))
            .Callback<(KeyCode key, bool isDown)>((data) =>
            {
                reactor.OnNext(data);
            });

        var keyboard = CreateKeyboard();
        this.mockKeyboardReactable.Object.PushNotification(keyState);

        // Act
        var actual = keyboard.GetState();

        // Assert
        AssertExtensions.AllItemsAre(actual.GetKeyStates(),
            state => state.Key == KeyCode.K || state.Value == false);
    }

    [Fact]
    public void Reactable_WhenReactorCompletes_DisposeOfSubscription()
    {
        // Arrange
        var mockUnsubscriber = new Mock<IDisposable>();
        IReactor<(KeyCode key, bool isDown)>? reactor = null;
        mockUnsubscriber.Name = nameof(mockUnsubscriber);

        this.mockKeyboardReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(KeyCode key, bool isDown)>>()))
            .Returns(mockUnsubscriber.Object)
            .Callback<IReactor<(KeyCode key, bool isDown)>>(reactorObj =>
            {
                reactor = reactorObj;
            });
        var unused = new Keyboard(this.mockKeyboardReactable.Object);

        // Act
        reactor.OnCompleted();

        // Assert
        mockUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Keyboard"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Keyboard CreateKeyboard() => new Keyboard(this.mockKeyboardReactable.Object);
}
