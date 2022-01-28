// <copyright file="RemoveBatchItemReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="RemoveBatchItemReactor"/> class.
    /// </summary>
    public class RemoveBatchItemReactorTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var observer = new Mock<IObserver<RemoveBatchItemData>>();

            var reactor = new RemoveBatchItemReactor();
            reactor.Subscribe(observer.Object);

            // Act
            var data = new RemoveBatchItemData(123u);
            reactor.PushNotification(data, unsubscribe);

            // Assert
            observer.Verify(m => m.OnNext(data), Times.Once());

            Assert.Equal(expected, reactor.Observers.Count);
        }
        #endregion
    }
}
