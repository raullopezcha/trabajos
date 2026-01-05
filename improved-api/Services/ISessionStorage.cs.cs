namespace ApiGateServiceLayer.Services
{
    /// <summary>
    /// Interface for session storage operations
    /// </summary>
    public interface ISessionStorage
    {
        /// <summary>
        /// Stores session cookies
        /// </summary>
        /// <param name="cookies">Collection of cookie strings</param>
        void Store(IEnumerable<string> cookies);

        /// <summary>
        /// Retrieves stored session cookies
        /// </summary>
        /// <returns>Cookie string or empty if not found</returns>
        string Retrieve();
    }
}
