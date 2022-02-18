// <copyright file="DisposeTexturesReactableTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="DisposeTexturesReactable"/> class.
    /// </summary>
    public class DisposeTexturesReactableTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<DisposeTextureData>>();

            var reactable = new DisposeTexturesReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            reactable.PushNotification(new DisposeTextureData(123u), unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(new DisposeTextureData(123u)), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
