// <copyright file="IInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    using Silk.NET.Input;

    /// <summary>
    /// Creates an input object that gives access to native system input hardware.
    /// </summary>
    internal interface IInputFactory
    {
        /// <summary>
        /// Creates a new input object.
        /// </summary>
        /// <returns>The input object to gain access to the system input.</returns>
        IInputContext CreateInput();
    }
}
