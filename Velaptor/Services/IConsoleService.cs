// <copyright file="IConsoleService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;

/// <summary>
/// Represents the standard input, output, and error streams for console applications.
/// </summary>
internal interface IConsoleService
{
    /// <inheritdoc cref="Console.ForegroundColor"/>
    ConsoleColor ForegroundColor { get; set; }

    /// <inheritdoc cref="Console.Write(string)"/>
    void Write(string value);

    /// <inheritdoc cref="Console.WriteLine()"/>
    void WriteLine(string value);
}
