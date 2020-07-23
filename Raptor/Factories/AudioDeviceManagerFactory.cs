using Raptor.Audio;
using Raptor.OpenAL;

namespace Raptor.Factories
{
    internal static class AudioDeviceManagerFactory
    {
        public static IAudioManager CreateManager()
        {
            var alInvoker = IoC.Container.GetInstance<IALInvoker>();
            var alcInvoker = IoC.Container.GetInstance<IALCInvoker>();

            return AudioManager.GetInstance(alInvoker, alcInvoker);
        }
    }
}
