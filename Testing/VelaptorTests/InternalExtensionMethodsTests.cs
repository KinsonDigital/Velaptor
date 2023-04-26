// <copyright file="InternalExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable CS8524
#pragma warning disable SA1202
namespace VelaptorTests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.GPUData;
using Xunit;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectF = System.Drawing.RectangleF;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Tests the <see cref="Velaptor.InternalExtensionMethods"/> class.
/// </summary>
public class InternalExtensionMethodsTests
{
    private const char CrossPlatDirSeparatorChar = '/';

    #region Unit Test Data
    // ReSharper disable HeapView.BoxingAllocation

    /// <summary>
    /// Provides unit test data for the <see cref="InternalExtensionMethods.HasValidDriveSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> ContainsValidDriveTestData()
    {
        yield return new object[] { string.Empty, false };
        yield return new object[] { null, false };
        yield return new object[] { "windows", false };
        yield return new object[] { ":", false };
        yield return new object[] { "C", false };
        yield return new object[] { ":C", false };
        yield return new object[] { "1:", false };
        yield return new object[] { "windowsC:system32", false };
        yield return new object[] { @"C:\Windows\System32", true };
        yield return new object[] { @"C:windows", true };
        yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", true };
    }

    /// <summary>
    /// Provides unit test data for the <see cref="InternalExtensionMethods.HasValidFullDirPathSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsFullyQualifiedDirPathTestData()
    {
        yield return new object[] { string.Empty, false };
        yield return new object[] { null, false };
        yield return new object[] { @"\Windows\System32", false };
        yield return new object[] { $@"{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", false };
        yield return new object[] { "C:Windows", false };
        yield return new object[] { $@"{CrossPlatDirSeparatorChar}WindowsC:", false };
        yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32{CrossPlatDirSeparatorChar}fake-file.txt", false };
        yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", true };
        yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32{CrossPlatDirSeparatorChar}", true };
    }

    /// <summary>
    /// Provides unit test data for the <see cref="InternalExtensionMethods.HasValidUNCPathSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsUNCPathTestData()
    {
        yield return new object[] { string.Empty, false };
        yield return new object[] { null, false };
        yield return new object[] { @"\\", false };
        yield return new object[] { @"directory", false };
        yield return new object[] { $@"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}", false };
        yield return new object[] { $@"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}directory", true };
    }

    /// <summary>
    /// Gets the rectangle vertice data for the <see cref="CreateRectFromLine_WhenInvoked_ReturnsCorrectResult"/> unit test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> GetExpectedRectPointData()
    {
        // X and Y axis aligned rectangle
        yield return new object[]
        {
            new LineBatchItem(new Vector2(50f, 100f), new Vector2(200f, 100f), NETColor.White, 20),
            new Vector2(50f, 90f),
            new Vector2(200f, 90f),
            new Vector2(200f, 110f),
            new Vector2(50f, 110f),
        };

        // X and Y axis aligned rectangle rotated 45 degrees clockwise
        yield return new object[]
        {
            new LineBatchItem(new Vector2(100f, 100f), new Vector2(200f, 200f), NETColor.White, 100f),
            new Vector2(135.35535f, 64.64465f),
            new Vector2(235.35535f, 164.64467f),
            new Vector2(164.64465f, 235.35533f),
            new Vector2(64.64465f, 135.35535f),
        };
    }

    // ReSharper restore HeapView.BoxingAllocation
    #endregion

