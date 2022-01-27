// <copyright file="ReactorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables.Core
{
    using Velaptor.Observables.Core;
    using VelaptorTests.Fakes;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Reactor{TData}"/> class.
    /// </summary>
    public class ReactorTests
    {
        #region Method Tests
        [Fact]
        public void Subscribe_WhenAddingNewObserver_ObserverAddedToReactor()
        {
            // Arrange
            var observable = CreateReactor<bool>();

            // Act
            observable.Subscribe(new Observer<bool>());

            // Assert
            Assert.Single(observable.Observers);
        }

        [Fact]
        public void Subscribe_WhenAddingNewObserver_ReturnsUnsubscriber()
        {
            // Arrange
            var observable = CreateReactor<bool>();
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
            var observable = CreateReactor<bool>();

            observable.Subscribe(new Observer<bool>());

            // Act
            observable.Dispose();
            observable.Dispose();

            // Assert
            Assert.Empty(observable.Observers);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the abstract <see cref="Reactor{TData}"/> for the purpose of testing.
        /// </summary>
        /// <typeparam name="T">The type of data that the observable will deal with.</typeparam>
        /// <returns>The instance to test.</returns>
        private static ReactorFake<T> CreateReactor<T>()
            => new ();
    }
}
