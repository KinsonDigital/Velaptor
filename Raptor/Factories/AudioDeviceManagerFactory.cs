using Raptor.Audio;
using Raptor.OpenAL;

namespace Raptor.Factories
{
    public static class AudioDeviceManagerFactory
    {
        public static IAudioDeviceManager CreateManager()
        {
            var alInvoker = IoC.Container.GetInstance<IALInvoker>();
            var alcInvoker = IoC.Container.GetInstance<IALCInvoker>();
            var oggDecoder = IoC.Container.GetInstance<ISoundDecoder<float>>();
            var mp3Decoder = IoC.Container.GetInstance<ISoundDecoder<byte>>();

            return AudioDeviceManager.GetInstance(alInvoker, alcInvoker, oggDecoder, mp3Decoder);
        }
    }
}
