// <copyright file="TextureGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Drawing;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Exceptions;
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
/// Tests the <see cref="TextureGpuBuffer"/> class.
/// </summary>
public class TextureGpuBufferTests : TestsBase
{
    private const uint VertexArrayId = 111;
    private const uint VertexBufferId = 222;
    private const uint IndexBufferId = 333;
    private const string BufferName = "Texture";
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable mockPushReactable;
    private readonly IPushReactable<BatchSizeData> mockBatchSizeReactable;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGpuBufferTests"/> class.
    /// </summary>
    public TextureGpuBufferTests()
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

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable.Subscribe(Arg.Do<IReceiveSubscription>(reactor =>
            {
                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
            }));

        var mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Subscribe(Arg.Do<IReceiveSubscription<ViewPortSizeData>>(
                reactor => this.viewPortSizeReactor = reactor));

        this.mockBatchSizeReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Subscribe(Arg.Do<IReceiveSubscription<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.batchSizeReactor = reactor;
            }));

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
        this.mockReactableFactory.CreateBatchSizeReactable().Returns(this.mockBatchSizeReactable);
    }

    /// <summary>
    /// Gets the sample data to test if the correct data is being sent to the GPU.
    /// </summary>
    /// <returns>The data to test against.</returns>
    public static TheoryData<RenderEffects, float[]> GetGpuUploadTestData =>
        new ()
        {
            {
                RenderEffects.None,
                [
                    -0.847915947f, 0.916118085f, 0.142857149f, 0.75f, 147f, 112f, 219f, 255f, -0.964588523f,
                    0.760554552f, 0.142857149f, 0.25f, 147f, 112f, 219f, 255f, -0.760411441f, 0.79944545f,
                    0.571428597f, 0.75f, 147f, 112f, 219f, 255f, -0.877084076f, 0.643881917f, 0.571428597f,
                    0.25f, 147f, 112f, 219f, 255f
                ]
            },
            {
                RenderEffects.FlipHorizontally,
                [
                    -0.760411441f, 0.79944545f, 0.142857149f, 0.75f, 147f, 112f, 219f, 255f, -0.877084076f,
                    0.643881917f, 0.142857149f, 0.25f, 147f, 112f, 219f, 255f, -0.847915947f, 0.916118085f,
                    0.571428597f, 0.75f, 147f, 112f, 219f, 255f, -0.964588523f, 0.760554552f, 0.571428597f,
                    0.25f, 147f, 112f, 219f, 255f,
                ]
            },
            {
                RenderEffects.FlipVertically,
                [
                    -0.964588523f, 0.760554552f, 0.142857149f, 0.75f, 147f, 112f, 219f, 255f, -0.847915947f,
                    0.916118085f, 0.142857149f, 0.25f, 147f, 112f, 219f, 255f, -0.877084076f, 0.643881917f,
                    0.571428597f, 0.75f, 147f, 112f, 219f, 255f, -0.760411441f, 0.79944545f, 0.571428597f,
                    0.25f, 147f, 112f, 219f, 255f,
                ]
            },
            {
                RenderEffects.FlipBothDirections,
                [
                    -0.877084076f, 0.643881917f, 0.142857149f, 0.75f, 147f, 112f, 219f, 255f, -0.760411441f,
                    0.79944545f, 0.142857149f, 0.25f, 147f, 112f, 219f, 255f, -0.964588523f, 0.760554552f,
                    0.571428597f, 0.75f, 147f, 112f, 219f, 255f, -0.847915947f, 0.916118085f, 0.571428597f,
                    0.25f, 147f, 112f, 219f, 255f,
                ]
            },
        };

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureGpuBuffer(
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
    [Trait("Category", Method)]
    public void UploadVertexData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadVertexData(default, 0);

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The texture buffer has not been initialized.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void UploadVertexData_WithInvalidRenderEffects_ThrowsException()
    {
        // Arrange
        var textureQuad = new TextureBatchItem(
            new RectangleF(1, 1, 1, 1),
            new RectangleF(1, 1, 1, 1),
            1,
            1,
            Color.White,
            (RenderEffects)1234,
            1);
        var sut = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        var act = () => sut.UploadVertexData(textureQuad, 0);

        // Assert
        act.Should().Throw<InvalidRenderEffectsException>("The 'RenderEffects' value of '1234' is not valid.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void UploadVertexData_WhenInvoked_CreatesOpenGLDebugGroups()
    {
        // Arrange
        var batchItem = new TextureBatchItem(
            default,
            default,
            1,
            0,
            default,
            RenderEffects.None,
            1);

        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Update Texture Quad - BatchItem(0) Data");
        this.mockGLService.Received(5).EndGroup();
    }

    [Theory]
    [Trait("Category", Method)]
    [MemberData(nameof(GetGpuUploadTestData))]
    public void UploadVertexData_WhenInvoked_UploadsData(RenderEffects effects, float[] expected)
    {
        // Arrange
        var batchItem = new TextureBatchItem(
            new RectangleF(11, 22, 33, 44),
            new RectangleF(55, 66, 77, 88),
            1.5f,
            45,
            Color.MediumPurple,
            effects,
            1);
        var viewPortSizeData = new ViewPortSizeData { Width = 800, Height = 600 };

        var actual = Array.Empty<float>();

        this.mockGL.When(x =>
            x.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();
        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGLService.Received(3).BindVBO(VertexBufferId);
        this.mockGL.Received(1).BufferSubData(GLBufferTarget.ArrayBuffer, 0, 128u, Arg.Any<float[]>());
        actual.Should().BeEquivalentTo(expected);
        this.mockGLService.Received().UnbindVBO();
    }

    [Fact]
    [Trait("Category", Method)]
    public void PrepareForUpload_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.PrepareForUpload();
        }, "The texture buffer has not been initialized.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void PrepareForUpload_WhenInvoked_BindsVertexArrayObject()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        sut.PrepareForUpload();

        // Assert
        this.mockGLService.Received(3).BindVAO(VertexArrayId);
    }

    [Fact]
    [Trait("Category", Method)]
    public void GenerateData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.GenerateData();
        }, "The texture buffer has not been initialized.");
    }

    [Fact]
    [Trait("Category", Method)]
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
    [Trait("Category", Method)]
    public void SetupVAO_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.SetupVAO();
        }, "The texture buffer has not been initialized.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void SetupVAO_WhenInvoked_SetsUpVertexArrayObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGLService.Received(1).BeginGroup("Setup Texture Buffer Vertex Attributes");

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
    [Trait("Category", Method)]
    public void GenerateIndices_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.GenerateIndices();
        }, "The texture buffer has not been initialized.");
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void PushReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange & Assert
        this.mockPushReactable.Subscribe(Arg.Do<IReceiveSubscription>(reactor =>
        {
            reactor.Should().NotBeNull("it is required for unit testing.");
        }));

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void BatchSizeReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange & Assert
        this.mockBatchSizeReactable.Subscribe(Arg.Do<IReceiveSubscription<BatchSizeData>>(reactor =>
        {
            reactor.Should().NotBeNull("it is required for unit testing.");
            reactor.Name.Should().Be($"TextureGpuBuffer.ctor() - {PushNotifications.BatchSizeChangedId}");
        }));

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationThatIsNotCorrectBatchType_UpdatesBatchSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Line });

        // Assert
        sut.BatchSize.Should().Be(100);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationWhenNotInitialized_UpdatesBatchSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Texture });

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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Texture });

        // Assert
        sut.BatchSize.Should().Be(123);

        this.mockGLService.Received(2).BeginGroup($"Set size of {BufferName} Vertex Data");
        this.mockGLService.Received(6).EndGroup();
        this.mockGLService.Received(2).BeginGroup($"Set size of {BufferName} Indices Data");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureGpuBuffer CreateSystemUnderTest() => new (
        this.mockGL,
        this.mockGLService,
        this.mockReactableFactory);
}
