using OpenToolkit.Mathematics;
using Raptor;
using System.Drawing;
using Xunit;

namespace RaptorTests
{
    /// <summary>
    /// Unit tests to test the <see cref="ExtensionMethods"/> class.
    /// </summary>
    public class ExtensionMethodsTests
    {
        #region Method Tests
        [Fact]
        public void ForcePositive_WhenUsingNegativeValue_ReturnsPositiveValue()
        {
            //Act & Assert
            Assert.Equal(123f, (-123f).ForcePositive());
        }


        [Fact]
        public void ForcePositive_WhenUsingPositiveValue_ReturnsPositiveValue()
        {
            //Act & Assert
            Assert.Equal(123f, 123f.ForcePositive());
        }


        [Fact]
        public void ForceNegative_WhenUsingPositiveValue_ReturnsNegativeValue()
        {
            //Act & Assert
            Assert.Equal(-123f, 123f.ForceNegative());
        }


        [Fact]
        public void ForceNegative_WhenUsingNegativeValue_ReturnsNegativeValue()
        {
            //Act & Assert
            Assert.Equal(-123f, (-123f).ForceNegative());
        }


        [Fact]
        public void ToRadians_WhenInvoking_ReturnsCorrectValue()
        {
            //Act & Assert
            Assert.Equal(70710.06f, 1234.1234f.ToDegrees());
        }


        [Fact]
        public void RotateAround_WhenInvoked_ReturnsCorrectResult()
        {
            //Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            var angle = 13f;
            var expected = new Vector2(1.25290489f, -0.996605873f);

            //Act
            var actual = vectorToRotate.RotateAround(origin, angle);

            //Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }


        [Fact]
        public void RotateAround_WhenInvokedWithClockwiseFalse_ReturnsCorrectResult()
        {
            //Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            var angle = 45f;
            var expected = new Vector2(-2.07106781f, 5f);

            //Act
            var actual = vectorToRotate.RotateAround(origin, angle, false);

            //Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }


        [Fact]
        public void ToVector4_WhenInvoked_ReturnsCorrectResult()
        {
            //Arrange
            var color = Color.FromArgb(11, 22, 33, 44);
            var expected = new Vector4(22, 33, 44, 11);

            //Act
            var actual = color.ToVector4();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ToGLColor_WhenInvoked_ReturnsCorrectResult()
        {
            //Arrange
            var color = Color.FromArgb(11, 22, 33, 44);
            var expected = new Vector4(0.08627451f, 0.12941177f, 0.17254902f, 0.043137256f);

            //Act
            var actual = color.ToGLColor();

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void MapValue_WhenUsingFloatType_ReturnsCorrectResult()
        {
            //Arrange
            var testValue = 5f;

            //Act
            var actual = testValue.MapValue(0f, 10f, 0f, 21f);

            //Assert
            Assert.Equal(10.5f, actual);
        }


        [Fact]
        public void MapValue_WhenUsingByteValues_ReturnsCorrectResult()
        {
            //Arrange
            byte testValue = 5;

            //Act
            var actual = testValue.MapValue(0, 10, 0, 20);

            //Assert
            Assert.Equal(10, actual);
        }


        [Fact]
        public void MapValue_WhenInvokedWithIntegerType_ReturnsCorrectResult()
        {
            //Arrange
            var testValue = 500;

            //Act
            var actual = testValue.MapValue(0, 1_000, 0, 100_000);

            //Assert
            Assert.Equal(50_000, actual);
        }
    #endregion
}
}
