// <copyright file="GpuBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Fakes;
using Helpers;
using NSubstitute;
using Shouldly;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Initializes a new instance of <see cref="GpuBufferBaseTests"/>.
/// </summary>
public class GpuBufferBaseTests : TestsBase
{
    private const string BufferName = "UNKNOWN BUFFER";
    private const uint VertexArrayId = 1256;
    private const uint VertexBufferId = 1234;
    private const uint IndexBufferId = 5678;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable mockPushReactable;
    private readonly IPushReactable<ViewPortSizeData> mockViewPortReactable;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription? shutDownReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GpuBufferBaseTests"/> class.
    /// </summary>
    public GpuBufferBaseTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGL.GenBuffer().Returns((_) =>
        {
            if (!this.vertexBufferCreated)
            {
                this.vertexBufferCreated = true;
                return VertexBufferId;
            }

            if (this.indexBufferCreated)
            {
                return 0;
            }

            this.indexBufferCreated = true;
            return IndexBufferId;
        });

        this.mockGL.GenVertexArray().Returns(VertexArrayId);

        this.mockGLService = Substitute.For<IOpenGLService>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();

                reactor.ShouldNotBeNull("It is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            });

        this.mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockViewPortReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription<ViewPortSizeData>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<ViewPortSizeData>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");

                if (reactor.Id == PushNotifications.ViewPortSizeChangedId)
                {
                    this.viewPortSizeReactor = reactor;
                }
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(this.mockViewPortReactable);
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GpuBufferFake(
                null,
                this.mockGLService,
                this.mockReactableFactory);
        }, "Value cannot be null. (Parameter 'gl')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GpuBufferFake(
                this.mockGL,
                null,
                this.mockReactableFactory);
        }, "Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GpuBufferFake(
                this.mockGL,
                this.mockGLService,
                null);
        }, "Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Props Tests
    [Fact]
    [Trait("Category", Prop)]
    public void BatchSize_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var buffer = CreateSystemUnderTest();

        // Act
        var actual = buffer.BatchSize;

        // Assert
        actual.ShouldBe(100u);
    }

    [Fact]
    [Trait("Category", Prop)]
    public void IsInitialized_AfterGLInitializes_ReturnsTrue()
    {
        // Arrange
        var buffer = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        buffer.IsInitialized.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_CreatesVertexArrayObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Received(1).GenVertexArray();
        this.mockGLService.Received(2).BindVAO(VertexArrayId);
        this.mockGLService.Received(2).UnbindVAO();
        this.mockGLService.Received(1).LabelVertexArray(VertexArrayId, BufferName);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_CreatesVertexBufferObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        // These are all invoked once per quad
        this.mockGL.Received(2).GenBuffer();
        this.mockGLService.Received(2).BindVBO(VertexBufferId);
        this.mockGLService.Received(1).UnbindVBO();
        this.mockGLService.Received(1).LabelBuffer(VertexBufferId, BufferName, OpenGLBufferType.VertexBufferObject);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_CreatesElementBufferObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        // First invoke is done creating the Vertex Buffer, the second is the index buffer
        this.mockGL.Received(2).GenBuffer();
        this.mockGLService.Received(2).BindEBO(IndexBufferId);
        this.mockGLService.Received(2).UnbindEBO();
        this.mockGLService.Received(1).LabelBuffer(IndexBufferId, BufferName, OpenGLBufferType.IndexArrayObject);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_GeneratesVertexData()
    {
        // Arrange
        const string becauseMsg = $"The method '{nameof(GpuBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.";
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        sut.GenerateDataInvoked.ShouldBeTrue(becauseMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_GeneratesIndicesData()
    {
        // Arrange
        const string becauseMsg = $"The method '{nameof(GpuBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.";
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        sut.GenerateIndicesInvoked.ShouldBeTrue(becauseMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_UploadsVertexData()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Received(1).BufferData(GLBufferTarget.ArrayBuffer, Arg.Any<float[]>(), GLBufferUsageHint.DynamicDraw);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_UploadsIndicesData()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Received(1).BufferData(GLBufferTarget.ElementArrayBuffer, Arg.Any<uint[]>(), GLBufferUsageHint.StaticDraw);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_SetsUpVertexArrayObject()
    {
        // Arrange
        const string becauseMsg = $"The method '{nameof(GpuBufferBase<TextureBatchItem>.SetupVAO)}'() has not been invoked.";
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        sut.SetupVAOInvoked.ShouldBeTrue(becauseMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void OpenGLInit_WhenInvoked_SetsUpProperGLGrouping()
    {
        // Arrange
        const string setupDataGroupName = $"Setup {BufferName} Data";
        const string uploadVertexDataGroupName = $"Set size of {BufferName} Vertex Data";
        const string uploadIndicesDataGroupName = $"Set size of {BufferName} Indices Data";
        var totalInvokes = 0;
        var setupDataGroupSequence = 0;
        var uploadVertexDataGroupSequence = 0;
        var uploadIndicesDataGroupSequence = 0;

        this.mockGLService.When(x => x.BeginGroup(setupDataGroupName))
            .Do((_) =>
            {
                totalInvokes += 1;
                setupDataGroupSequence = totalInvokes;
            });

        this.mockGLService.When(x => x.BeginGroup(uploadVertexDataGroupName))
            .Do((_) =>
            {
                totalInvokes += 1;
                uploadVertexDataGroupSequence = totalInvokes;
            });

        this.mockGLService.When(x => x.BeginGroup(uploadIndicesDataGroupName))
            .Do((_) =>
            {
                totalInvokes += 1;
                uploadIndicesDataGroupSequence = totalInvokes;
            });

        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        VerifyBatchDataIsUploadedToGpu();
        this.mockGLService.Received(3).BeginGroup(Arg.Any<string>());
        this.mockGLService.Received(1).BeginGroup(setupDataGroupName);
        this.mockGLService.Received(1).BeginGroup(uploadVertexDataGroupName);
        this.mockGLService.Received(1).BeginGroup(uploadIndicesDataGroupName);
        this.mockGLService.Received(3).EndGroup();

        // Check that the setup data group was called first
        setupDataGroupSequence.ShouldBe(1);
        uploadVertexDataGroupSequence.ShouldBe(2);
        uploadIndicesDataGroupSequence.ShouldBe(3);
    }

    [Fact]
    [Trait("Category", Method)]
    public void UploadData_WhenInvoked_PreparesGpuForDataUpload()
    {
        // Arrange
        const string becauseMsg = $"the method '{nameof(GpuBufferBase<TextureBatchItem>.PrepareForUpload)}'() has not been invoked.";
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        sut.PrepareForUseInvoked.ShouldBeTrue(becauseMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void UploadData_WhenInvoked_UpdatesGpuData()
    {
        // Arrange
        const string becauseMsg = $"the method '{nameof(GpuBufferBase<TextureBatchItem>.UploadVertexData)}'() has not been invoked.";
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        sut.UpdateVertexDataInvoked.ShouldBeTrue(becauseMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void WithShutDownNotification_ShutsDownBuffer()
    {
        // Arrange
        CreateSystemUnderTest();

        this.glInitReactor.OnReceive();

        // Act
        this.shutDownReactor?.OnReceive();
        this.shutDownReactor?.OnReceive();

        // Assert
        this.mockGL.Received(1).DeleteVertexArray(VertexArrayId);
        this.mockGL.Received(1).DeleteBuffer(VertexBufferId);
        this.mockGL.Received(1).DeleteBuffer(IndexBufferId);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void PushReactable_WhenCreatingSubscriptions_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();

                reactor.ShouldNotBeNull("it is required for this unit test.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    reactor.Name.ShouldBe($"GpuBufferBase.ctor() - {PushNotifications.GLInitializedId}");
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    reactor.Name.ShouldBe($"GpuBufferBase.ctor() - {PushNotifications.SystemShuttingDownId}");
                }
            });

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void ViewPortSizeReactable_WhenCreatingSubscriptions_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockViewPortReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<ViewPortSizeData>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<ViewPortSizeData>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"GpuBufferBase.ctor() - {PushNotifications.ViewPortSizeChangedId}");
            });
    }

    [Fact]
    public void ViewPortSizeReactable_WhenReceivingNotification_UpdatesViewPortSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.viewPortSizeReactor.OnReceive(new ViewPortSizeData { Width = 11, Height = 22 });

        // Assert
        sut.ViewPortSize.ShouldBeEquivalentTo(new SizeU(11, 22));
    }
    #endregion

    /// <summary>
    /// Creates an instance of the type <see cref="GpuBufferFake"/> for the purpose of
    /// testing the abstract class <see cref="GpuBufferBase{TData}"/>.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GpuBufferFake CreateSystemUnderTest() => new (
        this.mockGL,
        this.mockGLService,
        this.mockReactableFactory);

    /// <summary>
    /// Verifies that the correct GPU data has been sent to the GPU.
    /// </summary>
    private void VerifyBatchDataIsUploadedToGpu()
    {
        this.mockGLService.Received(2).BindVBO(VertexBufferId);

        this.mockGL.Received(1).BufferData(GLBufferTarget.ArrayBuffer, Arg.Any<float[]>(), GLBufferUsageHint.DynamicDraw);
        this.mockGL.Received(1).BufferData(GLBufferTarget.ElementArrayBuffer, Arg.Any<uint[]>(), GLBufferUsageHint.StaticDraw);

        this.mockGLService.Received(1).UnbindVBO();
    }
}
