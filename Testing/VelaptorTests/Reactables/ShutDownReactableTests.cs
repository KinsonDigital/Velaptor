// <copyright file="ShutDownReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Moq;
using Velaptor.Reactables;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables;

/// <summary>
/// Tests the <see cref="ShutDownReactable"/> class.
/// </summary>
public class ShutDownReactableTests
{
    #region Method Tests
    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<ShutDownData>>();

        var reactable = new ShutDownReactable();
        reactable.Subscribe(reactor.Object);

        // Act
        reactable.PushNotification(default);

        // Assert
        reactor.Verify(m => m.OnNext(default), Times.Once());
    }
    #endregion
}
