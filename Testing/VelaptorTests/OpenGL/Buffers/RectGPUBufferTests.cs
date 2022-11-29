// <copyright file="RectGPUBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Moq;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Exceptions;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="RectGPUBuffer"/> class.
/// </summary>
public class RectGPUBufferTests
{
    private const uint VAO = 123u;
    private const uint VBO = 456u;
    private const uint EBO = 789u;
    private const uint ViewPortWidth = 100u;
    private const uint ViewPortHeight = 200u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
    private readonly Mock<IReactable<ShutDownData>> mockShutdownReactable;
    private IReactor<GLInitData>? glInitReactor;
    private bool vboGenerated;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectGPUBufferTests"/> class.
    /// </summary>
    public RectGPUBufferTests()
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
                    Assert.True(false, "GL initialization reactable subscription failed.  Reactor is null.");
                }

                this.glInitReactor = reactor;
            });

        this.mockShutdownReactable = new Mock<IReactable<ShutDownData>>();
    }

    #region Method Tests
    [Fact]
    public void UploadVertexData_WhenInvoked_BeginsAndEndsGLDebugGroup()
    {
        // Arrange
        var executionLocations = new List<string>
        {
            $"1 time in the '{nameof(GPUBufferBase<RectShape>.UploadVertexData)}()' method.",
            "3 times in the private '{nameof(GPUBufferBase<RectShape>)}.Init()' method.",
        };
        var failMessage = string.Join(Environment.NewLine, executionLocations);

        var rect = default(RectBatchItem);

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
        var failMessage = string.Join("{Environment.NewLine}", executionLocations);

        var rect = default(RectBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        this.mockGLService.Verify(m => m.BindVBO(VBO),
            Times.Exactly(2),
            failMessage);

        this.mockGLService.Verify(m => m.UnbindVBO(),
            Times.Exactly(2),
            failMessage);
    }

    [Fact]
    public void UploadVertexData_WithSolidColor_UpdatesGPUData()
    {
        // Arrange
        nint expectedOffset = 0;
        const uint expectedTotalBytes = 256u;
        var expectedRawData = new[]
        {
            -1.00999999f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -1.00999999f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 1f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
            -0.949999988f, 0.959999979f, 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f,
        };

        var arrayRegions = CreateArrayRegions(16, 4);

        var rect = new RectBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.None,
            Color.FromArgb(14, 15, 16, 17),
            Color.FromArgb(18, 19, 20, 21),
            0);

        float[]? actualRawData = null;

        this.mockGL.Setup(m =>
                m.BufferSubData(It.IsAny<GLBufferTarget>(), It.IsAny<nint>(), It.IsAny<nuint>(), It.IsAny<float[]>()))
            .Callback<GLBufferTarget, nint, nuint, float[]>((_, _, _, data) =>
            {
                actualRawData = data;
            });

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadVertexData(rect, 0);

        // Assert
        this.mockGL.Verify(m =>
            m.BufferSubData(GLBufferTarget.ArrayBuffer, expectedOffset, expectedTotalBytes, It.IsAny<float[]>()), Times.Once);

        AssertExtensions.ItemsEqual(expectedRawData, actualRawData, arrayRegions);
    }

    [Fact]
    public void UploadVertexData_WithInvalidColorGradientValue_ThrowsException()
    {
        // Arrange
        var rect = new RectBatchItem(gradientType: (ColorGradient)1234);

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
        var rect = new RectBatchItem(
            position: new Vector2(1, 2),
            width: 3,
            height: 4,
            color: Color.FromArgb(5, 6, 7, 8),
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
        var rect = new RectBatchItem(
            position: new Vector2(1, 2),
            width: 3,
            height: 4,
            color: Color.FromArgb(5, 6, 7, 8),
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

#pragma warning disable SA1117
        var rect = new RectBatchItem(
            position: new Vector2(1, 2),
            width: 3,
            height: 4,
            color: Color.FromArgb(5, 6, 7, 8),
            borderThickness: 9,
            cornerRadius: new CornerRadius(10, 11, 12, 13),
            gradientType: ColorGradient.Vertical,
            gradientStart: Color.FromArgb(14, 15, 16, 17),
            gradientStop: Color.FromArgb(18, 19, 20, 21));
#pragma warning restore SA1117

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
            $"1 time in the '{nameof(RectGPUBuffer.PrepareForUpload)}()' method.",
            $"1 time in the '{nameof(GPUBufferBase<RectShape>)}.Init()' method.",
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
            .LoadTestData<float>(string.Empty, $"{nameof(GenerateData_WhenInvoked_ReturnsCorrectResult)}_TestData.json");
        var sut = CreateSystemUnderTest(false);

        // Act
        var actual = sut.GenerateData();

        // Assert
        Assert.Equal(1000u, sut.BatchSize);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetupVAO_WhenInvoked_SetsUpTheOpenGLVertexArrayObject()
    {
        // Arrange
        var paramData = new (uint Index, int size, bool normalized, uint stride, uint offset, string label)[]
        {
            (0u, 2, false, 64u, 0u, "VertexPosition"),
            (1u, 4, false, 64u, 8u, "Rectangle"),
            (2u, 4, false, 64u, 24u, "Color"),
            (3u, 1, false, 64u, 40u, "IsFilled"),
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
            (3u, "IsFilled"),
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
                    m.VertexAttribPointer(data.Index,
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
            .LoadTestData<uint>(string.Empty, $"{nameof(GenerateIndices_WhenInvoked_ReturnsCorrectResult)}_TestData.json");
        var sut = CreateSystemUnderTest(false);

        // Act
        var actual = sut.GenerateIndices();

        // Assert
        Assert.Equal(1000u, sut.BatchSize);
        Assert.Equal(expected, actual);
    }
    #endregion

    /// <summary>
    /// Creates a list of items that describe the different regions of an array of data.
    /// </summary>
    /// <param name="dataStride">The stride of a single section of data.</param>
    /// <param name="totalSections">The total number of sections.</param>
    /// <returns>The data that describes all of the sections of the data.</returns>
    private static IEnumerable<(int ThreadStart, int stop, string name)> CreateArrayRegions(int dataStride, int totalSections)
    {
        var result = new List<(int ThreadStart, int stop, string name)>();

        for (var i = 0; i < totalSections; i++)
        {
            var offset = i * dataStride;
            result.AddRange(new[]
            {
                (offset + 0, offset + 1, $"Vertex {i} Position"),
                (offset + 2, offset + 5, $"Vertex {i} Rectangle"),
                (offset + 6, offset + 9, $"Vertex {i} Color"),
                (offset + 10, offset + 10, $"Vertex {i} Is Filled"),
                (offset + 11, offset + 11, $"Vertex {i} Border Thickness"),
                (offset + 12, offset + 12, $"Vertex {i} Top Left Radius"),
                (offset + 13, offset + 13, $"Vertex {i} Bottom Left Radius"),
                (offset + 14, offset + 14, $"Vertex {i} Bottom Right Radius"),
                (offset + 15, offset + 15, $"Vertex {i} Top Right Radius"),
            });
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a new instance of <see cref="RectGPUBuffer"/> class for the purpose of testing.
    /// </summary>
    /// <param name="initialize">If true, will mock the initialization of the mocked sut.</param>
    /// <returns>The instance to test.</returns>
    private RectGPUBuffer CreateSystemUnderTest(bool initialize = true)
    {
        var result = new RectGPUBuffer(
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockGLInitReactable.Object,
            this.mockShutdownReactable.Object)
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
