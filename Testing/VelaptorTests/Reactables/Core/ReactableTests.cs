// <copyright file="ReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables.Core
{
    using System;
    using Velaptor.Reactables.Core;
    using VelaptorTests.Fakes;
    using VelaptorTests.Helpers;
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
            var reactable = CreateReactable<bool>();

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
            var reactable = CreateReactable<bool>();

            // Act
            reactable.Subscribe(new Reactor<bool>());

            // Assert
            Assert.Single(reactable.Reactors);
        }

        [Fact]
        public void Subscribe_WhenAddingNewReactor_ReturnsUnsubscriber()
        {
            // Arrange
            var reactable = CreateReactable<bool>();
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
            var reactable = CreateReactable<bool>();

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
        private static ReactableFake<T> CreateReactable<T>()
            => new ();
    }
}
