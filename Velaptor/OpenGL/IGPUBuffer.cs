// <copyright file="IGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    /// <summary>
    /// Manages buffer data in the GPU.
    /// </summary>
    /// <typeparam name="TData">The type of data in the buffer.</typeparam>
    internal interface IGPUBuffer<in TData>
        where TData : struct
    {
        /// <summary>
        /// Updates the GPU buffer using the given <paramref name="data"/>
        /// at the given <paramref name="index"/> location.
        /// </summary>
        /// <param name="data">The data to update.</param>
        /// <param name="index">The index location of the data to update.</param>
        /// <remarks>
        ///     Think of the <paramref name="index"/> as the offset/location of
        ///     the data in GPU memory. For example, if you think of the memory
        ///     being laid out like an array of data, this would be the location
        ///     of the 'chunk' of <paramref name="data"/> in the array.
        /// </remarks>
        void UploadData(TData data, uint index);
    }
}
