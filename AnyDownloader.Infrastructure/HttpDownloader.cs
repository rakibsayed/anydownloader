using System.Net.Http;
using AnyDownloader.Core.Interfaces;

namespace AnyDownloader.Infrastructure
{
    /// <summary>
    /// Handles downloading files over HTTP.
    /// </summary>
    public class HttpDownloader : IHttpDownloader
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDownloader"/> class.
        /// </summary>
        public HttpDownloader()
        {
            _httpClient = new HttpClient();
        }

        public async Task DownloadFileAsync(string url, string destination)
        {
            // Validate the URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException($"Invalid URL: {url}");
            }

            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var fileStream = new FileStream(destination, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
