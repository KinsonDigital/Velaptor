// <copyright file="RendererBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using Carbonate;
using Carbonate.Core;
using Fakes;
using FluentAssertions;
using Moq;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="RendererBase"/> class.
/// </summary>
public class RendererBaseTests
{
    private readonly Mock<IGLInvoker> mockGLInvoker;
    private readonly Mock<IPushReactable> mockReactable;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBaseTests"/> class.
    /// </summary>
    public RendererBaseTests()
    {
        this.mockGLInvoker = new Mock<IGLInvoker>();
        this.mockReactable = new Mock<IPushReactable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RendererBaseFake(null, this.mockReactable.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RendererBaseFake(this.mockGLInvoker.Object, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsGLProperty()
    {
        // Arrange & Act
        var sut = new RendererBaseFake(this.mockGLInvoker.Object, this.mockReactable.Object);

        // Assert
        sut.GL.Should().BeSameAs(this.mockGLInvoker.Object);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WhenReceivingShutDownNotification_ShutsDownRenderer()
    {
        // Arrange
        IReceiveReactor? shutDownReactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(_ => this.mockShutDownUnsubscriber.Object)
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull();
                shutDownReactor = reactor;
            });

        _ = new RendererBaseFake(this.mockGLInvoker.Object, this.mockReactable.Object);

        // Act
        shutDownReactor.OnReceive();
        shutDownReactor.OnReceive();

        // Assert
        this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }
    #endregion
}
