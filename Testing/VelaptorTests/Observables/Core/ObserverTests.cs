// <copyright file="ObserverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables.Core
{
    using System;
    using Moq;
    using Velaptor.Observables.Core;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Observer{T}"/> class.
    /// </summary>
    public class ObserverTests
    {
        #region Method Tests
        [Fact]
        public void OnNext_WithNullOnNextDelegate_DoesNotThrowException()
        {
            // Arrange
            var observer = new Observer<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                observer.OnNext(It.IsAny<bool>());
            });
        }

        [Fact]
        public void OnNext_WhenInvoked_ExecutesOnNext()
        {
            // Arrange
            var onNextInvoked = false;

            var observer = new Observer<bool>(onNext: _ => onNextInvoked = true);

            // Act
            observer.OnNext(It.IsAny<bool>());

            // Assert
            Assert.True(onNextInvoked);
        }

        [Fact]
        public void OnCompleted_WithNullOnCompletedDelegate_DoesNotThrowException()
        {
            // Arrange
            var observer = new Observer<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                observer.OnCompleted();
            });
        }

        [Fact]
        public void OnCompleted_WhenInvoked_ExecutesOnCompleted()
        {
            // Arrange
            var onCompletedInvoked = false;

            var observer = new Observer<bool>(onCompleted: () => onCompletedInvoked = true);

            // Act
            observer.OnCompleted();

            // Assert
            Assert.True(onCompletedInvoked);
        }

        [Fact]
        public void OnError_WithNullOnErrorDelegate_DoesNotThrowException()
        {
            // Arrange
            var observer = new Observer<bool>();

            // Act & Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                observer.OnError(It.IsAny<Exception>());
            });
        }

        [Fact]
        public void OnError_WhenInvoked_ExecutesOnError()
        {
            // Arrange
            var onErrorInvoked = false;

            var observer = new Observer<bool>(onError: _ => onErrorInvoked = true);

            // Act
            observer.OnError(It.IsAny<Exception>());

            // Assert
            Assert.True(onErrorInvoked);
        }
        #endregion
    }
}
