using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Raptor.OpenAL;

namespace Raptor.Audio
{
    [ExcludeFromCodeCoverage]
    public static class AudioDevice
    {
        private static IAudioDeviceManager deviceManager = AudioDeviceManager.GetInstance(new ALInvoker());

        public static string[] DeviceNames => deviceManager.DeviceNames;

        public static void ChangeDevice(string name) => deviceManager.ChangeDevice(name);
    }
}
