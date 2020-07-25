using System;
using System.Collections.Generic;
using System.Text;
using MP3Sharp;
using NVorbis;

namespace Raptor.Audio
{
    internal class OggSoundDecoder : ISoundDecoder<float>
    {
        public SoundData<float> LoadData(string fileName)
        {
            var result = new SoundData<float>();

            using var reader = new VorbisReader(fileName);

            result.SampleRate = reader.SampleRate;
            result.TotalSeconds = (float)reader.TotalTime.TotalSeconds;
            result.Channels = reader.Channels;

            var dataResult = new List<float>();

            float[] buffer = new float[reader.Channels * reader.SampleRate]; // 200ms

            while (reader.ReadSamples(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            switch (reader.Channels)
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
    }
}
