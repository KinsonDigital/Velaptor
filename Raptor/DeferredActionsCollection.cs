// <copyright file="DeferredActionsCollection.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Maintains a list of actions that are meant to be executed only once at a later time.
    /// </summary>
    public class DeferredActionsCollection : IEnumerable, IList<Action>
    {
        private readonly List<Action> actions = new List<Action>();

        /// <inheritdoc/>
        public int Count => this.actions.Count;

        /// <inheritdoc/>
        public bool IsReadOnly { get; }

        /// <inheritdoc/>
        public Action this[int index]
        {
            get => this.actions[index];
            set => this.actions[index] = value;
        }

        /// <summary>
        /// Executes all of the <see cref="Action"/>s.  Each <see cref="Action"/> will be destroyed after execution.
        /// </summary>
        public void ExecuteAll()
        {
            this.actions.ForEach(a => a());
            Clear();
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Add(Action action) => this.actions.Add(action);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Clear() => this.actions.Clear();

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Contains(Action action) => this.actions.Contains(action);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void CopyTo(Action[] array, int arrayIndex) => this.actions.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            for (var i = 0; i < this.actions.Count; i++)
            {
                yield return this.actions[i];
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public int IndexOf(Action action) => this.actions.IndexOf(action);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Insert(int index, Action action) => this.actions.Insert(index, action);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Remove(Action action) => this.actions.Remove(action);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void RemoveAt(int index) => this.actions.RemoveAt(index);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator<Action> IEnumerable<Action>.GetEnumerator() => this.actions.GetEnumerator();
    }
}
