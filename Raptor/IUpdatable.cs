// <copyright file="IUpdatable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    /// <summary>
    /// Provides the ability for an object to be updated.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="engineTime">The game engine time.</param>
        void Update(FrameTime engineTime);
    }
}
