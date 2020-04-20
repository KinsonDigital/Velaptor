using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Raptor
{
    /// <summary>
    /// Maintains a list of actions that are meant to be executed only once at a later time.
    /// </summary>
    public class DeferredActionsCollection : IEnumerable, IList<Action>
    {
        #region Private Fields
        private readonly List<Action> _actions = new List<Action>();
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the index item of the <see cref="DeferredActionsCollection"/> list.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns></returns>
        public Action this[int index]
        {
            get
            {
                return _actions[index];
            }
            set
            {
                _actions[index] = value;
            }
        }

        /// <summary>
        /// Gets the total number of actions.
        /// </summary>
        public int Count => _actions.Count;

        /// <summary>
        /// Gets a value indicating if the action items can be changed.
        /// </summary>
        public bool IsReadOnly { get; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Executes all of the <see cref="Action"/>s.  Each <see cref="Action"/> will be destroyed after execution.
        /// </summary>
        public void ExecuteAll()
        {
            _actions.ForEach(a => a());
            Clear();
        }


        /// <summary>
        /// Adds the given <paramref name="action"/> to the list.
        /// </summary>
        /// <param name="action">An action to be added.</param>
        [ExcludeFromCodeCoverage]
        public void Add(Action action) => _actions.Add(action);


        /// <summary>
        /// Removes all of the <see cref="Action"/>s.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Clear() => _actions.Clear();


        /// <summary>
        /// Returns a value indicating if the given <paramref name="action"/> is in the list.
        /// </summary>
        /// <param name="action">The action to check for.</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public bool Contains(Action action) => _actions.Contains(action);


        /// <summary>
        /// Copies the <see cref="Action"/>s starting at the given <paramref name="arrayIndex"/> to 
        /// the given destination <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The destination array to copy the items to.</param>
        /// <param name="arrayIndex">The starting index at which to start copying the items.</param>
        [ExcludeFromCodeCoverage]
        public void CopyTo(Action[] array, int arrayIndex) => _actions.CopyTo(array, arrayIndex);


        /// <summary>
        /// Returns the enumerator to allow enumeration of the list of <see cref="Action"/>s.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                yield return _actions[i];
            }
        }


        /// <summary>
        /// Returns the index location of the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action to get the index of.</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public int IndexOf(Action action) => _actions.IndexOf(action);


        /// <summary>
        /// Inserts the given <paramref name="action"/> at the given <paramref name="index"/> location within the list.
        /// </summary>
        /// <param name="index">The index location to insert the action at.</param>
        /// <param name="action">The action to insert.</param>
        [ExcludeFromCodeCoverage]
        public void Insert(int index, Action action) => _actions.Insert(index, action);


        /// <summary>
        /// Removes the given <paramref name="action"/> from the list.
        /// </summary>
        /// <param name="action">The action to remove.</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public bool Remove(Action action) => _actions.Remove(action);


        /// <summary>
        /// Removes the action that is located at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index location of the action.</param>
        [ExcludeFromCodeCoverage]
        public void RemoveAt(int index) => _actions.RemoveAt(index);


        /// <summary>
        /// Returns the enumerator to allow enumeration of the list of <see cref="Action"/>s.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        IEnumerator<Action> IEnumerable<Action>.GetEnumerator() => _actions.GetEnumerator();
        #endregion
    }
}
