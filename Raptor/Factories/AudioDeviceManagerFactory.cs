// <copyright file="AudioDeviceManagerFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Audio;
    using Raptor.OpenAL;

    /// <summary>
    /// Creates an singleton instance of <see cref="AudioDeviceManager"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class AudioDeviceManagerFactory
    {
        /// <summary>
        /// Returns a singleton instance of an audio device manager.
        /// </summary>
        /// <returns>The device manager instance.</returns>
        public static IAudioDeviceManager CreateDeviceManager()
        {
            var alInvoker = IoC.Container.GetInstance<IALInvoker>();

            return AudioDeviceManager.GetInstance(alInvoker);
        }
    }
}
