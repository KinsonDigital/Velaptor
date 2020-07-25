using System;

namespace Raptor.Audio
{
    internal interface IAudioDeviceManager : IDisposable
    {
        event EventHandler? DeviceChanged;

        string[] DeviceNames { get; }

        bool IsInitialized { get; }

        void InitDevice(string name = null);

        (int srcId, int bufferId) InitSound();

        void ChangeDevice(string name);

        void UpdateSoundSource(SoundSource soundSrc);
    }
}
