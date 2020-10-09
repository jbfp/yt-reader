using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace YouTubeReader.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public sealed class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty, Required]
        public string? IdOrUrl { get; set; }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Input: {IdOrUrl}", IdOrUrl);
            var videoId = ParseIdOrUrl(IdOrUrl ?? string.Empty);
            _logger.LogInformation("Parsed video ID {VideoId}", videoId);
            return RedirectToPage("Transcript", new { videoId });
        }

        public static string ParseIdOrUrl(string idOrUrl)
        {
            if (string.IsNullOrWhiteSpace(idOrUrl))
            {
                throw new ArgumentException($"'{nameof(idOrUrl)}' cannot be null or whitespace", nameof(idOrUrl));
            }

            string videoId;

            var uriBuilder = new UriBuilder(idOrUrl);

            if (uriBuilder.Host == "youtu.be")
            {
                videoId = uriBuilder.Path.TrimStart('/');
            }
            else if (uriBuilder.Host == "youtube.com" || uriBuilder.Host == "www.youtube.com")
            {
                videoId = HttpUtility.ParseQueryString(uriBuilder.Query).Get("v");
            }
            else
            {
                videoId = idOrUrl;
            }

            return videoId;
        }
    }
}
