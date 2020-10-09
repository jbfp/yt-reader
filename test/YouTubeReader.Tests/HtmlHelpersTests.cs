using System.ComponentModel;
using HtmlAgilityPack;
using Xunit;

namespace YouTubeReader.Tests
{
    [Category("Unit")]
    public class HtmlHelpersTests
    {
        [Fact]
        public void ExtractTitle_NoTitleElement()
        {
            // Arrange
            var html = new HtmlDocument();
            html.LoadHtml(@"
                <!DOCTYPE html>
                <html>
                    <head>
                    </head>
                    <body>
                    </body>
                </html>");

            // Act
            var actual = html.ExtractTitle();

            // Assert
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("Sheep Discovers How To Use A Trampoline")]
        [InlineData("Witchcraft: Crash Course European History #10")]
        public void ExtractTitle_TitleElement(string expected)
        {
            // Arrange
            var html = new HtmlDocument();
            html.LoadHtml($@"
                <!DOCTYPE html>
                <html>
                    <head>
                        <title>{expected}</title>
                    </head>
                    <body>
                    </body>
                </html>");

            // Act
            var actual = html.ExtractTitle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(" - YouTube", "")]
        [InlineData("hello - YouTube", "hello")]
        [InlineData("hello - YouTube - YouTube", "hello - YouTube")]
        [InlineData("Sheep Discovers How To Use A Trampoline - YouTube", "Sheep Discovers How To Use A Trampoline")]
        public void ExtractTitle_RemovesYouTubeSuffix(string title, string expected)
        {
            // Arrange
            var html = new HtmlDocument();
            html.LoadHtml($@"
                <!DOCTYPE html>
                <html>
                    <head>
                        <title>{title}</title>
                    </head>
                    <body>
                    </body>
                </html>");

            // Act
            var actual = html.ExtractTitle();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
