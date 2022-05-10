// <copyright file="MouseWheelReactableTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="MouseWheelReactable"/> class.
    /// </summary>
    public class MouseWheelReactableTests
    {
        #region Method Tests
        [Fact]
        public void PushNotification_WhenInvoked_SendsPushNotification()
        {
            // Arrange
            var reactor = new Mock<IReactor<(MouseScrollDirection, int)>>();

            var reactable = new MouseWheelReactable();
            reactable.Subscribe(reactor.Object);

            var data = (MouseScrollDirection.ScrollDown, 123);

            // Act
            reactable.PushNotification(data);

            // Assert
            reactor.Verify(m => m.OnNext(data), Times.Once());
        }
        #endregion
    }
}
