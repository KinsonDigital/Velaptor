// <copyright file="AssertExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnusedMember.Global
namespace VelaptorTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Provides helper methods for the <see cref="Xunit"/>'s <see cref="Assert"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AssertExtensions : Assert
    {
        private const string TableFlip = "(╯‵□′)╯︵┻━┻  ";

        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type) and that
        /// the exception message matches the given <paramref name="expectedMessage"/>.
        /// </summary>
        /// <typeparam name="T">The type of exception that the test is verifying.</typeparam>
        /// <param name="testCode">The code that will be throwing the expected exception.</param>
        /// <param name="expectedMessage">The expected message of the exception.</param>
        public static void ThrowsWithMessage<T>(Action testCode, string expectedMessage)
            where T : Exception
        {
            Assert.Equal(expectedMessage, Assert.Throws<T>(testCode).Message);
        }

        /// <summary>
        /// Asserts that the given test code does not throw the exception of type <typeparamref name="T"/> is not thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception to check for.</typeparam>
        /// <param name="testCode">The test code that should not throw the exception.</param>
        public static void DoesNotThrow<T>(Action testCode)
            where T : Exception
        {
            if (testCode is null)
            {
                Assert.True(false, $"{TableFlip}Cannot perform assertion with null '{testCode}' parameter.");
            }

            try
            {
                testCode();
            }
            catch (T)
            {
                Assert.True(false, $"{TableFlip}Expected the exception {typeof(T).Name} to not be thrown.");
            }
        }

        /// <summary>
        /// Asserts that the given <paramref name="testCode"/> does not throw a null reference exception.
        /// </summary>
        /// <param name="testCode">The test that should not throw an exception.</param>
        public static void DoesNotThrowNullReference(Action testCode)
        {
            if (testCode is null)
            {
                Assert.True(false, $"{TableFlip}Cannot perform assertion with null '{testCode}' parameter.");
            }

            try
            {
                testCode();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    Assert.True(false, $"{TableFlip}Expected not to raise a {nameof(NullReferenceException)} exception.");
                }
                else
                {
                    Assert.True(true);
                }
            }
        }

        /// <summary>
        /// Asserts that all of the individual <paramref name="expectedItems"/> and <paramref name="actualItems"/>
        /// are equal on a per item basis.
        /// </summary>
        /// <typeparam name="T">The type of item in the lists.</typeparam>
        /// <param name="expectedItems">The list of expected items.</param>
        /// <param name="actualItems">The list of actual items to compare to the expected items.</param>
        /// <remarks>
        ///     Will fail assertion when one item is null and the other is not.
        ///     Will fail assertion when the total number of <paramref name="expectedItems"/> does not match the total number of <paramref name="actualItems"/>.
        /// </remarks>
        public static void ItemsEqual<T>(IEnumerable<T> expectedItems, IEnumerable<T> actualItems)
            where T : IEquatable<T>
        {
            if (expectedItems is null && actualItems is not null)
            {
                Assert.True(
                false,
                $"{TableFlip}Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is null and the '{nameof(actualItems)}' is not null.");
            }

            if (expectedItems is not null && actualItems is null)
            {
                Assert.True(
                false,
                $"{TableFlip}Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is not null and the '{nameof(actualItems)}' is null.");
            }

            var expected = expectedItems as T[] ?? expectedItems.ToArray();
            var actual = actualItems as T[] ?? actualItems.ToArray();
            if (expected.Length != actual.Length)
            {
                throw new AssertActualExpectedException(
                    expected.Length,
                    actual.Length,
                    $"{TableFlip}The quantity of items for '{nameof(expectedItems)}' and '{nameof(actualItems)}' do not match.");
            }

            var expectedArrayItems = expected.ToArray();
            var actualArrayItems = actual.ToArray();

            for (var i = 0; i < expectedArrayItems.Length; i++)
            {
                if (expectedArrayItems[i] is null && actualArrayItems[i] is not null)
                {
                    throw new AssertActualExpectedException(
                        expectedArrayItems[i],
                        actualArrayItems[i],
                        $"{TableFlip}Both the expected and actual items must both be null or not null to be equal.\n\nThe expected item at index '{i}' is null and the actual item at index '{i}' is not null.");
                }

                if (expectedArrayItems[i] is not null && actualArrayItems[i] is null)
                {
                    throw new AssertActualExpectedException(
                        expectedArrayItems[i],
                        actualArrayItems[i],
                        $"{TableFlip}Both the expected and actual items must both be null or not null to be equal.\n\nThe expected item at index '{i}' is not null and the actual item at index '{i}' is null.");
                }

                if (expectedArrayItems[i].Equals(actualArrayItems[i]) is false)
                {
                    throw new AssertActualExpectedException(
                        expectedArrayItems[i] + "\n------------------------------------------------------------------------------------------------------------------------",
                        actualArrayItems[i],
                        $"{TableFlip}The expected and actual items at index '{i}' are not equal.");
                }
            }

            Assert.True(true);
        }

        /// <summary>
        /// Asserts that all of the individual <paramref name="expectedItems"/> and <paramref name="actualItems"/>
        /// are equal on an item to item basis that are at the same index between the arrays.
        /// </summary>
        /// <typeparam name="T">The type of item in the lists.</typeparam>
        /// <param name="expectedItems">The list of expected items.</param>
        /// <param name="actualItems">The list of actual items to compare to the expected items.</param>
        /// <param name="arrayRegions">
        ///     The list of regions within the array that describe the start, stop, and label of the region.
        /// </param>
        /// <remarks>
        ///     Will fail assertion when one item is null and the other is not.
        ///     Will fail assertion when the total number of <paramref name="expectedItems"/> does not match the total number of <paramref name="actualItems"/>.
        /// </remarks>
        public static void ItemsEqual<T>(
            IEnumerable<T> expectedItems,
            IEnumerable<T> actualItems,
            IEnumerable<(int start, int stop, string name)> arrayRegions)
            where T : IEquatable<T>
        {
            if (expectedItems is null && actualItems is not null)
            {
                Assert.True(
                false,
                $"{TableFlip}Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is null and the '{nameof(actualItems)}' is not null.");
            }

            if (expectedItems is not null && actualItems is null)
            {
                Assert.True(
                false,
                $"{TableFlip}Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is not null and the '{nameof(actualItems)}' is null.");
            }

            var expected = expectedItems as T[] ?? expectedItems.ToArray();
            var actual = actualItems as T[] ?? actualItems.ToArray();
            if (expected.Length != actual.Length)
            {
                throw new AssertActualExpectedException(
                    expected.Length,
                    actual.Length,
                    $"{TableFlip}The quantity of items for '{nameof(expectedItems)}' and '{nameof(actualItems)}' do not match.");
            }

            var expectedArrayItems = expected.ToArray();
            var actualArrayItems = actual.ToArray();

            for (var i = 0; i < expectedArrayItems.Length; i++)
            {
                if (expectedArrayItems[i] is null && actualArrayItems[i] is not null)
                {
                    throw new AssertActualExpectedException(
                        expectedArrayItems[i],
                        actualArrayItems[i],
                        $"{TableFlip}Both the expected and actual items must both be null or not null to be equal.\n\nThe expected item at index '{i}' is null and the actual item at index '{i}' is not null.");
                }

                if (expectedArrayItems[i] is not null && actualArrayItems[i] is null)
                {
                    throw new AssertActualExpectedException(
                        expectedArrayItems[i],
                        actualArrayItems[i],
                        $"{TableFlip}Both the expected and actual items must both be null or not null to be equal.\n\nThe expected item at index '{i}' is not null and the actual item at index '{i}' is null.");
                }

                if (expectedArrayItems[i].Equals(actualArrayItems[i]) is false)
                {
                    var sectionName = (from s in arrayRegions
                        where i >= s.start && i <= s.stop
                        select s.name).FirstOrDefault();

                    throw new AssertActualExpectedException(
                        expectedArrayItems[i],
                        actualArrayItems[i],
                        $"{TableFlip}The expected and actual items at index '{i}' are not equal in the '{sectionName}' of the array.");
                }
            }

            Assert.True(true);
        }

        /// <summary>
        /// Asserts that all of the given <paramref name="items"/> are <see langword="true"/> which is dictated
        /// by the given <paramref name="arePredicate"/> predicate.
        /// </summary>
        /// <typeparam name="T">The type of item in the list of items.</typeparam>
        /// <param name="items">The list of items to assert.</param>
        /// <param name="arePredicate">Fails the assertion when returning <see langword="false"/>.</param>
        public static void AllItemsAre<T>(IEnumerable<T> items, Predicate<T> arePredicate)
        {
            if (arePredicate is null)
            {
                Assert.True(false, $"{TableFlip}Cannot perform assertion with null '{arePredicate}' parameter.");
            }

            var itemsToCheck = items.ToArray();

            for (var i = 0; i < itemsToCheck.Length; i++)
            {
                if (arePredicate(itemsToCheck[i]))
                {
                    continue;
                }

                Assert.True(false, $"{TableFlip}The item '{itemsToCheck[i]}' at index '{i}' returned false with the '{nameof(arePredicate)}'");
            }
        }

        /// <summary>
        /// Verifies that all of the given <paramref name="items"/> in the collection pass as long as the
        /// <paramref name="assertAction"/> does not contain an assertion failure.  If the <paramref name="includePredicate"/>
        /// returns true, the item is checked.
        /// </summary>
        /// <param name="items">The items to check.</param>
        /// <param name="includePredicate">Runs the <paramref name="assertAction"/> if the predicate returns true.</param>
        /// <param name="assertAction">
        ///     Contains the code to perform an assertion.  If the action code returns, then the assertion has passed.
        /// </param>
        /// <typeparam name="T">The data type of the items.</typeparam>
        public static void AllIncluded<T>(IEnumerable<T> items, Predicate<T> includePredicate, Action<T> assertAction)
        {
            if (includePredicate is null)
            {
                Assert.True(false, $"{TableFlip}Cannot perform assertion with null '{nameof(includePredicate)}' parameter.");
            }

            if (assertAction is null)
            {
                Assert.True(false, $"{TableFlip}Cannot perform assertion with null '{nameof(assertAction)}' parameter.");
            }

            var itemsToCheck = items.ToArray();

            foreach (var t in itemsToCheck)
            {
                if (includePredicate(t))
                {
                    assertAction(t);
                }
            }
        }

        /// <summary>
        /// Asserts that all of the items between the <paramref name="expectedItems"/> and <paramref name="actualItems"/>
        /// arrays are equal for all of the items inclusively between <paramref name="indexStart"/> and <paramref name="indexStop"/>.
        /// </summary>
        /// <param name="expectedItems">The expected items.</param>
        /// <param name="actualItems">The actual items.</param>
        /// <param name="indexStart">The inclusive starting index of the range of items to check.</param>
        /// <param name="indexStop">The inclusive ending index of the range of items to check.</param>
        /// <exception cref="AssertActualExpectedException">
        ///     Thrown to fail the unit test if any of the items in the ranges between the arrays are not equal.
        /// </exception>
        public static void SectionEquals(float[] expectedItems, float[] actualItems, int indexStart, int indexStop)
        {
            if (expectedItems is null)
            {
                Assert.True(false,
                    $"The '{nameof(SectionEquals)}()' method param '{nameof(expectedItems)}' must not be null.");
            }

            if (actualItems is null)
            {
                Assert.True(false,
                    $"The '{nameof(SectionEquals)}()' method param '{nameof(actualItems)}' must not be null.");
            }

            if (expectedItems.Length - 1 < indexStart)
            {
                Assert.True(false,
                    $"The '{nameof(indexStart)}' value must be less than the '{nameof(expectedItems)}'.Length.");
            }

            if (expectedItems.Length - 1 < indexStop)
            {
                Assert.True(false,
                    $"The '{nameof(indexStop)}' value must be less than the '{nameof(actualItems)}'.Length.");
            }

            if (actualItems.Length - 1 < indexStart)
            {
                Assert.True(false,
                    $"The '{nameof(indexStart)}' value must be less than the '{nameof(expectedItems)}'.Length.");
            }

            if (actualItems.Length - 1 < indexStop)
            {
                Assert.True(false,
                    $"The '{nameof(indexStop)}' value must be less than the '{nameof(actualItems)}'.Length.");
            }

            for (var i = indexStart; i < indexStop; i++)
            {
                var failMessage = $"The items in both arrays are not equal at index '{i}'";

                if (Math.Abs(expectedItems[i] - actualItems[i]) != 0f)
                {
                    throw new AssertActualExpectedException(expectedItems[i], actualItems[i], failMessage);
                }
            }
        }

        /// <summary>
        /// Verifies that an expression is true.
        /// </summary>
        /// <param name="condition">The condition to be inspected.</param>
        /// <param name="message">The message to be shown when the condition is <see langword="false"/>.</param>
        /// <param name="expected">The expected message to display if the condition is <see langword="false"/>.</param>
        /// <param name="actual">The actual message to display if the condition is <see langword="false"/>.</param>
        public static void True(bool condition, string message, string expected = "", string actual = "")
        {
            XunitException assertException;

            if (!string.IsNullOrEmpty(expected) && string.IsNullOrEmpty(actual))
            {
                assertException = new XunitException(
                    $"Message: {message}\n" +
                    $"Expected: {expected}");
            }
            else if (string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                assertException = new XunitException(
                    $"Message: {message}\n" +
                    $"Actual: {actual}\n");
            }
            else if (!string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                assertException = new XunitException(
                    $"Message: {message}\n" +
                    $"Expected: {expected}\n" +
                    $"Actual:   {actual}");
            }
            else
            {
                assertException = new AssertActualExpectedException(
                    true,
                    condition,
                    message);
            }

            if (condition is false)
            {
                throw assertException;
            }
        }

        /// <summary>
        /// Verifies that all items in the collection pass when executed against the given action.
        /// </summary>
        /// <typeparam name="T">The type of object to be verified.</typeparam>
        /// <param name="collection">The 2-dimensional collection.</param>
        /// <param name="width">The width of the first dimension.</param>
        /// <param name="height">The height of the second dimension.</param>
        /// <param name="action">The action to test each item against.</param>
        /// <remarks>
        ///     The last 2 <see langword="in"/> parameters T2 and T3 of type <see langword="int"/> of the <paramref name="action"/>
        ///     is the X and Y location within the <paramref name="collection"/> that failed the assertion.
        /// </remarks>
        public static void All<T>(T[,] collection, uint width, uint height, Action<T, int, int> action)
        {
            var actionInvoked = false;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    actionInvoked = true;
                    action(collection[x, y], x, y);
                }
            }

            var userMessage = TableFlip;
            userMessage += $"{TableFlip}No assertions were actually made in {nameof(AssertExtensions)}.{nameof(All)}<T>.";
            userMessage += "  Are there any items?";

            Assert.True(actionInvoked, userMessage);
        }

        /// <summary>
        /// Verifies that all items in the collection pass when executed against the given action.
        /// </summary>
        /// <typeparam name="T">The type of object to be verified.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action to test each item against.</param>
        public static void All<T>(T[] collection, Action<T, int> action)
        {
            var actionInvoked = false;

            for (var i = 0; i < collection.Length; i++)
            {
                actionInvoked = true;
                action(collection[i], i);
            }

            var userMessage = TableFlip;
            userMessage += $"No assertions were actually made in {nameof(AssertExtensions)}.{nameof(All)}<T>.";
            userMessage += "  Are there any items?";

            Assert.True(actionInvoked, userMessage);
        }

        /// <summary>
        /// Verifies that the two integers are equivalent.
        /// </summary>
        /// <param name="expected">The expected <see langword="int"/> value.</param>
        /// <param name="actual">The actual <see langword="int"/> value.</param>
        /// <param name="message">The message to be shown about the failed assertion.</param>
        public static void EqualWithMessage(int expected, int actual, string message)
        {
            var assertException = new AssertActualExpectedException(expected, actual, $"{TableFlip}{message}");
            try
            {
                Equal(expected, actual);
            }
            catch (Exception)
            {
                throw assertException;
            }
        }

        /// <summary>
        /// Verifies that the two enums are equivalent.
        /// </summary>
        /// <param name="expected">The expected enum.</param>
        /// <param name="actual">The actual enum.</param>
        /// <param name="message">The message to be shown about the failed assertion.</param>
        public static void EqualWithMessage(Enum expected, Enum actual, string message)
        {
            var assertException = new AssertActualExpectedException(
                expected.ToString(),
                actual.ToString(),
                $"{TableFlip}{message}");

            try
            {
                Assert.True(expected.Equals(actual), string.IsNullOrEmpty(message) ? TableFlip : message);
            }
            catch (Exception)
            {
                throw assertException;
            }
        }

        /// <summary>
        /// Verifies if the <paramref name="expected"/> and <paramref name="actual"/> arguments are equal.
        /// </summary>
        /// <typeparam name="T">The <see cref="IEquatable{T}"/> type of the <paramref name="expected"/> and <paramref name="actual"/>.</typeparam>
        /// <param name="expected">The expected <see langword="int"/> value.</param>
        /// <param name="actual">The actual <see langword="int"/> value.</param>
        /// <param name="message">The message to be shown about the failed assertion.</param>
        public static void EqualWithMessage<T>(T? expected, T? actual, string message)
            where T : IEquatable<T>
        {
            try
            {
                Assert.True(expected.Equals(actual), string.IsNullOrEmpty(message) ? string.Empty : message);
            }
            catch (Exception)
            {
                var expectedStr = expected is null ? "NULL" : expected.ToString();
                var actualStr = actual is null ? "NULL" : actual.ToString();

                throw new AssertActualExpectedException(expectedStr, actualStr, $"{TableFlip}{message}");
            }
        }

        /// <summary>
        /// Verifies if the <paramref name="expected"/> and <paramref name="actual"/> arguments are equal.
        /// </summary>
        /// <typeparam name="T">
        ///     The <see cref="IEquatable{T}"/> type of the <paramref name="expected"/> and <paramref name="actual"/>.
        /// </typeparam>
        /// <param name="expected">The expected <see langword="int"/> value.</param>
        /// <param name="actual">The actual <see langword="int"/> value.</param>
        /// <param name="typeName">The name of the type that is being compared.</param>
        /// <param name="memberName">The name of the type member being compared.</param>
        public static void TypeMemberEquals<T>(T? expected, T? actual, string typeName, string memberName)
            where T : IEquatable<T>
        {
            var message = $"{typeName}.{memberName} incorrect.";

            try
            {
                Assert.True(expected.Equals(actual), string.IsNullOrEmpty(message) ? string.Empty : message);
            }
            catch (Exception)
            {
                var expectedStr = expected is null ? "NULL" : expected.ToString();
                var actualStr = actual is null ? "NULL" : actual.ToString();

                throw new AssertActualExpectedException(expectedStr, actualStr, $"{TableFlip}{message}");
            }
        }

        /// <summary>
        /// Verifies that an expression is true for a member of a type.
        /// </summary>
        /// <param name="condition">The condition to be expected.</param>
        /// <param name="typeName">The name of the type that contains the member.</param>
        /// <param name="memberName">The name of the member that is not <c>true</c>.</param>
        public static void TypeMemberTrue(bool condition, string typeName, string memberName)
        {
            var message = $"{TableFlip}{typeName}.{memberName} not true.";
            Assert.True(condition, message);
        }

        /// <summary>
        /// Verifies that an expression is false for a member of a type.
        /// </summary>
        /// <param name="condition">The condition to be expected.</param>
        /// <param name="typeName">The name of the type that contains the member.</param>
        /// <param name="memberName">The name of the member that is not <c>false</c>.</param>
        public static void TypeMemberFalse(bool condition, string typeName, string memberName)
        {
            var message = $"{TableFlip}{typeName}.{memberName} not true.";
            Assert.False(condition, message);
        }

        /// <summary>
        /// Verifies that an event with the exact event args is not raised.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments to expect.</typeparam>
        /// <param name="attach">Code to attach the event handler.</param>
        /// <param name="detach">Code to detach the event handler.</param>
        /// <param name="testCode">A delegate to the code to be tested.</param>
        public static void DoesNotRaise<T>(Action<EventHandler<T>> attach, Action<EventHandler<T>> detach, Action testCode)
            where T : EventArgs
        {
            try
            {
                Assert.Raises(attach, detach, testCode);
                Assert.Equal("No event was raised", "An event was raised.");
            }
            catch (Exception ex)
            {
                Assert.Equal("(No event was raised)\r\nEventArgs\r\n(No event was raised)", ex.Message);
            }
        }
    }
}
