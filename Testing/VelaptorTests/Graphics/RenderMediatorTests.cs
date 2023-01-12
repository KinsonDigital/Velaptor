// <copyright file="RenderMediatorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using Carbonate;
using Carbonate.Core;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="RenderMediator"/> class.
/// </summary>
public class RenderMediatorTests
{
    private readonly Mock<IPushReactable> mockReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorTests"/> class.
    /// </summary>
    public RenderMediatorTests() => this.mockReactable = new Mock<IPushReactable>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsUpReactable()
    {
        // Arrange
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(Subscribe);

        // Act
        _ = new RenderMediator(this.mockReactable.Object);

        // Assert
        void Subscribe(IReceiveReactor reactor)
        {
            reactor.Should().NotBeNull();
            reactor.Name.Should().Be("RenderMediatorTests.Ctor - RenderBatchEndedId");
            reactor.Id.Should().Be(NotificationIds.RenderBatchEndedId);
        }
    }

    [Fact]
    public void PushReactable_WithBatchEndNotification_CoordinatesRenderCalls()
    {
        // Arrange
        IReceiveReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(_ =>
            {
                _.Should().NotBeNull();
                reactor = _;
            });

        _ = new RenderMediator(this.mockReactable.Object);

        // Act
        reactor.OnReceive();

        // Assert
        this.mockReactable.Verify(m => m.Push(NotificationIds.RenderTexturesId), Times.Once);
        this.mockReactable.Verify(m => m.Push(NotificationIds.RenderRectsId), Times.Once);
        this.mockReactable.Verify(m => m.Push(NotificationIds.RenderFontsId), Times.Once);
        this.mockReactable.Verify(m => m.Push(NotificationIds.RenderLinesId), Times.Once);
    }
    #endregion
}
