using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MP3Sharp;
using NVorbis;

namespace Raptor.Audio
{
    internal class OggSoundDecoder : ISoundDecoder<float>
    {
        private readonly IAudioDataStream<float> audioDataStream;
        private bool isDisposed;

        public OggSoundDecoder(IAudioDataStream<float> dataStream) => this.audioDataStream = dataStream;

        /// <inheritdoc/>
        public SoundData<float> LoadData(string fileName)
        {
            var result = default(SoundData<float>);

            this.audioDataStream.FileName = fileName;

            result.SampleRate = this.audioDataStream.SampleRate;
            result.Channels = this.audioDataStream.Channels;

            var dataResult = new List<float>();

            var buffer = new float[this.audioDataStream.Channels * this.audioDataStream.SampleRate]; // 200ms

            while (this.audioDataStream.ReadSamples(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            result.TotalSeconds = dataResult.Count / 4f / this.audioDataStream.SampleRate;

            switch (this.audioDataStream.Channels)
            {
                case 1:
                    result.Format = AudioFormat.Mono32Float;
                    break;
                case 2:
                    result.Format = AudioFormat.StereoFloat32;
                    break;
            }

            result.BufferData = dataResult.ToArray();

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OggSoundDecoder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
