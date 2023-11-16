// <copyright file="FontGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Drawing;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.ReactableData;
using Xunit;

public class FontGpuBufferTests
{
    private const uint VertexArrayId = 111;
    private const uint VertexBufferId = 222;
    private const uint IndexBufferId = 333;
    private const string BufferName = "Font";
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private IReceiveSubscription? shutDownReactor;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBufferTests"/> class.
    /// </summary>
    public FontGpuBufferTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGL.Setup(m => m.GenVertexArray()).Returns(VertexArrayId);
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
                else
                {
                    Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                }
            });

        var mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<ViewPortSizeData>>()))
            .Callback<IReceiveSubscription<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.viewPortSizeReactor = reactor;
            });

        var mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<BatchSizeData>>()))
            .Callback<IReceiveSubscription<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.batchSizeReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(mockBatchSizeReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateViewPortReactable()).Returns(mockViewPortReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontGpuBuffer(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void UploadVertexData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.UploadVertexData(It.IsAny<FontGlyphBatchItem>(), It.IsAny<uint>());
        }, "The font buffer has not been initialized.");
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_CreatesOpenGLDebugGroups()
    {
        // Arrange
        var batchItem = new FontGlyphBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            'g',
            2.5F,
            90,
            Color.Empty,
            RenderEffects.None,
            0);

        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Update Font Quad - BatchItem(0)"), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(5));
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_UploadsData()
    {
        // Arrange
        var expected = new[]
        {
            -0.784718275f, 0.883709013f, 0.142857149f, 0.75f, 147f, 112f, 219f, 255f, -0.862500012f,
            0.779999971f, 0.142857149f, 0.25f, 147f, 112f, 219f, 255f, -0.726381958f, 0.805927277f,
            0.571428597f, 0.75f, 147f, 112f, 219f, 255f, -0.804163694f, 0.702218235f, 0.571428597f, 0.25f, 147f,
            112f, 219f, 255f,
        };
        var batchItem = new FontGlyphBatchItem(
            new RectangleF(11, 22, 33, 44),
            new RectangleF(55, 66, 77, 88),
            'g',
            1.5f,
            45f,
            Color.MediumPurple,
            RenderEffects.None,
            1);

        var viewPortSizeData = new ViewPortSizeData { Width = 800, Height = 600 };

        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();
        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGLService.Verify(m => m.BindVBO(VertexBufferId), Times.AtLeastOnce);
        this.mockGL.Verify(m
            => m.BufferSubData(GLBufferTarget.ArrayBuffer, 0, 128u, expected));
        this.mockGLService.Verify(m => m.UnbindVBO(), Times.AtLeastOnce);
    }

    [Fact]
    public void PrepareForUpload_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.PrepareForUpload();
        }, "The font buffer has not been initialized.");
    }

    [Fact]
    public void PrepareForUpload_WhenInvoked_BindsVertexArrayObject()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        sut.PrepareForUpload();

        // Assert
        this.mockGLService.Verify(m => m.BindVAO(VertexArrayId), Times.AtLeastOnce);
    }

    [Fact]
    public void GenerateData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.GenerateData();
        }, "The font buffer has not been initialized.");
    }

    [Fact]
    public void GenerateData_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();

        // Act
        var actual = sut.GenerateData();

        // Assert
        actual.Length.Should().Be(3_200);
    }

    [Fact]
    public void SetupVAO_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.SetupVAO();
        }, "The font buffer has not been initialized.");
    }

    [Fact]
    public void SetupVAO_WhenInvoked_SetsUpVertexArrayObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Setup Font Buffer Vertex Attributes"), Times.Once);

        // Assert Vertex Position Attribute
        this.mockGL.Verify(m
            => m.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, 32, 0), Times.Once);
        this.mockGL.Verify(m => m.EnableVertexAttribArray(0));

        // Assert Texture Coordinate Attribute
        this.mockGL.Verify(m
            => m.VertexAttribPointer(1, 2, GLVertexAttribPointerType.Float, false, 32, 8), Times.Once);
        this.mockGL.Verify(m => m.EnableVertexAttribArray(1));

        // Assert Tint Color Attribute
        this.mockGL.Verify(m
            => m.VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, 32, 16), Times.Once);
        this.mockGL.Verify(m => m.EnableVertexAttribArray(2));

        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(4));
    }

    [Fact]
    public void GenerateIndices_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.GenerateIndices();
        }, "The font buffer has not been initialized.");
    }
    #endregion

    #region Rectable Tests
    [Fact]
    public void BatchSizeReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<BatchSizeData>>()))
            .Callback<IReceiveSubscription<BatchSizeData>>(Act);

        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(ISubscription reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("FontGpuBufferTests.Ctor - BatchSizeChangedId");
        }
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationThatIsNotCorrectBatchType_UpdatesBatchSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Texture });

        // Assert
        sut.BatchSize.Should().Be(100);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationWhenNotInitialized_UpdatesBatchSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Font });

        // Assert
        sut.BatchSize.Should().Be(123);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationWhenAlreadyInitialized_UpdatesBatchSizeAndResizesBatchDataOnGpu()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Font });

        // Assert
        sut.BatchSize.Should().Be(123);

        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Vertex Data"), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.EndGroup(), Times.AtLeast(2));
        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Indices Data"), Times.AtLeastOnce);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontGpuBuffer CreateSystemUnderTest() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockReactableFactory.Object);
}
