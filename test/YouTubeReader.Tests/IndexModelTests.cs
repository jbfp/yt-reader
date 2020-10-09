using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using YouTubeReader.Pages;

namespace YouTubeReader.Tests
{
    [Category("Unit")]
    public class IndexModelTests
    {
        private readonly IndexModel _sut;

        public IndexModelTests()
        {
            _sut = new IndexModel(NullLogger<IndexModel>.Instance);
        }

        [Fact]
        public void OnPost_InvalidModelState_ReturnsPage()
        {
            // Arrange
            _sut.ModelState.AddModelError(nameof(_sut.IdOrUrl), "test");

            // Act
            var actual =  _sut.OnPost();

            // Assert
            Assert.IsType<PageResult>(actual);
        }

        [Fact]
        public void OnPost_ValidModelState_RedirectsToTranscript()
        {
            // Arrange
            _sut.IdOrUrl = "youtu.be/hello";

            // Act
            var actual =  _sut.OnPost();

            // Assert
            var result = Assert.IsType<RedirectToPageResult>(actual);
            Assert.Equal("Transcript", result.PageName);
            Assert.Equal("hello", result.RouteValues["videoId"]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ParseIdOrUrl_Arg_CannotBeNullOrWhitespace(string idOrUrl)
        {
            Assert.Throws<ArgumentException>(() => IndexModel.ParseIdOrUrl(idOrUrl));
        }

        [Theory]
        [InlineData("youtu.be/hello", "hello")]
        [InlineData("youtube.com/?v=test2", "test2")]
        [InlineData("www.youtube.com/watch?v=hello", "hello")]
        [InlineData("www.youtube.com/?v=test3", "test3")]
        [InlineData("https://www.youtube.com/watch?v=test4", "test4")]
        [InlineData("https://youtube.com/watch?v=test5", "test5")]
        [InlineData("ofiwejfoiwefo", "ofiwejfoiwefo")]
        [InlineData("http://example.org", "http://example.org")]
        [InlineData("rW4XFiHUQAs", "rW4XFiHUQAs")]
        public void ParseIdOrUrl_CanParse(string idOrUrl, string expected)
        {
            Assert.Equal(expected, IndexModel.ParseIdOrUrl(idOrUrl));
        }
    }
}