// <copyright file="ShapeGPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
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
/// Tests the <see cref="ShapeGPUBuffer"/> class.
/// </summary>
public class ShapeGPUBufferTests
{
    private const uint VAO = 123u;
    private const uint VBO = 456u;
    private const uint EBO = 789u;
    private const uint ViewPortWidth = 100u;
    private const uint ViewPortHeight = 200u;
    private const string BufferName = "Rectangle";
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReceiveReactor? glInitReactor;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;
    private IReceiveReactor<ViewPortSizeData>? viewPortSizeReactor;
    private IReceiveReactor? shutDownReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGPUBufferTests"/> class.
    /// </summary>
    public ShapeGPUBufferTests()
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
        var mockViewPortSizeUnsubscriber = new Mock<IDisposable>();
        var mockShutDownUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
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
                else
                {
                    Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                }
            })
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId || reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            });

        var mockViewPortReactable = new Mock<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<ViewPortSizeData>>()))
            .Callback<IReceiveReactor<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.viewPortSizeReactor = reactor;
            })
            .Returns<IReceiveReactor<ViewPortSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                return mockViewPortSizeUnsubscriber.Object;
            });

        var mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.batchSizeReactor = reactor;
            })
            .Returns<IReceiveReactor<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                return this.mockBatchSizeUnsubscriber.Object;
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
            _ = new TextureGPUBuffer(
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
    public void UploadVertexData_WhenInvoked_BeginsAndEndsGLDebugGroup()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the '{nameof(GPUBufferBase<RectShape>.UploadVertexData)}()' method.",
            $"3 times in the private '{nameof(GPUBufferBase<RectShape>)}.Init()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var rect = default(ShapeBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 123);

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Update Rectangle - BatchItem(123)"), Times.Once);
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
            $"1 time in the private '{nameof(GPUBufferBase<RectShape>)}.Init()' method.",
            $"1 time in the '{nameof(GPUBufferBase<RectShape>.UploadVertexData)}()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var rect = default(ShapeBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        this.mockGLService.Verify(m => m.BindVBO(VBO),
            Times.AtLeastOnce,
            failMessage);

        this.mockGLService.Verify(m => m.UnbindVBO(),
            Times.AtLeastOnce,
            failMessage);
    }

    [Fact]
    public void UploadVertexData_WithSolidColor_UpdatesGPUData()
    {
        // Arrange
        const nint expectedOffset = 0;
        const uint expectedTotalBytes = 256u;
        var expectedRawData = new[]
        {
            -1.00999999f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -1.00999999f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
        };

        var rect = new ShapeBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.None,
            Color.FromArgb(14, 15, 16, 17),
            Color.FromArgb(18, 19, 20, 21));

        float[]? actualRawData = null;

        this.mockGL.Setup(m =>
                m.BufferSubData(It.IsAny<GLBufferTarget>(), It.IsAny<nint>(), It.IsAny<nuint>(), It.IsAny<float[]>()))
            .Callback<GLBufferTarget, nint, nuint, float[]>((_, _, _, data) =>
            {
                actualRawData = data;
            });

        var viewPortSizeData = new ViewPortSizeData { Width = ViewPortWidth, Height = ViewPortHeight };

        var sut = CreateSystemUnderTest();

        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        this.mockGL.Verify(m =>
            m.BufferSubData(GLBufferTarget.ArrayBuffer, expectedOffset, expectedTotalBytes, It.IsAny<float[]>()), Times.Once);

        actualRawData.Should().BeEquivalentTo(expectedRawData);
    }

    [Fact]
    public void UploadVertexData_WithInvalidColorGradientValue_ThrowsException()
    {
        // Arrange
        var rect = BatchItemFactory.CreateRectItemWithOrderedValues(gradientType: (ColorGradient)1234);

        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
        {
            sut.UploadVertexData(rect, 0);
        }, "The gradient type is invalid. (Parameter 'GradientType')");
    }

    [Fact]
    public void UploadVertexData_WithSolidColor_UpdatesGPUDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expectedRawData = new float[]
        {
            // Items 0-15 = Vertex 1
            /*                   Color.R    Color.G     Color.B     Color.A                 */
            0, 0, 0, 0, 0, 0,       6,         7,          8,         5,     0, 0, 0, 0, 0, 0,

            // Items 16-31 = Vertex 2
            /*                   Color.R    Color.G     Color.B     Color.A                 */
            0, 0, 0, 0, 0, 0,       6,         7,          8,         5,     0, 0, 0, 0, 0, 0,

            // Items 32-47 = Vertex 3
            /*                   Color.R    Color.G     Color.B     Color.A                 */
            0, 0, 0, 0, 0, 0,       6,         7,          8,         5,     0, 0, 0, 0, 0, 0,

            // Items 48-63 = Vertex 4
            /*                   Color.R    Color.G     Color.B     Color.A                 */
            0, 0, 0, 0, 0, 0,       6,         7,          8,         5,     0, 0, 0, 0, 0, 0,
        };

        float[]? actualRawData = null;

        this.mockGL.Setup(m =>
                m.BufferSubData(It.IsAny<GLBufferTarget>(), It.IsAny<nint>(), It.IsAny<nuint>(), It.IsAny<float[]>()))
            .Callback<GLBufferTarget, nint, nuint, float[]>((_, _, _, data) =>
            {
                actualRawData = data;
            });

