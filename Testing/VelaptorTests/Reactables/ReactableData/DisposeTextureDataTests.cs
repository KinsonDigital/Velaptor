// <copyright file="DisposeTextureDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Reactables.ReactableData;

using Velaptor.Reactables.ReactableData;
using Xunit;

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
        var data = new DisposeTextureData { TextureId = 1234u };
        var actual = data.TextureId;

        // Assert
        Assert.Equal(1234u, actual);
    }
    #endregion
}
