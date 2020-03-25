using Xunit;

namespace Raptor
{
    /// <summary>
    /// Unit tests to test the <see cref="Vector"/> class.
    /// </summary>
    public class VectorTests
    {
        #region Prop Tests
        [Fact]
        public void IsZero_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var expected = new Vector(0, 0);

            //Act
            var actual = Vector.Zero;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Method Tests
        [Fact]
        public void IsZero_WhenInvokingWithXAndYZero_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(0, 0);
            var expected = true;

            //Act
            var actual = vector.IsZero();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void IsZero_WhenInvokingWithXNotZeroAndYZero_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(10, 0);
            var expected = false;

            //Act
            var actual = vector.IsZero();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Cross_UsingSingleVectorParam_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);

            var expected = -2;
            
            //Act
            var actual = vector.Cross(new Vector(3, 5));

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Cross_UsingStaticMethod_ReturnsCorrectValue()
        {
            //Arrange
            var vectorA = new Vector(4, 2);
            var vectorB = new Vector(5, 3);

            var expected = 2;

            //Act
            var actual = Vector.Cross(vectorA, vectorB);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void GetLength_WhenInvoking_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);

            var expected = 4.472136f;

            //Act
            var actual = Vector.GetLength(vector);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Equals_WhenInvoking_ReturnsAsEqual()
        {
            //Arrange
            var vectorA = new Vector(2, 4);
            var vectorB = new Vector(2, 4);

            var expected = true;

            //Act
            var actual = vectorA == vectorB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void GetHashCode_WhenInvoking_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);
            var expected = 2002245821;

            //Act
            var actual = vector.GetHashCode();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ToString_WhenInvoking_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);
            var expected = "X: 2, Y: 4";

            //Act
            var actual = vector.ToString();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ToString_WhenInvokingWithRoundParam_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2.34567f, 4.56789f);
            var expected = "X: 2.35, Y: 4.57";

            //Act
            var actual = vector.ToString(2);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Normalize_WhenInvoking_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);
            var expected = new Vector(0.4472136f, 0.8944272f);
            
            //Act
            var actual = Vector.Normalize(vector);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Negate_WhenInvoking_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(2, 4);
            var expected = new Vector(-2, -4);

            //Act
            var actual = Vector.Negate(vector);

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Overloaded Operator Tests
        [Fact]
        public void SubtractOperator_WhenSubtracting2Vectors_ReturnsCorrectValue()
        {
            //Arrange
            var vectorA = new Vector(2, 6);
            var vectorB = new Vector(4, 9);

            var expected = new Vector(-2, -3);

            //Act
            var actual = vectorA - vectorB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void SubtractOperator_WhenSubtractingVectorAndScalar_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(3, 6);
            var scalar = 3;

            var expected = new Vector(0, 3);

            //Act
            var actual = vector - scalar;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void AdditionOperator_WhenAdding2Vectors_ReturnsCorrectValue()
        {
            //Arrange
            var vectorA = new Vector(2, 6);
            var vectorB = new Vector(4, 9);

            var expected = new Vector(6, 15);

            //Act
            var actual = vectorA + vectorB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void AdditionOperator_WhenAddingVectorAndScalar_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(3, 6);
            var scalar = 3;

            var expected = new Vector(6, 9);

            //Act
            var actual = vector + scalar;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void MultiplicationOperator_WhenMultiplying2Vectors_ReturnsCorrectValue()
        {
            //Arrange
            var vectorA = new Vector(2, 6);
            var vectorB = new Vector(4, 9);

            var expected = new Vector(8, 54);

            //Act
            var actual = vectorA * vectorB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void MultiplicationOperator_WhenMultiplyingVectorAndScalar_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(3, 6);
            var scalar = 3;

            var expected = new Vector(9, 18);

            //Act
            var actual = vector * scalar;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void MultiplicationOperator_WhenMultiplyingScalarAndVector_ReturnsCorrectValue()
        {
            //Arrange
            var vector = new Vector(3, 6);
            var scalar = 30;

            var expected = new Vector(90, 180);

            //Act
            var actual = scalar * vector;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void DivisionOperator_WhenDividingVectorAndScalar_ReturnsCorrectValue()
        {
            //Arrange
            var vectorA = new Vector(44, 90);
            var scalar = 2;

            var expected = new Vector(22, 45);

            //Act
            var actual = vectorA / scalar;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void NotEqualOperator_WhenComparing2UnequalVectors_ReturnsAsNotEqual()
        {
            //Arrange
            var vectorA = new Vector(2, 6);
            var vectorB = new Vector(4, 9);

            var expected = true;

            //Act
            var actual = vectorA != vectorB;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
