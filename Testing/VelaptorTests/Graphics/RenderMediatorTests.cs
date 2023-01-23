// <copyright file="RenderMediatorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="RenderMediator"/> class.
/// </summary>
public class RenderMediatorTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorTests"/> class.
    /// </summary>
    public RenderMediatorTests()
    {
        this.mockPushReactable = new Mock<IPushReactable>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(this.mockPushReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsUpReactable()
    {
        // Arrange
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(Subscribe);

        // Act
        _ = new RenderMediator(this.mockReactableFactory.Object);

        // Assert
        void Subscribe(IReceiveReactor reactor)
        {
            reactor.Should().NotBeNull();
            reactor.Name.Should().Be("RenderMediatorTests.Ctor - RenderBatchEndedId");
            reactor.Id.Should().Be(PushNotifications.RenderBatchEndedId);
        }
    }

    [Fact]
    public void PushReactable_WithBatchEndNotification_CoordinatesRenderCalls()
    {
        // Arrange
        IReceiveReactor? reactor = null;

        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(_ =>
            {
                _.Should().NotBeNull();
                reactor = _;
            });

        _ = new RenderMediator(this.mockReactableFactory.Object);

        // Act
        reactor.OnReceive();

        // Assert
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderTexturesId), Times.Once);
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderRectsId), Times.Once);
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderFontsId), Times.Once);
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderLinesId), Times.Once);
    }
    #endregion
}
