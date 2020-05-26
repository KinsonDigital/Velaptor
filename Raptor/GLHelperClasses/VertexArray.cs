using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Raptor.GLHelperClasses
{
    /// <summary>
    /// A VAO (Vertex Array Object) in OpenGL
    /// </summary>
    /// <typeparam name="T">The the data structure of the vertex buffer that the VAO will bind to.</typeparam>
    internal class VertexArray<T> : IDisposable where T : struct
    {
        #region Private Fields
        private bool _disposedValue = false;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="VertexArray"/>.
        /// </summary>
        /// <param name="vb">The <see cref="VertexBuffer"/> and <see cref="IndexBuffer"/> required to create the <see cref="VertextArray"/>.</param>
        /// <param name="ib">The <see cref="IndexBuffer"/> that describes the layout of the <see cref="VertextBuffer"/> for the <see cref="VertexArray"/>.</param>
        public VertexArray(VertexArrayBuffer<T> vb, IndexBuffer ib)
        {
            if (vb is null)
                throw new ArgumentNullException(nameof(vb), "The param must not be null");

            if (ib is null)
                throw new ArgumentNullException(nameof(ib), "The param must not be null");

            ID = GL.GenVertexArray();

            Bind();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vb.ID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib.ID);


            //Setup vertex shader aPosition attribute
            GL.EnableVertexArrayAttrib(ID, 0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);


            //Setup vertex shader aTexCoord attribute
            GL.EnableVertexArrayAttrib(ID, 1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));


            //Setup fragment shader u_TintClr attribute
            GL.EnableVertexArrayAttrib(ID, 2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), 5 * sizeof(float));
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the ID of the vertex array on the GPU.
        /// </summary>
        public int ID { get; private set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Binds the <see cref="VertexArray"/>.
        /// </summary>
        public void Bind() => GL.BindVertexArray(ID);


        /// <summary>
        /// Disposes of the <see cref="VertexArray"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Disposes of the internal resources if the given <paramref name="disposing"/> value is true.
        /// </summary>
        /// <param name="disposing">True to dispose of internal resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            //Clean up unmanaged resources
            GL.DeleteVertexArray(ID);

            _disposedValue = true;
        }
        #endregion
    }
}
