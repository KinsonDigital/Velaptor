// <copyright file="ReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables.Core
{
    using Velaptor.Observables.Core;
    using VelaptorTests.Fakes;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Reactable{TData}"/> class.
    /// </summary>
    public class ReactableTests
    {
        #region Method Tests
        [Fact]
        public void Subscribe_WhenAddingNewReactor_ReactorAddedToReactable()
        {
            // Arrange
            var reactable = CreateReactor<bool>();

            // Act
            reactable.Subscribe(new Reactor<bool>());

            // Assert
            Assert.Single(reactable.Reactors);
        }

        [Fact]
        public void Subscribe_WhenAddingNewReactor_ReturnsUnsubscriber()
        {
            // Arrange
            var reactable = CreateReactor<bool>();
            var reactor = new Reactor<bool>();

            // Act
            var actual = (ReactorUnsubscriber<bool>)reactable.Subscribe(reactor);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ReactorUnsubscriber<bool>>(actual);
            Assert.Same(reactor, actual.Reactor);
        }

        [Fact]
        public void Dispose_WhenInvokedWithReactors_RemovesReactors()
        {
            // Arrange
            var reactable = CreateReactor<bool>();

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
        private static ReactableFake<T> CreateReactor<T>()
            => new ();
    }
}
