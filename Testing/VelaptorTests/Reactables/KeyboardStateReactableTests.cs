// <copyright file="KeyboardStateReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Moq;
using Velaptor.Input;
using Velaptor.Reactables;
using Velaptor.Reactables.Core;
using Xunit;

namespace VelaptorTests.Reactables;

public class KeyboardStateReactableTests
{
    #region Method Tests
    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<(KeyCode key, bool isDown)>>();

        var reactable = new KeyboardStateReactable();
        reactable.Subscribe(reactor.Object);

        var data = (KeyCode.Space, true);

        // Act
        reactable.PushNotification(data);

        // Assert
        reactor.Verify(m => m.OnNext(data), Times.Once());
    }
    #endregion
}
