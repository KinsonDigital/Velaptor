// <copyright file="GLContextDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables.ReactableData;

/// <summary>
/// Tests the <see cref="GLContextData"/> struct.
/// </summary>
public class GLContextDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsDataProperty()
    {
        // Arrange
        var obj = new object();

        // Act
        var contextData = new GLContextData(obj);
        var actual = contextData.Data;

        // Assert
        Assert.Same(obj, actual);
    }
    #endregion
}
