using System;
using System.Collections.Generic;
using OpenToolkit.Graphics.OpenGL4;

namespace Raptor.GLHelperClasses
{
    /// <summary>
    /// The index buffer used describe the layout of a <see cref="VertexBuffer"/>.
    /// </summary>
    internal class IndexBuffer : IDisposable
    {
        #region Private Fields
        //TODO:  Need to create a static list of bound buffers. This will allow  the ability
        //to keep track if the buffer for a particular instance is bound
        private readonly static List<int> _boundIDNumbers = new List<int>();
        private int _id;
        private bool _disposedValue = false;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="IndexBuffer"/>.
        /// </summary>
        /// <param name="data">The index buffer data.</param>
        public IndexBuffer(uint[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The param must not be null");

            Count = data.Length;
            _id = GL.GenBuffer();

            UploadDataToGPU(data);
        }
        #endregion


        #region Props
        /// <summary>
        /// The total number of indexes in the buffer.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The ID of the <see cref="IndexBuffer"/>.
        /// </summary>
        public int ID => _id;
        #endregion


        #region Public Methods
        /// <summary>
        /// Binds the <see cref="IndexBuffer"/>.
        /// </summary>
        public void Bind()
        {
            //NOTE: Only one index buffer can be bound at a time
            if (_boundIDNumbers.Contains(_id))
                return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
            _boundIDNumbers.Add(_id);
        }


        /// <summary>
        /// Unbinds the <see cref="IndexBuffer"/>.
        /// </summary>
        public void Unbind()
        {
            //If the buffer is already unbound
            if (!_boundIDNumbers.Contains(_id))
                return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            _boundIDNumbers.Remove(_id);
        }


        /// <summary>
        /// Disposes of <see cref="IndexBuffer"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Uploads the given <paramref name="data"/> to the GPU.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        private void UploadDataToGPU(uint[] data)
        {
            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.DynamicDraw);
            Unbind();
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
            GL.DeleteBuffers(1, ref _id);

            _disposedValue = true;
        }
        #endregion
    }
}
