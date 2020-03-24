using System;

namespace KDScorpionCore
{
    /// <summary>
    /// Adds various game loop events for a game engine.
    /// </summary>
    public interface IEngineEvents
    {
        #region Events
        /// <summary>
        /// Occurs one time during game initialization. This event is fired before the <see cref="OnLoadContent"/> event is fired. Add initialization code here.
        /// </summary>
        event EventHandler OnInitialize;

        /// <summary>
        /// Occurs one time during game intialization after the <see cref="OnInitialize"/> event is fired.
        /// </summary>
        event EventHandler OnLoadContent;

        /// <summary>
        /// Occurs once every frame before the OnDraw event before the <see cref="OnRender"/> event is invoked.
        /// </summary>
        event EventHandler<OnUpdateEventArgs> OnUpdate;

        /// <summary>
        /// Occurs once every frame after the <see cref="OnUpdate"/> event has been been invoked.
        /// </summary>
        event EventHandler<OnRenderEventArgs> OnRender;
        #endregion
    }
}
