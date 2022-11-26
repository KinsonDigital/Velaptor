// <copyright file="ReactorUnsubscriberTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables.Core;

using System;
using System.Collections.Generic;
using Moq;
using Velaptor.Reactables.Core;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="ReactorUnsubscriber{T}"/> class.
/// </summary>
public class ReactorUnsubscriberTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactorsParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ReactorUnsubscriber<bool>(null, new Reactor<bool>());
        }, "The parameter must not be null. (Parameter 'reactors')");
    }

    [Fact]
    public void Ctor_WithNullReactorParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ReactorUnsubscriber<bool>(new List<IReactor<bool>>(), null);
        }, "The parameter must not be null. (Parameter 'reactor')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsReactorProperty()
    {
        // Arrange
        var expected = new Mock<IReactor<It.IsAnyType>>();

        var unsubscriber = new ReactorUnsubscriber<It.IsAnyType>(
            new List<IReactor<It.IsAnyType>>(),
            expected.Object);

        // Act
        var actual = unsubscriber.Reactor;

        // Assert
        Assert.Same(expected.Object, actual);
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsReactors()
    {
        // Arrange
        var reactors = new Mock<List<IReactor<It.IsAnyType>>>();
        reactors.Object.Add(It.IsAny<IReactor<It.IsAnyType>>());
        reactors.Object.Add(It.IsAny<IReactor<It.IsAnyType>>());
        reactors.Object.Add(It.IsAny<IReactor<It.IsAnyType>>());

        var unsubscriber = new ReactorUnsubscriber<It.IsAnyType>(
            reactors.Object,
            new Mock<IReactor<It.IsAnyType>>().Object);

        // Act
        var actual = unsubscriber.TotalReactors;

        // Assert
        Assert.Equal(3, actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WhenInvoked_RemovesReactorFromReactorsList()
    {
        // Arrange
        var reactorA = new Reactor<int>();
        var reactorB = new Reactor<int>();
        var reactors = new List<IReactor<int>> { reactorA, reactorB };

        /* NOTE: The reactor is added to the list of reactors
         * via the Reactor<T> class and then both the list and the single
         * reactor are passed to the ReactorUnsubscriber.  The single
         * reactor is added to the reactors list above to simulate
         * this process for the purpose of testing.
         */
        var unsubscriber = new ReactorUnsubscriber<int>(
            reactors,
            reactorB);

        // Act
        unsubscriber.Dispose();
        unsubscriber.Dispose();

        // Assert
        Assert.Equal(1, unsubscriber.TotalReactors);
    }
    #endregion
}
