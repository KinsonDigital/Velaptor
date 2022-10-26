// <copyright file="IDateTimeService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;

    /// <summary>
    /// Provides date and time related information.
    /// </summary>
    internal interface IDateTimeService
    {
        /// <inheritdoc cref="DateTime.Now"/>
        DateTime Now();
    }
}
