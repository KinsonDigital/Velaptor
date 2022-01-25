// <copyright file="OpenGLInitObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="OpenGLInitObservable"/> class.
    /// </summary>
    public class OpenGLInitObservableTests
    {
        #region Method Tests
        [Fact]
        public void OnOpenGLInitialized_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<bool>>();

            var glInitObservable = new OpenGLInitObservable();
            glInitObservable.Subscribe(observer.Object);

            // Act
            glInitObservable.PushNotification(true);

            // Assert
            observer.Verify(m => m.OnNext(true), Times.Once());
        }
        #endregion
    }
}
