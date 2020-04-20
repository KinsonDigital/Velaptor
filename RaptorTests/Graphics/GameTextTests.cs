using Moq;
using Raptor.Graphics;
using Raptor;
using Xunit;

namespace RaptorTests.Graphics
{
    /// <summary>
    /// Unit tests to test the <see cref="GameText"/> class.
    /// </summary>
    public class GameTextTests
    {
        #region Prop Tests
        [Fact]
        public void Text_WhenGettingAndSettingValue_ProperlyReturnsAndSetsInternalValue()
        {
            //Arrange
            var mockInternalText = new Mock<IText>();
            mockInternalText.SetupProperty(m => m.Text);

            var gameText = new GameText()
            {
                InternalText = mockInternalText.Object
            };
            var expected = "Hello World";

            //Act
            gameText.Text = "Hello World";
            var actual = gameText.Text;

            //Assert
            Assert.Equal(expected, actual);
            mockInternalText.VerifySet(p => p.Text = "Hello World", Times.Once());
        }


        [Fact]
        public void Width_WhenSettingValue_ProperlyReturnsInternalValue()
        {
            //Arrange
            var mockInternalText = new Mock<IText>();
            mockInternalText.Setup(m => m.Width).Returns(40);
            var gameText = new GameText()
            {
                InternalText = mockInternalText.Object
            };
            var expected = 40;

            //Act
            var actual = gameText.Width;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Height_WhenSettingValue_ProperlyReturnsInternalValue()
        {
            //Arrange
            var mockInternalText = new Mock<IText>();
            mockInternalText.Setup(m => m.Height).Returns(40);
            var gameText = new GameText()
            {
                InternalText = mockInternalText.Object
            };
            var expected = 40;

            //Act
            var actual = gameText.Height;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Color_WhenSettingValue_ProperlySetsInternalValue()
        {
            //Arrange
            var mockInternalText = new Mock<IText>();
            mockInternalText.SetupProperty(m => m.Color, new GameColor(0, 0, 0, 0));
            var gameText = new GameText()
            {
                InternalText = mockInternalText.Object
            };
            var expected = new GameColor(44, 11, 22, 33);

            //Act
            gameText.Color = new GameColor(44, 11, 22, 33);
            var actual = gameText.Color;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Color_WhenComparingColorsThatAreNotEqual_ReturnsFalse()
        {
            //Arrange
            var colorA = new GameColor(4, 1, 2, 3);
            var colorB = new GameColor(44, 11, 22, 33);

            //Act & Assert
            Assert.NotEqual(colorA, colorB);
        }
        #endregion


        #region Overloaded Operator Tests
        [Theory]
        [InlineData("Hello ", "World", "Hello World")]
        [InlineData(null, "World", "World")]
        [InlineData("Hello ", null, "Hello ")]
        [InlineData(null, null, "")]
        public void AddOperator_WhenAddingTwoGameTexts_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockTextA = new Mock<IText>();
            mockTextA.SetupProperty(m => m.Text);

            var mockTextB = new Mock<IText>();
            mockTextB.SetupProperty(m => m.Text);

            var textA = new GameText()
            {
                InternalText = mockTextA.Object,
                Text = stringA
            };
            var textB = new GameText()
            {
                InternalText = mockTextB.Object,
                Text = stringB
            };

            //Act
            var actual = textA + textB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData("Hello ", "World", "Hello World")]
        [InlineData(null, "World", "World")]
        [InlineData("Hello ", null, "Hello ")]
        [InlineData(null, null, "")]
        public void AddOperator_WhenAddingGameTextAndString_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockText = new Mock<IText>();
            mockText.SetupProperty(m => m.Text);

            var textA = new GameText()
            {
                InternalText = mockText.Object,
                Text = stringA
            };

            //Act
            var actual = textA + stringB;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData("Hello ", "World", "Hello World")]
        [InlineData(null, "World", "World")]
        [InlineData("Hello ", null, "Hello ")]
        [InlineData(null, null, "")]
        public void AddOperator_WhenAddingStringAndGameText_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockText = new Mock<IText>();
            mockText.SetupProperty(m => m.Text);

            var textB = new GameText()
            {
                InternalText = mockText.Object,
                Text = stringB
            };

            //Act
            var actual = stringA + textB;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
