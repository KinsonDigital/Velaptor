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
        {
            setup.Callback(() =>
            {
                AssertExtensions.EqualWithMessage(
                    expectedOrder,
                    callOrder++,
                    $"Method '{methodName}' called out of order.");
            });
        }

        /// <summary>
        /// Specifies a callback to invoke when the method is called and then asserts that the call is in the correct order
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
        {
            setup.Callback(() =>
            {
                AssertExtensions.EqualWithMessage(
                    expectedOrder,
                    callOrder++,
                    $"Method '{methodName}' with ID '{id}' called out of order.");
            });
        }

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
        {
            mock.Verify(expression, Times.Never);
        }
    }
}
