using OpenToolkit.Mathematics;
using Raptor.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RaptorTests.OpenGL
{
    public class QuadDataTesting
    {
        [Fact]
        public void Equals_WithEqualParam_ReturnsTrue()
        {
            //Arrange
            var quadA = new QuadData();
            var quadB = new QuadData();

            //Act
            var actual = quadA.Equals(quadB);

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
        {
            //Arrange
            var quadA = new QuadData();
            var quadB = new QuadData();

            //Act
            var actual = quadA == quadB;

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
        {
            //Arrange
            var quadA = new QuadData();
            var quadB = new QuadData()
            {
                Vertex1 = new VertexData()
                {
                    Vertex = new Vector3(11, 22, 33)
                }
            };

            //Act
            var actual = quadA != quadB;

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
        {
            //Arrange
            var quadA = new QuadData();
            var quadB = new object();

            //Act
            var actual = quadA.Equals(quadB);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenInvokedWithEqualParamOfSameType_ReturnsTrue()
        {
            //Arrange
            var quadA = new QuadData();
            object quadB = new QuadData();

            //Act
            var actual = quadA.Equals(quadB);

            //Assert
            Assert.True(actual);
        }
    }
}
