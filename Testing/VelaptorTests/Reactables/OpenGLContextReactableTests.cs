// <copyright file="OpenGLContextReactableTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="OpenGLContextReactable"/> class.
    /// </summary>
    public class OpenGLContextReactableTests
    {
        #region Method Tests
        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void PushNotification_WhenInvoked_SendsPushNotification(bool unsubscribe, int expected)
        {
            // Arrange
            var reactor = new Mock<IReactor<GLContextData>>();

            var reactable = new OpenGLContextReactable();
            reactable.Subscribe(reactor.Object);

            // Act
            reactable.PushNotification(default, unsubscribe);

            // Assert
            reactor.Verify(m => m.OnNext(default), Times.Once());
            Assert.Equal(expected, reactable.Reactors.Count);
        }
        #endregion
    }
}
