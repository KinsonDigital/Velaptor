// <copyright file="TheoryForLinux.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed class TheoryForLinux : TheoryAttribute
    {
        public TheoryForLinux()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }

            Skip = $"Only executed on {OSPlatform.Linux}.";
        }
    }
}
