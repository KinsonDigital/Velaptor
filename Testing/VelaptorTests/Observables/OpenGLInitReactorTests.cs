// <copyright file="OpenGLInitReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="OpenGLInitReactor"/> class.
    /// </summary>
    public class OpenGLInitReactorTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<GLInitData>>();

            var glInitReactor = new OpenGLInitReactor();
            glInitReactor.Subscribe(observer.Object);

            // Act
            glInitReactor.PushNotification(default);

            // Assert
            observer.Verify(m => m.OnNext(default), Times.Once());
        }
        #endregion
    }
}
