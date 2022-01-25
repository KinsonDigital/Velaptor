// <copyright file="DisposeTexturesObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="DisposeTexturesObservable"/> class.
    /// </summary>
    public class DisposeTexturesObservableTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<uint>>();

            var observable = new DisposeTexturesObservable();
            observable.Subscribe(observer.Object);

            // Act
            observable.PushNotification(123u);

            // Assert
            observer.Verify(m => m.OnNext(123u), Times.Once());
        }
        #endregion
    }
}
