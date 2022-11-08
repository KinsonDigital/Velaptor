// <copyright file="OpenGLContextReactableTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Moq;
using Velaptor.Reactables;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables;

/// <summary>
/// Tests the <see cref="OpenGLContextReactable"/> class.
/// </summary>
public class OpenGLContextReactableTests
{
    #region Method Tests
    [Fact]
    public void PushNotification_WhenInvoked_SendsPushNotification()
    {
        // Arrange
        var reactor = new Mock<IReactor<GLContextData>>();

        var reactable = new OpenGLContextReactable();
        reactable.Subscribe(reactor.Object);

        // Act
        reactable.PushNotification(default);

        // Assert
        reactor.Verify(m => m.OnNext(default), Times.Once());
    }
    #endregion
}
