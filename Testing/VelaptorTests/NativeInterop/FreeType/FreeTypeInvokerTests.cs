// <copyright file="FreeTypeInvokerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.FreeType;

using System;
using FluentAssertions;
using FreeTypeSharp.Native;
using Moq;
using Velaptor.NativeInterop.FreeType;
using Xunit;

/// <summary>
/// Tests the <see cref="FreeTypeInvoker"/> class.
/// </summary>
public class FreeTypeInvokerTests
{
    #region Method Tests
    [Fact]
    public void FTGetKerning_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Get_Kerning(0, It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'face' cannot be a value of zero.");
    }

    [Fact]
    public void FTGetCharIndex_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Get_Char_Index(0, It.IsAny<uint>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'face' cannot be a value of zero.");
    }

    [Fact]
    public void FTLoadChar_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Load_Char(0, It.IsAny<uint>(), It.IsAny<int>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'face' cannot be a value of zero.");
    }

    [Fact]
    public void FTRenderGlyph_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Render_Glyph(0, It.IsAny<FT_Render_Mode>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'slot' cannot be a value of zero.");
    }

    [Fact]
    public void FTNewFace_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_New_Face(0, It.IsAny<string>(), It.IsAny<int>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'library' cannot be a value of zero.");
    }

    [Fact]
    public void FTSetCharSize_WithNullFacePointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Set_Char_Size(
            0,
            123,
            456,
            It.IsAny<uint>(),
            It.IsAny<uint>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'face' cannot be a value of zero.");
    }

    [Fact]
    public void FTSetCharSize_WithNullCharWidthPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Set_Char_Size(
            123,
            0,
            456,
            It.IsAny<uint>(),
            It.IsAny<uint>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'char_width' cannot be a value of zero.");
    }

    [Fact]
    public void FTSetCharSize_WithNullCharHeightPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Set_Char_Size(
            123,
            456,
            0,
            It.IsAny<uint>(),
            It.IsAny<uint>());

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'char_height' cannot be a value of zero.");
    }

    [Fact]
    public void FTDoneFace_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Done_Face(0);

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'face' cannot be a value of zero.");
    }

    [Fact]
    public void FTDoneGlyph_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Done_Glyph(0);

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'glyph' cannot be a value of zero.");
    }

    [Fact]
    public void FTDoneFreeType_WithNullPointer_ThrowsException()
    {
        // Arrange
        var sut = new FreeTypeInvoker();

        // Act
        var act = () => sut.FT_Done_FreeType(0);

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage("The pointer parameter 'library' cannot be a value of zero.");
    }
    #endregion
}
