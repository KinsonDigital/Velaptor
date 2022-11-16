// <copyright file="MouseButtonReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Moq;
using Velaptor.Input;
using Velaptor.Reactables;
using Velaptor.Reactables.Core;
using Xunit;

namespace VelaptorTests.Reactables;

/// <summary>
/// Tests the <see cref="MouseButtonReactable"/> class.
/// </summary>
public class MouseButtonReactableTests
{
    #region Method Tests
    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<(MouseButton, bool)>>();

        var reactable = new MouseButtonReactable();
        reactable.Subscribe(reactor.Object);

        var data = (MouseButton.LeftButton, true);

        // Act
        reactable.PushNotification(data);

        // Assert
        reactor.Verify(m => m.OnNext(data), Times.Once());
    }
    #endregion
}
