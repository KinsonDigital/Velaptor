using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenToolkit.Audio.OpenAL;
using Raptor.Factories;
using Raptor.OpenAL;

namespace Raptor.Audio
{
    internal sealed class AudioManager : IAudioManager
    {
        private static AudioManager _instance = new AudioManager();
        private const string DeviceNamePrefix = "OpenAL Soft on ";// All device names returned are prefixed with this
        private static readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioManager)}' has already been destroyed.\nInvoked the '{nameof(AudioManager.Init)}()' to re-setup the device manager.";
        private static Dictionary<Guid, SoundSource>? _soundSources = new Dictionary<Guid, SoundSource>();
        private static List<SoundState>? _continuePlaybackCache = new List<SoundState>();
        private static ALDevice _device;
        private static ALContext _context;
        private static ALContextAttributes? _attributes;
        private static IALInvoker? _alInvoker;
        private static IALCInvoker? _alcInvoker;
        private static bool isDisposed;
        public event EventHandler? DeviceChanged;

        private AudioManager() { }

        public bool IsInitialized => !AudioIsNull() &&
            !InvokersAreNull();

        public string[] DeviceNames
        {
            get
            {
                if (isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                var result = _alcInvoker.GetString(_device, AlcGetStringList.AllDevicesSpecifier)
                    .Select(n => n.Replace(DeviceNamePrefix, "")).ToArray();

                return result;
            }
        }

        public void Init(string name = null)
        {
            if (_device.Handle == IntPtr.Zero)
                _device = _alcInvoker.OpenDevice(name == null ? null : $"{DeviceNamePrefix}{name}");

            if (_attributes is null)
                _attributes = new ALContextAttributes();

            if (_context.Handle == IntPtr.Zero)
                _context = _alcInvoker.CreateContext(_device, _attributes);

            _alcInvoker.MakeContextCurrent(_context);

            isDisposed = false;
        }

        public Guid CreateSoundID(string fileName)
        {
            if (isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            // TODO: Add isDiposed check and throw exception
            return CreateSoundID(fileName, Guid.Empty);
        }

        public Guid CreateSoundID(string fileName, Guid guid)
        {
            if (isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            Guid newId;

            if (guid == Guid.Empty)
                newId = Guid.NewGuid();
            else
                newId = guid;

            SoundSource soundSrc;
            soundSrc.SampleRate = 0;
            soundSrc.TotalTime = -1f;

            soundSrc.SourceId = _alInvoker.GenSource();
            soundSrc.BufferId = _alInvoker.GenBuffer();

            _soundSources.Add(newId, soundSrc);

            return newId;
        }

        public void UploadOggData(SoundStats<float> data, Guid soundId)
        {
            if (isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            if (!_soundSources.Keys.Contains(soundId))
                throw new Exception($"The sound id '{soundId}' does not exist.");

            var soundSrc = _soundSources[soundId];

            soundSrc.TotalTime = data.TotalSeconds;

            soundSrc.SampleRate = data.SampleRate;

            _alInvoker.BufferData(soundSrc.BufferId,
                            MapFormat(data.Format),
                            data.BufferData,
                            data.BufferData.Length * sizeof(float),
                            data.SampleRate);

            // Bind the buffer to the source
            _alInvoker.Source(soundSrc.SourceId, ALSourcei.Buffer, soundSrc.BufferId);

            _soundSources[soundId] = soundSrc;
        }

        public void UploadMp3Data(SoundStats<byte> data, Guid soundId)
        {
            if (isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            if (!_soundSources.Keys.Contains(soundId))
                throw new Exception($"The sound id '{soundId}' does not exist.");

            var soundSrc = _soundSources[soundId];

            soundSrc.TotalTime = data.TotalSeconds;

            soundSrc.SampleRate = data.SampleRate;

            _alInvoker.BufferData(soundSrc.BufferId,
                            MapFormat(data.Format),
                            data.BufferData,
                            data.BufferData.Length,
                            data.SampleRate);

            // Bind the buffer to the source
            _alInvoker.Source(soundSrc.SourceId, ALSourcei.Buffer, soundSrc.BufferId);

            _soundSources[soundId] = soundSrc;
        }

        public void PlaySound(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if guid exists and throw exception if it does not
            var soundSrc = _soundSources[soundId];

            _alInvoker.SourcePlay(soundSrc.SourceId);
        }

        public void PauseSound(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if guid exists and throw exception if it does not
            var soundSrc = _soundSources[soundId];

            _alInvoker.SourceStop(soundSrc.SourceId);
        }

        public void StopSound(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if guid exists and throw exception if it does not
            var soundSrc = _soundSources[soundId];

            _alInvoker.SourceStop(soundSrc.SourceId);
        }

        public void ResetSound(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if guid exists and throw exception if it does not
            var soundSrc = _soundSources[soundId];

            _alInvoker.SourceRewind(soundSrc.SourceId);
        }

        public bool IsSoundLooping(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO:  Check if the soundId exists first and if not, throw an exception
            var soundSrc = _soundSources[soundId];

            return _alInvoker.GetSource(soundSrc.SourceId, ALSourceb.Looping);
        }

        public void SetLooping(Guid soundId, bool value)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if guid exists and if it does not, throw an exceptoin

            var soundSrc = _soundSources[soundId];

            _alInvoker.Source(soundSrc.SourceId, ALSourceb.Looping, value);
        }

        public float GetVolume(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO:  Check if the soundId exists first and if not, throw an exception
            var soundSrc = _soundSources[soundId];

            var result = _alInvoker.GetSource(soundSrc.SourceId, ALSourcef.Gain);

            return result * 100f;
        }

        public void SetVolume(Guid soundId, float value)
        // TODO: Add isDiposed check and throw exception
        {
            /*NOTE:
                Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ?]
                A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
                attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
                A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
                as zero volume - the channel is effectively disabled.
            */

            // TODO:  Check if the soundId exists first and if not, throw an exception
            var soundSrc = _soundSources[soundId];

            value = value > 100f ? 100f : value;
            value = value < 0f ? 0f : value;

            value /= 100f;

            _alInvoker.Source(soundSrc.SourceId, ALSourcef.Gain, value);
        }

        public float GetTimePosition(Guid soundId)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if the sourceId exists first and if not, throw an exception
            var soundSrc = _soundSources[soundId];

            var sampleOffset = _alInvoker.GetSource(soundSrc.SourceId, ALGetSourcei.SampleOffset);

            return sampleOffset / (float)soundSrc.SampleRate;
        }

        public void SetTimePosition(Guid soundId, float seconds)
        {
            // TODO: Add isDiposed check and throw exception
            //TODO: Check if the sourceId exists first and if not, throw an exception

            // Prevent negative number
            seconds = seconds < 0f ? 0.0f : seconds;

            var soundSrc = _soundSources[soundId];

            seconds = seconds > soundSrc.TotalTime ? soundSrc.TotalTime : seconds;

            _alInvoker.Source(soundSrc.SourceId, ALSourcef.SecOffset, seconds);
        }

        public void ChangeDevice(string name)
        {
            // TODO: Add isDiposed check and throw exception
            if (!DeviceNames.Contains(name))
                throw new AudioDeviceDoesNotExistException();

            var availableDevices = DeviceNames;

            CacheSoundSources();

            DestroyDevice();
            Init(name);

            _soundSources.Clear();

            DeviceChanged?.Invoke(this, new EventArgs());

            // Reset all of the states such as if playing or paused and the current time position
            foreach (var cachedState in _continuePlaybackCache)
            {
                // TODO: if the sound source is not found, throw an exception
                var soundSrc = _soundSources[cachedState.SoundID];

                // Set the current position of the sound
                SetTimePosition(cachedState.SoundID, cachedState.TimePosition);

                // Set the state of the sound
                switch (cachedState.PlaybackState)
                {
                    case PlaybackState.Playing:
                        PlaySound(cachedState.SoundID);
                        break;
                    case PlaybackState.Paused:
                        PauseSound(cachedState.SoundID);
                        break;
                }
            }
        }

        public void UnloadSoundData(Guid soundId, bool preserveSound = false)
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if the sourceId exists first and if not, throw an exception

            var soundSrc = _soundSources[soundId];

            if (!preserveSound)
                _soundSources.Remove(soundId);

            _alInvoker.DeleteSource(soundSrc.SourceId);
            _alInvoker.DeleteBuffer(soundSrc.BufferId);
        }

        internal static AudioManager GetInstance(IALInvoker alInvoker, IALCInvoker alcInvoker)
        // TODO: Add isDiposed check and throw exception
        {
            if (_instance.IsInitialized)
                return _instance;

            if (_alInvoker is null)
            {
                _alInvoker = alInvoker;
                _alInvoker.ErrorCallback = ALErrorCallback;
            }

            if (_alcInvoker is null)
            {
                _alcInvoker = alcInvoker;
                _alcInvoker.ErrorCallback = ALCErrorCallback;
            }

            _instance.Init();

            return _instance;
        }

        private void CacheSoundSources()
        {
            // Create a cache of all the songs currently playing and record the current playback position
            // Cache only if the sound was currently playing or paused

            // Guarentee that the cache is clear
            _continuePlaybackCache.Clear();

            foreach (var soundSrcKVP in _soundSources)
            {
                var sourceState = _alInvoker.GetSourceState(soundSrcKVP.Value.SourceId);

                if (sourceState != ALSourceState.Playing && sourceState != ALSourceState.Paused)
                    continue;

                SoundState soundState;
                soundState.SoundID = soundSrcKVP.Key;
                soundState.PlaybackState = default;
                soundState.TimePosition = -1;

                if (sourceState == ALSourceState.Playing)
                {
                    soundState.PlaybackState = PlaybackState.Playing;
                    soundState.TimePosition = GetTimePosition(soundSrcKVP.Key);
                }
                else if (sourceState == ALSourceState.Paused)
                {
                    soundState.PlaybackState = PlaybackState.Paused;
                }

                _continuePlaybackCache.Add(soundState);
            }
        }

        private bool AudioIsNull() => _device == ALDevice.Null && _context == ALContext.Null && _attributes is null;

        private bool InvokersAreNull() => _alInvoker is null && _alcInvoker is null;

        private ALFormat MapFormat(AudioFormat format)
        {
            switch (format)
            {
                case AudioFormat.Mono8:
                    return ALFormat.Mono8;
                case AudioFormat.Mono16:
                    return ALFormat.Mono16;
                case AudioFormat.Mono32Float:
                    return ALFormat.MonoFloat32Ext;
                case AudioFormat.Stereo8:
                    return ALFormat.Stereo8;
                case AudioFormat.Stereo16:
                    return ALFormat.Stereo16;
                case AudioFormat.StereoFloat32:
                    return ALFormat.StereoFloat32Ext;
                default:
                    return default;
            }
        }

        private void DestroyDevice()
        {
            foreach (var soundId in _soundSources.Keys)
            {
                UnloadSoundData(soundId, true);
            }

            if (_context != ALContext.Null)
            {
                _alcInvoker.MakeContextCurrent(ALContext.Null);
                _alcInvoker.DestroyContext(_context);
            }

            _context = ALContext.Null;

            if (_device != ALDevice.Null)
                _alcInvoker.CloseDevice(_device);

            _device = ALDevice.Null;

            _attributes = null;
        }

        private static void ALErrorCallback(string errorMsg)
        {
            Debugger.Break();
        }

        private static void ALCErrorCallback(string errorMsg)
        {
            Debugger.Break();
        }

        public void Dispose()
        {
            _soundSources?.Clear();

            _continuePlaybackCache?.Clear();

            _device = ALDevice.Null;
            _context = ALContext.Null;
            _attributes = null;
            _alInvoker = null;
            _alcInvoker = null;

            _instance = new AudioManager();
            isDisposed = true;
        }
    }
}
