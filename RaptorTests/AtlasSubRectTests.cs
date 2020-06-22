using Moq;
using Raptor;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RaptorTests
{
    public class AtlasSubRectTests
    {
        [Fact]
        public void Ctor_WhenInvoked_SetsNameProp()
        {
            //Act
            var rect = new AtlasSubRect("test-name", It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.Equal("test-name", rect.Name);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsXProp()
        {
            //Act
            var rect = new AtlasSubRect(It.IsAny<string>(), 1234, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.Equal(1234, rect.X);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsYProp()
        {
            //Act
            var rect = new AtlasSubRect(It.IsAny<string>(), It.IsAny<int>(), 1234, It.IsAny<int>(), It.IsAny<int>());
            //Assert
            Assert.Equal(1234, rect.Y);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsWidthProp()
        {
            //Act
            var rect = new AtlasSubRect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 1234, It.IsAny<int>());
            //Assert
            Assert.Equal(1234, rect.Width);
        }


        [Fact]
        public void Ctor_WhenInvoked_SetsHeightProp()
        {
            //Act
            var rect = new AtlasSubRect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 1234);
            //Assert
            Assert.Equal(1234, rect.Height);
        }


        [Fact]
        public void EqualsOperator_WhenInvoked_ReturnTrue()
        {
            //Arrange
            var rectA = new AtlasSubRect("rect", 11, 22, 33, 44);
            var rectB = new AtlasSubRect("rect", 11, 22, 33, 44);

            //Act
            var actual = rectA == rectB;

            //Assert
            Assert.True(actual);
        }


        [Fact]
        public void NotEqualsOperator_WhenInvoked_ReturnTrue()
        {
            //Arrange
            var rectA = new AtlasSubRect("rect-A", 11, 22, 33, 44);
            var rectB = new AtlasSubRect("rect-B", 55, 66, 77, 88);

            //Act
            var actual = rectA != rectB;

            //Assert
            Assert.True(actual);
        }


        [Fact]
        public void Equals_WhenParamObjectIsDifferentType_ReturnFalse()
        {
            //Arrange
            var rectA = new AtlasSubRect("rect-A", 11, 22, 33, 44);
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
            var rectA = new AtlasSubRect("rect", 11, 22, 33, 44);
            object rectB = new AtlasSubRect("rect", 11, 22, 33, 44);

            //Act
            var actual = rectA.Equals(rectB);

            //Assert
            Assert.True(actual);
        }
    }
}
