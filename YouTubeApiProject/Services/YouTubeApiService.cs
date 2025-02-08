using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YouTubeApiProject.Models;

namespace YouTubeApiProject.Services
{
    public class YouTubeApiService
    {
        private readonly string _apiKey;

        public string NextPageToken { get; private set; }
        public string PrevPageToken { get; private set; }

        public YouTubeApiService(IConfiguration configuration)
        {
            _apiKey = configuration["YouTubeApiKey"]; // Fetch API key from appsettings.json
        }

        public async Task<List<YouTubeVideoModel>> SearchVideosAsync(string query, string uploadDate, string pageToken)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _apiKey,
                    ApplicationName = "YoutubeProject"
                });

                var searchRequest = youtubeService.Search.List("snippet");
                searchRequest.Q = query;
                searchRequest.MaxResults = 30;

                // Apply filter for upload date (if selected)
                if (!string.IsNullOrEmpty(uploadDate))
                {
                    if (uploadDate == "today")
                    {
                        var startOfTodayUtc = DateTime.UtcNow.Date;
                        searchRequest.PublishedAfterDateTimeOffset = startOfTodayUtc;
                    }
                    if (uploadDate == "this_week")
                    {
                        searchRequest.PublishedAfterDateTimeOffset = DateTime.UtcNow.AddDays(-7);
                    }
                    if (uploadDate == "this_month")
                    {
                        searchRequest.PublishedAfterDateTimeOffset = DateTime.UtcNow.AddMonths(-1);
                    }
                }

                // Handle pagination
                if (!string.IsNullOrEmpty(pageToken))
                {
                    searchRequest.PageToken = pageToken;
                }

                var searchResponse = await searchRequest.ExecuteAsync();

                // Store pagination tokens
                NextPageToken = searchResponse.NextPageToken;
                PrevPageToken = searchResponse.PrevPageToken;

                var videos = searchResponse.Items.Select(item => new YouTubeVideoModel
                {
                    Title = item.Snippet.Title,
                    Description = item.Snippet.Description,
                    ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                    VideoUrl = "https://www.youtube.com/watch?v=" + item.Id.VideoId,
                    PublishedDate = item.Snippet.PublishedAt?.ToUniversalTime()
                }).ToList();

                return videos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching YouTube data", ex);
            }
        }
    }
}
