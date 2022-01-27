// <copyright file="UnitTestDetector.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Detects if the code ahs been executed from a unit test vs the rest of the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class UnitTestDetector
    {
        /// <summary>
        /// Initializes static members of the <see cref="UnitTestDetector"/> class.
        /// </summary>
        static UnitTestDetector()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assemblies is null)
            {
                throw new InvalidOperationException("Something went wrong.  No assemblies loaded.");
            }

            foreach (var assembly in assemblies)
            {
                if (string.IsNullOrEmpty(assembly.FullName) ||
                    assembly.FullName.ToLowerInvariant().DoesNotStartWith("xunit."))
                {
                    continue;
                }

                IsRunningFromUnitTest = true;
                break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the code is being executed from a unit test.
        /// </summary>
        public static bool IsRunningFromUnitTest { get; }
    }
}
