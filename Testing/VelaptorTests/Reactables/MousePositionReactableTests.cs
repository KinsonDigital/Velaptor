// <copyright file="MousePositionReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables
{
    using Moq;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="MousePositionReactable"/> class.
    /// </summary>
    public class MousePositionReactableTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<(int x, int y)>>();

            var reactable = new MousePositionReactable();
            reactable.Subscribe(reactor.Object);

            var data = (11, 22);

            // Act
            reactable.PushNotification(data, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(data), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
