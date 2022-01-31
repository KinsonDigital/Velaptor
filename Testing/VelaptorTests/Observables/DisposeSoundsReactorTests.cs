// <copyright file="DisposeSoundsReactorTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="DisposeSoundsReactable"/> class.
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
            var reactor = new Mock<IReactor<DisposeSoundData>>();

            var reactable = new DisposeSoundsReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            var soundData = new DisposeSoundData(123u);
            reactable.PushNotification(soundData, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(soundData), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
