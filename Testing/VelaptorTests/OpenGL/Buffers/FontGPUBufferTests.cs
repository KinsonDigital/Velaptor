// <copyright file="FontGPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Drawing;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.ReactableData;
using Xunit;

public class FontGPUBufferTests
{
    private const uint BatchSize = 1000u;
    private const uint VertexArrayId = 111;
    private const uint VertexBufferId = 222;
    private const uint IndexBufferId = 333;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReceiveReactor? glInitReactor;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;
    private IReceiveReactor<ViewPortSizeData>? viewPortSizeReactor;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGPUBufferTests"/> class.
    /// </summary>
    public FontGPUBufferTests()
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

        this.mockBatchSizeUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == NotificationIds.GLInitializedId || reactor.Id == NotificationIds.SystemShuttingDownId)
                {
                    // RETURN NULL TO IGNORE THIS EVENT ID
                    return null!;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == NotificationIds.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.Id == NotificationIds.SystemShuttingDownId)
                {
                    // EMPTY ON PURPOSE. IGNORING THIS EVENT ID
                }
                else
                {
                    Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                }
            });

        var mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Returns<IReceiveReactor<BatchSizeData>>((reactor) =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                return this.mockBatchSizeUnsubscriber.Object;
            })
            .Callback<IReceiveReactor<BatchSizeData>>((reactor) =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.batchSizeReactor = reactor;
            });

        var mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<ViewPortSizeData>>()))
            .Callback<IReceiveReactor<ViewPortSizeData>>((reactor) =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.viewPortSizeReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataReactable()).Returns(mockPushReactable.Object);
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
            _ = new FontGPUBuffer(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenReactableNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(FontGPUBuffer)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.batchSizeReactor.OnReceive(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
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
            0,
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
            1,
            0);

        var mockMessage = new Mock<IMessage<ViewPortSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => new ViewPortSizeData { Width = 800, Height = 600 });

        var sut = CreateSystemUnderTest();

        this.glInitReactor.OnReceive();
        this.viewPortSizeReactor.OnReceive(mockMessage.Object);

        // Act
        sut.UploadVertexData(batchItem, 0u);

        // Assert
        this.mockGL.Verify(m
            => m.BufferSubData(GLBufferTarget.ArrayBuffer, 0, 128u, expected));
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

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = BatchSize });

        this.batchSizeReactor.OnReceive(mockMessage.Object);

        // Act
        var actual = sut.GenerateData();

        // Assert
        Assert.Equal(32_000, actual.Length);
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
    public void Se3tupVAO_WhenInvoked_SetsUpVertexArrayObject()
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

    #region Indirect Tests
    [Fact]
    public void BatchSizeReactable_OnComplete_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        this.mockBatchSizeUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontGPUBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontGPUBuffer CreateSystemUnderTest() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockReactableFactory.Object);
}
