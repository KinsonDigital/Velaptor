// <copyright file="AppInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Input;

/// <summary>
/// Generates input type objects for processing input such as the keyboard and mouse.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AppInputFactory
{
    /// <summary>
    /// Creates a keyboard object.
    /// </summary>
    /// <returns>The keyboard singleton object.</returns>
    public static IAppInput<KeyboardState> CreateKeyboard() =>
        (UnitTestDetector.IsRunningFromUnitTest ? null : IoC.Container.GetInstance<IAppInput<KeyboardState>>()) !;

    /// <summary>
    /// Creates a mouse object.
    /// </summary>
    /// <returns>The keyboard singleton object.</returns>
    public static IAppInput<MouseState> CreateMouse() =>
        (UnitTestDetector.IsRunningFromUnitTest ? null : IoC.Container.GetInstance<IAppInput<MouseState>>()) !;
}
