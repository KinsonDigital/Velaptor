// <copyright file="ShutDownObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ShutDownObservable"/> class.
    /// </summary>
    public class ShutDownObservableTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<bool>>();

            var observable = new ShutDownObservable();
            observable.Subscribe(observer.Object);

            // Act
            observable.OnShutDown();

            // Assert
            observer.Verify(m => m.OnNext(true), Times.Once());
        }
        #endregion
    }
}
