// <copyright file="LineGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable SeparateLocalFunctionsWithJumpStatement
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
using NSubstitute;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
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
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuBufferTests"/> class.
    /// </summary>
    public LineGpuBufferTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGL.GenVertexArray().Returns(VAO);
        this.mockGL.GenBuffer().Returns(_ =>
            {
                if (this.vboGenerated)
                {
                    return EBO;
                }

                this.vboGenerated = true;
                return VBO;
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
            _ = new LineGpuBuffer(
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
        var sut = CreateSystemUnderTest(false);

        // Act
        var act = () => sut.UploadVertexData(default, 0);

        // Assert
        act.Should().Throw<BufferNotInitializedException>("The line buffer has not been initialized.");
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_BeginsAndEndsGLDebugGroup()
    {
        // Arrange
        var line = default(LineBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(line, 123);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Update Line - BatchItem(123)");
        this.mockGLService.Received(4).EndGroup();
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_BindsAndUnbindsVBO()
    {
        // Arrange
        var line = default(LineBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(line, 0);

        // Assert
        this.mockGLService.Received().BindVBO(VBO);
        this.mockGLService.Received().UnbindVBO();
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_UploadsGpuBufferData()
    {
        // Arrange
        var expectedData = new[]
        {
            // ReSharper disable MultipleSpaces
            // Vertex 1
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.923431456f, 1.00828433f, 5f, 6f, 7f, 4f,

            // Vertex 2
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -1.03656852f, 0.951715708f, 5f, 6f, 7f, 4f,

            // Vertex 3
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.883431435f, 0.98828429f, 5f, 6f, 7f, 4f,

            // Vertex 4
            // Vertex X     Vert Pos Y       Red,    Green,  Blue,   Alpha
            -0.996568561f, 0.931715727f, 5f, 6f, 7f, 4f,

            // ReSharper restore MultipleSpaces
        };

        var actual = Array.Empty<float>();

        this.mockGL.When(x =>
            x.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

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
        this.mockGL.Received(1).BufferSubData(GLBufferTarget.ArrayBuffer, 0x3c0, 96, Arg.Any<float[]>());
        actual.Should().BeEquivalentTo(expectedData);
        this.mockGLService.Received(2).UnbindVBO();
        this.mockGLService.Received(4).EndGroup();
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
        var sut = CreateSystemUnderTest();

        // Act
        sut.PrepareForUpload();

        // Assert
        this.mockGLService.Received(3).BindVAO(VAO);
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
            (0u, 2, false, 24u, 0u, "VertexPosition"), (1u, 4, false, 24u, 8u, "Color"),
        };
        var enableVertexAttribArrayParamData = new (uint index, string label)[] { (0u, "VertexPosition"), (1u, "Color"), };

        var sut = CreateSystemUnderTest(false);

        // Act
        sut.SetupVAO();

        // Assert
        Assert.All(
            paramData,
            data =>
            {
                this.mockGL.Received(1).VertexAttribPointer(
                    data.index,
                    data.size,
                    GLVertexAttribPointerType.Float,
                    data.normalized,
                    data.stride,
                    data.offset);
            });

        Assert.All(
            enableVertexAttribArrayParamData,
            data =>
            {
                var (index, _) = data;
                this.mockGL.Received(1).EnableVertexAttribArray(index);
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
        var mockReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        mockReactable.When(x =>
            x.Subscribe(Arg.Do<IReceiveSubscription<BatchSizeData>>(Act)));

        this.mockReactableFactory.CreateBatchSizeReactable().Returns(mockReactable);

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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Line });

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

        this.mockGLService.Received().BeginGroup($"Set size of {BufferName} Vertex Data");
        this.mockGLService.Received(8).EndGroup();
        this.mockGLService.Received().BeginGroup($"Set size of {BufferName} Indices Data");
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

            result.AddRange(new[] { maxIndex, maxIndex + 1u, maxIndex + 2u, maxIndex + 2u, maxIndex + 1u, maxIndex + 3u, });
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
        var result = new LineGpuBuffer(this.mockGL, this.mockGLService, this.mockReactableFactory);

        if (initialize)
        {
            this.glInitReactor.OnReceive();
        }

        return result;
    }
}
