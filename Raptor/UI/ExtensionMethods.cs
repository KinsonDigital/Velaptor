using System.Collections.Generic;

namespace Raptor.UI
{
    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Methods
        /// <summary>
        /// Adds the range of numbers to the list starting at the given 
        /// <paramref name="start"/> up to the given <paramref name="stop"/>.
        /// </summary>
        /// <param name="list">The list to add the numbers to.</param>
        /// <param name="start">The start of the range of numbers to add. This is inclusive.</param>
        /// <param name="stop">The stop of the range of numbers to add. This is inclusive.</param>
        public static void AddRange(this List<int> list, int start, int stop)
        {
            for (int i = start; i <= stop; i++)
            {
                list.Add(i);
            }
        }
        #endregion
    }
}
