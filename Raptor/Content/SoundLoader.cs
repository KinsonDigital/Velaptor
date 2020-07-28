using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raptor.Audio;
using Raptor.OpenAL;

namespace Raptor.Content
{
    public class SoundLoader : ILoader<ISound>
    {
        private readonly IContentSource contentSource;

        public SoundLoader(IContentSource contentSource) => this.contentSource = contentSource;

        public ISound Load(string name)
        {
            var extension = Path.GetExtension(name);

            if (!new[] { ".ogg", ".mp3" }.Contains(extension))
                throw new Exception($"The extension $'{extension}' is not supported. Supported audio files are '.ogg' and '.mp3'.");

            var alInvoker = IoC.Container.GetInstance<IALInvoker>();
            var oggDecoder = IoC.Container.GetInstance<ISoundDecoder<float>>();
            var mp3Decoder = IoC.Container.GetInstance<ISoundDecoder<byte>>();

            return new Sound(
                name,
                alInvoker,
                AudioDeviceManager.GetInstance(alInvoker),
                oggDecoder,
                mp3Decoder,
                this.contentSource);
        }
    }
}
