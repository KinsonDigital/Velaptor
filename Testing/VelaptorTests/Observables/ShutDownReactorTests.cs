// <copyright file="ShutDownReactorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Observables
{
    using Moq;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ShutDownReactable"/> class.
    /// </summary>
    public class ShutDownReactorTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<ShutDownData>>();

            var reactable = new ShutDownReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            reactable.PushNotification(default, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(default), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
