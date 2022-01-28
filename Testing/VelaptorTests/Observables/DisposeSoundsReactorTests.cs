// <copyright file="DisposeSoundsReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="DisposeSoundsReactor"/> class.
    /// </summary>
    public class DisposeSoundsReactorTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var observer = new Mock<IObserver<DisposeSoundData>>();

            var reactor = new DisposeSoundsReactor();
            reactor.Subscribe(observer.Object);

            // Act
            var soundData = new DisposeSoundData(123u);
            reactor.PushNotification(soundData, unsubscribe);

            // Assert
            observer.Verify(m => m.OnNext(soundData), Times.Once());
            Assert.Equal(expected, reactor.Observers.Count);
        }
        #endregion
    }
}
