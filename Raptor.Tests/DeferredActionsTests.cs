using System;
using Raptor;
using Xunit;

namespace KDScorpionCoreTests
{
    /// <summary>
    /// Unit tests to test the <see cref="DeferredActions"/> class.
    /// </summary>
    public class DeferredActionsTests
    {
        #region Method Tests
        [Fact]
        public void ExecuteAll_WhenInvoking_ExecutesAllActions()
        {
            //Arrange
            var actions = new DeferredActions();
            var expectedCount = 0;
            var expectedActionAExecuted = true;
            var expectedActionBExecuted = true;
            var actualActionAExecuted = false;
            var actualActionBExecuted = false;
            actions.Add(testActionA);
            actions.Add(testActionB);
            void testActionA() { actualActionAExecuted = true; }
            void testActionB() { actualActionBExecuted = true; }


            //Act
            actions.ExecuteAll();
            var actualCount = actions.Count;
            
            //Assert
            Assert.Equal(expectedActionAExecuted, actualActionAExecuted);
            Assert.Equal(expectedActionBExecuted, actualActionBExecuted);
            Assert.Equal(expectedCount, actualCount);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void Count_WhenGettingValue_ReturnsCorrectResult()
        {
            //Arrange
            var actions = new DeferredActions();
            void testAction() { }
            var expected = 1;

            //Act
            actions.Add(testAction);
            var actual = actions.Count;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void IsReadOnly_WhenGettingValue_ReturnsCorrectResult()
        {
            //Arrange
            var actions = new DeferredActions();
            var expected = false;

            //Act
            var actual = actions.IsReadOnly;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void IndexProp_WhenGettingAndSettingValue_ReturnsCorrectAction()
        {
            //Arrange
            var actions = new DeferredActions();
            void actionA() { }
            void actionB() { }
            void actionC() { }
            actions.Add(actionA);
            actions.Add(actionB);

            var expected = (Action)actionC;

            //Act
            actions[0] = actionC;
            var actual = actions[0];

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
