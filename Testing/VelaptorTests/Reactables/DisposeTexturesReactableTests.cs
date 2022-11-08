// <copyright file="DisposeTexturesReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Moq;
using Velaptor.Reactables;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables;

/// <summary>
/// Tests the <see cref="DisposeTexturesReactable"/> class.
/// </summary>
public class DisposeTexturesReactableTests
{
    #region Method Tests
    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<DisposeTextureData>>();

        var reactable = new DisposeTexturesReactable();
        reactable.Subscribe(reactor.Object);

        // Act
        reactable.PushNotification(new DisposeTextureData(123u));

        // Assert
        reactor.Verify(m => m.OnNext(new DisposeTextureData(123u)), Times.Once());
    }
    #endregion
}
