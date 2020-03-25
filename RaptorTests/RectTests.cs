using Raptor;
using Xunit;

namespace RaptorTests
{
    /// <summary>
    /// Unit tests to test the <see cref="Rect"/> class.
    /// </summary>
    public class RectTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokingWithXAndYAndWidthAndHeight_SetsXAndYProps()
        {
            //Arrange
            var expectedWidth = 11.22f;
            var expectedHeight = 33.44f;

            //Act
            var rect = new Rect(0, 0, 11.22f, 33.44f);
            var actualWidth = rect.Width;
            var actualHeight = rect.Height;

            //Assert
            Assert.Equal(expectedWidth, actualWidth);
            Assert.Equal(expectedHeight, actualHeight);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void X_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect();
            var expected = 11.22f;

            //Act
            rect.X = 11.22f;
            var actual = rect.X;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Y_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect();
            var expected = 11.22f;

            //Act
            rect.Y = 11.22f;
            var actual = rect.Y;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Position_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect();
            var expected = new Vector(11.22f, 33.44f);

            //Act
            rect.Position = new Vector(11.22f, 33.44f);
            var actual = rect.Position;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Width_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect();
            var expected = 11.22f;

            //Act
            rect.Width = 11.22f;
            var actual = rect.Width;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Height_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect();
            var expected = 11.22f;

            //Act
            rect.Height = 11.22f;
            var actual = rect.Height;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void HalfWidth_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange & Act
            var rect = new Rect
            {
                Width = 10.5f
            };

            //Assert
            Assert.Equal(5.25f, rect.HalfWidth);
        }


        [Fact]
        public void HalfHeight_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange & Act
            var rect = new Rect
            {
                Height = 10.5f
            };

            //Assert
            Assert.Equal(5.25f, rect.HalfHeight);
        }


        [Fact]
        public void Left_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect()
            {
                X = 11.22f
            };
            var expected = 11.22f;

            //Act
            var actual = rect.Left;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Right_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect()
            {
                X = 11.22f,
                Width = 100f
            };
            var expected = 111.22f;

            //Act
            var actual = rect.Right;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Top_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect()
            {
                Y = 11.22f
            };
            var expected = 11.22f;

            //Act
            var actual = rect.Top;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Bottom_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var rect = new Rect()
            {
                Y = 100f,
                Height = 50f
            };
            var expected = 150f;

            //Act
            var actual = rect.Bottom;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Method Tests
        [Fact]
        public void Contains_WhenContainingXAndY_ReturnsTrue()
        {
            //Arrange
            var rect = new Rect()
            {
                Width = 10f,
                Height = 10f
            };
            var expected = true;

            //Act
            var actual = rect.Contains(5.5f, 5.5f);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Contains_WhenNotContainingXAndY_ReturnsFalse()
        {
            //Arrange
            var rect = new Rect()
            {
                Width = 10f,
                Height = 10f
            };
            var expected = false;

            //Act
            var actual = rect.Contains(50.5f, 50.50f);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Contains_WhenContainingVector_ReturnsTrue()
        {
            //Arrange
            var rect = new Rect()
            {
                Width = 10f,
                Height = 10f
            };
            var expected = true;
            var location = new Vector(5.5f, 6.0f);

            //Act
            var actual = rect.Contains(location);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Contains_WhenNotContainingVector_ReturnsFalse()
        {
            //Arrange
            var rect = new Rect()
            {
                Width = 10f,
                Height = 10f
            };
            var expected = false;
            var location = new Vector(50.5f, 60.0f);

            //Act
            var actual = rect.Contains(location);

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
