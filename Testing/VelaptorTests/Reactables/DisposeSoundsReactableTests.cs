// <copyright file="DisposeSoundsReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables
{
    using Moq;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="DisposeSoundsReactable"/> class.
    /// </summary>
    public class DisposeSoundsReactableTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var reactor = new Mock<IReactor<DisposeSoundData>>();

            var reactable = new DisposeSoundsReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            var soundData = new DisposeSoundData(123u);
            reactable.PushNotification(soundData);

            // Assert
            reactor.Verify(m => m.OnNext(soundData), Times.Once());
        }
        #endregion
    }
}
