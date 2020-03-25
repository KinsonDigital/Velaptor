using Raptor.Input;
namespace Raptor.Plugins
{
    /// <summary>
    /// Represents a mouse that provides user input.
    /// </summary>
    public interface IMouse
    {
        #region Props
        /// <summary>
        /// Gets sets the X position of the mouse.
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Gets sets the Y position of the mouse.
        /// </summary>
        int Y { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Update the current state of the mouse.
        /// </summary>
        void UpdateCurrentState();


        /// <summary>
        /// Update the previous state of the mouse.
        /// </summary>
        void UpdatePreviousState();


        /// <summary>
        /// Sets the position of the mouse using the given <paramref name="x"/> and <paramref name="y"/> values.
        /// </summary>
        /// <param name="x">The horizontal X position to set the mouse in the game window.</param>
        /// <param name="y">The vertical Y position to set the mouse in the game window.</param>
        void SetPosition(int x, int y);


        /// <summary>
        /// Returns true if the given input is in the down position.
        /// </summary>
        /// <param name="inputButton">The input to check.</param>
        /// <returns></returns>
        bool IsButtonDown(InputButton inputButton);


        /// <summary>
        /// Returns true if the given <paramref name="inputButton"/> is in the up position.
        /// </summary>
        /// <param name="inputButton">The input button to check.</param>
        /// <returns></returns>
        bool IsButtonUp(InputButton inputButton);


        /// <summary>
        /// Returns true if the given mouse <paramref name="inputButton"/> has been released from the down position.
        /// </summary>
        /// <param name="inputButton">The input button to check.</param>
        /// <returns></returns>
        bool IsButtonPressed(InputButton inputButton);
        #endregion
    }
}
