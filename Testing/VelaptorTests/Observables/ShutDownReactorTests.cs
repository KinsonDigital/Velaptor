// <copyright file="ShutDownReactorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Velaptor.Observables.ObservableData;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ShutDownReactor"/> class.
    /// </summary>
    public class ShutDownReactorTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<ShutDownData>>();

            var observable = new ShutDownReactor();
            observable.Subscribe(observer.Object);

            // Act
            observable.PushNotification(default);

            // Assert
            observer.Verify(m => m.OnNext(default), Times.Once());
        }
        #endregion
    }
}
