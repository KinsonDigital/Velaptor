// <copyright file="ITemplateProcessorService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// Processes template variables in textual data.
    /// </summary>
    public interface ITemplateProcessorService
    {
        /// <summary>
        /// Finds all occurrences of the given variable names in the given <paramref name="variables"/> and
        /// replaces them and the associated template variable syntax with the variable values in given <paramref name="variables"/>.
        /// </summary>
        /// <param name="shaderSrc">The shader source code with the template variables to process.</param>
        /// <param name="variables">The template variables to process.</param>
        /// <returns>
        ///     The given <paramref name="shaderSrc"/> with all template variables replaced.
        /// </returns>
        string ProcessTemplateVariables(string shaderSrc, IEnumerable<(string name, string varValue)> variables);
    }
}
