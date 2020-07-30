using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using MP3Sharp;

namespace Raptor.Audio
{
    [ExcludeFromCodeCoverage]
    public class Mp3AudioDataStream : IAudioDataStream<byte>
    {
        private MP3Stream mp3Reader;
        private string fileName;
        private bool isDisposed;


        public string FileName
        {
            get => this.fileName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Empty File Name");

                if (!File.Exists(value))
                    throw new FileNotFoundException("The file was not found.", value);

                if (this.mp3Reader is null)
                {
                    this.mp3Reader = new MP3Stream(value);
                }
                else
                {
                    if (value != this.fileName)
                    {
                        this.mp3Reader.Dispose();
                        this.mp3Reader = new MP3Stream(value);
                    }
                }

                this.fileName = value;
            }
        }

        public int Channels
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                    throw new Exception("Empty filename");

                return mp3Reader.ChannelCount;
            }
        }

        public AudioFormat Format
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                    throw new Exception("Empty filename");

                return this.mp3Reader.Format == SoundFormat.Pcm16BitMono ? AudioFormat.Mono16: AudioFormat.Stereo16;
            }
        }

        public int SampleRate
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                    throw new Exception("Empty filename");

                return this.mp3Reader.Frequency;
            }
        }

        public int ReadSamples(byte[] buffer, int offset, int count)
        {
            if (string.IsNullOrEmpty(FileName))
                throw new Exception("Empty filename");

            return this.mp3Reader.Read(buffer, offset, count);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                    this.mp3Reader?.Dispose();

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
