// <copyright file="LineGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

/// <summary>
/// Tests the <see cref="LineGpuBuffer"/> class.
/// </summary>
public class LineGpuBufferTests
{
    private const uint VAO = 123u;
    private const uint VBO = 456u;
    private const uint EBO = 789u;
    private const uint ViewPortWidth = 100u;
    private const uint ViewPortHeight = 200u;
    private const string BufferName = "Line";
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuBufferTests"/> class.
    /// </summary>
    public LineGpuBufferTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGL.Setup(m => m.GenVertexArray()).Returns(VAO);

        this.mockGL.Setup(m => m.GenBuffer())
            .Returns(() =>
            {
                if (this.vboGenerated)
                {
                    return EBO;
                }

                this.vboGenerated = true;
                return VBO;
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
            _ = new LineGpuBuffer(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void UploadVertexData_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest(false);

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.UploadVertexData(It.IsAny<LineBatchItem>(), It.IsAny<uint>());
        }, "The line buffer has not been initialized.");
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_BeginsAndEndsGLDebugGroup()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the '{nameof(GpuBufferBase<Line>.UploadVertexData)}()' method.",
            $"3 times in the private $'{nameof(GpuBufferBase<Line>)}.Init()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var line = default(LineBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(line, 123);

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Update Line - BatchItem(123)"), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(),
            Times.Exactly(4),
            failMessage);
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_BindsAndUnbindsVBO()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the private '{nameof(GpuBufferBase<Line>)}.Init()' method.",
            $"1 time in the '{nameof(GpuBufferBase<Line>.UploadVertexData)}()' method.",
        };
        var failMessage = string.Join($"{Environment.NewLine}", executionLocations);

        var line = default(LineBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(line, 0);

        // Assert
        this.mockGLService.Verify(m => m.BindVBO(VBO),
            Times.AtLeastOnce,
            failMessage);

        this.mockGLService.Verify(m => m.UnbindVBO(),
            Times.AtLeastOnce,
            failMessage);
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_UploadsGpuBufferData()
    {
        // Arrange
        var expectedData = new[]
        {
            // Vertex 1
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.923431456f,  1.00828433f,     5f,     6f,     7f,     4f,

            // Vertex 2
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -1.03656852f,   0.951715708f,     5f,     6f,     7f,     4f,

            // Vertex 3
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.883431435f,  0.98828429f,     5f,     6f,     7f,     4f,

            // Vertex 4
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.996568561f,  0.931715727f,     5f,     6f,     7f,     4f,
        };

        var p1 = new Vector2(1, 2);
        var p2 = new Vector2(3, 4);
        var color = Color.FromArgb(4, 5, 6, 7);
        const int thickness = 8;

        var batchItem = new LineBatchItem(p1, p2, color, thickness);

        var viewPortSizeData = new ViewPortSizeData { Width = ViewPortWidth, Height = ViewPortHeight };

        var sut = CreateSystemUnderTest();
        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(batchItem, 10);

        // Assert
        this.mockGL.VerifyOnce(m =>
            m.BufferSubData(GLBufferTarget.ArrayBuffer, 0x3c0, 96, expectedData));
    }

    [Fact]
    public void PrepareForUpload_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest(false);

        // Act
        var act = () => sut.PrepareForUpload();

        // Assert
        act.Should().Throw<BufferNotInitializedException>()
            .WithMessage("The line buffer has not been initialized.");
    }

    [Fact]
    public void PrepareForUpload_WhenInitialized_BindsVAO()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the '{nameof(LineGpuBuffer.PrepareForUpload)}()' method.",
            $"1 time in the '{nameof(GpuBufferBase<Line>)}.Init()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var sut = CreateSystemUnderTest();

        // Act
        sut.PrepareForUpload();

        // Assert
        this.mockGLService.VerifyExactly(m => m.BindVAO(VAO), 3, failMessage);
    }

    [Fact]
    public void GenerateData_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest(false);

        // Act
        var actual = sut.GenerateData();

        // Assert
        sut.BatchSize.Should().Be(100u);
        actual.Should().HaveCount(2400);
        actual.Should().AllSatisfy(expected => expected.Should().Be(0));
    }

    [Fact]
    public void SetupVAO_WhenInvoked_SetsUpTheOpenGLVertexArrayObject()
    {
        // Arrange
        var paramData = new (uint index, int size, bool normalized, uint stride, uint offset, string label)[]
        {
            (0u, 2, false, 24u, 0u, "VertexPosition"),
            (1u, 4, false, 24u, 8u, "Color"),
        };
        var enableVertexAttribArrayParamData = new (uint index, string label)[]
        {
            (0u, "VertexPosition"),
            (1u, "Color"),
        };

        var sut = CreateSystemUnderTest(false);

        // Act
        sut.SetupVAO();

        // Assert
        Assert.All(paramData, data =>
        {
            this.mockGL.Verify(m =>
                    m.VertexAttribPointer(data.index,
                        data.size,
                        GLVertexAttribPointerType.Float,
                        data.normalized,
                        data.stride,
                        data.offset), Times.Once,
                $"The '{data.label}' vertex attribute pointer layout is incorrect.");
        });

        Assert.All(enableVertexAttribArrayParamData, data =>
        {
            var (index, label) = data;
            this.mockGL.Verify(m => m.EnableVertexAttribArray(index),
                $"Issue enabling vertex attribute pointer '{label}'.");
        });
    }

    [Fact]
    public void GenerateIndices_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = CreateExpectedIndicesData(100u);
        var sut = CreateSystemUnderTest(false);

        // Act
        var actual = sut.GenerateIndices();

        // Assert
        sut.BatchSize.Should().Be(100u);
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Reactable Tests
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
            reactor.Name.Should().Be("LineGpuBufferTests.Ctor - BatchSizeChangedId");
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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Line  });

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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Line });

        // Assert
        sut.BatchSize.Should().Be(123);

        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Vertex Data"), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.EndGroup(), Times.AtLeast(2));
        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Indices Data"), Times.AtLeastOnce);
    }
    #endregion

    /// <summary>
    /// Creates the expected indices test data.
    /// </summary>
    /// <returns>The data to use for unit testing.</returns>
    private static IEnumerable<uint> CreateExpectedIndicesData(uint batchSize)
    {
        var result = new List<uint>();

        for (var i = 0u; i < batchSize; i++)
        {
            var maxIndex = result.Count <= 0 ? 0 : result.Max() + 1;

            result.AddRange(new[]
            {
                maxIndex,
                maxIndex + 1u,
                maxIndex + 2u,
                maxIndex + 2u,
                maxIndex + 1u,
                maxIndex + 3u,
            });
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a new instance of <see cref="LineGpuBuffer"/> class for the purpose of testing.
    /// </summary>
    /// <param name="initialize">If true, will mock the initialization of the mocked sut.</param>
    /// <returns>The instance to test.</returns>
    private LineGpuBuffer CreateSystemUnderTest(bool initialize = true)
    {
        var result = new LineGpuBuffer(
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object);

        if (initialize)
        {
            this.glInitReactor.OnReceive();
        }

        return result;
    }
}
