// <copyright file="BatchSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables.ReactableData;

/// <summary>
/// Tests the <see cref="BatchSizeData"/> struct.
/// </summary>
public class BatchSizeDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsBatchSizeProperty()
    {
        // Arrange & Act
        var data = new BatchSizeData(123u);
        var actual = data.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion
}
