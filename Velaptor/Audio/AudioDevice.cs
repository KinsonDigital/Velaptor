// <copyright file="AudioDevice.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Audio
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents audio devices in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AudioDevice
    {
        private static readonly IAudioDeviceManager DeviceManager = IoC.Container.GetInstance<IAudioDeviceManager>();

        /// <summary>
        /// Gets a list of devices in the system.
        /// </summary>
        public static ReadOnlyCollection<string> DeviceNames => new (DeviceManager.DeviceNames);

        /// <summary>
        /// Changes the device tot he given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the device to change to.</param>
        /// <exception cref="AudioDeviceManagerNotInitializedException">
        ///     Thrown if the audio device has not been initialized.
        /// </exception>"
        /// <exception cref="AudioDeviceDoesNotExistException">
        ///     Thrown if a device with the given <paramref name="name"/> does not exist.
        /// </exception>
        public static void ChangeDevice(string name)
        {
            if (!DeviceManager.IsInitialized)
            {
                DeviceManager.InitDevice();
            }

            DeviceManager.ChangeDevice(name);
        }
    }
}
