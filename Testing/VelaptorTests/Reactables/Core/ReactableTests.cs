// <copyright file="ReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables.Core;

using System;
using FluentAssertions;
using Moq;
using Velaptor.Reactables.Core;
using Fakes;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="Reactable{TData}"/> class.
/// </summary>
public class ReactableTests
{
    #region Method Tests
    [Fact]
    public void Subscribe_WithNullReactorParam_ThrowsException()
    {
        // Arrange
        var reactable = CreateSystemUnderTest<bool>();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            reactable.Subscribe(null);
        }, "The parameter must not be null. (Parameter 'reactor')");
    }

    [Fact]
    public void Subscribe_WhenAddingNewReactor_ReactorAddedToReactable()
    {
        // Arrange
        var reactable = CreateSystemUnderTest<bool>();

        // Act
        reactable.Subscribe(new Reactor<bool>());

        // Assert
        Assert.Single(reactable.Reactors);
    }

    [Fact]
    public void Subscribe_WhenAddingNewReactor_ReturnsUnsubscriber()
    {
        // Arrange
        var reactable = CreateSystemUnderTest<bool>();
        var reactor = new Reactor<bool>();

        // Act
        var actual = (ReactorUnsubscriber<bool>)reactable.Subscribe(reactor);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<ReactorUnsubscriber<bool>>(actual);
        Assert.Same(reactor, actual.Reactor);
    }

    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<bool>>();

        var reactable = new ReactableFake<bool>();
        reactable.Subscribe(reactor.Object);

        // Act
        reactable.PushNotification(true);

        // Assert
        reactor.Verify(m => m.OnNext(true), Times.Once());
    }

    [Fact]
    public void EndNotifications_WhenInvoked_CompletesAllReactors()
    {
        // Arrange
        var reactable = CreateSystemUnderTest<bool>();
        var mockReactorA = new Mock<IReactor<bool>>();
        var mockReactorB = new Mock<IReactor<bool>>();

        reactable.Subscribe(mockReactorA.Object);
        reactable.Subscribe(mockReactorB.Object);

        // Act
        reactable.EndNotifications();
        reactable.EndNotifications();

        // Assert
        mockReactorA.VerifyOnce(m => m.OnCompleted());
    }

    [Fact]
    public void UnsubscribeAll_WhenInvoked_UnsubscribesFromAllReactors()
    {
        // Arrange
        IReactor<bool> reactorA = new Reactor<bool>();
        IReactor<bool> reactorB = new Reactor<bool>();

        var sut = CreateSystemUnderTest<bool>();

        sut.Subscribe(reactorA);
        sut.Subscribe(reactorB);

        // Act
        sut.UnsubscribeAll();

        // Assert
        sut.Reactors.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_WhenInvokedWithReactors_RemovesReactors()
    {
        // Arrange
        var reactable = CreateSystemUnderTest<bool>();

        reactable.Subscribe(new Reactor<bool>());

        // Act
        reactable.Dispose();
        reactable.Dispose();

        // Assert
        Assert.Empty(reactable.Reactors);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of the abstract <see cref="Reactable{TData}"/> for the purpose of testing.
    /// </summary>
    /// <typeparam name="T">The type of data that the reactable will deal with.</typeparam>
    /// <returns>The instance to test.</returns>
    private static ReactableFake<T> CreateSystemUnderTest<T>()
        => new ();
}
