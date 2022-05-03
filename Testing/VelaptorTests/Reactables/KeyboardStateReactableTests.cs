// <copyright file="KeyboardStateReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables
{
    using Moq;
    using Velaptor.Input;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Xunit;

    public class KeyboardStateReactableTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<(KeyCode key, bool isDown)>>();

            var reactable = new KeyboardStateReactable();
            reactable.Subscribe(reactor.Object);

            var data = (KeyCode.Space, true);

            // Act
            reactable.PushNotification(data, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(data), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
