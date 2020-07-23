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
        public ISound Load(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (!new[] { ".ogg", ".mp3" }.Contains(extension))
                throw new Exception($"The extension $'{extension}' is not supported. Supported audio files are '.ogg' and '.mp3'.");

            var alInvoker = IoC.Container.GetInstance<IALInvoker>();
            var alcInvoker = IoC.Container.GetInstance<IALCInvoker>();
            var oggDecoder = new OggSoundDecoder();
            var mp3Decoder = new MP3SoundDecoder();

            return new Sound(
                AudioManager.GetInstance(alInvoker, alcInvoker),
                filePath,
                oggDecoder,
                mp3Decoder);
        }
    }
}
