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
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var observer = new Mock<IObserver<GLContextData>>();

            var reactor = new OpenGLContextReactor();
            reactor.Subscribe(observer.Object);

            // Act
            reactor.PushNotification(default, unsubscribe);

            // Assert
            observer.Verify(m => m.OnNext(default), Times.Once());
            Assert.Equal(expected, reactor.Observers.Count);
        }
        #endregion
    }
}