    #region Method Tests
    [Theory]
    [InlineData('x', true)]
    [InlineData('k', false)]
    public void DoesNotStartWidth_WhenCheckingForCharacters_ReturnsCorrectResult(char character, bool expected)
    {
        // Arrange
        const string stringToCheck = "kinson";

        // Act
        var actual = stringToCheck.DoesNotStartWith(character);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("digital", true)]
    [InlineData("kinson", false)]
    public void DoesNotStartWith_WhenCheckingForStrings_ReturnsCorrectResult(string stringValue, bool expected)
    {
        // Arrange
        const string stringToCheck = "kinson digital";

        // Act
        var actual = stringToCheck.DoesNotStartWith(stringValue);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData('x', true)]
    [InlineData('n', false)]
    public void DoesNotEndWith_WhenCheckingForCharacters_ReturnsCorrectResult(char character, bool expected)
    {
        // Arrange
        const string stringToCheck = "kinson";

        // Act
        var actual = stringToCheck.DoesNotEndWith(character);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("kinson", true)]
    [InlineData("digital", false)]
    public void DoesNotEndWith_WhenCheckingForStrings_ReturnsCorrectResult(string stringValue, bool expected)
    {
        // Arrange
        const string stringToCheck = "kinson digital";

        // Act
        var actual = stringToCheck.DoesNotEndWith(stringValue);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("${{TEST_VAR}}", "}}", ' ', "${{TEST_VAR}}")]
    [InlineData("${{TEST_VAR }}", "}}", ' ', "${{TEST_VAR}}")]
    [InlineData("${{TEST_VAR    }}", "}}", ' ', "${{TEST_VAR}}")]
    [InlineData("${{TEST_VAR~}}", "}}", '~', "${{TEST_VAR}}")]
    [InlineData("${{TEST_VAR~~}}", "}}", '~', "${{TEST_VAR}}")]
    public void TrimLeftOf_WhenInvoked_ReturnsCorrectResult(
        string content,
        string value,
        char trimChar,
        string expected)
    {
        // Arrange & Act
        var actual = content.TrimLeftOf(value, trimChar);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("${{TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
    [InlineData("${{ TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
    [InlineData("${{    TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
    [InlineData("${{~TEST_VAR}}", "${{", '~', "${{TEST_VAR}}")]
    [InlineData("${{~~TEST_VAR}}", "${{", '~', "${{TEST_VAR}}")]
    public void TrimRightOf_WhenInvoked_ReturnsCorrectResult(
        string content,
        string value,
        char trimChar,
        string expected)
    {
        // Arrange & Act
        var actual = content.TrimRightOf(value, trimChar);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(@"C:\", true)]
    [InlineData(@"C:/", true)]
    [InlineData(@"C:", false)]
    [InlineData(@"C\", false)]
    [InlineData(@"C/", false)]
    [InlineData(@"C:\test-file.txt", false)]
    [InlineData(@"C:/test-file.txt", false)]
    public void OnlyContainsDrive_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
    {
        // Act
        var actual = value.OnlyContainsDrive();

        // Assert
        actual.Should().Be(expected);
    }

    [TheoryForWindows]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(".txt", "")]
    [InlineData("test-dir", "test-dir")]
    [InlineData(@"C:\", "C:/")]
    [InlineData("C:/", "C:/")]
    [InlineData(@"C:\temp", "temp")]
    [InlineData("C:/temp", "temp")]
    [InlineData(@"C:\temp\", "temp")]
    [InlineData("C:/temp/", "temp")]
    [InlineData(@"C:\test-file.txt", "C:/")]
    [InlineData("C:/test-file.txt", "C:/")]
    [InlineData(@"C:\temp\test-file.txt", "temp")]
    [InlineData("C:/temp/test-file.txt", "temp")]
    [InlineData("C:/temp/extra-dir/test-file.txt", "extra-dir")]
    public void GetLastDirName_WhenRunningOnWindows_ReturnsCorrectResult(string value, string expected)
    {
        // Act
        var actual = value.GetLastDirName();

        // Assert
        actual.Should().Be(expected);
    }

    [TheoryForLinux]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(".txt", "")]
    [InlineData("test-dir", "test-dir")]
    [InlineData("/home/user-dir", "user-dir")]
    [InlineData("/home/user-dir/test-file.txt", "user-dir")]
    [InlineData("/home/test-file.text", "home")]
    [InlineData("/test-file.txt", "/")]
    [InlineData(@"\home\user-dir", "user-dir")]
    [InlineData(@"\home\user-dir\test-file.txt", "user-dir")]
    [InlineData(@"\home\test-file.text", "home")]
    [InlineData(@"\test-file.txt", "/")]
    public void GetLastDirName_WhenRunningOnLinux_ReturnsCorrectResult(string value, string expected)
    {
        // Act
        var actual = value.GetLastDirName();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(ContainsValidDriveTestData))]
    public void HasValidDriveSyntax_WhenInvoked_ReturnsCorrectResult(string dirPath, bool expected)
    {
        // Act
        var actual = dirPath.HasValidDriveSyntax();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsFullyQualifiedDirPathTestData))]
    public void HasValidFullDirPathSyntax_WhenInvoked_ReturnsCorrectResult(string dirPath, bool expected)
    {
        // Act
        var actual = dirPath.HasValidFullDirPathSyntax();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsUNCPathTestData))]
    public void HasValidUNCPathSyntax_WhenInvoked_ReturnsCorrectResult(string path, bool expected)
    {
        // Act
        var actual = path.HasValidUNCPathSyntax();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("test\n", "test")]
    [InlineData("test\r", "test")]
    [InlineData("test\n\r", "test")]
    [InlineData("test\r\n", "test")]
    public void TrimNewLineFromEnd_WhenInvoked_ReturnsCorrectResult(string value, string expected)
    {
        // Arrange & Act
        var actual = value.TrimNewLineFromEnd();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(@"test\")]
    [InlineData(@"test\\")]
    [InlineData(@"test/")]
    [InlineData(@"test//")]
    [InlineData(@"test\/")]
    [InlineData(@"test\\//")]
    [InlineData(@"test/\")]
    [InlineData(@"test//\\")]
    public void TrimDirSeparatorFromEnd_WhenInvoked_ReturnsCorrectResult(string value)
    {
        // Arrange
        const string expected = "test";

        // Act
        var actual = value.TrimDirSeparatorFromEnd();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(@"C:\dir-1\dir-2", "C:/dir-1/dir-2")]
    [InlineData(@"C:\dir-1\dir-2\", "C:/dir-1/dir-2/")]
    public void ToCrossPlatPath_WhenInvoked_ReturnsCorrectResult(string path, string expected)
    {
        // Act
        var actual = path.ToCrossPlatPath();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void RemoveAll_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var testValue = "keep-remove-keep-keep-remove-keep";

        // Act
        var actual = testValue.RemoveAll("remove");

        // Assert
        actual.Should().Be("keep--keep-keep--keep");
    }

    [Fact]
    public void ToSixLaborImage_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var imageData = new ImageData(new NETColor[2, 3], 2, 3);

        var expectedPixels = new Rgba32[2, 3];

        // Act
        var sixLaborsImage = imageData.ToSixLaborImage();
        var actualPixels = GetSixLaborPixels(sixLaborsImage);

        // Assert
        actualPixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void ToImageData_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var rowColors = new Dictionary<uint, NETColor>
        {
            { 0, NETColor.Red },
            { 1, NETColor.Green },
            { 2, NETColor.Blue },
        };

        var sixLaborsImage = CreateSixLaborsImage(2, 3, rowColors);
        var expectedPixels = CreateImageDataPixels(2, 3, rowColors);

        // Act
        var actual = TestHelpers.ToImageData(sixLaborsImage);

        // Assert
        actual.Pixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void SetVertexPos_WithRectGPUDataAndInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

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
    public void SetVertexPos_WhenInvokedWithRectGPUData_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void Scale_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(150, 150);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, NETColor.White, 0f);

        // Act
        var actual = sut.Scale(0.5f);

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void FlipEnd_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(0, 0);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, NETColor.White, 0f);

        // Act
        var actual = sut.FlipEnd();

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void Clamp_WhenInvoked_ClampsRadiusValues()
    {
        // Arrange
        var sut = new CornerRadius(200f, 200, -200f, -200f);

        // Act
        sut = sut.Clamp(0f, 100f);

        // Assert
        sut.TopLeft.Should().Be(100f);
        sut.BottomLeft.Should().Be(100f);
        sut.BottomRight.Should().Be(0f);
        sut.TopRight.Should().Be(0f);
    }

    [Fact]
    public void Length_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float startX = 124.6f;
        const float startY = 187.5f;
        const float stopX = 257.3f;
        const float stopY = 302.4f;

        var line = new LineBatchItem(
            new Vector2(startX, startY),
            new Vector2(stopX, stopY),
            NETColor.White,
            0f);

        // Act
        var actual = line.Length();

        // Assert
        actual.Should().Be(175.53146f);
    }

    [Theory]
    [MemberData(nameof(GetExpectedRectPointData))]
    internal void CreateRectFromLine_WhenInvoked_ReturnsCorrectResult(
        LineBatchItem lineItem,
        Vector2 topLeftCorner,
        Vector2 topRightCorner,
        Vector2 bottomRightCorner,
        Vector2 bottomLeftCorner)
    {
        // Arrange
        var expected = new[]
        {
            topLeftCorner,
            topRightCorner,
            bottomRightCorner,
            bottomLeftCorner,
        };

        var line = new LineBatchItem(lineItem.P1, lineItem.P2, lineItem.Color, lineItem.Thickness);

        // Act
        var actual = line.CreateRectFromLine();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetP1_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP1(new Vector2(10, 20));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetP2_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(20, 30),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP2(new Vector2(20, 30));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SwapEnds_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(2, 3),
            new Vector2(1, 2),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SwapEnds();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetVertexPos_WithInvalidVertexNumber_ThrowsException()
    {
        // Arrange
        var gpuData = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty));

        // Act
        var act = () => gpuData.SetVertexPos(Vector2.Zero, (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData(VertexNumber.One)]
    [InlineData(VertexNumber.Two)]
    [InlineData(VertexNumber.Three)]
    [InlineData(VertexNumber.Four)]
    internal void SetVertexPos_WhenInvokedWithLineGPUData_ReturnsCorrectResult(VertexNumber vertexNumber)
    {
        // Arrange
        var expectedPos = new Vector2(10, 20);

        var gpuData = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty));

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

    [Fact]
    public void SetRectangle_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(new Vector4(1111f, 2222f, 3333f, 4444f));
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
        var gpuData = GenerateGPUDataInSequence(0);
        var expected = new Vector4(111, 222, 333, 444);

        // Act
        var actual = gpuData.SetRectangle(new Vector4(111, 222, 333, 444));

        // Assert
        actual.Vertex1.Rectangle.Should().Be(expected);
        actual.Vertex2.Rectangle.Should().Be(expected);
        actual.Vertex3.Rectangle.Should().Be(expected);
        actual.Vertex4.Rectangle.Should().Be(expected);
    }

    [Fact]
    public void SetAsSolid_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);
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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);
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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);
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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);
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
        var gpuData = GenerateGPUDataInSequence(0);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
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
        var gpuData = GenerateGPUDataInSequence(0);
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
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetColor(It.IsAny<NETColor>(), (VertexNumber)1234);

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
        var gpuData = GenerateGPUDataInSequence(0);
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
            VertexNumber.One => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(NETColor.Blue);
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
        var gpuData = GenerateGPUDataInSequence(0);
        var expected = NETColor.FromArgb(220, 230, 240, 250);

        // Act
        var actual = gpuData.SetColor(NETColor.FromArgb(220, 230, 240, 250));

        // Assert
        actual.Vertex1.Color.Should().Be(expected);
        actual.Vertex2.Color.Should().Be(expected);
        actual.Vertex3.Color.Should().Be(expected);
        actual.Vertex4.Color.Should().Be(expected);
    }

    [Fact]
    public void SetColor_WhenSettingLineGPUData_SetsColorToAllVertexData()
    {
        // Arrange
        var data = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White));

        // Act
        var actual = data.SetColor(NETColor.CornflowerBlue);

        // Assert
        actual.Vertex1.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex2.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex3.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex4.Color.Should().Be(NETColor.CornflowerBlue);
    }

    [Fact]
    public void GetPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var rect = new NETRectF(11f, 22f, 33f, 44f);

        // Act
        var actual = rect.GetPosition();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Fact]
    public void TrimAllEnds_WhenUsingDefaultParamValue_TrimsEndsOfAllStrings()
    {
        // Arrange
        var values = new[] { "item ", "item " };

        // Act
        var actual = values.TrimAllEnds();

        // Assert
        actual.Should().AllBe("item");
    }

    [Fact]
    public void TrimAllEnds_WhenTrimmingSpecificCharacter_TrimsEndsOfAllStrings()
    {
        // Arrange
        var values = new[] { "item~", "item~" };

        // Act
        var actual = values.TrimAllEnds('~');

        // Assert
        actual.Should().AllBe("item");
    }

    [Theory]
    [InlineData(@"C:\dir1\dir2", "C:/dir1/dir2")]
    [InlineData(@"C:\dir1\dir2\", "C:/dir1/dir2/")]
    [InlineData("C:/dir1/dir2", "C:/dir1/dir2")]
    [InlineData("C:/dir1/dir2/", "C:/dir1/dir2/")]
    public void NormalizePaths_WhenInvoked_ReturnsCorrectResult(string path, string expected)
    {
        // Arrange
        var paths = new[] { path };

        // Act
        var actual = paths.NormalizePaths().ToArray();

        // Assert
        actual.Should().ContainSingle();
        actual[0].Should().Be(expected);
    }

    [Fact]
    public void ToVector2_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new NETPoint(11, 22);

        // Act
        var actual = point.ToVector2();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Fact]
    public void ToPoint_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new Vector2(11, 22);

        // Act
        var actual = point.ToPoint();

        // Assert
        actual.X.Should().Be(11);
        actual.Y.Should().Be(22);
    }

    [Fact]
    public void DequeueWhile_WithNoItems_DoesNotInvokedPredicate()
    {
        // Arrange
        var queue = new Queue<int>();

        var untilPredicate = new Predicate<int>(_ =>
        {
            Assert.Fail("The 'untilPredicate' should not be invoked with 0 queue items.");
            return false;
        });

        // Act & Assert
        queue.DequeueWhile(untilPredicate);
    }

    [Fact]
    public void DequeueWhile_WhenInvoked_PerformsDequeueWhenPredicateIsTrue()
    {
        // Arrange
        var totalInvokes = 0;
        var queue = new Queue<int>();
        queue.Enqueue(11);
        queue.Enqueue(22);

        var untilPredicate = new Predicate<int>(_ =>
        {
            totalInvokes += 1;
            return true;
        });

        // Act
        queue.DequeueWhile(untilPredicate);

        // Assert
        totalInvokes.Should().Be(2);
        queue.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3, 2)]
    [InlineData(40, -1)]
    public void IndexOf_WithEnumerableItemsAndPredicate_ReturnsCorrectResult(int value, int expected)
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4 };

        // Act
        var actual = items.IndexOf(i => i == value);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateReturnsTrue_ReturnsCorrectIndex()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-C");

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-D");

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(30);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerDoesNotExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(300);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void TotalOnLayer_WithLayerGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(20);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void TotalOnLayer_WithNoLayersGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(200);

        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            default,
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void IncreaseBy_WhenInvoked_CorrectlyIncreasesItems()
    {
        // Arrange
        var expected = new[] { 1, 2, 3, 4, 0, 0 };
        var items = new Memory<int>(new[] { 1, 2, 3, 4, });

        // Act
        items.IncreaseBy(2);

        // Assert
        items.Span.ToArray().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithRectShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            3f,
            4f,
            NETColor.FromArgb(5, 6, 7, 8),
            true,
            9f,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.Horizontal,
            NETColor.FromArgb(14, 15, 16, 17),
            NETColor.FromArgb(18, 19, 20, 21));

        var sut = new RectShape
        {
            Position = new Vector2(1, 2),
            Width = 3,
            Height = 4,
            Color = NETColor.FromArgb(5,6,7,8),
            IsSolid = true,
            BorderThickness = 9,
            CornerRadius = new CornerRadius(10, 11, 12, 13),
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(14, 15, 16, 17),
            GradientStop = NETColor.FromArgb(18, 19, 20, 21),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithCircleShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            100f,
            100f,
            NETColor.FromArgb(4, 5, 6, 7),
            true,
            50f,
            new CornerRadius(50f),
            ColorGradient.Horizontal,
            NETColor.FromArgb(9, 10, 11, 12),
            NETColor.FromArgb(13, 14, 15, 16));

        var sut = new CircleShape
        {
            Position = new Vector2(1, 2),
            Diameter = 100,
            Color = NETColor.FromArgb(4, 5, 6, 7),
            IsSolid = true,
            BorderThickness = 50,
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(9, 10, 11, 12),
            GradientStop = NETColor.FromArgb(13, 14, 15, 16),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    /// <summary>
    /// Creates a Six Labors image type of <see cref="Image{Rgba32}"/> with the given <paramref name="width"/>
    /// and <paramref name="height"/> with each row having its own colors described by the given
    /// <paramref name="rowColors"/> dictionary.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="rowColors">The color for each row.</param>
    /// <returns>An image with the given row colors.</returns>
    /// <remarks>
    ///     The <paramref name="rowColors"/> dictionary key is the zero based row index and the
    ///     value is the color to make the entire row.
    /// </remarks>
    [ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
    private static Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        if (height != rowColors.Count)
        {
            Assert.Fail($"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
        }

        var availableRows = rowColors.Keys.ToArray();

        foreach (var row in availableRows)
        {
            if (row > height - 1)
            {
                Assert.Fail($"The row '{row}' is not within the range of rows for the image height '{height}' for the definition of row colors.");
            }
        }

        var result = new Image<Rgba32>(width, height);

        for (var y = 0; y < height; y++)
        {
            var row = y;
            result.ProcessPixelRows(accessor =>
            {
                var rowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < width; x++)
                {
                    rowSpan[x] = new Rgba32(
                        rowColors[(uint)row].R,
                        rowColors[(uint)row].G,
                        rowColors[(uint)row].B,
                        rowColors[(uint)row].A);
                }
            });
        }

        return result;
    }

    private static NETColor[,] CreateImageDataPixels(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        var result = new NETColor[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[x, y] = NETColor.FromArgb(rowColors[(uint)y].A, rowColors[(uint)y].R, rowColors[(uint)y].G, rowColors[(uint)y].B);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the <see cref="Rgba32"/> pixels from the given <paramref name="sixLaborsImage"/>.
    /// </summary>
    /// <param name="sixLaborsImage">The six labors image.</param>
    /// <returns>The two dimensional pixel colors of the image.</returns>
    private static Rgba32[,] GetSixLaborPixels(Image<Rgba32> sixLaborsImage)
    {
        var result = new Rgba32[sixLaborsImage.Width, sixLaborsImage.Height];

        for (var y = 0; y < sixLaborsImage.Height; y++)
        {
            var row = y;
            sixLaborsImage.ProcessPixelRows(accessor =>
            {
                var pixelRow = accessor.GetRowSpan(row);

                for (var x = 0; x < sixLaborsImage.Width; x++)
                {
                    result[x, row] = pixelRow[x];
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Generates GPU data with sequential, numerical values throughout
    /// the struct, starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment from.</param>
    /// <returns>The GPU data to test.</returns>
    private static RectGPUData GenerateGPUDataInSequence(int startValue)
    {
        var vertex1 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex2 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex3 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex4 = GenerateVertexDataInSequence(startValue);

        return new RectGPUData(vertex1, vertex2, vertex3, vertex4);
    }

    /// <summary>
    /// Generates vertex data with numerical values sequentially throughout
    /// the struct starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment from.</param>
    /// <returns>The vertex data to test.</returns>
    private static RectVertexData GenerateVertexDataInSequence(int startValue)
    {
        return new RectVertexData(
            new Vector2(startValue + 1f, startValue + 2f),
            new Vector4(startValue + 3, startValue + 4, startValue + 5, startValue + 6),
            NETColor.FromArgb(startValue + 7, startValue + 8, startValue + 9, startValue + 10),
            false,
            startValue + 7f,
            startValue + 8f,
            startValue + 9f,
            startValue + 10f,
            startValue + 11f);
    }
}
