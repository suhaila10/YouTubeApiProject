using Microsoft.AspNetCore.Mvc;
using YouTubeApiProject.Models;
using YouTubeApiProject.Services;

namespace YouTubeApiProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly YouTubeApiService _youtubeService;

        public HomeController(YouTubeApiService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<IActionResult> Index()
        {
            // Get Trending Videos and Music
            var trendingVideos = await _youtubeService.GetTrendingVideosAsync();
            var trendingMusic = await _youtubeService.GetTrendingMusicAsync();

            // Combine both into a single model
            var model = new Tuple<List<YouTubeVideoModel>, List<YouTubeVideoModel>>(trendingVideos, trendingMusic);

            // Pass the model to the view
            return View(model);
        }
    }
}
