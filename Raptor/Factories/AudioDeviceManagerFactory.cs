// <copyright file="AudioDeviceManagerFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Audio;

    /// <summary>
    /// Creates a singleton instance of <see cref="AudioDeviceManager"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class AudioDeviceManagerFactory
    {
        /// <summary>
        /// Returns a singleton instance of an audio device manager.
        /// </summary>
        /// <returns>The device manager instance.</returns>
        public static IAudioDeviceManager CreateDeviceManager()
            => IoC.Container.GetInstance<IAudioDeviceManager>();
    }
}
