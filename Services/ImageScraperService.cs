using HtmlAgilityPack;
using System.Net.Http;
using WebScraper_Project.Services;

namespace WebScraper_Project.Services
{
    /// <summary>
    /// Service responsible for extracting image URLs from a given URL
    /// </summary>
    public class ImageScraperService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageScraperService> _logger;

        public ImageScraperService(IHttpClientFactory httpClientFactory, ILogger<ImageScraperService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Fetches and extracts all image URLs from the given webpage.
        /// </summary>
        public async Task<List<string>> GetImageUrlsAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogWarning("Empty or null URL provided to GetImageUrlsAsync.");
                return [];
            }
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetStringAsync(url).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning($"Received empty response from {url}");
                    return [];
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                // Extract image URLs from <img> html tags
                var images = doc.DocumentNode.SelectNodes("//img")
                              ?.Select(img => img.GetAttributeValue("src", "").Trim()) // Get src attribute
                              .Where(src => !string.IsNullOrEmpty(src)) //Remove empty URLs
                              .ToHashSet() //Remove duplicates
                              .ToList() ?? [];

                _logger.LogInformation($"Extracted {images.Count} images from {url}");
                return images;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while scraping images: {ex.Message}");
                return [];
            }
           
        }
    }
}



