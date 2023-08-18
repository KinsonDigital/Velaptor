// <copyright file="GpuDataTypeExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CS8524

namespace VelaptorTests.ExtensionMethods;

using System;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Moq;
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
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetVertexPos(It.IsAny<Vector2>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(new Vector2(1111f, 2222f));
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetVertexPos_WithInvalidVertexNumber_ThrowsException()
    {
        // Arrange
        var gpuData = new LineGpuData(
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty),
            new LineVertexData(Vector2.Zero, Color.Empty));

        // Act
        var act = () => gpuData.SetVertexPos(Vector2.Zero, (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Fact]
    public void SetRectangle_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act & Assert
        var act = () => gpuData.SetRectangle(It.IsAny<Vector4>(), (VertexNumber)1234);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(new Vector4(1111f, 2222f, 3333f, 4444f));
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.BoundingBox.Should().Be(expected);
        actual.Vertex2.BoundingBox.Should().Be(expected);
        actual.Vertex3.BoundingBox.Should().Be(expected);
        actual.Vertex4.BoundingBox.Should().Be(expected);
    }

    [Fact]
    public void SetAsSolid_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetAsSolid(It.IsAny<bool>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        actual.IsSolid.Should().BeTrue();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetAsSolid_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var actual = gpuData.SetAsSolid(true);

        // Assert
        actual.Vertex1.IsSolid.Should().BeTrue();
        actual.Vertex2.IsSolid.Should().BeTrue();
        actual.Vertex3.IsSolid.Should().BeTrue();
        actual.Vertex4.IsSolid.Should().BeTrue();
    }

    [Fact]
    public void SetBorderThickness_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetBorderThickness(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        actual.BorderThickness.Should().Be(123f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.IsSolid.Should().Be(expectedVertex.IsSolid);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.BorderThickness.Should().Be(expected);
        actual.Vertex2.BorderThickness.Should().Be(expected);
        actual.Vertex3.BorderThickness.Should().Be(expected);
        actual.Vertex4.BorderThickness.Should().Be(expected);
    }

    [Fact]
    public void SetTopLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetTopLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.TopLeftCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex2.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex3.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex4.TopLeftCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetBottomLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetBottomLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.BottomLeftCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex2.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex3.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex4.BottomLeftCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetBottomRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetBottomRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.BottomRightCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex2.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex3.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex4.BottomRightCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetTopRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetTopRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.TopRightCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
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
        actual.Vertex1.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex2.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex3.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex4.TopRightCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetColor_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGpuDataInSequence(0);

        // Act
        var act = () => gpuData.SetColor(It.IsAny<Color>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
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
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.BoundingBox.Should().Be(expectedVertex.BoundingBox);
        actual.Color.Should().Be(Color.Blue);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
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
        actual.Vertex1.Color.Should().Be(expected);
        actual.Vertex2.Color.Should().Be(expected);
        actual.Vertex3.Color.Should().Be(expected);
        actual.Vertex4.Color.Should().Be(expected);
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
        actual.Vertex1.Color.Should().Be(Color.CornflowerBlue);
        actual.Vertex2.Color.Should().Be(Color.CornflowerBlue);
        actual.Vertex3.Color.Should().Be(Color.CornflowerBlue);
        actual.Vertex4.Color.Should().Be(Color.CornflowerBlue);
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
        actual.VertexPos.Should().BeEquivalentTo(expectedPos);
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
