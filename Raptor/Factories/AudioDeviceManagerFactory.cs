using Raptor.Audio;
using Raptor.OpenAL;

namespace Raptor.Factories
{
    internal static class AudioDeviceManagerFactory
    {
        public static IAudioDeviceManager CreateManager()
        {
            var alInvoker = IoC.Container.GetInstance<IALInvoker>();
            var alcInvoker = IoC.Container.GetInstance<IALCInvoker>();

            return AudioDeviceManager.GetInstance(alInvoker, alcInvoker);
        }
    }
}
