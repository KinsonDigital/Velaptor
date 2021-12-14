using Velaptor.NativeInterop.OpenGL;
using Velaptor.Observables;
using Velaptor.OpenGL.Services;

namespace Velaptor.OpenGL
{

    [ShaderName("Font")]
    internal class FontShader : ShaderProgram
    {
        public int _fontTextureUniformLocation = -1;

        public FontShader(
            IGLInvoker gl,
            IShaderLoaderService<uint> shaderLoaderService,
            OpenGLInitObservable glObservable)
            : base(gl, shaderLoaderService, glObservable)
        {
        }

        public override void Use()
        {
            base.Use();

            if (_fontTextureUniformLocation < 0)
            {
                _fontTextureUniformLocation = this.gl.GetUniformLocation(ShaderId, "fontTexture");
            }

            this.gl.ActiveTexture(GLTextureUnit.Texture1);
            this.gl.Uniform1(_fontTextureUniformLocation, 1);
        }
    }
}
