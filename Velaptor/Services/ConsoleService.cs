// <copyright file="ConsoleService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the dotnet {nameof(System)}.{nameof(Console)} API.")]
internal sealed class ConsoleService : IConsoleService
{
    /// <inheritdoc/>
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    /// <inheritdoc/>
    public void Write(string value) => Console.Write(value);

    /// <inheritdoc/>
    public void WriteLine(string? value) => Console.WriteLine(value);
}
