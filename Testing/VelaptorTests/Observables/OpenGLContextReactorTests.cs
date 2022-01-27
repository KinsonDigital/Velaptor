// <copyright file="OpenGLContextReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="OpenGLContextReactor"/> class.
    /// </summary>
    public class OpenGLContextReactorTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<GLContextData>>();

            var reactor = new OpenGLContextReactor();
            reactor.Subscribe(observer.Object);

            // Act
            reactor.PushNotification(default);

            // Assert
            observer.Verify(m => m.OnNext(default), Times.Once());
        }
        #endregion
    }
}
