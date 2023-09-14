// <copyright file="RendererBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Fakes;
using FluentAssertions;
using Moq;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="RendererBase"/> class.
/// </summary>
public class RendererBaseTests
{
    private readonly Mock<IGLInvoker> mockGLInvoker;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBaseTests"/> class.
    /// </summary>
    public RendererBaseTests()
    {
        this.mockGLInvoker = new Mock<IGLInvoker>();
        this.mockPushReactable = new Mock<IPushReactable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(this.mockPushReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RendererBaseFake(null, this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullParam_ReactableFactoryThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RendererBaseFake(this.mockGLInvoker.Object, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsGLProperty()
    {
        // Arrange & Act
        var sut = new RendererBaseFake(this.mockGLInvoker.Object, this.mockReactableFactory.Object);

        // Assert
        sut.GL.Should().BeSameAs(this.mockGLInvoker.Object);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WhenReceivingShutDownNotification_ShutsDownRenderer()
    {
        // Arrange
        IReceiveSubscription? shutDownReactor = null;

        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Returns<IReceiveSubscription>(_ => this.mockShutDownUnsubscriber.Object)
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                shutDownReactor = reactor;
            });

        _ = new RendererBaseFake(this.mockGLInvoker.Object, this.mockReactableFactory.Object);

        // Act
        shutDownReactor.OnReceive();
        shutDownReactor.OnReceive();

        // Assert
        this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }
    #endregion
}
