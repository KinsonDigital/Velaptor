// <copyright file="LineBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.OpenGL.Batching;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Tests the <see cref="LineBatchItem"/> struct.
/// </summary>
public class LineBatchItemTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchItemTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">Provides test output.</param>
    public LineBatchItemTests(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

    /// <summary>
    /// Gets all the test data related to testing the <see cref="IsEmptyTestData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<Vector2, Vector2, Color, float, string, bool> IsEmptyTestData =>
        new ()
        {
            // p1, p2, color, thickness, TEST NAME, Expected
            { Vector2.Zero, Vector2.Zero, Color.Empty, 0f, "Fully Empty", true },
            { new Vector2(1, 2), Vector2.Zero, Color.Empty, 0f, "p1", false },
            { Vector2.Zero, new Vector2(1, 2), Color.Empty, 0f, "p2", false },
            { Vector2.Zero, Vector2.Zero, Color.FromArgb(1, 2, 3, 4), 0f, "color", false },
            { Vector2.Zero, Vector2.Zero, Color.Empty, 1f, "thickness", false },
        };

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsPropertyValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);
        var expectedClr = Color.FromArgb(5, 6, 7, 8);
        const int expectedThickness = 9;

        // Act
        var sut = new LineBatchItem(expectedP1, expectedP2, expectedClr, expectedThickness);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
        sut.Color.Should().BeEquivalentTo(expectedClr);
        sut.Thickness.Should().Be(expectedThickness);
    }

    [Theory]
    [MemberData(nameof(IsEmptyTestData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 p1,
        Vector2 p2,
        Color color,
        float thickness,
        string testName, // Only used for test output
        bool expected)
    {
        // Arrange
        var sut = new LineBatchItem(p1, p2, color, thickness);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        this.testOutputHelper.WriteLine($"Test Param: {testName}");
        actual.Should().Be(expected);
    }
    #endregion
}
