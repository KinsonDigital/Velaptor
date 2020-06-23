namespace Raptor.Content
{
    using System.Text.Json;
    using FileIO.Core;

    public class AtlasDataLoader : ILoader<AtlasRegionRectangle[]>
    {
        private readonly ITextFile textFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataLoader"/> class.
        /// </summary>
        /// <param name="textFile"></param>
        public AtlasDataLoader(ITextFile textFile) => this.textFile = textFile;

        /// <inheritdoc/>
        public AtlasRegionRectangle[] Load(string filePath)
        {
            var rawData = this.textFile.Load(filePath);

            return JsonSerializer.Deserialize<AtlasRegionRectangle[]>(rawData);
        }
    }
}
