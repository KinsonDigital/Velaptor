// <copyright file="SpriteBatchSizeAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using Velaptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SpriteBatchSizeAttribute"/> class.
    /// </summary>
    public class SpriteBatchSizeAttributeTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsProperty()
        {
            // Arrange & Act
            var attribute = new SpriteBatchSizeAttribute(123u);

            // Assert
            Assert.Equal(123u, attribute.BatchSize);
        }
        #endregion
    }
}
