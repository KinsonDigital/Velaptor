using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FileIO.Core;

namespace Raptor.Content
{
    public class ContentSource : IContentSource
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private string contentRootDirectory = @$"{BaseDir}Content\";
        private string graphicsDirName = "Graphics";
        private string soundsDirName = "Sounds";
        private string atlasDirName = "AtlasData";
        private readonly IDirectory directory;

        public ContentSource(IDirectory directory)
        {
            this.directory = directory;
        }

        /// <inheritdoc/>
        public string ContentRootDirectory
        {
            get => contentRootDirectory;
            set
            {
                value = string.IsNullOrEmpty(value) ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith('\\') ? value : $@"{value}\";

                contentRootDirectory = $@"{value}Content\";
            }
        }

        public string GraphicsDirectoryName
        {
            get => graphicsDirName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception($"The '{nameof(GraphicsDirectoryName)}' must not be null or empty.");

#pragma warning disable CA1307 // Specify StringComparison
                value = value.Replace("\\", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison

                graphicsDirName = value;
            }
        }

        public string SoundsDirectoryName
        {
            get => soundsDirName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception($"The '{nameof(GraphicsDirectoryName)}' must not be null or empty.");

#pragma warning disable CA1307 // Specify StringComparison
                value = value.Replace("\\", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison

                soundsDirName = value;
            }
        }

        public string AtlasDirectoryName
        {
            get => atlasDirName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception($"The '{nameof(GraphicsDirectoryName)}' must not be null or empty.");

#pragma warning disable CA1307 // Specify StringComparison
                value = value.Replace("\\", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison

                atlasDirName = value;
            }
        }

        public string GetGraphicsPath()
        {
            return $@"{contentRootDirectory}{graphicsDirName}\";
        }

        public string GetSoundsPath()
        {
            return $@"{contentRootDirectory}{soundsDirName}\";
        }

        public string GetAtlasPath()
        {
            return $@"{contentRootDirectory}{atlasDirName}\";
        }

        public string GetContentPath(ContentType contentType, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(name), "The parameter must not be null or empty.");

            // If the name ends with a \, throw ane exception
            if (name.EndsWith('\\'))
                throw new ArgumentException($"The '{name}' cannot end with folder.  It must end with a file name with or without the extension.");

            // If the name has an extension, remove it
            if (Path.HasExtension(name))
#pragma warning disable CA1307 // Specify StringComparison
                name = $@"{Path.GetDirectoryName(name)}\{Path.GetFileNameWithoutExtension(name)}".Replace(@"\", "");
#pragma warning restore CA1307 // Specify StringComparison

            var filePath = string.Empty;

            switch (contentType)
            {
                case ContentType.Graphics:
                    filePath = $@"{GetGraphicsPath()}{name}";
                    break;
                case ContentType.Sounds:
                    filePath = $@"{GetSoundsPath()}{name}";
                    break;
                case ContentType.Atlas:
                    filePath = $@"{GetAtlasPath()}{name}";
                    break;
                default:
                    break;
            }

            var directory = Path.GetDirectoryName(filePath);
            var fileNameNoExt = Path.GetFileNameWithoutExtension(filePath).ToUpperInvariant();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(directory)
                .Where(f => Path.GetFileNameWithoutExtension(f).ToUpperInvariant() == fileNameNoExt).ToArray();

            if (files.Length <= 0)
                throw new Exception($"Could not load texture '{Path.GetFileNameWithoutExtension(filePath)}'.  It does not exist.");

            if (files.Length > 1)
                throw new Exception($"Could not load texture '{Path.GetFileNameWithoutExtension(filePath)}'.\nThere are duplicate textures with that name.\nThe file name must be unique without the extension.");

            return files[0];
        }
    }
}
