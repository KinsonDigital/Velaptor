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
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<DisposeTextureData>>();

            var observable = new DisposeTexturesReactor();
            observable.Subscribe(observer.Object);

            // Act
            observable.PushNotification(new DisposeTextureData(123u));

            // Assert
            observer.Verify(m => m.OnNext(new DisposeTextureData(123u)), Times.Once());
        }
        #endregion
    }
}