#pragma warning disable SA1117
        var rect = new ShapeBatchItem(
            position: new Vector2(1, 2),
            width: 3,
            height: 4,
            color: Color.FromArgb(5, 6, 7, 8),
            isSolid: true,
            borderThickness: 9,
            cornerRadius: new CornerRadius(10, 11, 12, 13),
            gradientType: ColorGradient.None,
            gradientStart: Color.FromArgb(14, 15, 16, 17),
            gradientStop: Color.FromArgb(17, 18, 19, 20));
#pragma warning restore SA1117

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 6, 9); // Vertex 1 Color
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 22, 26); // Vertex 2 Color
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 38, 42); // Vertex 3 Color
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 54, 58); // Vertex 4 Color
    }

    [Fact]
    public void UploadVertexData_WithHorizontalColorGradient_UpdatesGPUDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expectedRawData = new float[]
        {
            // Items 0-15 = Vertex 1
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       15,       16,          17,         14,     0, 0, 0, 0, 0, 0,

            // Items 16-31 = Vertex 2
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       15,       16,          17,         14,     0, 0, 0, 0, 0, 0,

            // Items 32-47 = Vertex 3
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       19,       20,          21,         18,     0, 0, 0, 0, 0, 0,

            // Items 48-63 = Vertex 4
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       19,       20,          21,         18,     0, 0, 0, 0, 0, 0,
        };

        float[]? actualRawData = null;

        this.mockGL.Setup(m =>
                m.BufferSubData(It.IsAny<GLBufferTarget>(), It.IsAny<nint>(), It.IsAny<nuint>(), It.IsAny<float[]>()))
            .Callback<GLBufferTarget, nint, nuint, float[]>((_, _, _, data) =>
            {
                actualRawData = data;
            });

#pragma warning disable SA1117
        var rect = new ShapeBatchItem(
            position: new Vector2(1, 2),
            width: 3,
            height: 4,
            color: Color.FromArgb(5, 6, 7, 8),
            isSolid: true,
            borderThickness: 9,
            cornerRadius: new CornerRadius(10, 11, 12, 13),
            gradientType: ColorGradient.Horizontal,
            gradientStart: Color.FromArgb(14, 15, 16, 17),
            gradientStop: Color.FromArgb(18, 19, 20, 21));
