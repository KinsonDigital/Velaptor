// <copyright file="AssertHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides helper methods for the <see cref="XUnit"/>'s <see cref="Assert"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AssertHelpers
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
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

            if (!(expectedItems is null) && actualItems is null)
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

                if (!(expectedArrayItems[i] is null) && (actualArrayItems[i] is null))
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
        /// Asserts that all of the given <paramref name="items"/> are true which is dictacted
        /// by the given <paramref name="arePredicate"/> predicate.
        /// </summary>
        /// <typeparam name="T">The type of item in the list of items.</typeparam>
        /// <param name="items">The list of items to assert.</param>
        /// <param name="arePredicate">Fails the assertion when returning false.</param>
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
    }
}
