using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Raptor.Audio
{
    public struct SoundData<T> : IEquatable<SoundData<T>>
    {
        public T[] BufferData;

        public int SampleRate;

        public int Channels;

        public AudioFormat Format;

        public float TotalSeconds;

        public static bool operator ==(SoundData<T> left, SoundData<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SoundData<T> left, SoundData<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SoundData<T> data))
                return false;

            return Equals(data);
        }

        public bool Equals(SoundData<T> other)
            => Enumerable.SequenceEqual<T>(other.BufferData, BufferData) &&
            other.Channels == this.Channels &&
            other.Format == this.Format &&
            other.SampleRate == this.SampleRate &&
            other.TotalSeconds == this.TotalSeconds;

        public override int GetHashCode()
            => HashCode.Combine(
                this.BufferData,
                this.Channels,
                this.Format,
                this.SampleRate,
                this.TotalSeconds);
    }
}
