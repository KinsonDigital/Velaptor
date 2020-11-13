// <copyright file="CachedValue.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;

    /// <summary>
    /// Caches a value as long as caching is turned on.
    /// </summary>
    /// <typeparam name="T">The type of value to cache.</typeparam>
    internal class CachedValue<T>
    {
        private readonly Func<T> getterWhenNotCaching;
        private readonly Action<T> setterWhenNotCaching;
        private T cachedValue;
        private bool isCaching = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedValue{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="getterWhenNotCaching">
        ///     Executed to get the value of type <typeparamref name="T"/> when the <see cref="GetValue()"/>
        ///     method is invoked while caching is turned off.
        /// </param>
        /// <param name="setterWhenNotCaching">
        ///     Executed to set the value of type <typeparamref name="T"/> when the <see cref="SetValue(T)"/>
        ///     is invoked while the caching is turned off.
        /// </param>
        /// <remarks>
        /// <para>
        ///     If caching is turned on, then getting a value will return the cached value.  Otherwise it will return the value
        ///     of <paramref name="getterWhenNotCaching"/> delegate.
        /// </para>
        /// <para>
        ///     If caching is turned on, the value being will be cached.  Otherwise the
        ///     <paramref name="getterWhenNotCaching"/> delegate will be used to set the value.
        /// </para>
        /// </remarks>
        public CachedValue(T defaultValue, Func<T> getterWhenNotCaching, Action<T> setterWhenNotCaching)
        {
            this.getterWhenNotCaching = getterWhenNotCaching;
            this.setterWhenNotCaching = setterWhenNotCaching;
            this.cachedValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value be cached.
        /// </summary>
        public bool IsCaching
        {
            get => this.isCaching;
            set
            {
                // If caching is being turned on
                if (value)
                {
                    this.cachedValue = this.getterWhenNotCaching();
                }
                else
                {
                    this.setterWhenNotCaching(this.cachedValue);
                }

                this.isCaching = value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The cached or non cached value.</returns>
        /// <remarks>
        /// <para>
        ///     If caching is turned on, then the cached value of type <typeparamref name="T"/> will be returned.
        /// </para>
        /// <para>
        ///     If caching is turned off, then the result of the getter implementation will return
        ///     the value of type <typeparamref name="T"/>.
        /// </para>
        /// </remarks>
        public T GetValue() => IsCaching ? this.cachedValue : this.getterWhenNotCaching();

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value to cache or simply use for setting a value.</param>
        /// <remarks>
        /// <para>
        ///     If caching is turned on, then the <paramref name="value"/> will be cached.
        /// </para>
        /// <para>
        ///     If caching is turned off, then the <paramref name="value"/> will be used in the setter implementation to set the value.
        /// </para>
        /// </remarks>
        public void SetValue(T value)
        {
            if (this.isCaching)
            {
                this.cachedValue = value;
            }
            else
            {
                this.setterWhenNotCaching(value);
            }
        }
    }
}
