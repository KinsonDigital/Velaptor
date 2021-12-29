// <copyright file="TestOnlyExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides extension methods to ease the writing of unit tests.
    /// </summary>
    public static class TestOnlyExtensions
    {
        internal static Dictionary<uint, T> ToDictionary<T>(this List<T> items)
        {
            var result = new Dictionary<uint, T>();

            for (var i = 0u; i < items.Count; i++)
            {
                result.Add(i, items[(int)i]);
            }

            return result;
        }

        /// <summary>
        /// Converts the items of type <see cref="IEnumerable{T}"/> to type <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the <see cref="IEnumerable{T}"/> list.</typeparam>
        /// <param name="items">The items to convert.</param>
        /// <returns>The items as a read only collection.</returns>
        internal static ReadOnlyDictionary<uint, T> ToReadOnlyDictionary<T>(this List<T> items)
            => new (items.ToDictionary());
    }
}
