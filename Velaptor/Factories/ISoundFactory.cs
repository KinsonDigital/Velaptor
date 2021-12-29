// <copyright file="ISoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Content;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates sounds based on the sound file at a location.
    /// </summary>
    public interface ISoundFactory
    {
        /// <summary>
        /// Creates a new sound from a sound file at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to the sound file.</param>
        /// <returns>The sound.</returns>
        ISound CreateSound(string filePath);
    }
}
