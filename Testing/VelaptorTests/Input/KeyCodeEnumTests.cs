// <copyright file="KeyCodeEnumTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using Velaptor.Input;
    using Xunit;

    /*Purpose:
     * In the new OpenTK v4.0.0 update, the key code enum values of the Keys enumeration was changed.
     * This broke related things in the KeyCode enumeration of the Velaptor library.  These
     * tests will check for changes when upgrading to verify that no changes were made.
     */

    /// <summary>
    /// Tests the <see cref="KeyCode"/> <see cref="Keys"/> enum to make sure nothing has changed.
    /// </summary>
    public class KeyCodeEnumTests
    {
        private readonly Dictionary<string, int> velaptorEnumNamesAndValues = new ();
        private readonly Dictionary<string, int> openTKEnumNamesAndValues = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCodeEnumTests"/> class.
        /// </summary>
        public KeyCodeEnumTests()
        {
            this.velaptorEnumNamesAndValues = GetEnumNamesAndValues<KeyCode>((enumName) =>
            {
                return enumName == "LastKey";
            }, (enumName) =>
            {
                return enumName == "LastKey" ? "Menu" : enumName;
            });

            this.openTKEnumNamesAndValues = GetEnumNamesAndValues<Keys>((enumName) =>
            {
                return false;
            }, (enumName) =>
            {
                return enumName == "LastKey" ? "Menu" : enumName;
            });
        }

        [Fact]
        public void EnumTest_WhenCompared_AllNamesExistAndValuesMatchInOpenTKEnum()
        {
            foreach (var velaptorEnum in this.velaptorEnumNamesAndValues)
            {
                Assert.True(this.openTKEnumNamesAndValues.ContainsKey(velaptorEnum.Key), $"The Velaptor enum '{velaptorEnum.Key}' does not exist in the OpenTK Keys enum.");

                var nameExists = this.openTKEnumNamesAndValues.Keys.Contains(velaptorEnum.Key);
                var openTKValue = this.openTKEnumNamesAndValues[velaptorEnum.Key];
                var valueMatches = velaptorEnum.Value == openTKValue;

                Assert.True(valueMatches, $"The Velaptor enum '{nameof(KeyCode)}.{velaptorEnum.Key}' value of '{velaptorEnum.Value}' does not match the '{nameof(Keys)}.{velaptorEnum.Key}' value of '{openTKValue}'.");
            }
        }

        /// <summary>
        /// Gets all of the enum names and values for the enum of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The enumeration to process.</typeparam>
        /// <param name="ignore">If returns true, ignores the current enum name.</param>
        /// <param name="replaceWith">Replaces a name with another name.</param>
        /// <returns>The dictionary of enum names (key) and enum values (value).</returns>
        [ExcludeFromCodeCoverage]
        private static Dictionary<string, int> GetEnumNamesAndValues<T>(Func<string, bool> ignore, Func<string, string> replaceWith)
            where T : Enum
        {
            var result = new Dictionary<string, int>();

            var keyCodeValues = Enum.GetValues(typeof(T));

            foreach (var enumValue in keyCodeValues)
            {
                if (enumValue is null)
                {
                    continue;
                }

                var name = Enum.GetName(typeof(T), enumValue);

                if (!string.IsNullOrEmpty(name) && ignore(name))
                {
                    continue;
                }

                name = string.IsNullOrEmpty(name) ? name : replaceWith(name);

                if (!string.IsNullOrEmpty(name) && !result.ContainsKey(name))
                {
                    result.Add(name, (int)enumValue);
                }
            }

            return result;
        }
    }
}
