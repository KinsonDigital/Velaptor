// <copyright file="GpuDataTypeExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CS8524

namespace VelaptorTests.ExtensionMethods;

using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Shouldly;
using Velaptor.ExtensionMethods;
using Velaptor.OpenGL;
using Velaptor.OpenGL.GpuData;
using Xunit;

/// <summary>
/// Tests the extension methods for the <see cref="ShapeGpuData"/> and <see cref="LineGpuData"/> types.
/// </summary>
public class GpuDataTypeExtensionsTests
{
    [Fact]
    public void SetVertexPos_WithRectGpuDataAndInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetVertexPos(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetVertexPos_WhenInvokedWithRectGpuData_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.VertexPos.ShouldBe(new Vector2(1111f, 2222f));
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetVertexPos_WithInvalidVertexNumber_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = new LineGpuData(
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty));

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetVertexPos(Vector2.Zero, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Fact]
    public void SetRectangle_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetRectangle(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetRectangle_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(new Vector4(1111f, 2222f, 3333f, 4444f));
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetRectangle_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        var expected = new Vector4(111, 222, 333, 444);

        // Act
        var actual = gpuData.SetRectangle(new Vector4(111, 222, 333, 444));

        // Assert
        actual.Vertex1.BoundingBox.ShouldBe(expected);
        actual.Vertex2.BoundingBox.ShouldBe(expected);
        actual.Vertex3.BoundingBox.ShouldBe(expected);
        actual.Vertex4.BoundingBox.ShouldBe(expected);
    }

