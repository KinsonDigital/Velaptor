// <copyright file="MouseButtonReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables
{
    using Moq;
    using Velaptor.Input;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="MouseButtonReactable"/> class.
    /// </summary>
    public class MouseButtonReactableTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<(MouseButton, bool)>>();

            var reactable = new MouseButtonReactable();
            reactable.Subscribe(reactor.Object);

            var data = (MouseButton.LeftButton, true);

            // Act
            reactable.PushNotification(data, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(data), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
