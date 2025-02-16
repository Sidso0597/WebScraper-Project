# WebScraper Project
To run the project:
1. Clone the project in Visual Studio.
1. Launch solution in *Administrator mode* into Visual Studio to avoid warnings wrt dependencies.
2. For local testing, the application should be run using IIS Express.


This project is a web scraping API built using ASP.NET Core that extracts images and word frequency from a given URL.
# Installed Packages
1) HtmlAgilityPack - Required for scraping images and text)
2) System.Net.Http - Required for fetching webpage HTML)
3) Newtonsoft.Json - Converts data to and from JSON format


# ImageScraperService.cs
1. Used HtmlAgilityPack to efficiently parse HTML and extract <img> tags.
2. ILogger for logging errors.
3. Filters duplicate URLs with HashSet, improving performance.

# WebScraperController.cs
1. Dependency injection implemented for ImageScraperService, Logging, Caching and HttpClientFactory.
2. async and await were used to ensure non-blocking
3. IMemoryCache that stores results for 10 minutes.
4. ILogger used for logging errors. 
5. Precompiled Regex for word extraction to improve efficiency.

# Image Extraction API (GetImages) :
Uses ImageScraperService to extract images from a given URL.

# Word Count API (GetWordCount) :
1. Extracts text only from relevant HTML elements.
2. Uses Regex for fast word extraction, removing special characters.
3. Ignores common noise words like class, div, menu to improve accuracy.
4. Returns total word count and top 10 words with their frequencies.

# site.js
1. Used async/await with Promise.all() to fetch images and word count concurrently.
2. Wrapped API calls in try-catch blocks to handle errors gracefully.
3. Added URL validation before making requests.
4. Smoothly scrolls to the word count section after fetching results.



