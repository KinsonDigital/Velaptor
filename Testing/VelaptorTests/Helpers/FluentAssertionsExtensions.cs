// <copyright file="FluentAssertionsExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Specialized;

/// <summary>
/// Provides extensions to the <see cref="FluentAssertions"/> library.
/// </summary>
public static class FluentAssertionsExtensions
{
    /// <summary>
    /// Asserts that the current <see cref="Delegate" /> throws an exception of type <see name="ArgumentException"/>.
    /// </summary>
    /// <param name="actionAssertions">
    /// Contains a number of methods to assert that an <see cref="Action"/> yields the expected result.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <returns>The exception assertion object.</returns>
    public static ExceptionAssertions<ArgumentException> ThrowArgException(
        this ActionAssertions actionAssertions,
        string because = "",
        params object[] becauseArgs) =>
        actionAssertions.Throw<ArgumentException>(because, becauseArgs);

    /// <summary>
    /// Asserts that the current <see cref="Delegate" /> throws an exception of type <see name="ArgumentException"/>.
    /// </summary>
    /// <param name="functionAssertions">
    /// Contains a number of methods to assert that a synchronous function yields the expected result.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <typeparam name="T">The type returned by the <see cref="Func{T}"/>.</typeparam>
    /// <returns>The exception assertion object.</returns>
    public static ExceptionAssertions<ArgumentException> ThrowArgException<T>(
        this FunctionAssertions<T> functionAssertions,
        string because = "",
        params object[] becauseArgs) =>
        functionAssertions.Throw<ArgumentException>(because, becauseArgs);

    /// <summary>
    /// Asserts that the current <see cref="Delegate" /> throws an exception of type <see name="ArgumentNullException"/>.
    /// </summary>
    /// <param name="actionAssertions">
    /// Contains a number of methods to assert that an <see cref="Action"/> yields the expected result.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <returns>The exception assertion object.</returns>
    public static ExceptionAssertions<ArgumentNullException> ThrowArgNullException(
        this ActionAssertions actionAssertions,
        string because = "",
        params object[] becauseArgs) =>
        actionAssertions.Throw<ArgumentNullException>(because, becauseArgs);

    /// <summary>
    /// Asserts that the current <see cref="Delegate" /> throws an exception of type <see name="ArgumentNullException"/>.
    /// </summary>
    /// <param name="functionAssertions">
    /// Contains a number of methods to assert that a synchronous function yields the expected result.
    /// </param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    /// <typeparam name="T">The type returned by the <see cref="Func{T}"/>.</typeparam>
    /// <returns>The exception assertion object.</returns>
    public static ExceptionAssertions<ArgumentNullException> ThrowArgNullException<T>(
        this FunctionAssertions<T> functionAssertions,
        string because = "",
        params object[] becauseArgs) =>
        functionAssertions.Throw<ArgumentNullException>(because, becauseArgs);

    /// <summary>
    /// Asserts that the thrown exception has a message that matches the expected dotnet empty string parameter exception message.
    /// </summary>
    /// <param name="exAssertion">
    /// Contains a number of methods to assert that an <see cref="Exception" /> is in the correct state.
    /// </param>
    /// <param name="paramName">The name of the parameter that is being asserted.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    /// <returns>The exception assertion object.</returns>
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Ignored for possible future use.")]
    public static ExceptionAssertions<ArgumentException> WithEmptyStringParamMsg(
        this ExceptionAssertions<ArgumentException> exAssertion,
        string paramName,
        string because = "",
        params object[] becauseArgs) =>
        exAssertion.WithMessage($"The value cannot be an empty string. (Parameter '{paramName}')", because, becauseArgs);

    /// <summary>
    /// Asserts that the thrown exception has a message that matches the expected dotnet null parameter exception message.
    /// </summary>
    /// <param name="exAssertion">
    /// Contains a number of methods to assert that an <see cref="Exception" /> is in the correct state.
    /// </param>
    /// <param name="paramName">The name of the parameter that is being asserted.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
    /// </param>
    /// <returns>The exception assertion object.</returns>
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Ignored for possible future use.")]
    public static ExceptionAssertions<ArgumentNullException> WithNullParamMsg(
        this ExceptionAssertions<ArgumentNullException> exAssertion,
        string paramName,
        string because = "",
        params object[] becauseArgs) =>
        exAssertion.WithMessage($"Value cannot be null. (Parameter '{paramName}')", because, becauseArgs);
}
