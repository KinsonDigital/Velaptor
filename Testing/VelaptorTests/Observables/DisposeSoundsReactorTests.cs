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
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<DisposeSoundData>>();

            var observable = new DisposeSoundsReactor();
            observable.Subscribe(observer.Object);

            // Act
            var soundData = new DisposeSoundData(123u);
            observable.PushNotification(soundData);

            // Assert
            observer.Verify(m => m.OnNext(soundData), Times.Once());
        }
        #endregion
    }
}
