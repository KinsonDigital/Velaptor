// <copyright file="ReactorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables.Core
{
    using System;
    using Moq;
    using Velaptor.Reactables.Core;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Reactor{T}"/> class.
    /// </summary>
    public class ReactorTests
    {
        #region Method Tests
        [Fact]
        public void OnNext_WithNullOnNextDelegate_DoesNotThrowException()
        {
            // Arrange
            var reactor = new Reactor<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                reactor.OnNext(It.IsAny<bool>());
            });
        }

        [Fact]
        public void OnNext_WhenInvoked_ExecutesOnNext()
        {
            // Arrange
            var onNextInvoked = false;

            var reactor = new Reactor<bool>(onNext: _ => onNextInvoked = true);

            // Act
            reactor.OnNext(It.IsAny<bool>());

            // Assert
            Assert.True(onNextInvoked);
        }

        [Fact]
        public void OnCompleted_WithNullOnCompletedDelegate_DoesNotThrowException()
        {
            // Arrange
            var reactor = new Reactor<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                reactor.OnCompleted();
            });
        }

        [Fact]
        public void OnCompleted_WhenInvoked_ExecutesOnCompleted()
        {
            // Arrange
            var onCompletedInvoked = false;

            var reactor = new Reactor<bool>(onCompleted: () => onCompletedInvoked = true);

            // Act
            reactor.OnCompleted();

            // Assert
            Assert.True(onCompletedInvoked);
        }

        [Fact]
        public void OnError_WithNullOnErrorDelegate_DoesNotThrowException()
        {
            // Arrange
            var reactor = new Reactor<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                reactor.OnError(It.IsAny<Exception>());
            });
        }

        [Fact]
        public void OnError_WhenInvoked_ExecutesOnError()
        {
            // Arrange
            var onErrorInvoked = false;

            var reactor = new Reactor<bool>(onError: _ => onErrorInvoked = true);

            // Act
            reactor.OnError(It.IsAny<Exception>());

            // Assert
            Assert.True(onErrorInvoked);
        }
        #endregion
    }
}
