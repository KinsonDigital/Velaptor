// <copyright file="OpenGLContextObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="OpenGLContextObservable"/> class.
    /// </summary>
    public class OpenGLContextObservableTests
    {
        #region Method Tests
        [Fact]
        public void OnOpenGLInitialized_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var notificationData = new object();

            var observer = new Mock<IObserver<object>>();

            var openGLObservable = new OpenGLContextObservable();
            openGLObservable.Subscribe(observer.Object);

            // Act
            openGLObservable.OnGLContextCreated(notificationData);

            // Assert
            observer.Verify(m => m.OnNext(notificationData), Times.Once());
        }
        #endregion
    }
}
