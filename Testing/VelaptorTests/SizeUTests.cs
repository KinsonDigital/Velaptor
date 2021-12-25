using Velaptor;
using Xunit;

namespace VelaptorTests
{
    public class SizeUTests
    {
        #region Prop Tests
        [Fact]
        public void Width_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var size = default(SizeU);

            // Act
            size.Width = 1234;

            // Assert
            Assert.Equal(1234u, size.Width);
        }

        [Fact]
        public void Height_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var size = default(SizeU);

            // Act
            size.Height = 1234;

            // Assert
            Assert.Equal(1234u, size.Height);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(11u, 22u, true)]
        [InlineData(33u, 22u, false)]
        [InlineData(11u, 44u, false)]
        [InlineData(33u, 44u, false)]
        public void Equals_WithSizeUParam_ReturnsCorrectResult(uint width, uint height, bool expected)
        {
            // Arrange
            var sizeA = default(SizeU);
            sizeA.Width = 11;
            sizeA.Height = 22;

            var sizeB = default(SizeU);
            sizeB.Width = width;
            sizeB.Height = height;

            // Act
            var actual = sizeA.Equals(sizeB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(11u, 22u, true)]
        [InlineData(33u, 22u, false)]
        [InlineData(11u, 44u, false)]
        [InlineData(33u, 44u, false)]
        public void Equals_WithObjectParam_ReturnsCorrectResult(uint width, uint height, bool expected)
        {
            // Arrange
            var sizeA = default(SizeU);
            sizeA.Width = 11;
            sizeA.Height = 22;

            object sizeB = new SizeU { Width = width, Height = height };

            // Act
            var actual = sizeA.Equals(sizeB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equals_WithNullObjParam_ReturnsFalse()
        {
            // Arrange
            var sizeA = default(SizeU);
            sizeA.Width = 11;
            sizeA.Height = 22;

            object? sizeB = null;

            // Act
            var actual = sizeA.Equals(sizeB);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(11u, 22u, true)]
        [InlineData(33u, 22u, false)]
        [InlineData(11u, 44u, false)]
        [InlineData(33u, 44u, false)]
        public void EqualsOperator_WhenInvoked_ReturnsCorrectResult(uint width, uint height, bool expected)
        {
            // Arrange
            var sizeA = default(SizeU);
            sizeA.Width = 11;
            sizeA.Height = 22;

            var sizeB = default(SizeU);
            sizeB.Width = width;
            sizeB.Height = height;

            // Act
            var actual = sizeA == sizeB;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(11u, 22u, false)]
        [InlineData(33u, 22u, true)]
        [InlineData(11u, 44u, true)]
        [InlineData(33u, 44u, true)]
        public void NotEqualsOperator_WhenInvoked_ReturnsCorrectResult(uint width, uint height, bool expected)
        {
            // Arrange
            var sizeA = default(SizeU);
            sizeA.Width = 11;
            sizeA.Height = 22;

            var sizeB = default(SizeU);
            sizeB.Width = width;
            sizeB.Height = height;

            // Act
            var actual = sizeA != sizeB;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
