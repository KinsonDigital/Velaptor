// <copyright file="ShapeGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
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
using Color = System.Drawing.Color;

/// <summary>
/// Tests the <see cref="ShapeGpuBuffer"/> class.
/// </summary>
public class ShapeGpuBufferTests
{
    private const uint VAO = 123u;
    private const uint VBO = 456u;
    private const uint EBO = 789u;
    private const uint ViewPortWidth = 100u;
    private const uint ViewPortHeight = 200u;
    private const string BufferName = "Shape";
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSizeReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuBufferTests"/> class.
    /// </summary>
    public ShapeGpuBufferTests()
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
    public void UploadVertexData_WhenInvoked_BeginsAndEndsGLDebugGroup()
    {
        // Arrange
        var shape = default(ShapeBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(shape, 123);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Update Shape - BatchItem(123)");
        this.mockGLService.Received(4).EndGroup();
    }

    [Fact]
    public void UploadVertexData_WhenInvoked_BindsAndUnbindsVBO()
    {
        // Arrange
        var shape = default(ShapeBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(shape, 0);

        // Assert
        this.mockGLService.Received(3).BindVBO(VBO);
        this.mockGLService.Received(2).UnbindVBO();
    }

    [Fact]
    public void UploadVertexData_WithSolidColor_UpdatesGpuData()
    {
        // Arrange
        const nint expectedOffset = 0;
        const uint expectedTotalBytes = 256u;
        var expected = new[]
        {
            -1.00999999f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -1.00999999f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
        };

        var shape = new ShapeBatchItem(
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

        var actual = Array.Empty<float>();

        this.mockGL.When(m =>
                m.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

        var viewPortSizeData = new ViewPortSizeData { Width = ViewPortWidth, Height = ViewPortHeight };

        var sut = CreateSystemUnderTest();

        this.viewPortSizeReactor.OnReceive(viewPortSizeData);

        // Act
        sut.UploadVertexData(shape, 0);

        // Assert
        this.mockGL.Received(1).BufferSubData(GLBufferTarget.ArrayBuffer, expectedOffset, expectedTotalBytes, Arg.Any<float[]>());
        actual.Should().BeEquivalentTo(expected);
        this.mockGLService.Received(2).UnbindVBO();
        this.mockGLService.Received(4).EndGroup();
    }

    [Fact]
    public void UploadVertexData_WithInvalidColorGradientValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'shape.{nameof(ShapeBatchItem.GradientType)}' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(ColorGradient)}'. (Parameter 'shape.{nameof(ShapeBatchItem.GradientType)}')";

        var shape = BatchItemFactory.CreateShapeItemWithOrderedValues(gradientType: (ColorGradient)invalidValue);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadVertexData(shape, 0);

        // Assert
        act.Should().Throw<InvalidEnumArgumentException>()
           .WithMessage(expected);
    }

    [Fact]
    public void UploadVertexData_WithSolidColor_UpdatesGpuDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expected = new float[]
        {
            // ReSharper disable MultipleSpaces
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
            // ReSharper restore MultipleSpaces
        };

        var actual = Array.Empty<float>();

        this.mockGL.When(m =>
                m.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

#pragma warning disable SA1117
        var shape = new ShapeBatchItem(
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
        sut.UploadVertexData(shape, 0);

        // Assert
        AssertExtensions.SectionEquals(expected, actual, 6, 9); // Vertex 1 Color
        AssertExtensions.SectionEquals(expected, actual, 22, 26); // Vertex 2 Color
        AssertExtensions.SectionEquals(expected, actual, 38, 42); // Vertex 3 Color
        AssertExtensions.SectionEquals(expected, actual, 54, 58); // Vertex 4 Color
    }

    [Fact]
    public void UploadVertexData_WithHorizontalColorGradient_UpdatesGpuDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expected = new float[]
        {
            // ReSharper disable MultipleSpaces
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
            // ReSharper restore MultipleSpaces
        };

        var actual = Array.Empty<float>();

        this.mockGL.When(m =>
                m.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

#pragma warning disable SA1117
        var shape = new ShapeBatchItem(
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
        sut.UploadVertexData(shape, 0);

        // Assert
        AssertExtensions.SectionEquals(expected, actual, 6, 9); // Vertex 1 Color | Grad Start
        AssertExtensions.SectionEquals(expected, actual, 22, 26); // Vertex 2 Color | Grad Start
        AssertExtensions.SectionEquals(expected, actual, 38, 42); // Vertex 3 Color | Grad Stop
        AssertExtensions.SectionEquals(expected, actual, 54, 58); // Vertex 4 Color | Grad Stop
    }

    [Fact]
    public void UploadVertexData_WithVerticalColorGradient_UpdatesGpuDataWithCorrectColor()
    {
        // Arrange
        // We only care about the section of raw data where the color is located.

        /* Color Locations
         * Vertex 1 Color: index 6-9
         * Vertex 2 Color: index 22-26
         * Vertex 3 Color: index 38-42
         * Vertex 4 Color: index 54-58
         */
        var expected = new float[]
        {
            // ReSharper disable MultipleSpaces
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
            // ReSharper restore MultipleSpaces
        };

        var actual = Array.Empty<float>();

        this.mockGL.When(m =>
                m.BufferSubData(Arg.Any<GLBufferTarget>(), Arg.Any<nint>(), Arg.Any<nuint>(), Arg.Any<float[]>()))
            .Do(callInfo => actual = callInfo.Arg<float[]>());

        var shape = BatchItemFactory.CreateShapeItemWithOrderedValues();

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(shape, 0);

        // Assert
        AssertExtensions.SectionEquals(expected, actual, 6, 9); // Vertex 1 Color | Grad Start
        AssertExtensions.SectionEquals(expected, actual, 22, 26); // Vertex 2 Color | Grad Stop
        AssertExtensions.SectionEquals(expected, actual, 38, 42); // Vertex 3 Color | Grad Start
        AssertExtensions.SectionEquals(expected, actual, 54, 58); // Vertex 4 Color | Grad Stop
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
        }, "The shape buffer has not been initialized.");
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
        var expected = TestDataLoader
            .LoadTestData<float[]>(string.Empty,
                $"{nameof(ShapeGpuBufferTests)}.{nameof(GenerateData_WhenInvoked_ReturnsCorrectResult)}.json");
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
        paramData.Should().AllSatisfy(data =>
        {
            this.mockGL.Received(1).VertexAttribPointer(data.index,
                data.size,
                GLVertexAttribPointerType.Float,
                data.normalized,
                data.stride,
                data.offset);
        });

        enableVertexAttribArrayParamData.Should().AllSatisfy(data =>
        {
            this.mockGL.Received(1).EnableVertexAttribArray(data.index);
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
            reactor.Name.Should().Be("ShapeGpuBufferTests.Ctor - BatchSizeChangedId");
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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Rect });

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
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Rect });

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
    /// Creates a new instance of <see cref="ShapeGpuBuffer"/> class for the purpose of testing.
    /// </summary>
    /// <param name="initialize">If true, will mock the initialization of the mocked sut.</param>
    /// <returns>The instance to test.</returns>
    private ShapeGpuBuffer CreateSystemUnderTest(bool initialize = true)
    {
        var result = new ShapeGpuBuffer(this.mockGL, this.mockGLService, this.mockReactableFactory);

        if (initialize)
        {
            this.glInitReactor.OnReceive();
        }

        return result;
    }
}
