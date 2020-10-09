using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace YouTubeReader.Pages
{
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    public sealed class TranscriptModel : PageModel
    {
        private readonly YouTube _yt;
        private readonly ILogger<TranscriptModel> _logger;

        public TranscriptModel(
            YouTube yt,
            ILogger<TranscriptModel> logger)
        {
            _yt = yt;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true), Required]
        public string? VideoId { get; set; }

        public string? VideoTitle { get; set; }
        public IReadOnlyList<string>? Texts { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("VideoId: {VideoId}", VideoId);

            var videoId = VideoId ?? string.Empty;

            await Task.WhenAll(
                GetVideoTitle(videoId, cancellationToken),
                GetCaptions(videoId, cancellationToken));
        }

        private async Task GetVideoTitle(
            string videoId,
            CancellationToken cancellationToken)
        {
            VideoTitle = await _yt.GetVideoTitleAsync(videoId, cancellationToken);
        }

        private async Task GetCaptions(
            string videoId,
            CancellationToken cancellationToken)
        {
            var captions = await _yt.GetCaptionsAsync(videoId, cancellationToken);

            if (captions == null)
            {
                var name = await _yt.GetEnglishCaptionNameAsync(videoId, cancellationToken);

                _logger.LogInformation("Captions name: {Name}", name);

                if (name == null)
                {
                    return;
                }

                captions = await _yt.GetNamedCaptionsAsync(videoId, name, cancellationToken);
            }

            Texts = captions;
        }
    }
}
