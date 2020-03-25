using RaptorCore;
using Xunit;

namespace KDScorpionCoreTests
{
    /// <summary>
    /// Unit tests to test the <see cref="ExtensionMethods"/> class.
    /// </summary>
    public class ExtensionMethodTests
    {
        [Fact]
        public void ForcePositive_WhenUsingNegativeValue_ReturnsPositiveValue()
        {
            Assert.Equal(123f, (-123f).ForcePositive());
        }


        [Fact]
        public void ForcePositive_WhenUsingPositiveValue_ReturnsPositiveValue()
        {
            Assert.Equal(123f, 123f.ForcePositive());
        }


        [Fact]
        public void ForceNegative_WhenUsingPositiveValue_ReturnsNegativeValue()
        {
            Assert.Equal(-123f, 123f.ForceNegative());
        }


        [Fact]
        public void ForceNegative_WhenUsingNegativeValue_ReturnsNegativeValue()
        {
            Assert.Equal(-123f, (-123f).ForceNegative());
        }


        [Fact]
        public void ToRadians_WhenInvoking_ReturnsCorrectValue()
        {
            //Act/Assert
            Assert.Equal(70710.06f, 1234.1234f.ToDegrees());
        }


        [Fact]
        public void RotateAround_WhenInvoked_ReturnsCorrectResult()
        {
            //Arrange
            var vectorToRotate = new Vector(0, 0);
            var origin = new Vector(5, 5);
            var angle = 13f;
            var expected = new Vector(1.25290489f, -0.996605873f);

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
            var vectorToRotate = new Vector(0, 0);
            var origin = new Vector(5, 5);
            var angle = 45f;
            var expected = new Vector(-2.07106781f, 5f);

            //Act
            var actual = vectorToRotate.RotateAround(origin, angle, false);

            //Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }
    }
}
