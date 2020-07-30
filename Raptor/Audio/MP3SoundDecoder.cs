using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MP3Sharp;

namespace Raptor.Audio
{
    internal class MP3SoundDecoder : ISoundDecoder<byte>
    {
        private readonly IAudioDataStream<byte> audioDataStream;
        private bool isDisposed;

        public MP3SoundDecoder(IAudioDataStream<byte> dataStream) => this.audioDataStream = dataStream;

        public SoundData<byte> LoadData(string fileName)
        {
            //NOTE: the Mp3Sharp decoder library only deals with 16bit mp3 files.  Which is 99% of what is used now days anyways
            var result = new SoundData<byte>();

            this.audioDataStream.FileName = fileName;

            result.SampleRate = this.audioDataStream.SampleRate;
            result.Channels = this.audioDataStream.Channels;

            var dataResult = new List<byte>();

            const byte bitsPerSample = 16;
            const byte bytesPerSample = bitsPerSample / 8;

            var buffer = new byte[this.audioDataStream.Channels * this.audioDataStream.SampleRate * bytesPerSample];

            while (this.audioDataStream.ReadSamples(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            //This calculate is also not completely accurate.  It comes out to 1 second longer
            //thent he sound actually is.
            result.TotalSeconds = dataResult.Count / 4f / this.audioDataStream.SampleRate;

            result.Format = this.audioDataStream.Format;

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
        // ~MP3SoundDecoder()
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
