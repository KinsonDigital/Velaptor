using System;

namespace Raptor
{
    /// <summary>
    /// Adds various game related events for a game.
    /// </summary>
    public interface IGameEvents
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
