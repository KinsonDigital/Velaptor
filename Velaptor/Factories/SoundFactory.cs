// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content;

    /// <summary>
    /// Creates sounds based on the sound file at a location.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SoundFactory : ISoundFactory
    {
        /// <inheritdoc/>
        ISound ISoundFactory.CreateSound(string filePath) => new Sound(filePath);
    }
}
