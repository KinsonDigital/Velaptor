// <copyright file="LineGPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Carbonate;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="LineGPUBuffer"/> class.
/// </summary>
public class LineGPUBufferTests
{
    private const uint BatchSize = 10u;
    private const uint VAO = 123u;
    private const uint VBO = 456u;
    private const uint EBO = 789u;
    private const uint ViewPortWidth = 100u;
    private const uint ViewPortHeight = 200u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReactor? glInitReactor;
    private IReactor? batchSizeReactor;
    private IReactor? viewPortSizeReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGPUBufferTests"/> class.
    /// </summary>
    public LineGPUBufferTests()
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

        this.mockBatchSizeUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Returns<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.EventId == NotificationIds.GLInitializedId)
                {
                    // RETURN NULL TO SIMPLY IGNORE THIS EVENT ID
                    return null!;
                }

                if (reactor.EventId == NotificationIds.BatchSizeSetId)
                {
                    return this.mockBatchSizeUnsubscriber.Object;
                }

                if (reactor.EventId == NotificationIds.ViewPortSizeChangedId)
                {
                    // RETURN NULL TO SIMPLY IGNORE THIS EVENT ID
                    return null!;
                }

                if (reactor.EventId == NotificationIds.SystemShuttingDownId)
                {
                    // RETURN NULL TO SIMPLY IGNORE THIS EVENT ID
                    return null!;
                }

                Assert.Fail($"The event ID '{reactor.EventId}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.EventId == NotificationIds.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.EventId == NotificationIds.BatchSizeSetId)
                {
                    this.batchSizeReactor = reactor;
                }
                else if (reactor.EventId == NotificationIds.ViewPortSizeChangedId)
                {
                    this.viewPortSizeReactor = reactor;
                }
                else if (reactor.EventId == NotificationIds.SystemShuttingDownId)
                {
                    // EMPTY ON PURPOSE.  SIMPLY IGNORING THIS EVENT ID
                }
                else
                {
                    Assert.Fail($"The event ID '{reactor.EventId}' is not recognized or accounted for in the unit test.");
                }
            });
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineGPUBuffer(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
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
            $"1 time in the '{nameof(GPUBufferBase<Line>.UploadVertexData)}()' method.",
            $"3 times in the private $'{nameof(GPUBufferBase<Line>)}.Init()' method.",
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
            $"1 time in the private '{nameof(GPUBufferBase<Line>)}.Init()' method.",
            $"1 time in the '{nameof(GPUBufferBase<Line>.UploadVertexData)}()' method.",
        };
        var failMessage = string.Join($"{Environment.NewLine}", executionLocations);

        var line = default(LineBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(line, 0);

        // Assert
        this.mockGLService.Verify(m => m.BindVBO(VBO),
            Times.Exactly(2),
            failMessage);

        this.mockGLService.Verify(m => m.UnbindVBO(),
            Times.Exactly(2),
            failMessage);
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_UploadsGPUBufferData()
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
        const int layer = 9;

        var batchItem = new LineBatchItem(p1, p2, color, thickness, layer);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<ViewPortSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => new ViewPortSizeData { Width = ViewPortWidth, Height = ViewPortHeight });

        var sut = CreateSystemUnderTest();
        this.viewPortSizeReactor.OnNext(mockMessage.Object);

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
            $"1 time in the '{nameof(LineGPUBuffer.PrepareForUpload)}()' method.",
            $"1 time in the '{nameof(GPUBufferBase<Line>)}.Init()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var sut = CreateSystemUnderTest();

        // Act
        sut.PrepareForUpload();

        // Assert
        this.mockGLService.Verify(m => m.BindVAO(VAO),
            Times.Exactly(2),
            failMessage);
    }

    [Fact]
    public void GenerateData_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = TestDataLoader
            .LoadTestData<float[]>(string.Empty,
                $"{nameof(LineGPUBufferTests)}.{nameof(GenerateData_WhenInvoked_ReturnsCorrectResult)}.json");
        var sut = CreateSystemUnderTest(false);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = BatchSize });

        this.batchSizeReactor.OnNext(mockMessage.Object);

        // Act
        var actual = sut.GenerateData();

        // Assert
        Assert.Equal(10u, sut.BatchSize);
        Assert.Equal(expected, actual);
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
        var expected = TestDataLoader
            .LoadTestData<uint[]>(
                string.Empty,
                $"{nameof(LineGPUBufferTests)}.{nameof(GenerateIndices_WhenInvoked_ReturnsCorrectResult)}.json");
        var sut = CreateSystemUnderTest(false);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = BatchSize });

        this.batchSizeReactor.OnNext(mockMessage.Object);

        // Act
        var actual = sut.GenerateIndices();

        // Assert
        Assert.Equal(10u, sut.BatchSize);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void BatchSizeReactable_OnComplete_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest(false);

        // Act
        this.batchSizeReactor.OnComplete();
        this.batchSizeReactor.OnComplete();

        // Assert
        this.mockBatchSizeUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void BatchSizeReactable_WithNullMessageData_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(LineGPUBuffer)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest(false);

        // Act
        var act = () => this.batchSizeReactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineGPUBuffer"/> class for the purpose of testing.
    /// </summary>
    /// <param name="initialize">If true, will mock the initialization of the mocked sut.</param>
    /// <returns>The instance to test.</returns>
    private LineGPUBuffer CreateSystemUnderTest(bool initialize = true)
    {
        var result = new LineGPUBuffer(
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactable.Object);

        if (initialize)
        {
            this.glInitReactor.OnNext();
        }

        return result;
    }
}
