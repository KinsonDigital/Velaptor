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
        [Fact]
        public void OnRemoveBatchItem_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var observer = new Mock<IObserver<RemoveBatchItemData>>();

            var observable = new RemoveBatchItemReactor();
            observable.Subscribe(observer.Object);

            // Act
            var data = new RemoveBatchItemData(123u);
            observable.PushNotification(data);

            // Assert
            observer.Verify(m => m.OnNext(data), Times.Once());
        }
        #endregion
    }
}
