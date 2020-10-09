using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using YouTubeReader.Pages;

namespace YouTubeReader.Tests
{
    [Category("Integration")]
    public class TranscriptModelTests
    {
        private readonly TranscriptModel _sut;

        public TranscriptModelTests()
        {
            _sut = new TranscriptModel(
                new YouTube(
                    new HttpClient(),
                    NullLogger<YouTube>.Instance
                ),
                NullLogger<TranscriptModel>.Instance);
        }

        [Fact]
        public async Task OnGetAsync_InvalidVideoId_Works()
        {
            // Arrange
            _sut.VideoId = "tweutwetdtwdoitjwdotijwdotijwdt";

            // Act
            await _sut.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.Null(_sut.VideoTitle);
            Assert.Null(_sut.Texts);
        }

        [Fact]
        public async Task OnGetAsync_ValidVideoId_DefaultCaptions_Works()
        {
            // Arrange
            _sut.VideoId = "rW4XFiHUQAs";

            // Act
            await _sut.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.Equal("Witchcraft: Crash Course European History #10", _sut.VideoTitle);
            Assert.NotEmpty(_sut.Texts);
        }

        [Fact]
        public async Task OnGetAsync_ValidVideoId_EnglishCaptions_Works()
        {
            // Arrange
            _sut.VideoId = "st571DYYTR8";

            // Act
            await _sut.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.Equal("The Ultimate French Press Technique", _sut.VideoTitle);
            Assert.NotEmpty(_sut.Texts);
        }
    }
}