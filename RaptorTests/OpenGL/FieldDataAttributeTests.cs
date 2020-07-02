using Raptor.OpenGL;
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
            //Act
            var attribute = new FieldDataAttribute(4, 3);

            //Assert
            Assert.Equal(4u, attribute.BytesPerElement);
            Assert.Equal(3u, attribute.TotalElements);
            Assert.Equal(12u, attribute.TotalBytes);
        }
    }
}
