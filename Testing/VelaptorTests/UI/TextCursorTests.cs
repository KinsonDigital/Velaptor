// <copyright file="TextCursorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Drawing;
using System.Numerics;
using System.Text;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using Moq;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="TextCursor"/> class.
/// </summary>
public class TextCursorTests
{
    private readonly Mock<IPushReactable<TextBoxStateData>> textBoxStateReactable;
    private IReceiveSubscription<TextBoxStateData>? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextCursorTests"/> class.
    /// </summary>
    public TextCursorTests()
    {
        var textBoxStateUnsubscriber = new Mock<IDisposable>();
        this.textBoxStateReactable = new Mock<IPushReactable<TextBoxStateData>>();
        this.textBoxStateReactable.Setup(m =>
            m.Subscribe(It.IsAny<IReceiveSubscription<TextBoxStateData>>()))
            .Callback<IReceiveSubscription<TextBoxStateData>>(reactorObj =>
            {
                this.reactor = reactorObj;
            })
            .Returns(() => textBoxStateUnsubscriber.Object);
    }

    #region Property Tests
    [Fact]
    public void Cursor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new RectShape { Color = Color.CornflowerBlue, Width = 2, IsSolid = true, };
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Cursor;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Width = 123;
        var actual = sut.Width = 123;

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Height = 123;
        var actual = sut.Height = 123;

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Vector2(123, 456);
        var actual = sut.Position;

        // Assert
        actual.Should().Be(new Vector2(123, 456));
    }

    [Fact]
    public void Color_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Color = Color.MediumPurple;
        var actual = sut.Color;

        // Assert
        actual.Should().Be(Color.MediumPurple);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("t", "", 15, 20, 0, 10, 100)]
    [InlineData("t", "t", 15, 20, 0, 10, 115)]
    [InlineData("", "test-value", 40, 50, 6, 10, 115)]
    [InlineData("", "", 0, 0, 0, 0, 100)]
    [InlineData("t", "", 20, 15, 0, 0, 100)]
    [InlineData("t", "z", 20, 15, 0, 0, 125)]
    public void Update_WhenAddingCharacterWhenCursorIsAtRightEndOfText_SetsCursorToLeftSideOfChar(
        string preText,
        string postText,
        int cursorRight,
        int preTextRight,
        int postCharIndex,
        int postTextLength,
        int expectedCursorLeft)
    {
        // Arrange
        var preMutateState = new TextBoxStateData
        {
            TextMutateType = MutateType.PreMutate,
            Text = new StringBuilder(preText),
            TextRight = preTextRight,
        };

        var postMutateState = new TextBoxStateData
        {
            TextMutateType = MutateType.PostMutate,
            Event = TextBoxEvent.AddingCharacter,
            Text = new StringBuilder(postText),
            CharIndex = postCharIndex,
            TextLeft = 100,
            TextView = new RectShape { Left = 100 },
            TextLength = postTextLength,
            CurrentCharLeft = 115,
            CurrentCharRight = 125,
        };

        var sut = CreateSystemUnderTest();
        sut.Cursor = new RectShape { Right = cursorRight, };

        // Act
        this.reactor.OnReceive(preMutateState);
        this.reactor.OnReceive(postMutateState);
        sut.Update();

        // Assert
        sut.Cursor.Left.Should().Be(expectedCursorLeft);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextCursor"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextCursor CreateSystemUnderTest() => new (this.textBoxStateReactable.Object);
}
