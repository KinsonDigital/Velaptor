// <copyright file="AtlasSubTextureDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasSubTextureData"/> class.
/// </summary>
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer", Justification = "Intention is to be more explicit.")]
public class AtlasSubTextureDataTests
{
    #region Prop Tests
    [Fact]
    public void Bounds_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Rectangle(11, 22, 33, 44);
        var data = new AtlasSubTextureData
        {
            Bounds = new Rectangle(11, 22, 33, 44),
        };

        // Act
        var actual = data.Bounds;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Name_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var data = new AtlasSubTextureData { Name = "test-name" };

        // Act
        var actual = data.Name;

        // Assert
        Assert.Equal("test-name", actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Equals_WhenComparingDifferentObjectTypes_ReturnsFalse()
    {
        // Arrange
        var dataA = default(AtlasSubTextureData);
        var dataB = new object();

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenComparingObjectsWithDifferentNameProp_ReturnsFalse()
    {
        // Arrange
        var dataA = default(AtlasSubTextureData);
        var dataB = new AtlasSubTextureData
        {
            Name = "DataB",
        };

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenComparingObjectsWithDifferentBoundsProp_ReturnsFalse()
    {
        // Arrange
        var dataA = default(AtlasSubTextureData);
        var dataB = new AtlasSubTextureData
        {
            Bounds = new Rectangle(11, 22, 33, 44),
        };

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenComparingObjectsWithDifferentFrameIndexProp_ReturnsFalse()
    {
        // Arrange
        var dataA = default(AtlasSubTextureData);
        var dataB = new AtlasSubTextureData
        {
            FrameIndex = 111,
        };

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.False(actual);
    }
    #endregion
}
