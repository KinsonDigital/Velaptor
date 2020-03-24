using Xunit;
using KDScorpionCore.Input;
using KDScorpionCore;

namespace KDScorpionCoreTests.Input
{
    /// <summary>
    /// Unit tests to test the <see cref="MouseInputState"/> class.
    /// </summary>
    public class MouseInputStateTests
    {
        [Fact]
        public void Position_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = new Vector(11, 22);

            //Act
            state.Position = new Vector(11, 22);
            var actual = state.Position;

            //Arrange
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ScrollWheelValue_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = 1234;

            //Act
            state.ScrollWheelValue = 1234;
            var actual = state.ScrollWheelValue;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void X_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = 1234;

            //Act
            state.X = 1234;
            var actual = state.X;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Y_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var eYpected = 1234;

            //Act
            state.Y = 1234;
            var actual = state.Y;

            //Assert
            Assert.Equal(eYpected, actual);
        }


        [Fact]
        public void LeftButtonDown_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = true;

            //Act
            state.LeftButtonDown = true;
            var actual = state.LeftButtonDown;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void RightButtonDown_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = true;

            //Act
            state.RightButtonDown = true;
            var actual = state.RightButtonDown;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void MiddleButtonDown_WhenSettingAndGettingValue_GetsAndSetsCorrectValue()
        {
            //Arrange
            var state = new MouseInputState();
            var expected = true;

            //Act
            state.MiddleButtonDown = true;
            var actual = state.MiddleButtonDown;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
