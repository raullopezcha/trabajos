namespace ApiGateServiceLayer.Services
{
    /// <summary>
    /// Interface for SAP Service Layer client operations
    /// </summary>
    public interface IServiceLayerClient
    {
        /// <summary>
        /// Ensures an active session exists with Service Layer
        /// </summary>
        Task EnsureSessionAsync();

        /// <summary>
        /// Executes a GET request to Service Layer
        /// </summary>
        /// <param name="path">Endpoint path</param>
        /// <returns>HTTP response from Service Layer</returns>
        Task<HttpResponseMessage> GetAsync(string path);

        /// <summary>
        /// Executes a POST request to Service Layer
        /// </summary>
        /// <param name="path">Endpoint path</param>
        /// <param name="payload">Request payload</param>
        /// <returns>HTTP response from Service Layer</returns>
        Task<HttpResponseMessage> PostAsync(string path, object payload);
    }
}
