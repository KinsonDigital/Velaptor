namespace Raptor.Content
{
    /// <summary>
    /// Loads data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public interface ILoader<T>
    {
        /// <summary>
        /// Loads data at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to the file with the data to load.</param>
        /// <returns>The data loaded from disk.</returns>
        T Load(string filePath);
    }
}
