// <copyright file="ObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Observables.Core
{
    using Raptor.Observables.Core;
    using RaptorTests.Fakes;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Observable{T}"/> class.
    /// </summary>
    public class ObservableTests
    {
        #region Method Tests
        [Fact]
        public void Subscribe_WhenAddingNewObserver_ObserverAddedToObservable()
        {
            // Arrange
            var observable = CreateObservable<bool>();

            // Act
            observable.Subscribe(new Observer<bool>());

            // Assert
            Assert.Single(observable.Observers);
        }

        [Fact]
        public void Subscribe_WhenAddingNewObserver_ReturnsUnsubscriber()
        {
            // Arrange
            var observable = CreateObservable<bool>();
            var observer = new Observer<bool>();

            // Act
            var actual = (ObserverUnsubscriber<bool>)observable.Subscribe(observer);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObserverUnsubscriber<bool>>(actual);
            Assert.Same(observer, actual.Observer);
        }

        [Fact]
        public void Dispose_WhenInvokedWithObservers_RemovesObservers()
        {
            // Arrange
            var observable = CreateObservable<bool>();

            observable.Subscribe(new Observer<bool>());

            // Act
            observable.Dispose();
            observable.Dispose();

            // Assert
            Assert.Empty(observable.Observers);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the abstract <see cref="Observable{T}"/> for the purpose of testing.
        /// </summary>
        /// <typeparam name="T">The type of data that the observable will deal with.</typeparam>
        /// <returns>The instance to test.</returns>
        private static ObservableFake<T> CreateObservable<T>()
            => new ();
    }
}
