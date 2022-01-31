// <copyright file="RemoveBatchItemReactorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using Moq;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="RemoveBatchItemReactable"/> class.
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
            var reactor = new Mock<IReactor<RemoveBatchItemData>>();

            var reactable = new RemoveBatchItemReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            var data = new RemoveBatchItemData(123u);
            reactable.PushNotification(data, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(data), Times.Once());

            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
