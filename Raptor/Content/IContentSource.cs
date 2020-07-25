namespace Raptor.Content
{
    public interface IContentSource
    {
        string AtlasDirectoryName { get; set; }
        string ContentRootDirectory { get; set; }
        string GraphicsDirectoryName { get; set; }
        string SoundsDirectoryName { get; set; }

        string GetAtlasPath();
        string GetContentPath(ContentType contentType, string name);
        string GetGraphicsPath();
        string GetSoundsPath();
    }
}