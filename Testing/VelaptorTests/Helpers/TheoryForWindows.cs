// <copyright file="TheoryForWindows.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed class TheoryForWindows : TheoryAttribute
    {
        public TheoryForWindows()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = $"Only executed on {OSPlatform.Windows}.";
        }
    }
}
