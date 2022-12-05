// <copyright file="LineGPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
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
    private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;
    private IReactor<GLInitData>? glInitReactor;
    private IReactor<BatchSizeData>? batchSizeReactor;
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

        this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
        this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
            .Callback<IReactor<GLInitData>>(reactor =>
            {
                if (reactor is null)
                {
                    reactor.Should().NotBeNull("the GL initialization reactable subscription failed.  Reactor is null.");
                }

                this.glInitReactor = reactor;
            });

        this.mockBatchSizeReactable = new Mock<IReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>((reactor) =>
            {
                if (reactor is null)
                {
                    reactor.Should().NotBeNull("the GL initialization reactable subscription failed.  Reactor is null.");
                }

                this.batchSizeReactor = reactor;
            });

        this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullBatchSizeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineGPUBuffer(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockGLInitReactable.Object,
                null,
                this.mockShutDownReactable.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'batchSizeReactable')");
    }

    [Fact]
    public void Ctor_WhenBatchSizeReactableEndsNotifications_UnsubscriberInvoked()
    {
        // Arrange
        var mockUnsubscriber = new Mock<IDisposable>();

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>((reactor) =>
            {
                if (reactor is null)
                {
                    Assert.True(false, "GL initialization reactable subscription failed.  Reactor is null.");
                }

                this.batchSizeReactor = reactor;
            })
            .Returns<IReactor<BatchSizeData>>(_ => mockUnsubscriber.Object);

        _ = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnCompleted();
        this.batchSizeReactor.OnCompleted();

        // Assert
        mockUnsubscriber.Verify(m => m.Dispose(), Times.Once);
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

        var sut = CreateSystemUnderTest();

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
        this.batchSizeReactor.OnNext(new BatchSizeData(BatchSize));

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
        this.batchSizeReactor.OnNext(new BatchSizeData(BatchSize));

        // Act
        var actual = sut.GenerateIndices();

        // Assert
        Assert.Equal(10u, sut.BatchSize);
        Assert.Equal(expected, actual);
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
            this.mockGLInitReactable.Object,
            this.mockBatchSizeReactable.Object,
            this.mockShutDownReactable.Object)
        {
            ViewPortSize = new SizeU { Width = ViewPortWidth, Height = ViewPortHeight },
        };

        if (initialize)
        {
            this.glInitReactor.OnNext(default);
        }

        return result;
    }
}
