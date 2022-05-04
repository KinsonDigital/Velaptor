// <copyright file="AppInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Input;

    // ReSharper restore RedundantNameQualifier

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
        public static IAppInput<KeyboardState> CreateKeyboard() => IoC.Container.GetInstance<IAppInput<KeyboardState>>();

        /// <summary>
        /// Creates a mouse object.
        /// </summary>
        /// <returns>The keyboard singleton object.</returns>
        public static IAppInput<MouseState> CreateMouse() => IoC.Container.GetInstance<IAppInput<MouseState>>();
    }
}
