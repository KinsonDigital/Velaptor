using System;
using System.Drawing;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using FileIO.File;
using Raptor.GLHelperClasses;
using Raptor.Graphics;
using Raptor.Plugins;

namespace Raptor.OpenGLImp
{
    /// <summary>
    /// Performs rendering using OpenGL.
    /// </summary>
    internal class GLRenderer : IRenderer
    {
        #region Private Fields
        private readonly VertexArrayBuffer<QuadBufferData> _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private readonly VertexArray<QuadBufferData> _vertexArray;
        private readonly ShaderProgram _shaderProgram;
        private QuadBufferData[] _vertexBufferData = Array.Empty< QuadBufferData>();
        private bool _disposedValue = false;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="GLRenderer"/>.
        /// </summary>
        /// <param name="viewportWidth">The width of the entire rendering surface.</param>
        /// <param name="viewportHeight">The height of the entire rendering surface.</param>
        public GLRenderer(int viewportWidth, int viewportHeight)
        {
            _shaderProgram = new ShaderProgram("shader.vert", "shader.frag", new TextFile());

            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //Cornflower blue
            GL.ClearColor(0.392156869f, 0.5803922f, 0.929411769f, 1.0f);//TODO: Allow changing of this

            _shaderProgram.UseProgram();

            InitBufferData();

            _vertexBuffer = new VertexArrayBuffer<QuadBufferData>(_vertexBufferData);
            _indexBuffer = new IndexBuffer(new uint[] { 0, 1, 3, 1, 2, 3, 4, 5});

            _vertexArray = new VertexArray<QuadBufferData>(_vertexBuffer, _indexBuffer);
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public int ViewportWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public int ViewportHeight { get; set; }
        #endregion



        #region Public Methods
        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate position on the surface to render.</param>
        /// <param name="y">The Y coordinate position on the surface to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        /// <param name="size">The size of the texture. 1.0 is 100%(noram) size.</param>
        /// <param name="color">The color to apply to the texture.</param>
        public void Render(ITexture texture, float x, float y, float angle, float size, GameColor color)
        {
            //NOTE: If every single texture uses the same vertex array, you only need to bind it once
            _vertexArray.Bind();
            texture.Bind();


            UpdateGPUColorData(color);
            UpdateGPUTransform(x,
                y,
                texture.Width,
                texture.Height,
                angle,
                size);


            //TODO: Try and use 4 instead of 8
            GL.DrawElements(PrimitiveType.Triangles, 8, DrawElementsType.UnsignedInt, IntPtr.Zero);

            texture.Unbind();
        }


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(IText text, float x, float y, GameColor color)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Renders a line using the given start and stop coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        public void RenderLine(float lineStartX, float lineStartY, float lineStopX, float lineStopY)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void RenderLine(float startX, float startY, float endX, float endY, GameColor color)
        {
            throw new NotImplementedException();
        }


        //TODO: Need to add docs
        public void RenderTextureArea(ITexture texture, Rect area, float x, float y)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Clears the screen to the given color.
        /// </summary>
        /// <param name="color">The color to clear the screen to.</param>
        public void Clear(GameColor color)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void FillRect(Rect rect, GameColor color)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="x">The X coordinate on the screen of where to render the circle.</param>
        /// <param name="y">The Y coordinate on the screen of where to render the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        public void FillCircle(float x, float y, float radius, GameColor color)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <paramref name="disposing">True if other managed resources need to be disposed of.</paramref>
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                _shaderProgram.Dispose();
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
                _vertexArray.Dispose();
            }

