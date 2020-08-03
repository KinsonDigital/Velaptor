using System.Diagnostics.CodeAnalysis;
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
