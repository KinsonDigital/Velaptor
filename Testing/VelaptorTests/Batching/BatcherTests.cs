// <copyright file="BatcherTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.Drawing;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Graphics.Renderers.Exceptions;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Batcher"/> class.
/// </summary>
public class BatcherTests
{
    private readonly IGLInvoker mockGLInvoker;
    private readonly IPushReactable mockPushReactable;
    private readonly IPushReactable<BatchSizeData> mockBatchSizeReactable;
    private ReceiveSubscription? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatcherTests"/> class.
    /// </summary>
    public BatcherTests()
    {
        this.mockGLInvoker = Substitute.For<IGLInvoker>();

        var mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable.Subscribe(Arg.Any<ReceiveSubscription>())
            .Returns(mockUnsubscriber)
            .AndDoes(callInfo =>
            {
                var reactorParam = callInfo.Arg<ReceiveSubscription>();

                if (reactorParam is null)
                {
                    const string methodName = $"{nameof(this.mockPushReactable)}.{nameof(IPushReactable)}.{nameof(IPushReactable.Subscribe)}()";
                    throw new AssertionFailedException($"The '{methodName}' parameter '{nameof(reactorParam)}' cannot be null.");
                }

                this.reactor = reactorParam;
            });

        this.mockBatchSizeReactable = Substitute.For<IPushReactable<BatchSizeData>>();
    }

    #region Reactable Tests
    [Fact]
    public void GLInitSubscription_WhenReceivingNotification_ExecutesCorrectly()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.reactor.OnReceive();
        this.reactor.OnReceive();

        // Assert
        this.mockGLInvoker.Received(1).Enable(GLEnableCap.Blend);
        this.mockGLInvoker.Received(1).BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

        // Testing the sending of batch size notifications for different batch types
        this.mockBatchSizeReactable.Received(1)
            .Push(new BatchSizeData { BatchSize = 1000, TypeOfBatch = BatchType.Texture }, PushNotifications.BatchSizeChangedId);
        this.mockBatchSizeReactable.Received(1)
            .Push(new BatchSizeData { BatchSize = 1000, TypeOfBatch = BatchType.Font }, PushNotifications.BatchSizeChangedId);
        this.mockBatchSizeReactable.Received(1)
            .Push(new BatchSizeData { BatchSize = 1000, TypeOfBatch = BatchType.Rect }, PushNotifications.BatchSizeChangedId);
        this.mockBatchSizeReactable.Received(1)
            .Push(new BatchSizeData { BatchSize = 1000, TypeOfBatch = BatchType.Line }, PushNotifications.BatchSizeChangedId);
    }
    #endregion

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Batcher(null, this.mockPushReactable, this.mockBatchSizeReactable);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'glInvoker')");
    }

    [Fact]
    public void Ctor_WithNullPushReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Batcher(this.mockGLInvoker, null, this.mockBatchSizeReactable);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'glInitReactable')");
    }

    [Fact]
    public void Ctor_WithNullBatchSizeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Batcher(this.mockGLInvoker, this.mockPushReactable, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'batchSizeReactable')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void ClearColor_WhenCachingAndSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        // Setup the GLInvoker to return a different color than magenta so we can
        // make sure that the cached value is being returned.
        var colorValues = new float[4];
        this.mockGLInvoker.When(x => x.GetFloat(GLGetPName.ColorClearValue, colorValues))
            .Do(ci =>
            {
                var values = ci.ArgAt<float[]>(1);

                values[0] = 1;
                values[1] = 0;
                values[2] = 1;
                values[3] = 1;
            });

        var sut = CreateSystemUnderTest();

        // Act
        sut.ClearColor = Color.Magenta;

        // Assert
        sut.ClearColor.Should().Be(Color.Magenta);
    }

    [Fact]
    public void ClearColor_WhenNotCachingAndSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        // Setup the GLInvoker to return a different color than magenta so we can
        // make sure that the cached value is being returned.
        this.mockGLInvoker.When(x => x.GetFloat(GLGetPName.ColorClearValue, Arg.Any<float[]>()))
            .Do(ci =>
            {
                var values = ci.ArgAt<float[]>(1);

                values[0] = 1;
                values[1] = 0;
                values[2] = 1;
                values[3] = 1;
            });

        var sut = CreateSystemUnderTest();
        this.reactor.OnReceive();

        // Act
        sut.ClearColor = Color.Magenta;
        var actual = sut.ClearColor;

        // Assert
        this.mockGLInvoker.Received(1).ClearColor(1, 0, 1, 1);
        this.mockGLInvoker.Received(1).GetFloat(GLGetPName.ColorClearValue, Arg.Any<float[]>());

        actual.A.Should().Be(Color.Magenta.A);
        actual.R.Should().Be(Color.Magenta.R);
        actual.G.Should().Be(Color.Magenta.G);
        actual.B.Should().Be(Color.Magenta.B);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Begin_WhenInvokedWithoutBeingInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Begin();

        // Assert
        act.Should().Throw<RendererException>()
            .WithMessage("The renderer is not initialized.");
    }

    [Fact]
    public void Begin_WhenInvokedAfterBeingInitialized_SendsBatchBegunNotification()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.reactor.OnReceive();

        // Act
        sut.Begin();

        // Assert
        this.mockPushReactable.Received(1).Push(PushNotifications.BatchHasBegunId);
        sut.HasBegun.Should().BeTrue();
    }

    [Fact]
    public void Clear_WhenInvokedWithoutBeingInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Clear();

        // Assert
        act.Should().Throw<RendererException>()
            .WithMessage("The renderer is not initialized.");
    }

    [Fact]
    public void Clear_WhenInvokedAfterBeingInitialized_SendsBatchBegunNotification()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.reactor.OnReceive();

        // Act
        sut.Clear();

        // Assert
        this.mockGLInvoker.Received(1).Clear(GLClearBufferMask.ColorBufferBit);
    }

    [Fact]
    public void End_WhenInvokedWithoutBeingInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.End();

        // Assert
        act.Should().Throw<RendererException>()
            .WithMessage("The renderer is not initialized.");
    }

    [Fact]
    public void End_WhenInvokedAfterBeingInitialized_SendsBatchBegunNotification()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.reactor.OnReceive();

        // Act
        sut.End();

        // Assert
        this.mockPushReactable.Received(1).Push(PushNotifications.BatchHasEndedId);
        sut.HasBegun.Should().BeFalse();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Batcher"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Batcher CreateSystemUnderTest()
        => new (this.mockGLInvoker, this.mockPushReactable, this.mockBatchSizeReactable);
}
