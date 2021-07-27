// <copyright file="ObserverUnsubscriberTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables.Core
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Velaptor.Observables.Core;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ObserverUnsubscriber{T}"/> class.
    /// </summary>
    public class ObserverUnsubscriberTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsObserverProperty()
        {
            // Arrange
            var expected = new Mock<IObserver<It.IsAnyType>>();
            var unsubscriber = new ObserverUnsubscriber<It.IsAnyType>(
                It.IsAny<List<IObserver<It.IsAnyType>>>(),
                expected.Object);

            // Act
            var actual = unsubscriber.Observer;

            // Assert
            Assert.Same(expected.Object, actual);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsObservers()
        {
            // Arrange
            var observers = new Mock<List<IObserver<It.IsAnyType>>>();
            observers.Object.Add(It.IsAny<IObserver<It.IsAnyType>>());
            observers.Object.Add(It.IsAny<IObserver<It.IsAnyType>>());
            observers.Object.Add(It.IsAny<IObserver<It.IsAnyType>>());

            var unsubscriber = new ObserverUnsubscriber<It.IsAnyType>(
                observers.Object,
                It.IsAny<IObserver<It.IsAnyType>>());

            // Act
            var actual = unsubscriber.TotalObservers;

            // Assert
            Assert.Equal(3, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Dispose_WhenInvoked_RemovesObseverFromObserversList()
        {
            // Arrange
            var observer = new Mock<IObserver<It.IsAnyType>>();

            var observers = new Mock<List<IObserver<It.IsAnyType>>>();
            observers.Object.Add(It.IsAny<IObserver<It.IsAnyType>>());
            observers.Object.Add(observer.Object);

            /* NOTE: The observer is added to the list of observers
             * via the Observable<T> class and then both the list and the single
             * observer are passed to the ObserverUnsubscriber.  The single
             * observer is added to the observers list above to simulate
             * this process for the purpose of testing.
             */

            var unsubscriber = new ObserverUnsubscriber<It.IsAnyType>(
                observers.Object,
                observer.Object);

            // Act
            unsubscriber.Dispose();

            // Assert
            Assert.Equal(1, unsubscriber.TotalObservers);
        }
        #endregion
    }
}
