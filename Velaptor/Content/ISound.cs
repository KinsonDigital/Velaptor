// <copyright file="ISound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using ICASLSound = CASL.ISound;

    /// <summary>
    /// A single sound that can be played, paused etc.
    /// </summary>
    public interface ISound : ICASLSound, IContent
    {
        /// <summary>
        /// Gets the unique ID of the sound.
        /// </summary>
        uint Id { get; }
    }
}
