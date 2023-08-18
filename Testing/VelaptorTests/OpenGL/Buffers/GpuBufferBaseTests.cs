// <copyright file="GpuBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
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
    private readonly Mock<IDisposable> mockGLInitUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IDisposable> mockViewPortSizeUnsubscriber;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;
    private IReceiveReactor? glInitReactor;
    private IReceiveReactor? shutDownReactor;
    private IReceiveReactor<ViewPortSizeData>? viewPortSizeReactor;

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

        this.mockGLInitUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        this.mockViewPortSizeUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    return this.mockGLInitUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReceiveReactor>(reactor =>
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
        mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<ViewPortSizeData>>()))
            .Returns<IReceiveReactor<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.ViewPortSizeChangedId)
                {
                    return this.mockViewPortSizeUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReceiveReactor<ViewPortSizeData>>(reactor =>
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
        }, "The parameter must not be null. (Parameter 'gl')");
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
        }, "The parameter must not be null. (Parameter 'openGLService')");
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
        }, "The parameter must not be null. (Parameter 'reactableFactory')");
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
        Assert.Equal(100u, actual);
    }

    [Fact]
    public void IsInitialized_AfterGLInitializes_ReturnsTrue()
    {
        // Arrange
        var buffer = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        Assert.True(buffer.IsInitialized);
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
        var sut  = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        Assert.True(sut.GenerateDataInvoked, $"The method '{nameof(GpuBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.");
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_GeneratesIndicesData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        Assert.True(sut.GenerateIndicesInvoked, $"The method '{nameof(GpuBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.");
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
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        Assert.True(sut.SetupVAOInvoked, $"The method '{nameof(GpuBufferBase<TextureBatchItem>.SetupVAO)}'() has not been invoked.");
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
        VerifyBatchDataIsUploadedToGPU();
        this.mockGLService.Verify(m => m.BeginGroup(It.IsAny<string>()), Times.Exactly(3));
        this.mockGLService.Verify(m => m.BeginGroup(setupDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadVertexDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadIndicesDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(3));

        // Check that the setup data group was called first
        Assert.Equal(1, setupDataGroupSequence);
        Assert.Equal(2, uploadVertexDataGroupSequence);
        Assert.Equal(3, uploadIndicesDataGroupSequence);
    }

    [Fact]
    public void UploadData_WhenInvoked_PreparesGPUForDataUpload()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        Assert.True(sut.PrepareForUseInvoked, $"The method '{nameof(GpuBufferBase<TextureBatchItem>.PrepareForUpload)}'() has not been invoked.");
    }

    [Fact]
    public void UploadData_WhenInvoked_UpdatesGPUData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        Assert.True(sut.UpdateVertexDataInvoked, $"The method '{nameof(GpuBufferBase<TextureBatchItem>.UploadVertexData)}'() has not been invoked.");
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
        this.mockViewPortSizeUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
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
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactor =>
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
        void Act(IReactor reactor)
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
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactor =>
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
        void Act(IReactor reactor)
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
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<ViewPortSizeData>>()))
            .Callback<IReceiveReactor<ViewPortSizeData>>(Act);

        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(IReactor reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("GpuBufferFake.Ctor - ViewPortSizeChangedId");
        }
    }

    [Fact]
    public void InitReactable_OnComplete_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnUnsubscribe();

        // Assert
        this.mockGLInitUnsubscriber.VerifyOnce(m => m.Dispose());
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
    private void VerifyBatchDataIsUploadedToGPU()
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
