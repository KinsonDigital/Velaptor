// <copyright file="EnsureThat.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Guards;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Performs analysis on particular values to ensure that they meet a criteria,
/// then invokes behavior based on a result.
/// </summary>
internal static class EnsureThat
{
    /// <summary>
    /// Throws a <see cref="NullReferenceException"/> if the given <paramref name="pointer"/> is a value of zero.
    /// </summary>
    /// <param name="pointer">The pointer to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="NullReferenceException">
    ///     Thrown if <paramref name="pointer"/> is a value of zero.
    /// </exception>
    [SuppressMessage("csharpsquid|should not be thrown by user code.", "S112", Justification = "Intentional")]
    public static void PointerIsNotNull(nint pointer, [CallerArgumentExpression("pointer")] string paramName = "")
    {
        if (pointer != nint.Zero)
        {
            return;
        }

        var exMsg = !string.IsNullOrEmpty(paramName)
            ? $"The pointer parameter '{paramName}' cannot be a value of zero."
            : "The pointer cannot be a value of zero.";

        throw new NullReferenceException(exMsg);
    }

    /// <summary>
    /// Throws a <see cref="NullReferenceException"/> if the given <paramref name="pointer"/> is a value of zero.
    /// </summary>
    /// <param name="pointer">The pointer to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="NullReferenceException">
    ///     Thrown if <paramref name="pointer"/> is a value of zero.
    /// </exception>
    [SuppressMessage("csharpsquid|should not be thrown by user code.", "S112", Justification = "Intentional")]
    public static void PointerIsNotNull(nuint pointer, [CallerArgumentExpression("pointer")] string paramName = "")
    {
        if (pointer != nuint.Zero)
        {
            return;
        }

        var exMsg = !string.IsNullOrEmpty(paramName)
            ? $"The pointer parameter '{paramName}' cannot be a value of zero."
            : "The pointer cannot be a value of zero.";

        throw new NullReferenceException(exMsg);
    }
}
