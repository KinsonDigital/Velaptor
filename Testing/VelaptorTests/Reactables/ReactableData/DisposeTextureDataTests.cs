// <copyright file="DisposeTextureDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables.ReactableData;

/// <summary>
/// Tests the <see cref="DisposeTextureData"/> struct.
/// </summary>
public class DisposeTextureDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsTextureIdProperty()
    {
        // Arrange & Act
        var data = new DisposeTextureData(1234u);
        var actual = data.TextureId;

        // Assert
        Assert.Equal(1234u, actual);
    }
    #endregion
}
