using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using NVorbis;

namespace Raptor.Audio
{
    [ExcludeFromCodeCoverage]
    public class OggAudioDataStream : IAudioDataStream<float>
    {
        private VorbisReader vorbisReader;
        private string fileName;
        private bool isDisposed;

        public string Filename
        {
            get => this.fileName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Empty File Name");

                if (!File.Exists(value))
                    throw new FileNotFoundException("The file was not found.", value);

                if (this.vorbisReader is null)
                {
                    this.vorbisReader = new VorbisReader(value);
                }
                else
                {
                    if (value != this.fileName)
                    {
                        this.vorbisReader.Dispose();
                        this.vorbisReader = new VorbisReader(value);
                    }
                }

                this.fileName = value;
            }
        }

        public int Channels
        {
            get
            {
                if (string.IsNullOrEmpty(Filename))
                    throw new Exception("Empty filename");

                return vorbisReader.Channels;
            }
        }

        public AudioFormat Format
        {
            get
            {
                if (string.IsNullOrEmpty(Filename))
                    throw new Exception("Empty filename");

                return Channels == 1 ? AudioFormat.Mono32Float : AudioFormat.StereoFloat32;
            }
        }

        public int SampleRate
        {
            get
            {
                if (string.IsNullOrEmpty(Filename))
                    throw new Exception("Empty filename");

                return this.vorbisReader.SampleRate;
            }
        }

        public int ReadSamples(float[] buffer, int offset, int count)
        {
            if (string.IsNullOrEmpty(Filename))
                throw new Exception("Empty filename");

            return this.vorbisReader.ReadSamples(buffer, offset, count);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                    this.vorbisReader?.Dispose();

                isDisposed = true;
            }
        }
    }
}
