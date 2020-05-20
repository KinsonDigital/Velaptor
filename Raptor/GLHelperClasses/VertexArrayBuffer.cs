using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenToolkit.Graphics.OpenGL4;

namespace Raptor.GLHelperClasses
{
    /// <summary>
    /// A vertex buffer object used to hold and describe data for a the GLSL shader program.
    /// </summary>
    internal class VertexArrayBuffer<T> : IDisposable where T : struct
    {
        #region Private Fields
        private static readonly List<int> _boundBuffers = new List<int>();
        private bool _disposedValue = false;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="VertexBuffer"/>.
        /// </summary>
        /// <param name="gl">Provides access to OpenGL funtionality.</param>
        /// <param name="data">The vertex data to send to the GPU.</param>
        public VertexArrayBuffer(T[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The param must not be null");

            ID = GL.GenBuffer();

            UploadDataToGPU(data);
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the ID of the <see cref="VertexBuffer"/>.
        /// </summary>
        public int ID { get; private set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Binds ths <see cref="VertexBuffer"/>.
        /// </summary>
        public void Bind()
        {
            if (_boundBuffers.Contains(ID))
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            _boundBuffers.Add(ID);
        }


        /// <summary>
        /// Unbinds the <see cref="VertexBuffer"/>.
        /// </summary>
        public void Unbind()
        {
            if (!_boundBuffers.Contains(ID))
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            _boundBuffers.Remove(ID);
        }


        /// <summary>
        /// Disposes of the <see cref="VertexBuffer"/>.
        /// </summary>
        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
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
            Unbind();

            var id = ID;
            GL.DeleteBuffers(1, ref id);

            _disposedValue = true;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Uploads the given <paramref name="data"/> to the GPU.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        private void UploadDataToGPU(T[] data)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
            Unbind();
        }
        #endregion
    }
}
