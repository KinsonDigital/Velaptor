// <copyright file="FieldDataAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using Velaptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FieldDataAttribute"/> class.
    /// </summary>
    public class FieldDataAttributeTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_CorrectlySetsPropertyValues()
        {
            // Act
            var attribute = new FieldDataAttribute(4, 3);

            // Assert
            Assert.Equal(4u, attribute.BytesPerElement);
            Assert.Equal(3u, attribute.TotalElements);
            Assert.Equal(12u, attribute.TotalBytes);
        }
        #endregion
    }
}
