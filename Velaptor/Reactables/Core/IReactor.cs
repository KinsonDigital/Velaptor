// <copyright file="IReactor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.Core;

using System;

/// <summary>
/// Provides a mechanism for receiving push-based notifications.
/// </summary>
/// <typeparam name="T">The information related to the notification.</typeparam>
public interface IReactor<in T>
{
    /// <summary>
    /// Provides the reactor with new data.
    /// </summary>
    /// <param name="value">The current notification information.</param>
    public void OnNext(T value);

    /// <summary>
    /// Notifies the reactor that the provider has finished sending push-based notifications.
    /// </summary>
    /// <remarks>
    ///     Will not be invoked more than once.
    /// </remarks>
    public void OnCompleted();

    /// <summary>
    /// Notifies the reactor that the provider has experiences an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    public void OnError(Exception error);
}
