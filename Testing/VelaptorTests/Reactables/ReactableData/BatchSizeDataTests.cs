// <copyright file="BatchSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables.ReactableData;

using FluentAssertions;
using Velaptor.Reactables.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="BatchSizeData"/> struct.
/// </summary>
public class BatchSizeDataTests
{
    #region Prop Tests
    [Fact]
    public void BatchSize_WhenSettingValue_ReturnsCorrectResult4()
    {
        // Arrange & Act
        var data = new BatchSizeData { BatchSize = 123u };
        var actual = data.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion
}
