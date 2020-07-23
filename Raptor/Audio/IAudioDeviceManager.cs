using System;

namespace Raptor.Audio
{
    public interface IAudioDeviceManager : IDisposable
    {
        event EventHandler? DeviceChanged;

        Guid CreateSoundID(string fileName, Guid guid = default);

        bool IsSoundRepeating(Guid soundId);

        void RepeatSound(Guid soundId, bool value);

        void SetVolume(Guid soundId, float value);

        float GetVolume(Guid soundId);

        float GetTimePosition(Guid soundId);

        void SetSoundTimePosition(Guid soundId, float seconds);

        void UploadData(string fileName, Guid soundId);

        void UnloadSoundData(Guid soundId, bool preserveSound = false);

        string[] DeviceNames { get; }

        bool IsInitialized { get; }

        void ChangeDevice(string name);

        void Dispose();

        void Init(string name = null);

        void PlaySound(Guid soundId);

        void PauseSound(Guid soundId);

        void StopSound(Guid soundId);

        void ResetSound(Guid soundId);
    }
}
