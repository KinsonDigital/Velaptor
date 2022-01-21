// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates sounds based on the sound file at a location.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class SoundFactory : ISoundFactory
    {
        /// <inheritdoc/>
        ISound ISoundFactory.Create(string filePath) => new Sound(filePath);
    }
}
