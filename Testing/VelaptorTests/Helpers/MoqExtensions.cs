// <copyright file="MoqExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System;
    using System.Linq.Expressions;
    using Moq;
    using Moq.Language.Flow;

    /// <summary>
    /// Provides extensions to the <see cref="Moq"/> library for ease of use and readability purposes.
    /// </summary>
    public static class MoqExtensions
    {
        private static int callOrder = 1;

        /// <summary>
        /// Specifies a callback to invoke when the method is called and then asserts that the call is in the correct order
        /// specified by the given <paramref name="expectedOrder"/>.
        /// </summary>
        /// <param name="setup">The mock setup to extend.</param>
        /// <param name="methodName">The name of the method expected to be invoked.</param>
        /// <param name="expectedOrder">The order sequence that the method should be invoked.</param>
        /// <typeparam name="T">The type that the setup is mocking.</typeparam>
        public static void CallbackInOrder<T>(this ISetup<T> setup, string methodName, int expectedOrder)
            where T : class
            => setup.Callback(() =>
            {
                AssertExtensions.EqualWithMessage(
                    expectedOrder,
                    callOrder++,
                    $"Method '{methodName}' called out of order.");
            });

        /// <summary>
        /// Specifies a callback to invoke when the method is called and then asserts that the call is in the correct order.
        /// specified by the given <paramref name="expectedOrder"/>.
        /// </summary>
        /// <param name="setup">The mock setup to extend.</param>
        /// <param name="methodName">The name of the method expected to be invoked.</param>
        /// <param name="id">
        ///     The ID to assign to the callback to help distinguish between
        ///     multiple calls to the same method with different parameter data.
        /// </param>
        /// <param name="expectedOrder">The order sequence that the method should be invoked.</param>
        /// <typeparam name="T">The type that the setup is mocking.</typeparam>
        public static void CallbackInOrder<T>(this ISetup<T> setup, string methodName, int id, int expectedOrder)
            where T : class
            => setup.Callback(() =>
            {
                AssertExtensions.EqualWithMessage(
                    expectedOrder,
                    callOrder++,
                    $"Method '{methodName}' with ID '{id}' called out of order.");
            });

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was never performed on the mock.
        /// Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was called when it was expected to never be called.
        /// </exception>
        public static void VerifyNever<T>(this Mock<T> mock, Expression<Action<T>> expression)
            where T : class
            => mock.Verify(expression, Times.Never);

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was only performed on the mock exactly one time.
        /// Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was called when it was expected to never be called.
        /// </exception>
        public static void VerifyOnce<T>(this Mock<T> mock, Expression<Action<T>> expression)
            where T : class
            => mock.Verify(expression, Times.Once);

        /// <summary>
        ///   Verifies that a specific invocation, matching the given expression, was performed on the mock.
        ///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was not called the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyOnce<T>(this Mock<T> mock, Expression<Action<T>> expression, Func<Times> times)
            where T : class
            => mock.Verify(expression, times);

        /// <summary>
        ///   Verifies that a property was set on the mock exactly one time.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="setterExpression">Expression to verify.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was not called exactly one time.
        /// </exception>
        public static void VerifySetOnce<T>(this Mock<T> mock, Action<T> setterExpression)
            where T : class
            => mock.VerifySet(setterExpression, Times.Once);

        /// <summary>
        ///   Verifies that a property was read on the mock exactly one time.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <typeparam name="TProperty">
        ///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
        /// </typeparam>
        /// <exception cref="MockException">
        ///   The invocation was not called exactly one time.
        /// </exception>
        public static void VerifyGetOnce<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression)
            where T : class
            => mock.VerifyGet(expression, Times.Once);

        /// <summary>
        ///   Verifies that an event was removed from the mock exactly once, specifying a failure message.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="removeExpression">Expression to verify.</param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was not called exactly once.
        /// </exception>
        public static void VerifyRemoveOnce<T>(this Mock<T> mock, Action<T> removeExpression, string failMessage = "")
            where T : class
            => mock.VerifyRemove(removeExpression, Times.Once(), failMessage);

        /// <summary>
        ///   Verifies that an event was added from the mock exactly once, specifying a failure message.
        /// </summary>
        /// <param name="mock">The mock object to extend.</param>
        /// <param name="removeExpression">Expression to verify.</param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
        /// <exception cref="MockException">
        ///   The invocation was not called exactly once.
        /// </exception>
        public static void VerifyAddOnce<T>(this Mock<T> mock, Action<T> removeExpression, string failMessage = "")
            where T : class
            => mock.VerifyAdd(removeExpression, Times.Once(), failMessage);
    }
}
