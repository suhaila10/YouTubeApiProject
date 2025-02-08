using Microsoft.AspNetCore.Mvc;
using YouTubeApiProject.Services;
using YouTubeApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeApiProject.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeApiService _youtubeService;

        public YouTubeController(YouTubeApiService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        // Display Search Page
        public IActionResult Index()
        {
            ViewData["PageNumber"] = 1;  // Start from page 1
            ViewData["TotalPages"] = 1;  // Default total pages
            return View(new List<YouTubeVideoModel>()); // Pass an empty list initially
        }

        // Handle the search query
        [HttpPost]
        public async Task<IActionResult> Search(string query, string uploadDate, string pageToken, int pageNumber = 1)
        {
            try
            {
                var videos = await _youtubeService.SearchVideosAsync(query, uploadDate, pageToken);

                if (videos == null || !videos.Any())
                {
                    TempData["ErrorMessage"] = "No videos found for your search criteria.";
                    return View("Index", new List<YouTubeVideoModel>());
                }

                // Fetch next and previous page tokens
                string nextPageToken = _youtubeService.NextPageToken ?? "";
                string prevPageToken = _youtubeService.PrevPageToken ?? "";

                // Ensure the correct page number is used (without unnecessary incrementing)
                int currentPage = pageNumber;

                // Store search filters and pagination data
                ViewData["Query"] = query;
                ViewData["UploadDate"] = uploadDate;
                ViewData["NextPageToken"] = nextPageToken;
                ViewData["PrevPageToken"] = prevPageToken;
                ViewData["PageNumber"] = currentPage;
                ViewData["TotalPages"] = 10;  // Example: Set dynamically if available from API

                return View("Index", videos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching videos. Please try again later.";
                return View("Index", new List<YouTubeVideoModel>());
            }
        }
    }
}