            _disposedValue = true;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Initializes the vertex buffer data that will go to the GPU.
        /// </summary>
        private void InitBufferData()
        {
            _vertexBufferData = new[]
{
                new QuadBufferData()
                {
                    CornerVertice = new Vector3(-1, 1, 0),
                    TextureCoords = new Vector2(0, 1),
                    TintColor = Color.FromArgb(255, 255, 0, 255).ToVector4(),
                },
                new QuadBufferData()
                {
                    CornerVertice = new Vector3(1, 1, 0),
                    TextureCoords = new Vector2(1, 1),
                    TintColor = Color.FromArgb(255, 255, 0, 255).ToVector4(),
                },
                new QuadBufferData()
                {
                    CornerVertice = new Vector3(1, -1, 0),
                    TextureCoords = new Vector2(1, 0),
                    TintColor = Color.FromArgb(255, 255, 0, 255).ToVector4(),
                },
                new QuadBufferData()
                {
                    CornerVertice = new Vector3(-1, -1, 0),
                    TextureCoords = new Vector2(0, 0),
                    TintColor = Color.FromArgb(255, 255, 0, 255).ToVector4(),
                }
            };
        }


        /// <summary>
        /// Updates the given <paramref name="tintClr"/> in the GPU.
        /// </summary>
        /// <param name="tintClr">The color data to send.</param>
        private void UpdateGPUColorData(GameColor tintClr)
        {
            for (int i = 0; i < _vertexBufferData.Length; i++)
            {
                _vertexBufferData[i].TintColor = tintClr.ToGLColor();
            }

            var dataSize = 48 * sizeof(float);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer.ID);
            GL.BufferData(BufferTarget.ArrayBuffer, dataSize, _vertexBufferData, BufferUsageHint.DynamicDraw);
        }


        /// <summary>
        /// Updates the transformation matrix data in the GPU using the given information.
        /// </summary>
        /// <param name="x">The X coordinate of a texture.</param>
        /// <param name="y">The Y coordinate of a texture.</param>
        /// <param name="width">The width of a texture.</param>
        /// <param name="height">The height of a texture.</param>
        /// <param name="angle">The angle of the texture in degrees.</param>
        /// <param name="size">The size of the texture.</param>
        private void UpdateGPUTransform(float x, float y, int width, int height, float angle, float size)
        {
            //Create and send the transformation data to the GPU
            var transMatrix = BuildTransformationMatrix(x,
                                               y,
                                               width,
                                               height,
                                               angle,
                                               size);

            var transDataLocation = GL.GetUniformLocation(_shaderProgram.ID, "u_Transform");
            GL.UniformMatrix4(transDataLocation, true, ref transMatrix);
        }


        /// <summary>
        /// Builds a complete transformation matrix using the given params.
        /// </summary>
        /// <param name="x">The x position of a texture.</param>
        /// <param name="y">The y position of a texture.</param>
        /// <param name="width">The width of a texture.</param>
        /// <param name="height">The height of a texture.</param>
        /// <param name="angle">The angle of the texture.</param>
        /// <param name="size">The size of a texture. 1 represents normal size and 1.5 represents 150%.</param>
        /// <returns></returns>
        private Matrix4 BuildTransformationMatrix(float x, float y, int width, int height, float angle, float size)
        {
            //NOTE: ndc = Normal Device Coordinates

            var scaleX = (float)width / ViewportWidth;
            var scaleY = (float)height / ViewportHeight;

            scaleX *= size;
            scaleY *= size;

            var ndcX = x.MapValue(0f, ViewportWidth, -1f, 1f);
            var ndcY = y.MapValue(0f, ViewportHeight, 1f, -1f);

            //NOTE: (+ degrees) rotates CCW and (- degress) rotates CW
            var angleRadians = MathHelper.DegreesToRadians(angle);

            //Invert angle to rotate CW instead of CCW
            angleRadians *= -1;

            var rotation = Matrix4.CreateRotationZ(angleRadians);
            var scaleMatrix = Matrix4.CreateScale(scaleX, scaleY, 1f);
            var positionMatrix = Matrix4.CreateTranslation(new Vector3(ndcX, ndcY, 0));


            return rotation * scaleMatrix * positionMatrix;
        }
        #endregion
    }
}
