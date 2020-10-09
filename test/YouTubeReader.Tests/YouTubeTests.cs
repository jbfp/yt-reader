using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace YouTubeReader.Tests
{
    [Category("Unit")]
    public class YouTubeTests
    {
        private readonly HttpMessageHandlerStub _server = new HttpMessageHandlerStub();
        private readonly CancellationToken _ct = CancellationToken.None;
        private readonly YouTube _sut;

        public YouTubeTests()
        {
            var httpClient = new HttpClient(_server);
            var logger = NullLogger<YouTube>.Instance;
            _sut = new YouTube(httpClient, logger);
        }

        [Fact]
        public async Task GetVideoTitle_NotFound()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Act
            var actual = await _sut.GetVideoTitleAsync("videoId", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetVideoTitle_Ok()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <title>title - YouTube</title>
                        </head>
                        <body></body>
                    </html>", Encoding.UTF8, "text/html"),
            };

            // Act
            var actual = await _sut.GetVideoTitleAsync("videoId", _ct);

            // Assert
            Assert.Equal("title", actual);
        }

        [Fact]
        public async Task GetEnglishCaptionName_NotFound()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Act
            var actual = await _sut.GetEnglishCaptionNameAsync("videoId", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetEnglishCaptionName_Empty()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetEnglishCaptionNameAsync("videoId", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetEnglishCaptionName_Ok()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <transcript_list docid=""12888875334951783711"">
                        <track id=""0"" name=""Default"" lang_code=""en"" lang_original=""English"" lang_translated=""English"" lang_default=""true""/>
                    </transcript_list>", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetEnglishCaptionNameAsync("videoId", _ct);

            // Assert
            Assert.Equal("Default", actual);
        }

        [Fact]
        public async Task GetNamedCaptions_NotFound()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Act
            var actual = await _sut.GetNamedCaptionsAsync("videoId", "name", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetNamedCaptions_Empty()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetNamedCaptionsAsync("videoId", "name", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetNamedCaptions_Ok()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <transcript>
                        <text start=""0.14"" dur=""3.05"">
                            testing the testt test extstet
                        </text>
                    </transcript>", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetNamedCaptionsAsync("videoId", "name", _ct);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task GetCaptions_NotFound()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Act
            var actual = await _sut.GetCaptionsAsync("videoId", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetCaptions_Empty()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetCaptionsAsync("videoId", _ct);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetCaptions_Ok()
        {
            // Arrange
            _server.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <transcript>
                        <text start=""0.14"" dur=""3.05"">
                            testing the testt test extstet
                        </text>
                    </transcript>", Encoding.UTF8, "application/xml"),
            };

            // Act
            var actual = await _sut.GetCaptionsAsync("videoId", _ct);

            // Assert
            Assert.NotEmpty(actual);
        }

        private class HttpMessageHandlerStub : HttpMessageHandler
        {
            public HttpResponseMessage Response { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(Response);
            }
        }
    }
}