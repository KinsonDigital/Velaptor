// <copyright file="OpenGLInitObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Observables
{
    using System;
    using Moq;
    using Raptor.Observables;
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

            var openGLObservable = new OpenGLInitObservable();
            openGLObservable.Subscribe(observer.Object);

            // Act
            openGLObservable.OnOpenGLInitialized();

            // Assert
            observer.Verify(m => m.OnNext(true), Times.Once());
        }
        #endregion
    }
}
