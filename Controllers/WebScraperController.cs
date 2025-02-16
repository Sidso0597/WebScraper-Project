using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using WebScraper_Project.Services;

namespace WebScraper_Project.Controllers
{
    public class WebScraperController : Controller
    {
        private readonly ImageScraperService _imageScraper;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WebScraperController> _logger;

        public WebScraperController(ImageScraperService imageScraper, IMemoryCache cache, ILogger<WebScraperController> logger)
        {
            _imageScraper = imageScraper;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// API to fetch images from a given URL.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetImages([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL cannot be empty.");

            if (_cache.TryGetValue(url, out List<string>? cachedImages))
                return Ok(cachedImages);

            try
            {
                var images = await _imageScraper.GetImageUrlsAsync(url);
                _cache.Set(url, images, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
                return Ok(images);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in API: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// API to get total word count & top 10 occurring words.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWordCount([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL cannot be empty.");

            string cacheKey = $"WordCount_{url}";
            if (_cache.TryGetValue(cacheKey, out object? cachedData))
                return Json(cachedData);

            try
            {
                var web = new HtmlWeb();
                var doc = await Task.Run(() => web.Load(url));

                // Extract meaningful content (paragraphs, headings, list items)
                var textNodes = doc.DocumentNode.SelectNodes("//p | //h1 | //h2 | //h3 | //li | //span");

                if (textNodes == null)
                    return Json(new { TotalWords = 0, TopWords = new List<object>() });

                string fullText = string.Join(" ", textNodes.Select(node => node.InnerText));
                // Split words, remove special characters, convert to lowercase
                var regex = new Regex(@"\W+", RegexOptions.Compiled);
                var words = regex.Split(fullText)
                                 .Where(w => w.Length > 2)
                                 .Select(w => w.ToLower())
                                 .ToList();

                var ignoredWords = new HashSet<string>
                {
                    "class", "target", "button", "menu", "link", "div", "span",
                    "text", "click", "image", "search", "submit", "nav", "html"
                };

                var topWords = words.Where(w => !ignoredWords.Contains(w))
                                    .GroupBy(w => w)
                                    .OrderByDescending(g => g.Count())
                                    .Take(10)
                                    .Select(g => new { Word = g.Key, Count = g.Count() })
                                    .ToList();
                                    
                var result = new { TotalWords = words.Count, TopWords = topWords };

                _cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing word count.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
