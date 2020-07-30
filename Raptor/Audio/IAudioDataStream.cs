using System;

namespace Raptor.Audio
{
    public interface IAudioDataStream<T> : IDisposable
    {
        string FileName { get; set; }

        int Channels { get; }

        AudioFormat Format { get; }

        int SampleRate { get; }

        int ReadSamples(T[] buffer, int offset, int count);
    }
}
