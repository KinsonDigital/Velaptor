using Velaptor.NativeInterop.OpenGL;
using Velaptor.Observables;
using Velaptor.OpenGL.Services;

namespace Velaptor.OpenGL
{
    [ShaderName("Texture")]
    internal class TextureShader : ShaderProgram
    {
        private int _mainTextureUniformLocation = -1;

        public TextureShader(
            IGLInvoker gl,
            IShaderLoaderService<uint> shaderLoaderService,
            OpenGLInitObservable glObservable)
            : base(gl, shaderLoaderService, glObservable)
        {
        }

        public override void Use()
        {
            base.Use();

            if (_mainTextureUniformLocation < 0)
            {
                _mainTextureUniformLocation = this.gl.GetUniformLocation(ShaderId, "mainTexture");
            }

            this.gl.ActiveTexture(GLTextureUnit.Texture0);
            this.gl.Uniform1(_mainTextureUniformLocation, 0);
        }
    }
}
