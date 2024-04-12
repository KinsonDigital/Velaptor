// <copyright file="StringExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

using FluentAssertions;
using Helpers;
using Velaptor.ExtensionMethods;
using Xunit;

#pragma warning disable SA1514

/// <summary>
/// Tests all the <see cref="string"/> extension methods.
/// </summary>
public class StringExtensionsTests
{
    private const char CrossPlatDirSeparatorChar = '/';

    #region Test Data
    /// <summary>
    /// Gets the unit test data for the <see cref="StringExtensions.HasValidDriveSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool> ContainsValidDriveTestData =>
        new ()
        {
            { string.Empty, false },
            { null, false },
            { "windows", false },
            { ":", false },
            { "C", false },
            { ":C", false },
            { "1:", false },
            { "windowsC:system32", false },
            { @"C:\Windows\System32", true },
            { "C:windows", true },
            { $"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", true },
        };

    /// <summary>
    /// Gets the unit test data for the <see cref="StringExtensions.HasValidFullDirPathSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool> IsFullyQualifiedDirPathTestData =>
        new ()
        {
            { string.Empty, false },
            { null, false },
            { @"\Windows\System32", false },
            { $"{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", false },
            { "C:Windows", false },
            { $"{CrossPlatDirSeparatorChar}WindowsC:", false },
            { $"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32{CrossPlatDirSeparatorChar}fake-file.txt", false },
            { $"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32", true },
            { $"C:{CrossPlatDirSeparatorChar}Windows{CrossPlatDirSeparatorChar}System32{CrossPlatDirSeparatorChar}", true },
        };

    /// <summary>
    /// Gets the unit test data for the <see cref="StringExtensions.HasValidUNCPathSyntax"/>() method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool> IsUNCPathTestData =>
        new ()
        {
            { string.Empty, false },
            { null, false },
            { @"\\", false },
            { "directory", false },
            { $"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}", false },
            { $"{CrossPlatDirSeparatorChar}{CrossPlatDirSeparatorChar}directory", true },
        };
    #endregion

    [Theory]
    [InlineData('x', true)]
    [InlineData('k', false)]
    public void DoesNotStartWith_WhenCheckingForCharacters_ReturnsCorrectResult(char character, bool expected)
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
    [InlineData("C:/", true)]
    [InlineData("C:", false)]
    [InlineData(@"C\", false)]
    [InlineData("C/", false)]
    [InlineData(@"C:\test-file.txt", false)]
    [InlineData("C:/test-file.txt", false)]
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
    public void GetLastDirName_WhenRunningOnWindows_ReturnsCorrectResult(string? value, string expected)
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
    public void GetLastDirName_WhenRunningOnLinux_ReturnsCorrectResult(string? value, string expected)
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
    public void TrimNewLineFromEnd_WhenInvoked_ReturnsCorrectResult(string? value, string expected)
    {
        // Arrange & Act
        var actual = value.TrimNewLineFromEnd();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(@"test\")]
    [InlineData(@"test\\")]
    [InlineData("test/")]
    [InlineData("test//")]
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
}
