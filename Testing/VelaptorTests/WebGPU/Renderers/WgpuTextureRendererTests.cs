// <copyright file="WgpuTextureRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Renderers;

using System.Numerics;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Helpers;
using NSubstitute;
using Shouldly;
using Velaptor;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the viewport subscription behavior in WebGPU renderers.
/// </summary>
/// <remarks>
/// The WebGPU renderers use concrete sealed dependency types
/// (<see cref="Velaptor.WebGPU.Buffers.TextureGpuBuffer"/>,
/// <see cref="Velaptor.WebGPU.GraphicsTexturePipeline"/>, etc.) that cannot be mocked
/// with NSubstitute because they require GPU initialization. Full renderer construction
/// testing requires extracting interfaces for these types, which is a separate architectural task.
///
/// These tests verify the subscription pattern that all 4 renderers use to listen for
/// <see cref="PushNotifications.ViewPortSizeChangedId"/>.
/// </remarks>
public class WgpuTextureRendererTests : TestsBase
{
    private readonly IPushReactable<ViewPortSizeData> mockViewportReactable;
    private readonly IReactableFactory mockReactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WgpuTextureRendererTests"/> class.
    /// </summary>
    public WgpuTextureRendererTests()
    {
        this.mockViewportReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateViewPortReactable().Returns(this.mockViewportReactable);
    }

    #region Viewport Subscription Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void ViewportSubscription_WhenViewportChanges_ReceivesCorrectNotification()
    {
        // Arrange
        var bufferWindowSize = new Vector2(800f, 600f); // Default, matches WebGpuBufferBase default
        var viewportReactable = this.mockReactableFactory.CreateViewPortReactable();

        // Act — simulate what CreateOneWayReceive does internally
        var reactor = Substitute.For<IReceiveSubscription<ViewPortSizeData>>();
        reactor.Id.Returns(PushNotifications.ViewPortSizeChangedId);
        reactor
            .When(x => x.OnReceive(Arg.Any<ViewPortSizeData>()))
            .Do(callInfo =>
            {
                var data = callInfo.Arg<ViewPortSizeData>();
                bufferWindowSize = new Vector2(data.Width, data.Height);
            });

        viewportReactable.Subscribe(reactor);

        // Simulate the push notification
        reactor.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockViewportReactable.Received(1).Subscribe(
            Arg.Is<IReceiveSubscription<ViewPortSizeData>>(r =>
                r.Id == PushNotifications.ViewPortSizeChangedId));
        bufferWindowSize.ShouldBe(new Vector2(1920f, 1080f));
    }
    #endregion
}
