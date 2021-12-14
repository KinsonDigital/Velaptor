using Velaptor.NativeInterop.OpenGL;
using Velaptor.Observables;
using Velaptor.OpenGL;

namespace Velaptor.Factories
{
    internal static class GPUBufferFactory
    {
        private static TextureGPUBuffer? textureBuffer;
        private static FontGPUBuffer? fontBuffer;

        public static TextureGPUBuffer CreateTextureGPUBuffer()
        {
            if (textureBuffer is not null)
            {
                return textureBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            textureBuffer = new TextureGPUBuffer(glInvoker, glInitObservable);

            return textureBuffer;
        }

        public static FontGPUBuffer CreateFontGPUBuffer()
        {
            if (fontBuffer is not null)
            {
                return fontBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            fontBuffer = new FontGPUBuffer(glInvoker, glInitObservable);

            return fontBuffer;
        }
    }
}