#pragma warning restore SA1117

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 6, 9); // Vertex 1 Color | Grad Start
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 22, 26); // Vertex 2 Color | Grad Start
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 38, 42); // Vertex 3 Color | Grad Stop
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 54, 58); // Vertex 4 Color | Grad Stop
    }

    [Fact]
    public void UploadVertexData_WithVerticalColorGradient_UpdatesGPUDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expectedRawData = new float[]
        {
            // Items 0-15 = Vertex 1
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       15,       16,          17,         14,     0, 0, 0, 0, 0, 0,

            // Items 16-31 = Vertex 2
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       19,       20,          21,         18,     0, 0, 0, 0, 0, 0,

            // Items 32-47 = Vertex 3
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       15,       16,          17,         14,     0, 0, 0, 0, 0, 0,

            // Items 48-63 = Vertex 4
            /*                   Color.R    Color.G     Color.B     Color.A                   */
            0, 0, 0, 0, 0, 0,       19,       20,          21,         18,     0, 0, 0, 0, 0, 0,
        };

        float[]? actualRawData = null;

        this.mockGL.Setup(m =>
                m.BufferSubData(It.IsAny<GLBufferTarget>(), It.IsAny<nint>(), It.IsAny<nuint>(), It.IsAny<float[]>()))
            .Callback<GLBufferTarget, nint, nuint, float[]>((_, _, _, data) =>
            {
                actualRawData = data;
            });

        var rect = BatchItemFactory.CreateRectItemWithOrderedValues();

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 6, 9); // Vertex 1 Color | Grad Start
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 22, 26); // Vertex 2 Color | Grad Stop
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 38, 42); // Vertex 3 Color | Grad Start
        AssertExtensions.SectionEquals(expectedRawData, actualRawData, 54, 58); // Vertex 4 Color | Grad Stop
    }

    [Fact]
    public void PrepareForUpload_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest(false);

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<BufferNotInitializedException>(() =>
        {
            sut.PrepareForUpload();
        }, "The rectangle buffer has not been initialized.");
    }

    [Fact]
    public void PrepareForUpload_WhenInitialized_BindsVAO()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the '{nameof(ShapeGPUBuffer.PrepareForUpload)}()' method.",
            $"1 time in the '{nameof(GPUBufferBase<RectShape>)}.Init()' method.",
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
        var expected = TestDataLoader
            .LoadTestData<float[]>(string.Empty,
                $"{nameof(ShapeGPUBufferTests)}.{nameof(GenerateData_WhenInvoked_ReturnsCorrectResult)}.json");
        var sut = CreateSystemUnderTest(false);

        // Act
        var actual = sut.GenerateData();

        // Assert
        sut.BatchSize.Should().Be(100u);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetupVAO_WhenInvoked_SetsUpTheOpenGLVertexArrayObject()
    {
        // Arrange
        var paramData = new (uint index, int size, bool normalized, uint stride, uint offset, string label)[]
        {
            (0u, 2, false, 64u, 0u, "VertexPosition"),
            (1u, 4, false, 64u, 8u, "Rectangle"),
            (2u, 4, false, 64u, 24u, "Color"),
            (3u, 1, false, 64u, 40u, "IsSolid"),
            (4u, 1, false, 64u, 44u, "BorderThickness"),
            (5u, 1, false, 64u, 48u, "TopLeftRadius"),
            (6u, 1, false, 64u, 52u, "BottomLeftRadius"),
            (7u, 1, false, 64u, 56u, "BottomRightRadius"),
            (8u, 1, false, 64u, 60u, "TopRightRadius"),
        };
        var enableVertexAttribArrayParamData = new (uint index, string label)[]
        {
            (0u, "VertexPosition"),
            (1u, "Rectangle"),
            (2u, "Color"),
            (3u, "IsSolid"),
            (4u, "BorderThickness"),
            (5u, "TopLeftRadius"),
            (6u, "BottomLeftRadius"),
            (7u, "BottomRightRadius"),
            (8u, "TopRightRadius"),
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
        Assert.Equal(100u, sut.BatchSize);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void BatchSizeReactable_WhenSubscribing_UsesCorrectReactorName()
    {
        // Arrange
        var mockReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(Act);

        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable())
            .Returns(mockReactable.Object);

        _ = CreateSystemUnderTest();

        // Act & Assert
        void Act(IReactor reactor)
        {
            reactor.Should().NotBeNull("it is required for this unit test.");
            reactor.Name.Should().Be("ShapeGPUBufferTests.Ctor - BatchSizeChangedId");
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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Rect  });

        // Assert
        sut.BatchSize.Should().Be(123);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotificationWhenAlreadyInitialized_UpdatesBatchSizeAndResizesBatchDataOnGPU()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Rect });

        // Assert
        sut.BatchSize.Should().Be(123);

        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Vertex Data"), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.EndGroup(), Times.AtLeast(2));
        this.mockGLService.Verify(m => m.BeginGroup($"Set size of {BufferName} Indices Data"), Times.AtLeastOnce);
    }

    [Fact]
    public void ShutDownReactable_WhenReceivingNotification_ShutsDownShader()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnReceive();
        this.shutDownReactor.OnReceive();

        // Assert
        this.mockBatchSizeUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates the expected indices test data.
    /// </summary>
    /// <returns>The data to use for unit testing.</returns>
    private static uint[] CreateExpectedIndicesData(uint batchSize)
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
    /// Creates a new instance of <see cref="ShapeGPUBuffer"/> class for the purpose of testing.
    /// </summary>
    /// <param name="initialize">If true, will mock the initialization of the mocked sut.</param>
    /// <returns>The instance to test.</returns>
    private ShapeGPUBuffer CreateSystemUnderTest(bool initialize = true)
    {
        var result = new ShapeGPUBuffer(
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
