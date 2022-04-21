// <copyright file="ShaderManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using Velaptor.Guards;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    internal class ShaderManager : IShaderManager
    {
        private readonly IShaderProgram textureShader;
        private readonly IShaderProgram fontShader;
        private readonly IShaderProgram rectShader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderManager"/> class.
        /// </summary>
        /// <param name="textureShader">The shader used for rendering textures.</param>
        /// <param name="fontShader">The shader used for rendering text.</param>
        /// <param name="rectShader">The shader used for rendering rectangles.</param>
        public ShaderManager(
            IShaderProgram textureShader,
            IShaderProgram fontShader,
            IShaderProgram rectShader)
        {
            EnsureThat.ParamIsNotNull(textureShader);
            EnsureThat.ParamIsNotNull(fontShader);
            EnsureThat.ParamIsNotNull(rectShader);

            this.textureShader = textureShader;
            this.fontShader = fontShader;
            this.rectShader = rectShader;
        }

        /// <inheritdoc/>
        public uint GetShaderId(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Texture:
                    return this.textureShader.ShaderId;
                case ShaderType.Font:
                    return this.fontShader.ShaderId;
                case ShaderType.Rectangle:
                    return this.rectShader.ShaderId;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(shaderType),
                        shaderType,
                        $"The enum '{nameof(ShaderType)}' value is invalid.");
            }
        }

        /// <inheritdoc/>
        public string GetShaderName(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Texture:
                    return this.textureShader.Name;
                case ShaderType.Font:
                    return this.fontShader.Name;
                case ShaderType.Rectangle:
                    return this.rectShader.Name;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(shaderType),
                        shaderType,
                        $"The enum '{nameof(ShaderType)}' value is invalid.");
            }
        }

        /// <inheritdoc/>
        public void Use(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Texture:
                    this.textureShader.Use();
                    break;
                case ShaderType.Font:
                    this.fontShader.Use();
                    break;
                case ShaderType.Rectangle:
                    this.rectShader.Use();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(shaderType),
                        shaderType,
                        $"The enum '{nameof(ShaderType)}' value is invalid.");
            }
        }
    }
}