    [Fact]
    public void SetAsSolid_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetAsSolid(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetAsSolid_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetAsSolid(true, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetAsSolid(true, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetAsSolid(true, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetAsSolid(true, vertexNumber).Vertex4,
        };

        // Assert
        actual.IsSolid.ShouldBeTrue();
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetAsSolid_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var actual = gpuData.SetAsSolid(true);

        // Assert
        actual.Vertex1.IsSolid.ShouldBeTrue();
        actual.Vertex2.IsSolid.ShouldBeTrue();
        actual.Vertex3.IsSolid.ShouldBeTrue();
        actual.Vertex4.IsSolid.ShouldBeTrue();
    }

    [Fact]
    public void SetBorderThickness_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetBorderThickness(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBorderThickness_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBorderThickness(123f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBorderThickness(123f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBorderThickness(123f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBorderThickness(123f, vertexNumber).Vertex4,
        };

        // Assert
        actual.BorderThickness.ShouldBe(123f);
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.IsSolid.ShouldBe(expectedVertex.IsSolid);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBorderThickness_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBorderThickness(123f);

        // Assert
        actual.Vertex1.BorderThickness.ShouldBe(expected);
        actual.Vertex2.BorderThickness.ShouldBe(expected);
        actual.Vertex3.BorderThickness.ShouldBe(expected);
        actual.Vertex4.BorderThickness.ShouldBe(expected);
    }

    [Fact]
    public void SetTopLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetTopLeftCornerRadius(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetTopLeftCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.TopLeftCornerRadius.ShouldBe(1234f);
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetTopLeftCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetTopLeftCornerRadius(123f);

        // Assert
        actual.Vertex1.TopLeftCornerRadius.ShouldBe(expected);
        actual.Vertex2.TopLeftCornerRadius.ShouldBe(expected);
        actual.Vertex3.TopLeftCornerRadius.ShouldBe(expected);
        actual.Vertex4.TopLeftCornerRadius.ShouldBe(expected);
    }

    [Fact]
    public void SetBottomLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetBottomLeftCornerRadius(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBottomLeftCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.BottomLeftCornerRadius.ShouldBe(1234f);
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBottomLeftCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBottomLeftCornerRadius(123f);

        // Assert
        actual.Vertex1.BottomLeftCornerRadius.ShouldBe(expected);
        actual.Vertex2.BottomLeftCornerRadius.ShouldBe(expected);
        actual.Vertex3.BottomLeftCornerRadius.ShouldBe(expected);
        actual.Vertex4.BottomLeftCornerRadius.ShouldBe(expected);
    }

    [Fact]
    public void SetBottomRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetBottomRightCornerRadius(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBottomRightCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.BottomRightCornerRadius.ShouldBe(1234f);
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBottomRightCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBottomRightCornerRadius(123f);

        // Assert
        actual.Vertex1.BottomRightCornerRadius.ShouldBe(expected);
        actual.Vertex2.BottomRightCornerRadius.ShouldBe(expected);
        actual.Vertex3.BottomRightCornerRadius.ShouldBe(expected);
        actual.Vertex4.BottomRightCornerRadius.ShouldBe(expected);
    }

    [Fact]
    public void SetTopRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetTopRightCornerRadius(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetTopRightCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.TopRightCornerRadius.ShouldBe(1234f);
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(expectedVertex.Color);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
    }

    [Fact]
    public void SetTopRightCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetTopRightCornerRadius(123f);

        // Assert
        actual.Vertex1.TopRightCornerRadius.ShouldBe(expected);
        actual.Vertex2.TopRightCornerRadius.ShouldBe(expected);
        actual.Vertex3.TopRightCornerRadius.ShouldBe(expected);
        actual.Vertex4.TopRightCornerRadius.ShouldBe(expected);
    }

    [Fact]
    public void SetColor_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'vertexNumber' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(VertexNumber)}'. (Parameter 'vertexNumber')";

        var gpuData = GenerateGpuDataInSequence(0);

        // Act && Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(() => gpuData.SetColor(default, (VertexNumber)invalidValue));
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetColor_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGpuDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetColor(Color.Blue, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetColor(Color.Blue, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetColor(Color.Blue, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetColor(Color.Blue, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.ShouldBeFalse();
        actual.VertexPos.ShouldBe(expectedVertex.VertexPos);
        actual.BoundingBox.ShouldBe(expectedVertex.BoundingBox);
        actual.Color.ShouldBe(Color.Blue);
        actual.BorderThickness.ShouldBe(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.ShouldBe(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.ShouldBe(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.ShouldBe(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.ShouldBe(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetColor_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);
        var expected = Color.FromArgb(220, 230, 240, 250);

        // Act
        var actual = gpuData.SetColor(Color.FromArgb(220, 230, 240, 250));

        // Assert
        actual.Vertex1.Color.ShouldBe(expected);
        actual.Vertex2.Color.ShouldBe(expected);
        actual.Vertex3.Color.ShouldBe(expected);
        actual.Vertex4.Color.ShouldBe(expected);
    }

    [Fact]
    public void SetColor_WhenSettingLineGpuData_SetsColorToAllVertexData()
    {
        // Arrange
        var data = new LineGpuData(
            new LineVertexData(Vector2.Zero, Color.White),
            new LineVertexData(Vector2.Zero, Color.White),
            new LineVertexData(Vector2.Zero, Color.White),
            new LineVertexData(Vector2.Zero, Color.White));

        // Act
        var actual = data.SetColor(Color.CornflowerBlue);

        // Assert
        actual.Vertex1.Color.ShouldBe(Color.CornflowerBlue);
        actual.Vertex2.Color.ShouldBe(Color.CornflowerBlue);
        actual.Vertex3.Color.ShouldBe(Color.CornflowerBlue);
        actual.Vertex4.Color.ShouldBe(Color.CornflowerBlue);
    }

    [Theory]
    [InlineData(VertexNumber.One)]
    [InlineData(VertexNumber.Two)]
    [InlineData(VertexNumber.Three)]
    [InlineData(VertexNumber.Four)]
    internal void SetVertexPos_WhenInvokedWithLineGpuData_ReturnsCorrectResult(VertexNumber vertexNumber)
    {
        // Arrange
        var expectedPos = new Vector2(10, 20);

        var gpuData = new LineGpuData(
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty));

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.One).Vertex1,
            VertexNumber.Two => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Two).Vertex2,
            VertexNumber.Three => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Three).Vertex3,
            VertexNumber.Four => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Four).Vertex4,
        };

        // Assert
        actual.VertexPos.ShouldBeEquivalentTo(expectedPos);
    }

    /// <summary>
    /// Generates GPU data with sequential, numerical values throughout
    /// the struct, starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment.</param>
    /// <returns>The GPU data to test.</returns>
    private static ShapeGpuData GenerateGpuDataInSequence(int startValue)
    {
        var vertex1 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex2 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex3 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex4 = GenerateVertexDataInSequence(startValue);

        return new ShapeGpuData(vertex1, vertex2, vertex3, vertex4);
    }

    /// <summary>
    /// Generates vertex data with numerical values sequentially throughout
    /// the struct starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment.</param>
    /// <returns>The vertex data to test.</returns>
    private static ShapeVertexData GenerateVertexDataInSequence(int startValue)
    {
        return new ShapeVertexData(
            new Vector2(startValue + 1f, startValue + 2f),
            new Vector4(startValue + 3, startValue + 4, startValue + 5, startValue + 6),
            Color.FromArgb(startValue + 7, startValue + 8, startValue + 9, startValue + 10),
            false,
            startValue + 7f,
            startValue + 8f,
            startValue + 9f,
            startValue + 10f,
            startValue + 11f);
    }
}
