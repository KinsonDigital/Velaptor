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
using NSubstitute;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="FontGpuBuffer"/> class.
/// </summary>
public class FontGpuBufferTests
{
    private const uint VertexArrayId = 111;
    private const uint VertexBufferId = 222;
    private const uint IndexBufferId = 333;
    private const string BufferName = "Font";
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBufferTests"/> class.
    /// </summary>
    public FontGpuBufferTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGL.GenVertexArray().Returns(VertexArrayId);
        this.mockGL.GenBuffer().Returns(_ =>
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

        this.mockGLService = Substitute.For<IOpenGLService>();

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable.Subscribe(Arg.Do<IReceiveSubscription>(reactor =>
        {
            reactor.Should().NotBeNull("it is required for unit testing.");

            if (reactor.Id == PushNotifications.GLInitializedId)
            {
                this.glInitReactor = reactor;
            }
            else if (reactor.Id == PushNotifications.SystemShuttingDownId)
            {
                // Do nothing
            }
            else
            {
                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
            }
        }));

        var mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Subscribe(Arg.Do<IReceiveSubscription<ViewPortSizeData>>(reactor =>
        {
            reactor.Should().NotBeNull("it is required for unit testing.");
            this.viewPortSizeReactor = reactor;
        }));

        var mockBatchSizeReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Subscribe(Arg.Do<IReceiveSubscription<BatchSizeData>>(reactor =>
        {
            reactor.Should().NotBeNull("it is required for unit testing.");
            this.batchSizeReactor = reactor;
        }));

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateBatchSizeReactable().Returns(mockBatchSizeReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontGpuBuffer(
                this.mockGL,
                this.mockGLService,
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

        // Act
        var act = () => sut.UploadVertexData(default, 0);

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The font buffer has not been initialized.");
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
        this.mockGLService.Received(1).BeginGroup("Update Font Quad - BatchItem(0)");
        this.mockGLService.Received(5).EndGroup();
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
        var actual = Array.Empty<float>();

        // Get the actual data for assertion later
        this.mockGL.When(x =>
                x.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

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

        var sut = new FontGpuBuffer(this.mockGL, this.mockGLService, this.mockReactableFactory);

        this.glInitReactor.OnReceive();
        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGLService.Received().BindVBO(VertexBufferId);
        this.mockGL.Received(1).BufferSubData(GLBufferTarget.ArrayBuffer, 0, 128u, Arg.Any<float[]>());
        actual.Should().BeEquivalentTo(expected);
        this.mockGLService.Received().UnbindVBO();
    }

    [Fact]
    public void PrepareForUpload_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.PrepareForUpload();

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The font buffer has not been initialized.");
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
        this.mockGLService.Received().BindVAO(VertexArrayId);
    }

    [Fact]
    public void GenerateData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GenerateData();

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The font buffer has not been initialized.");
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

        // Act
        var act = () => sut.SetupVAO();

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The font buffer has not been initialized.");
    }

    [Fact]
    public void SetupVAO_WhenInvoked_SetsUpVertexArrayObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGLService.Received(1).BeginGroup("Setup Font Buffer Vertex Attributes");

        // Assert Vertex Position Attribute
        this.mockGL.Received(1).VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, 32, 0);
        this.mockGL.Received(1).EnableVertexAttribArray(0);

        // Assert Texture Coordinate Attribute
        this.mockGL.Received(1).VertexAttribPointer(1, 2, GLVertexAttribPointerType.Float, false, 32, 8);
        this.mockGL.Received(1).EnableVertexAttribArray(1);

        // Assert Tint Color Attribute
        this.mockGL.Received(1).VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, 32, 16);
        this.mockGL.Received(1).EnableVertexAttribArray(2);

        this.mockGLService.Received(4).EndGroup();
    }

    [Fact]
    public void GenerateIndices_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GenerateIndices();

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The font buffer has not been initialized.");
    }
    #endregion

    #region Rectable Tests
    [Fact]
    public void BatchSizeReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        mockReactable.When(x =>
            x.Subscribe(Arg.Do<IReceiveSubscription<BatchSizeData>>(Act)));

        this.mockReactableFactory.CreateBatchSizeReactable().Returns(mockReactable);

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

        this.mockGLService.Received().BeginGroup($"Set size of {BufferName} Vertex Data");
        this.mockGLService.Received(6).EndGroup();
        this.mockGLService.Received().BeginGroup($"Set size of {BufferName} Indices Data");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontGpuBuffer CreateSystemUnderTest() => new (this.mockGL, this.mockGLService, this.mockReactableFactory);
}
