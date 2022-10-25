// <copyright file="DateTimeService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal sealed class DateTimeService : IDateTimeService
    {
        /// <inheritdoc/>
        public DateTime Now() => DateTime.Now;
    }
}
