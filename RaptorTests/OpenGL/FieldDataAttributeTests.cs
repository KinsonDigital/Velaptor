using Raptor.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RaptorTests.OpenGL
{
    /// <summary>
    /// Tests the <see cref="FieldDataAttribute"/> class.
    /// </summary>
    public class FieldDataAttributeTests
    {
        [Fact]
        public void Ctor_WhenInvoked_CorrectlySetsPropertyValues()
        {
            //Arrange

            //Act
            var attribute = new FieldDataAttribute(4, 3);

            //Assert
            Assert.Equal(4u, attribute.BytesPerElement);
            Assert.Equal(3u, attribute.TotalElements);
            Assert.Equal(12u, attribute.TotalBytes);
        }
    }
}
