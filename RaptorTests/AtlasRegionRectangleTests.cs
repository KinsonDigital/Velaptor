using Moq;
using Raptor;
using Xunit;

namespace RaptorTests
{
    public class AtlasRegionRectangleTests
    {
        [Fact]
        public void Ctor_WhenInvoked_SetsNameProp()
        {
            //Act
            var rect = new AtlasRegionRectangle("test-name", It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.Equal("test-name", rect.Name);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsXProp()
        {
            //Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), 1234, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.Equal(1234, rect.X);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsYProp()
        {
            //Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), 1234, It.IsAny<int>(), It.IsAny<int>());
            //Assert
            Assert.Equal(1234, rect.Y);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsWidthProp()
        {
            //Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 1234, It.IsAny<int>());
            //Assert
            Assert.Equal(1234, rect.Width);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsHeightProp()
        {
            //Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 1234);
            //Assert
            Assert.Equal(1234, rect.Height);
        }


        [Fact]
        public void EqualsOperator_WhenInvoked_ReturnTrue()
        {
            //Arrange
            var rectA = new AtlasRegionRectangle("rect", 11, 22, 33, 44);
            var rectB = new AtlasRegionRectangle("rect", 11, 22, 33, 44);

            //Act
            var actual = rectA == rectB;

            //Assert
            Assert.True(actual);
        }


        [Fact]
        public void NotEqualsOperator_WhenInvoked_ReturnTrue()
        {
            //Arrange
            var rectA = new AtlasRegionRectangle("rect-A", 11, 22, 33, 44);
            var rectB = new AtlasRegionRectangle("rect-B", 55, 66, 77, 88);

            //Act
            var actual = rectA != rectB;

            //Assert
            Assert.True(actual);
        }


        [Fact]
        public void Equals_WhenParamObjectIsDifferentType_ReturnFalse()
        {
            //Arrange
            var rectA = new AtlasRegionRectangle("rect-A", 11, 22, 33, 44);
            object differentObject = new object();

            //Act
            var actual = rectA.Equals(differentObject);

            //Assert
            Assert.False(actual);
        }


        [Fact]
        public void Equals_WhenParamObjectSameType_ReturnFalse()
        {
            //Arrange
            var rectA = new AtlasRegionRectangle("rect", 11, 22, 33, 44);
            object rectB = new AtlasRegionRectangle("rect", 11, 22, 33, 44);

            //Act
            var actual = rectA.Equals(rectB);

            //Assert
            Assert.True(actual);
        }
    }
}
