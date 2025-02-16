# WebScraper Project

This project is a web scraping API built using ASP.NET Core that extracts images and analyzes word frequency from a given URL.
Installed Packages:
1) HtmlAgilityPack - Required for scraping images and text)
2) System.Net.Http - Required for fetching webpage HTML)
3) Newtonsoft.Json - Converts data to and from JSON format


1.ImageScraperService.cs :
Used HtmlAgilityPack to efficiently parse HTML and extract <img> tags.
ILogger for logging errors.
Filters duplicate URLs with HashSet, improving performance.


2. Controller: WebScraperController.cs
Dependency injection implemented for ImageScraperService, Logging, Caching and HttpClientFactory.
async and await were used to ensure non-blocking
IMemoryCache that stores results for 10 minutes.
ILogger used for logging errors. 
Precompiled Regex for word extraction to improve efficiency.

Image Extraction API (GetImages) :
Uses ImageScraperService to extract images from a given URL.

Word Count API (GetWordCount) :
Extracts text only from relevant HTML elements.
Uses Regex for fast word extraction, removing special characters.
Ignores common noise words like class, div, menu to improve accuracy.
Returns total word count and top 10 words with their frequencies.

3. site.js
Used async/await with Promise.all() to fetch images and word count concurrently.
Wrapped API calls in try-catch blocks to handle errors gracefully.
Added URL validation before making requests.
Smoothly scrolls to the word count section after fetching results.



