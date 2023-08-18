// <copyright file="AssertExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Execution;
using Xunit;

/// <summary>
/// Provides helper methods for the <see cref="Xunit"/>'s <see cref="Assert"/> class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
// ReSharper disable once ClassNeverInstantiated.Global
public class AssertExtensions : Assert
{
    private const string TableFlip = "(╯'□')╯︵┻━┻  ";

    /// <summary>
    /// Verifies that the exact exception type (not a derived exception type) is thrown and that
    /// the exception message matches the given <paramref name="expectedMessage"/>.
    /// </summary>
    /// <typeparam name="T">The type of exception that the test is verifying.</typeparam>
    /// <param name="testCode">The code that will be throwing the expected exception.</param>
    /// <param name="expectedMessage">The expected message of the exception.</param>
    public static void ThrowsWithMessage<T>(Action testCode, string expectedMessage)
        where T : Exception
    {
        Equal(expectedMessage, Throws<T>(testCode).Message);
    }

    /// <summary>
    /// Asserts that the given test code does not throw the exception of type <typeparamref name="T"/> is not thrown.
    /// </summary>
    /// <typeparam name="T">The type of exception to check for.</typeparam>
    /// <param name="testCode">The test code that should not throw the exception.</param>
    public static void DoesNotThrow<T>(Action? testCode)
        where T : Exception
    {
        if (testCode is null)
        {
            Fail($"{TableFlip}Cannot perform assertion with null '{testCode}' parameter.");
        }

        try
        {
            testCode();
        }
        catch (T)
        {
            Fail($"{TableFlip}Expected the exception {typeof(T).Name} to not be thrown.");
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
    /// <exception cref="AssertionFailedException">
    ///     Thrown to fail the unit test if any of the items in the ranges between the arrays are not equal.
    /// </exception>
    public static void SectionEquals(float[]? expectedItems, float[]? actualItems, int indexStart, int indexStop)
    {
        if (expectedItems is null)
        {
            Fail($"The '{nameof(SectionEquals)}()' method param '{nameof(expectedItems)}' must not be null.");
        }

        if (actualItems is null)
        {
            Fail($"The '{nameof(SectionEquals)}()' method param '{nameof(actualItems)}' must not be null.");
        }

        if (expectedItems.Length - 1 < indexStart)
        {
            Fail($"The '{nameof(indexStart)}' value must be less than the '{nameof(expectedItems)}'.Length.");
        }

        if (expectedItems.Length - 1 < indexStop)
        {
            Fail($"The '{nameof(indexStop)}' value must be less than the '{nameof(actualItems)}'.Length.");
        }

        if (actualItems.Length - 1 < indexStart)
        {
            Fail($"The '{nameof(indexStart)}' value must be less than the '{nameof(expectedItems)}'.Length.");
        }

        if (actualItems.Length - 1 < indexStop)
        {
            Fail($"The '{nameof(indexStop)}' value must be less than the '{nameof(actualItems)}'.Length.");
        }

        for (var i = indexStart; i < indexStop; i++)
        {
            var failMessage = $"The items in both arrays are not equal at index '{i}'";

            if (Math.Abs(expectedItems[i] - actualItems[i]) == 0f)
            {
                continue;
            }

            throw new AssertionFailedException($"{failMessage}\nExpected: {expectedItems[i]}\nActual: ${actualItems[i]}");
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
    ///     The last two <see langword="in"/> parameters T2 and T3 of type <see langword="int"/> of the <paramref name="action"/>
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

        True(actionInvoked, userMessage);
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

        True(actionInvoked, userMessage);
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
            True(expected.Equals(actual), string.IsNullOrEmpty(message) ? string.Empty : message);
        }
        catch (Exception)
        {
            var expectedStr = expected is null ? "NULL" : expected.ToString();
            var actualStr = actual is null ? "NULL" : actual.ToString();
            var exceptionMsg = $"{TableFlip}{message}\nExpected: ${expectedStr}\nActual: ${actualStr}";

            throw new AssertionFailedException(exceptionMsg);
        }
    }
}
