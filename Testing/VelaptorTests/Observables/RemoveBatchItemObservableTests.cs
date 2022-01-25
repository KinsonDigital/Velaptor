// <copyright file="RemoveBatchItemObservableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using System;
    using Moq;
    using Velaptor.Observables;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="RemoveBatchItemObservable"/> class.
    /// </summary>
    public class RemoveBatchItemObservableTests
    {
        #region Method Tests
        [Fact]
        public void OnRemoveBatchItem_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<uint>>();

            var observable = new RemoveBatchItemObservable();
            observable.Subscribe(observer.Object);

            // Act
            observable.OnRemoveBatchItem(123u);

            // Assert
            observer.Verify(m => m.OnNext(123u), Times.Once());
        }
        #endregion
    }
}
