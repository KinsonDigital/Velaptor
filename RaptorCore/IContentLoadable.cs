using RaptorCore.Content;

namespace RaptorCore
{
    /// <summary>
    /// Provides the ability to load content.
    /// </summary>
    public interface IContentLoadable
    {
        #region Methods
        /// <summary>
        /// Load the content using the given <paramref name="contentLoader"/>.
        /// </summary>
        /// <param name="contentLoader">Used to load content.</param>
        void LoadContent(ContentLoader contentLoader);
        #endregion
    }
}
