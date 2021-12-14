using Velaptor.NativeInterop.OpenGL;
using Velaptor.Observables;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;

namespace Velaptor.Factories
{
    internal static class ShaderFactory
    {
        private static TextureShader? textureShader;
        private static FontShader? fontShader;

        public static TextureShader CreateTextureShader()
        {
            if (textureShader is not null)
            {
                return textureShader;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            textureShader = new TextureShader(glInvoker, shaderLoaderService, glInitObservable);

            return textureShader;
        }

        public static FontShader CreateFontShader()
        {
            if (fontShader is not null)
            {
                return fontShader;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            fontShader = new FontShader(glInvoker, shaderLoaderService, glInitObservable);

            return fontShader;
        }
    }
}
