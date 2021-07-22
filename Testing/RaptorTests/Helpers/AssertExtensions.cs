// <copyright file="AssertExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit;
    using Xunit.Sdk;
    
    /// <summary>
    /// Provides helper methods for the <see cref="XUnit"/>'s <see cref="Assert"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AssertExtensions : Assert
    {
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
                throw new ArgumentNullException(nameof(testCode), "The parameter must not be null");
            }

            try
            {
                testCode();
            }
            catch (T)
            {
                Assert.True(false, $"Expected the exception {typeof(T).Name} to not be thrown.");
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
                throw new ArgumentNullException(nameof(testCode), "The parameter must not be null");
            }

            try
            {
                testCode();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    Assert.True(false, $"Expected not to raise a {nameof(NullReferenceException)} exception.");
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
            where T : class
        {
            if (expectedItems is null && !(actualItems is null))
            {
                Assert.True(false, $"Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is null and the '{nameof(actualItems)}' is not null.");
            }

            if (expectedItems is not null && actualItems is null)
            {
                Assert.True(false, $"Both lists must be null or not null to be equal.\nThe '{nameof(expectedItems)}' is not null and the '{nameof(actualItems)}' is null.");
            }

            if (expectedItems.Count() != actualItems.Count())
            {
                Assert.True(false, $"The quantity of items for '{nameof(expectedItems)}' and '{nameof(actualItems)}' do not match.");
            }

            var expectedArrayItems = expectedItems.ToArray();
            var actualArrayItems = actualItems.ToArray();

            for (var i = 0; i < expectedArrayItems.Length; i++)
            {
                if ((expectedArrayItems[i] is null) && !(actualArrayItems[i] is null))
                {
                    Assert.True(false, $"Both the expected and actual item must both be null or not null to be equal.\n\nThe expected item at index '{i}' is null and the actual item at index '{i}' is not null.");
                }

                if (expectedArrayItems[i] is not null && (actualArrayItems[i] is null))
                {
                    Assert.True(false, $"Both the expected and actual item must both be null or not null to be equal.\n\nThe expected item at index '{i}' is not null and the actual item at index '{i}' is null.");
                }

                if (expectedArrayItems[i] != actualArrayItems[i])
                {
                    Assert.True(false, $"The expected and actual item at index '{i}' are not equal.");
                }
            }

            Assert.True(true);
        }

        /// <summary>
        /// Asserts that all of the given <paramref name="items"/> are <see langword="true"/> which is dictacted
        /// by the given <paramref name="arePredicate"/> predicate.
        /// </summary>
        /// <typeparam name="T">The type of item in the list of items.</typeparam>
        /// <param name="items">The list of items to assert.</param>
        /// <param name="arePredicate">Fails the assertion when returning <see langword="false"/>.</param>
        public static void AllItemsAre<T>(IEnumerable<T> items, Predicate<T> arePredicate)
        {
            if (arePredicate is null)
            {
                throw new ArgumentNullException(nameof(arePredicate), "The parameter must not be null.");
            }

            var itemsToCheck = items.ToArray();

            for (var i = 0; i < itemsToCheck.Length; i++)
            {
                if (arePredicate(itemsToCheck[i]))
                {
                    continue;
                }

                Assert.True(false, $"The item '{itemsToCheck[i]}' at index '{i}' returned false with the '{nameof(arePredicate)}'");
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
            XunitException assertExcption;

            if (!string.IsNullOrEmpty(expected) && string.IsNullOrEmpty(actual))
            {
                assertExcption = new XunitException(
                    $"Message: {message}\n" +
                    $"Expected: {expected}");
            }
            else if (string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                assertExcption = new XunitException(
                    $"Message: {message}\n" +
                    $"Actual: {actual}\n");
            }
            else if (!string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                assertExcption = new XunitException(
                    $"Message: {message}\n" +
                    $"Expected: {expected}\n" +
                    $"Actual:   {actual}");
            }
            else
            {
                assertExcption = new AssertActualExpectedException(
                    true,
                    condition,
                    message);
            }

            if (condition is false)
            {
                throw assertExcption;
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

            Assert.True(actionInvoked, $"No assertions were actually made in {nameof(AssertExtensions)}.{nameof(All)}<T>.  Are there any items?");
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

            Assert.True(actionInvoked, $"No assertions were actually made in {nameof(AssertExtensions)}.{nameof(All)}<T>.  Are there any items?");
        }

        /// <summary>
        /// Verifies that the two integers are equivalent.
        /// </summary>
        /// <param name="expected">The expected <see langword="int"/> value.</param>
        /// <param name="actual">The actual <see langword="int"/> value.</param>
        /// <param name="message">The message to be shown about the failed assertion.</param>
        public static void Equals(int expected, int actual, string message)
        {
            var assertException = new AssertActualExpectedException(expected, actual, message);
            try
            {
                Assert.Equal(expected, actual);
            }
            catch (Exception)
            {
                throw assertException;
            }
        }

        /// <summary>
        /// Verifies that an event with the exact event args is not raised.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments to expect.</typeparam>
        /// <param name="attach">Code to attach the event handler.</param>
        /// <param name="detach">Code to detatch the event handler.</param>
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
