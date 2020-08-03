using System;
using System.IO;
using System.Linq;
using Raptor.Audio;
using Raptor.OpenAL;

namespace Raptor.Content
{
    public class SoundLoader : ILoader<ISound>
    {
        private readonly IALInvoker alInvoker;
        private readonly IContentSource contentSource;
        private readonly ISoundDecoder<float> oggDecoder;
        private readonly ISoundDecoder<byte> mp3Decoder;

        public SoundLoader(IContentSource contentSource, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.alInvoker = new ALInvoker();
            this.contentSource = contentSource;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;
        }

        internal SoundLoader(IALInvoker alInvoker, IContentSource contentSource, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.alInvoker = alInvoker;
            this.contentSource = contentSource;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;
        }

        public ISound Load(string name)
        {
            var extension = Path.GetExtension(name);

            if (!new[] { ".ogg", ".mp3" }.Contains(extension))
                throw new Exception($"The extension $'{extension}' is not supported. Supported audio files are '.ogg' and '.mp3'.");

            return new Sound(
                name,
                this.alInvoker,
                AudioDeviceManager.GetInstance(this.alInvoker),
                this.oggDecoder,
                this.mp3Decoder,
                this.contentSource);
        }
    }
}
