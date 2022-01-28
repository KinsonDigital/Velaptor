// <copyright file="DisposeTexturesReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="DisposeTexturesReactor"/> class.
    /// </summary>
    public class DisposeTexturesReactorTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var observer = new Mock<IObserver<DisposeTextureData>>();

            var reactor = new DisposeTexturesReactor();
            reactor.Subscribe(observer.Object);

            // Act
            reactor.PushNotification(new DisposeTextureData(123u), unsubscribe);

            // Assert
            observer.Verify(m => m.OnNext(new DisposeTextureData(123u)), Times.Once());
            Assert.Equal(expected, reactor.Observers.Count);
        }
        #endregion
    }
}
