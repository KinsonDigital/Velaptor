// <copyright file="ShaderTemplateProcessorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Velaptor.OpenGL.Services;

/// <summary>
/// Processes template variables in shader source code by finding
/// variable template injection syntax and replacing them with variable values.
/// </summary>
internal sealed class ShaderTemplateProcessorService : ITemplateProcessorService
{
    private const string InjectionStart = "${{";
    private const string InjectionStop = "}}";

    /// <inheritdoc cref="ITemplateProcessorService.ProcessTemplateVariables"/>
    public string ProcessTemplateVariables(string shaderSrc, IEnumerable<(string name, string varValue)> variables)
    {
        // Enumerate to an array to avoid each iteration from reiterating through the list
        var varList = variables as (string name, string varValue)[] ?? variables.ToArray();

        foreach (var variable in varList)
        {
            var (invalid, reason) = IsVariableNameInvalid(variable.name);

            if (invalid)
            {
                throw new Exception(reason);
            }
        }

        var result = new StringBuilder(shaderSrc);

        // Process each variable by finding each variable name and
        // replacing the syntax and variable name with the value
        foreach (var (varName, varValue) in varList)
        {
            result = new StringBuilder(result.ToString().TrimRightOf(InjectionStart, ' '));
            result = new StringBuilder(result.ToString().TrimLeftOf(InjectionStop, ' '));

            var injectionSite = $"{InjectionStart}{varName}{InjectionStop}";

            result.Replace(injectionSite, varValue);
        }

        return result.ToString();
    }

    /// <summary>
    /// Returns a tuple verifying if the given <paramref name="varName"/> is invalid.
    /// </summary>
    /// <param name="varName">The name of the variable to check.</param>
    /// <returns>
    /// <list>
    ///     <item>
    ///         <term><b>(bool, string).invalid:</b></term>
    ///         <description> True if the variable name is invalid.</description>
    ///     </item>
    ///     <item>
    ///         <term><b>(bool, string).reason:</b></term>
    ///         <description> The reason for the variable name being invalid.</description>
    ///     </item>
    /// </list>
    /// </returns>
    private static (bool invalid, string reason) IsVariableNameInvalid(string varName)
    {
        var startsWithUnderscore = varName.StartsWith('_');
        if (startsWithUnderscore)
        {
            return (true, "Template variables cannot start with an '_' underscore character.");
        }

        var endsWithUnderscore = varName.EndsWith('_');
        if (endsWithUnderscore)
        {
            return (true, "Template variables cannot end with an '_' underscore character.");
        }

        var containsLowerCaseLetters = varName.Any(c => c is >= 'a' and <= 'z');
        if (containsLowerCaseLetters)
        {
            return (true, "The lower case letters 'a' through 'z' are not a valid character in a template variable.");
        }

        var containsNumbers = varName.Any(c => c is >= '0' and <= '9');
        if (containsNumbers)
        {
            return (true, "The numbers '0' through '9' are not a valid character in a template variable.");
        }

        var containsOtherInvalidChars =
            varName.Any(c =>
            {
                // Ignore the underscore character.  This is allowed
                if (c == '_')
                {
                    return false;
                }

                var isSymbolSet1 = c is >= ' ' and <= '/';
                var isSymbolSet2 = c is >= ':' and <= '@';
                var isSymbolSet3 = c is >= '[' and <= '`';
                var isSymbolSet4 = c is >= '{' and <= '~';

                return isSymbolSet1 || isSymbolSet2 || isSymbolSet3 || isSymbolSet4;
            });

        return containsOtherInvalidChars
            ? (true, "All symbol characters besides the '_' underscore character are invalid.")
            : (false, $"The variable name '{varName}' is valid.");
    }
}
