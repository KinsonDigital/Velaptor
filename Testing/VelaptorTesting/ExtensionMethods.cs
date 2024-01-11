// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using Container = SimpleInjector.Container;

public static class ExtensionMethods
{
    /// <summary>
    /// Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
    /// a <typeparamref name="TService"/> is requested (transient).
    /// </summary>
    /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
    /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="lifeStyle">The lifestyle that specifies how the returned instance will be cached.</param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <remarks>
    ///     This method uses the container's LifestyleSelectionBehavior to select the exact
    ///     lifestyle for the specified type. By default this will be Transient.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void Register<TService, TImplementation>(this Container container, Lifestyle lifeStyle, bool suppressDisposal = false)
        where TService : class
        where TImplementation : class, TService
    {
        container.Register<TService, TImplementation>(lifeStyle);

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TService>(container);
        }
    }

    /// <summary>
    /// Registers the specified delegate that allows returning transient instances of
    /// <typeparamref name="TService" />. The delegate is expected to always return a new instance on
    /// each call.
    /// </summary>
    /// <remarks>
    /// This method uses the container's
    /// <see cref="P:SimpleInjector.ContainerOptions.LifestyleSelectionBehavior">LifestyleSelectionBehavior</see> to select
    /// the exact lifestyle for the specified type. By default this will be
    /// <see cref="F:SimpleInjector.Lifestyle.Transient">Transient</see>.
    /// </remarks>
    /// <typeparam name="TService">The interface or base type that can be used to retrieve instances.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="instanceCreator">The delegate that allows building or creating new instances.</param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown when this container instance is locked and cannot be altered, or when the
    /// <typeparamref name="TService" /> has already been registered.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown when <paramref name="instanceCreator" /> is a null reference.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void Register<TService>(this Container container, Func<TService> instanceCreator, bool suppressDisposal = false)
        where TService : class
    {
        container.Register(instanceCreator);

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TService>(container);
        }
    }

    /// <summary>
    /// Registers the specified delegate that allows returning transient instances of
    /// <typeparamref name="TConcrete" />. The delegate is expected to always return a new instance on
    /// each call.
    /// </summary>
    /// <remarks>
    /// This method uses the container's
    /// <see cref="P:SimpleInjector.ContainerOptions.LifestyleSelectionBehavior">LifestyleSelectionBehavior</see> to select
    /// the exact lifestyle for the specified type. By default this will be
    /// <see cref="F:SimpleInjector.Lifestyle.Transient">Transient</see>.
    /// </remarks>
    /// <typeparam name="TConcrete">The interface or base type that can be used to retrieve instances.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="lifeStyle">The lifestyle that specifies how the returned instance will be cached.</param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown when this container instance is locked and cannot be altered, or when the
    /// <typeparamref name="TConcrete" /> has already been registered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void Register<TConcrete>(this Container container, Lifestyle lifeStyle, bool suppressDisposal = false)
        where TConcrete : class
    {
        container.Register<TConcrete>(lifeStyle);

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TConcrete>(container);
        }
    }

    public static Size ToSize(this Vector2 value) => new ((int)value.X, (int)value.Y);

    public static Vector2 ToVector2(this Point value) => new (value.X, value.Y);
    public static Vector2 ToVector2(this Size value) => new (value.Width, value.Height);

    public static Point ToPoint(this Vector2 value) => new ((int)value.X, (int)value.Y);

    /// <summary>
    /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
    /// inherit from <see cref="IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type to suppress against.</typeparam>
    /// <param name="container">The container that the suppression applies to.</param>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
    private static void SuppressDisposableTransientWarning<T>(this Container container)
    {
        var registration = container.GetRegistration(typeof(T))?.Registration;
        registration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");
    }
}
