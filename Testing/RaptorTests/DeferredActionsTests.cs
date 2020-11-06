// <copyright file="DeferredActionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests
{
    using System;
    using Raptor;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="DeferredActionsCollection"/> class.
    /// </summary>
    public class DeferredActionsTests
    {
        #region Method Tests
        [Fact]
        public void ExecuteAll_WhenInvoking_ExecutesAllActions()
        {
            // Arrange
            var actions = new DeferredActionsCollection();
            var expectedCount = 0;
            var expectedActionAExecuted = true;
            var expectedActionBExecuted = true;
            var actualActionAExecuted = false;
            var actualActionBExecuted = false;
            actions.Add(TestActionA);
            actions.Add(TestActionB);
            void TestActionA()
            {
                actualActionAExecuted = true;
            }

            void TestActionB()
            {
                actualActionBExecuted = true;
            }

            // Act
            actions.ExecuteAll();
            var actualCount = actions.Count;

            // Assert
            Assert.Equal(expectedActionAExecuted, actualActionAExecuted);
            Assert.Equal(expectedActionBExecuted, actualActionBExecuted);
            Assert.Equal(expectedCount, actualCount);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Count_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var actions = new DeferredActionsCollection();

            static void TestAction()
            {
            }

            var expected = 1;

            // Act
            actions.Add(TestAction);
            var actual = actions.Count;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsReadOnly_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var actions = new DeferredActionsCollection();
            var expected = false;

            // Act
            var actual = actions.IsReadOnly;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IndexProp_WhenGettingAndSettingValue_ReturnsCorrectAction()
        {
            // Arrange
            var actions = new DeferredActionsCollection();

            static void ActionA()
            {
            }

            static void ActionB()
            {
            }

            static void ActionC()
            {
            }

            actions.Add(ActionA);
            actions.Add(ActionB);

            var expected = (Action)ActionC;

            // Act
            actions[0] = ActionC;
            var actual = actions[0];

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
