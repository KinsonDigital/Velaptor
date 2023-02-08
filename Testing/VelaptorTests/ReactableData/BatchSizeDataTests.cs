// <copyright file="BatchSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="BatchSizeData"/> struct.
/// </summary>
public class BatchSizeDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var sut = new BatchSizeData
        {
            BatchSize = 1,
            TypeOfBatch = BatchType.Rect,
        };

        // Assert
        sut.BatchSize.Should().Be(1);
        sut.TypeOfBatch.Should().Be(BatchType.Rect);
    }
    #endregion
}
