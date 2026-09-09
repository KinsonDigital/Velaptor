// <copyright file="ITextureIdGenerator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Generates texture ID values.
/// </summary>
public interface ITextureIdGenerator
{
    /// <summary>
    /// Generates unique, only used once per texture, ID value.
    /// </summary>
    /// <returns>The ID.</returns>
    uint GenerateNextId();
}
