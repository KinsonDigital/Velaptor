// <copyright file="InternalExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CS8524 The switch expression does not handle some values of its input type.
namespace VelaptorTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Numerics;
    using FluentAssertions;
    using Moq;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Velaptor;
    using Velaptor.Graphics;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.GPUData;
    using VelaptorTests.Helpers;
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
        /// Provides unit test data for the <see cref="InternalExtensionMethods.HasValidFullFilePathSyntax"/>() method.
        /// </summary>
        /// <returns>The test data.</returns>
        public static IEnumerable<object[]> HasValidFullFilePathSyntaxTestData()
        {
            yield return new object[] { "C:windows", false };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}test-file", false };
            yield return new object[] { string.Empty, false };
            yield return new object[] { null, false };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}", false };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}", false };
            yield return new object[] { "non-path-value", false };
            yield return new object[] { $@"{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}", false };
            yield return new object[] { $@"test-dir{CrossPlatDirSeparatorChar}", false };
            yield return new object[] { @"C:\test-dir\test-file.txt", true };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}test-file.txt", true };
        }

        /// <summary>
        /// Provides unit test data for the <see cref="InternalExtensionMethods.HasInvalidFullFilePathSyntax"/>() method.
        /// </summary>
        /// <returns>The test data.</returns>
        public static IEnumerable<object[]> IsInvalidFilePathTestData()
        {
            yield return new object[] { $@"C:\test-dir\test-file.txt", false };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}test-file.txt", false };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}test-file", true };
            yield return new object[] { string.Empty, true };
            yield return new object[] { null, true };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}", true };
            yield return new object[] { $@"C:{CrossPlatDirSeparatorChar}", true };
            yield return new object[] { "non-path-value", true };
            yield return new object[] { $@"{CrossPlatDirSeparatorChar}test-dir{CrossPlatDirSeparatorChar}", true };
            yield return new object[] { $@"test-dir{CrossPlatDirSeparatorChar}", true };
            yield return new object[] { "C:windows", true };
        }

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
            yield return new object[] { $@"C:\Windows\System32", true };
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
            yield return new object[] { $@"\Windows\System32", false };
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
            yield return new object[] { $@"\\", false };
            yield return new object[] { $@"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}", false };
            yield return new object[] { $@"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}directory", true };
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(HasValidFullFilePathSyntaxTestData))]
        public void HasValidFullFilePathSyntax_WhenInvoked_ReturnsCorrectResult(string path, bool expected)
        {
            // Act
            var actual = path.HasValidFullFilePathSyntax();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsInvalidFilePathTestData))]
        public void HasInvalidFullFilePathSyntax_WhenInvoked_ReturnsCorrectResult(string path, bool expected)
        {
            // Act
            var actual = path.HasInvalidFullFilePathSyntax();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ContainsValidDriveTestData))]
        public void HasValidDriveSyntax_WhenInvoked_ReturnsCorrectResult(string dirPath, bool expected)
        {
            // Act
            var actual = dirPath.HasValidDriveSyntax();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsFullyQualifiedDirPathTestData))]
        public void HasValidFullDirPathSyntax_WhenInvoked_ReturnsCorrectResult(string dirPath, bool expected)
        {
            // Act
            var actual = dirPath.HasValidFullDirPathSyntax();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsUNCPathTestData))]
        public void HasValidUNCPathSyntax_WhenInvoked_ReturnsCorrectResult(string path, bool expected)
        {
            // Act
            var actual = path.HasValidUNCPathSyntax();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test\n")]
        [InlineData("test\r")]
        [InlineData("test\n\r")]
        [InlineData("test\r\n")]
        public void TrimNewLineFromEnd_WhenInvoked_ReturnsCorrectResult(string value)
        {
            // Arrange
            const string expected = "test";

            // Act
            var actual = value.TrimNewLineFromEnd();

            // Assert
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
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
        public void ToReadOnlyCollection_WithNullIEnumerableItems_ReturnsEmptyReadOnlyCollection()
        {
            // Arrange
            IEnumerable<int>? numbers = null;

            // Act
            var actual = numbers.ToReadOnlyCollection();

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
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
            Assert.Equal(expectedPixels, actualPixels);
        }

        [Fact]
        public void ToImageData_WhenInvoked_CorrectlyConvertsToSixLaborImage()
        {
            // Arrange
            var rowColors = new Dictionary<uint, NETColor>()
            {
                { 0, NETColor.Red },
                { 1, NETColor.Green },
                { 2, NETColor.Blue },
            };

            var sixLaborsImage = CreateSixLaborsImage(2, 3, rowColors);
            var expectedPixels = CreateImageDataPixels(2, 3, rowColors);

            // Act
            var actual = ToImageData(sixLaborsImage);

            // Assert
            Assert.Equal(expectedPixels, actual.Pixels);
        }

        [Fact]
        public void SetVertexPos_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetVertexPos(It.IsAny<Vector2>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
        }

        [Theory]
        [InlineData((int)VertexNumber.One)]
        [InlineData((int)VertexNumber.Two)]
        [InlineData((int)VertexNumber.Three)]
        [InlineData((int)VertexNumber.Four)]
        public void SetVertexPos_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
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
            Assert.Equal(new Vector2(1111f, 2222f), actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
        }

        [Fact]
        public void SetRectangle_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetRectangle(It.IsAny<Vector4>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(new Vector4(1111f, 2222f, 3333f, 4444f), actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.Rectangle);
            Assert.Equal(expected, actual.Vertex2.Rectangle);
            Assert.Equal(expected, actual.Vertex3.Rectangle);
            Assert.Equal(expected, actual.Vertex4.Rectangle);
        }

        [Fact]
        public void SetIsFilled_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetIsFilled(It.IsAny<bool>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
        }

        [Theory]
        [InlineData((int)VertexNumber.One)]
        [InlineData((int)VertexNumber.Two)]
        [InlineData((int)VertexNumber.Three)]
        [InlineData((int)VertexNumber.Four)]
        public void SetIsFilled_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
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
                VertexNumber.One => gpuData.SetIsFilled(true, vertexNumber).Vertex1,
                VertexNumber.Two => gpuData.SetIsFilled(true, vertexNumber).Vertex2,
                VertexNumber.Three => gpuData.SetIsFilled(true, vertexNumber).Vertex3,
                VertexNumber.Four => gpuData.SetIsFilled(true, vertexNumber).Vertex4,
            };

            // Assert
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.True(actual.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
        }

        [Fact]
        public void SetIsFilled_WhenUpdatingAll_ReturnsCorrectResult()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act
            var actual = gpuData.SetIsFilled(true);

            // Assert
            Assert.True(actual.Vertex1.IsFilled);
            Assert.True(actual.Vertex2.IsFilled);
            Assert.True(actual.Vertex3.IsFilled);
            Assert.True(actual.Vertex4.IsFilled);
        }

        [Fact]
        public void SetBorderThickness_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetBorderThickness(It.IsAny<float>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.Equal(expectedVertex.IsFilled, actual.IsFilled);
            Assert.Equal(123f, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.BorderThickness);
            Assert.Equal(expected, actual.Vertex2.BorderThickness);
            Assert.Equal(expected, actual.Vertex3.BorderThickness);
            Assert.Equal(expected, actual.Vertex4.BorderThickness);
        }

        [Fact]
        public void SetTopLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetTopLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(1234f, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.TopLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex2.TopLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex3.TopLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex4.TopLeftCornerRadius);
        }

        [Fact]
        public void SetBottomLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetBottomLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(1234f, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.BottomLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex2.BottomLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex3.BottomLeftCornerRadius);
            Assert.Equal(expected, actual.Vertex4.BottomLeftCornerRadius);
        }

        [Fact]
        public void SetBottomRightCornerRadius_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetBottomRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(1234f, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.BottomRightCornerRadius);
            Assert.Equal(expected, actual.Vertex2.BottomRightCornerRadius);
            Assert.Equal(expected, actual.Vertex3.BottomRightCornerRadius);
            Assert.Equal(expected, actual.Vertex4.BottomRightCornerRadius);
        }

        [Fact]
        public void SetTopRightCornerRadius_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetTopRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(expectedVertex.Color, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(1234f, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.TopRightCornerRadius);
            Assert.Equal(expected, actual.Vertex2.TopRightCornerRadius);
            Assert.Equal(expected, actual.Vertex3.TopRightCornerRadius);
            Assert.Equal(expected, actual.Vertex4.TopRightCornerRadius);
        }

        [Fact]
        public void SetColor_WithInvalidVertexValue_ThrowsException()
        {
            // Arrange
            var gpuData = GenerateGPUDataInSequence(0);

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                gpuData.SetColor(It.IsAny<NETColor>(), (VertexNumber)1234);
            }, "The vertex number is invalid. (Parameter 'vertexNumber')");
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
            Assert.Equal(expectedVertex.VertexPos, actual.VertexPos);
            Assert.Equal(expectedVertex.Rectangle, actual.Rectangle);
            Assert.Equal(NETColor.Blue, actual.Color);
            Assert.False(expectedVertex.IsFilled);
            Assert.Equal(expectedVertex.BorderThickness, actual.BorderThickness);
            Assert.Equal(expectedVertex.TopLeftCornerRadius, actual.TopLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomLeftCornerRadius, actual.BottomLeftCornerRadius);
            Assert.Equal(expectedVertex.BottomRightCornerRadius, actual.BottomRightCornerRadius);
            Assert.Equal(expectedVertex.TopRightCornerRadius, actual.TopRightCornerRadius);
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
            Assert.Equal(expected, actual.Vertex1.Color);
            Assert.Equal(expected, actual.Vertex2.Color);
            Assert.Equal(expected, actual.Vertex3.Color);
            Assert.Equal(expected, actual.Vertex4.Color);
        }

        [Fact]
        public void GetPosition_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var rect = new NETRectF(11f, 22f, 33f, 44f);

            // Act
            var actual = rect.GetPosition();

            // Assert
            Assert.Equal(11f, actual.X);
            Assert.Equal(22f, actual.Y);
        }

        [Fact]
        public void TrimAllEnds_WhenUsingDefaultParamValue_TrimsEndsOfAllStrings()
        {
            // Arrange
            var values = new[] { "item ", "item " };

            // Act
            var actual = values.TrimAllEnds();

            // Assert
            Assert.All(actual, i => Assert.Equal("item", i));
        }

        [Fact]
        public void TrimAllEnds_WhenTrimmingSpecificCharacter_TrimsEndsOfAllStrings()
        {
            // Arrange
            var values = new[] { "item~", "item~" };

            // Act
            var actual = values.TrimAllEnds('~');

            // Assert
            Assert.All(actual, i => Assert.Equal("item", i));
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
            var actual = paths.NormalizePaths();

            // Assert
            Assert.Single(actual);
            Assert.Equal(expected, actual[0]);
        }

        [Fact]
        public void ToVector2_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var point = new NETPoint(11, 22);

            // Act
            var actual = point.ToVector2();

            // Assert
            Assert.Equal(11f, actual.X);
            Assert.Equal(22f, actual.Y);
        }

        [Fact]
        public void ToPoint_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var point = new Vector2(11, 22);

            // Act
            var actual = point.ToPoint();

            // Assert
            Assert.Equal(11f, actual.X);
            Assert.Equal(22f, actual.Y);
        }

        [Fact]
        public void DequeueWhile_WithNoItems_DoesNotInvokedPredicate()
        {
            // Arrange
            var queue = new Queue<int>();

            var untilPredicate = new Predicate<int>(_ =>
            {
                Assert.True(false, "The 'untilPredicate' should not be invoked with 0 queue items.");
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
            Assert.Equal(2, totalInvokes);
            Assert.Empty(queue);
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
        [ExcludeFromCodeCoverage]
        private static Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
        {
            if (height != rowColors.Count)
            {
                Assert.True(false, $"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
            }

            var availableRows = rowColors.Keys.ToArray();

            foreach (var row in availableRows)
            {
                if (row > height - 1)
                {
                    Assert.True(false, $"The row '{row}' is not within the range of rows for the image height '{height}' for the definition of row colors.");
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
        /// <returns>The 2 dimensional pixel colors of the image.</returns>
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
        /// Converts the given <paramref name="image"/> of type <see cref="Image{Rgba32}"/>
        /// to the type of <see cref="ImageData"/>.
        /// </summary>
        /// <param name="image">The image to convert.</param>
        /// <returns>The image data of type <see cref="ImageData"/>.</returns>
        private static ImageData ToImageData(Image<Rgba32> image)
        {
            var pixelData = new NETColor[image.Width, image.Height];

            for (var y = 0; y < image.Height; y++)
            {
                var row = y;
                image.ProcessPixelRows(accessor =>
                {
                    var pixelRowSpan = accessor.GetRowSpan(row);

                    for (var x = 0; x < image.Width; x++)
                    {
                        pixelData[x, row] = NETColor.FromArgb(
                            pixelRowSpan[x].A,
                            pixelRowSpan[x].R,
                            pixelRowSpan[x].G,
                            pixelRowSpan[x].B);
                    }
                });
            }

            return new ImageData(pixelData, (uint)image.Width, (uint)image.Height);
        }

        /// <summary>
        /// Generates GPU data with numerical values sequentially throughout
        /// the struct starting with the given <paramref name="startValue"/> for the purpose of testing.
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
}
