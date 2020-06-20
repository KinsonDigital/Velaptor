using Moq;
using Raptor.Graphics;
using Raptor;
using Xunit;
using System.Drawing;

namespace RaptorTests.Graphics
{
    /// <summary>
    /// Unit tests to test the <see cref="Text"/> class.
    /// </summary>
    public class RenderTextTests
    {
        #region Prop Tests
        [Fact]
        public void Text_WhenGettingAndSettingValue_ProperlyReturnsAndSetsInternalValue()
        {
            //Arrange
            var mockInternalText = new Mock<IText>();
            mockInternalText.SetupProperty(m => m.Text);

            var gameText = new RenderText();
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
            var gameText = new RenderText();
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
            var gameText = new RenderText();
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
            mockInternalText.SetupProperty(m => m.Color, Color.FromArgb(0, 0, 0, 0));
            var gameText = new RenderText();
            var expected = Color.FromArgb(44, 11, 22, 33);

            //Act
            gameText.Color = Color.FromArgb(44, 11, 22, 33);
            var actual = gameText.Color;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Color_WhenComparingColorsThatAreNotEqual_ReturnsFalse()
        {
            //Arrange
            var colorA = Color.FromArgb(4, 1, 2, 3);
            var colorB = Color.FromArgb(44, 11, 22, 33);

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
        public void AddOperator_WhenAddingTwoRenderTexts_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockTextA = new Mock<IText>();
            mockTextA.SetupProperty(m => m.Text);

            var mockTextB = new Mock<IText>();
            mockTextB.SetupProperty(m => m.Text);

            var textA = new RenderText()
            {
                Text = stringA
            };
            var textB = new RenderText()
            {
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
        public void AddOperator_WhenAddingRenderTextAndString_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockText = new Mock<IText>();
            mockText.SetupProperty(m => m.Text);

            var textA = new RenderText()
            {
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
        public void AddOperator_WhenAddingStringAndRenderText_ReturnsCorrectValue(string stringA, string stringB, string expected)
        {
            //Arrange
            var mockText = new Mock<IText>();
            mockText.SetupProperty(m => m.Text);

            var textB = new RenderText()
            {
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
