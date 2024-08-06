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
using FluentAssertions;
using Helpers;
using Moq;
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
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IPushReactable<ViewPortSizeData>> mockViewPortReactable;
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
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGL.Setup(m => m.GenBuffer()).Returns(() =>
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

        this.mockGL.Setup(m => m.GenVertexArray()).Returns(VertexArrayId);

        this.mockGLService = new Mock<IOpenGLService>();

        this.mockPushReactable = new Mock<IPushReactable>();
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            });

        this.mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        this.mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<ViewPortSizeData>>()))
            .Callback<IReceiveSubscription<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");

                if (reactor.Id == PushNotifications.ViewPortSizeChangedId)
                {
                    this.viewPortSizeReactor = reactor;
                }
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(this.mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable()).Returns(this.mockViewPortReactable.Object);
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
                this.mockGLService.Object,
                this.mockReactableFactory.Object);
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
                this.mockGL.Object,
                null,
                this.mockReactableFactory.Object);
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
                this.mockGL.Object,
                this.mockGLService.Object,
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
        actual.Should().Be(100u);
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
        buffer.IsInitialized.Should().BeTrue();
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
        this.mockGL.Verify(m => m.GenVertexArray(), Times.Once);
        this.mockGLService.Verify(m => m.BindVAO(VertexArrayId), Times.Exactly(2));
        this.mockGLService.Verify(m => m.UnbindVAO(), Times.Exactly(2));
        this.mockGLService.Verify(m => m.LabelVertexArray(VertexArrayId, BufferName));
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
        this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
        this.mockGLService.VerifyExactly(m => m.BindVBO(VertexBufferId), 2);
        this.mockGLService.Verify(m => m.UnbindVBO(), Times.Once);
        this.mockGLService.Verify(m => m.LabelBuffer(VertexBufferId, BufferName, OpenGLBufferType.VertexBufferObject));
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
        this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.BindEBO(IndexBufferId), Times.Exactly(2));
        this.mockGLService.Verify(m => m.UnbindEBO(), Times.Exactly(2));
        this.mockGLService.Verify(m => m.LabelBuffer(IndexBufferId, BufferName, OpenGLBufferType.IndexArrayObject));
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
        sut.GenerateDataInvoked.Should().BeTrue(becauseMsg);
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
        sut.GenerateIndicesInvoked.Should().BeTrue(becauseMsg);
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
        this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ArrayBuffer,
                new[] { 1f, 2f, 3f, 4f },
                GLBufferUsageHint.DynamicDraw),
            Times.Once);
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
        this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ElementArrayBuffer,
                new uint[] { 11, 22, 33, 44 },
                GLBufferUsageHint.StaticDraw),
            Times.Once);
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
        sut.SetupVAOInvoked.Should().BeTrue(becauseMsg);
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

        this.mockGLService.Setup(m => m.BeginGroup(setupDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                setupDataGroupSequence = totalInvokes;
            });
        this.mockGLService.Setup(m => m.BeginGroup(uploadVertexDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                uploadVertexDataGroupSequence = totalInvokes;
            });
        this.mockGLService.Setup(m => m.BeginGroup(uploadIndicesDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                uploadIndicesDataGroupSequence = totalInvokes;
            });

        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        VerifyBatchDataIsUploadedToGpu();
        this.mockGLService.Verify(m => m.BeginGroup(It.IsAny<string>()), Times.Exactly(3));
        this.mockGLService.Verify(m => m.BeginGroup(setupDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadVertexDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadIndicesDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(3));

        // Check that the setup data group was called first
        setupDataGroupSequence.Should().Be(1);
        uploadVertexDataGroupSequence.Should().Be(2);
        uploadIndicesDataGroupSequence.Should().Be(3);
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
        sut.PrepareForUseInvoked.Should().BeTrue(becauseMsg);
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
        sut.UpdateVertexDataInvoked.Should().BeTrue(becauseMsg);
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
        this.mockGL.Verify(m => m.DeleteVertexArray(VertexArrayId), Times.Once());
        this.mockGL.Verify(m => m.DeleteBuffer(VertexBufferId), Times.Once());
        this.mockGL.Verify(m => m.DeleteBuffer(IndexBufferId), Times.Once());
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void PushReactable_WhenCreatingSubscriptions_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for this unit test.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    reactor.Name.Should().Be($"GpuBufferBase.ctor() - {PushNotifications.GLInitializedId}");
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    reactor.Name.Should().Be($"GpuBufferBase.ctor() - {PushNotifications.SystemShuttingDownId}");
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
        this.mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<ViewPortSizeData>>()))
            .Callback<IReceiveSubscription<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");
                reactor.Name.Should().Be($"GpuBufferBase.ctor() - {PushNotifications.ViewPortSizeChangedId}");
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
        sut.ViewPortSize.Should().BeEquivalentTo(new SizeU(11, 22));
    }
    #endregion

    /// <summary>
    /// Creates an instance of the type <see cref="GpuBufferFake"/> for the purpose of
    /// testing the abstract class <see cref="GpuBufferBase{TData}"/>.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GpuBufferFake CreateSystemUnderTest() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockReactableFactory.Object);

    /// <summary>
    /// Verifies that the correct GPU data has been sent to the GPU.
    /// </summary>
    private void VerifyBatchDataIsUploadedToGpu()
    {
        this.mockGLService.Verify(m => m.BindVBO(VertexBufferId), Times.AtLeastOnce);

        this.mockGL
            .Verify(m =>
                m.BufferData(GLBufferTarget.ArrayBuffer, new[] { 1f, 2f, 3f, 4f, }, GLBufferUsageHint.DynamicDraw), Times.AtLeastOnce);

        this.mockGL
            .Verify(m =>
                m.BufferData(GLBufferTarget.ElementArrayBuffer, new[] { 11u, 22u, 33u, 44u, }, GLBufferUsageHint.StaticDraw), Times.AtLeastOnce);

        this.mockGLService.Verify(m => m.UnbindVBO(), Times.AtLeastOnce);
    }
}
