// <copyright file="BatchSizeAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.OpenGL;
using Xunit;

namespace VelaptorTests.OpenGL;

/// <summary>
/// Tests the <see cref="BatchSizeAttribute"/> class.
/// </summary>
public class BatchSizeAttributeTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new BatchSizeAttribute(123u);

        // Assert
        Assert.Equal(123u, attribute.BatchSize);
    }
    #endregion
}
