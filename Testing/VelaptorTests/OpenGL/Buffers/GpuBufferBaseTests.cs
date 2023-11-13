// <copyright file="GpuBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using Carbonate.Core;
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
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Initializes a new instance of <see cref="GpuBufferBaseTests"/>.
/// </summary>
public class GpuBufferBaseTests
{
    private const string BufferName = "UNKNOWN BUFFER";
    private const uint VertexArrayId = 1256;
    private const uint VertexBufferId = 1234;
    private const uint IndexBufferId = 5678;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
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

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            });

        var mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<ViewPortSizeData>>()))
            .Callback<IReceiveSubscription<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.ViewPortSizeChangedId)
                {
                    this.viewPortSizeReactor = reactor;
                }
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable()).Returns(mockViewPortReactable.Object);
    }

    #region Constructor Tests
    [Fact]
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
    public void OpenGLInit_WhenInvoked_GeneratesVertexData()
    {
        // Arrange
        const string becauseMsg = $"The method '{nameof(GpuBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.";
        var sut  = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        sut.GenerateDataInvoked.Should().BeTrue(becauseMsg);
    }

    [Fact]
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

    #region Indirect Tests
    [Fact]
    public void PushReactable_WhenSubscribingToGLInitializedNotification_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = new Mock<IPushReactable>();
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    Act(reactor);
                }
            });

        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(ISubscription reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("GpuBufferFake.Ctor - GLInitializedId");
        }
    }

    [Fact]
    public void PushReactable_WhenSubscribingToSystemShutDownNotification_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = new Mock<IPushReactable>();
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    Act(reactor);
                }
            });

        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(ISubscription reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("GpuBufferFake.Ctor - SystemShuttingDownId");
        }
    }

    [Fact]
    public void ViewPortSizeReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<ViewPortSizeData>>()))
            .Callback<IReceiveSubscription<ViewPortSizeData>>(Act);

        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(ISubscription reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("GpuBufferFake.Ctor - ViewPortSizeChangedId");
        }
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
