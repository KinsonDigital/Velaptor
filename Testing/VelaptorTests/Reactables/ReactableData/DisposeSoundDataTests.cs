// <copyright file="DisposeSoundDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.ReactableData;
using Xunit;

namespace VelaptorTests.Reactables.ReactableData;

/// <summary>
/// Tests the <see cref="DisposeSoundData"/> struct.
/// </summary>
public class DisposeSoundDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsSoundIdProperty()
    {
        // Arrange & Act
        var data = new DisposeSoundData(1234u);
        var actual = data.SoundId;

        // Assert
        Assert.Equal(1234u, actual);
    }
    #endregion
}
