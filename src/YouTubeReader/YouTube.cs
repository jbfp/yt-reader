using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace YouTubeReader
{
    public sealed class YouTube
    {
        private const string VideoUrl = "https://www.youtube.com/watch?v={0}";
        private const string CaptionListUrl = "https://video.google.com/timedtext?type=list&v={0}";
        private const string CaptionUrl = "https://www.youtube.com/api/timedtext?v={0}&lang=en";
        private const string NamedCaptionUrl = "https://www.youtube.com/api/timedtext?v={0}&lang=en&name={1}";

        private readonly HttpClient _httpClient;
        private readonly ILogger<YouTube> _logger;

        public YouTube(
            HttpClient httpClient,
            ILogger<YouTube> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> GetVideoTitleAsync(
            string videoId,
            CancellationToken cancellationToken)
        {
            var requestUri = new Uri(
                string.Format(VideoUrl, videoId),
                UriKind.Absolute);

            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                var doc = new HtmlDocument();
                doc.Load(stream);
                return doc.ExtractTitle();
            }

            return null;
        }

        public async Task<string?> GetEnglishCaptionNameAsync(
            string videoId,
            CancellationToken cancellationToken)
        {
            var requestUri = new Uri(
               string.Format(CaptionListUrl, videoId),
               UriKind.Absolute);

            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var xml = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(xml))
                {
                    return null;
                }

                return XElement
                    .Parse(xml)
                    .ExtractEnglishCaptionName();
            }

            return null;
        }

        public async Task<IReadOnlyList<string>?> GetNamedCaptionsAsync(
            string videoId,
            string name,
            CancellationToken cancellationToken)
        {
            var requestUri = new Uri(
                string.Format(NamedCaptionUrl, videoId, name),
                UriKind.Absolute);

            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var xml = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(xml))
                {
                    return null;
                }

                return XElement
                    .Parse(xml)
                    .ExtractCaptions();
            }

            return null;
        }

        public async Task<IReadOnlyList<string>?> GetCaptionsAsync(
            string videoId,
            CancellationToken cancellationToken)
        {
            var requestUri = new Uri(
                string.Format(CaptionUrl, videoId),
                UriKind.Absolute);

            var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var xml = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(xml))
                {
                    return null;
                }

                return XElement
                    .Parse(xml)
                    .ExtractCaptions();
            }

            return null;
        }
    }
}