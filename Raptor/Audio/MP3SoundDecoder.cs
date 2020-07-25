using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3Sharp;

namespace Raptor.Audio
{
    internal class MP3SoundDecoder : ISoundDecoder<byte>
    {
        public SoundData<byte> LoadData(string fileName)
        {
            //NOTE: the Mp3Sharp decoder library only deals with 16bit mp3 files.  Which is 99% of what is used now days anyways
            var result = new SoundData<byte>();

            using var reader = new MP3Stream(fileName);

            result.SampleRate = reader.Frequency;
            result.Channels = reader.ChannelCount;

            var dataResult = new List<byte>();

            const byte bitsPerSample = 16;
            const byte bytesPerSample = bitsPerSample / 8;

            byte[] buffer = new byte[reader.ChannelCount * reader.Frequency * bytesPerSample];

            while (reader.Read(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            //TODO: Need to test this out with 8 bit.
            //Will probably have to use the constant 4f for 16bit and 2f for 8bit

            //This calculate is also not completely accurate.  It comes out to 1 second longer
            //thent he sound actually is.
            result.TotalSeconds = dataResult.Count / 4f / reader.Frequency;

            if (reader.Format == SoundFormat.Pcm16BitMono)
            {
                result.Format = AudioFormat.Mono16;
            }
            else if (reader.Format == SoundFormat.Pcm16BitStereo)
            {
                result.Format = AudioFormat.Stereo16;
            }

            result.BufferData = dataResult.ToArray();

            return result;
        }
    }
}
