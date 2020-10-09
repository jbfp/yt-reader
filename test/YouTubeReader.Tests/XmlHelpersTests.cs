using System.ComponentModel;
using System.Xml.Linq;
using Xunit;

namespace YouTubeReader.Tests
{
    [Category("Unit")]
    public class XmlHelpersTests
    {
        [Fact]
        public void ExtractEnglishCaptionName_NoTracks()
        {
            // Arrange            
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript_list docid=""1660711578795754023"">
                </transcript_list>");

            // Act
            var actual = xml.ExtractEnglishCaptionName();

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void ExtractEnglishCaptionName_NoEnglishTracks()
        {
            // Arrange            
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript_list docid=""12888875334951783711"">
                    <track id=""1"" name="""" lang_code=""ko"" lang_original=""한국어"" lang_translated=""Korean""/>
                    <track id=""2"" name="""" lang_code=""lv"" lang_original=""Latviešu"" lang_translated=""Latvian""/>
                    <track id=""3"" name="""" lang_code=""ru"" lang_original=""Русский"" lang_translated=""Russian""/>
                    <track id=""4"" name="""" lang_code=""th"" lang_original=""ไทย"" lang_translated=""Thai""/>
                    <track id=""5"" name="""" lang_code=""tr"" lang_original=""Türkçe"" lang_translated=""Turkish""/>
                </transcript_list>");

            // Act
            var actual = xml.ExtractEnglishCaptionName();

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void ExtractEnglishCaptionName_EnglishTrack()
        {
            // Arrange            
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript_list docid=""12888875334951783711"">
                    <track id=""0"" name=""Default"" lang_code=""en"" lang_original=""English"" lang_translated=""English"" lang_default=""true""/>
                    <track id=""1"" name="""" lang_code=""ko"" lang_original=""한국어"" lang_translated=""Korean""/>
                    <track id=""2"" name="""" lang_code=""lv"" lang_original=""Latviešu"" lang_translated=""Latvian""/>
                    <track id=""3"" name="""" lang_code=""ru"" lang_original=""Русский"" lang_translated=""Russian""/>
                    <track id=""4"" name="""" lang_code=""th"" lang_original=""ไทย"" lang_translated=""Thai""/>
                    <track id=""5"" name="""" lang_code=""tr"" lang_original=""Türkçe"" lang_translated=""Turkish""/>
                </transcript_list>");

            // Act
            var actual = xml.ExtractEnglishCaptionName();

            // Assert
            Assert.Equal("Default", actual);
        }

        [Fact]
        public void ExtractCaptions_NoTexts()
        {
            // Arrange
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript>
                </transcript>");

            // Act
            var actual = xml.ExtractCaptions();

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void ExtractCaptions_OneText()
        {
            // Arrange
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript>
                    <text start=""0.14"" dur=""3.05"">
                        Hi I’m John Green and this is Crash Course European History.
                    </text>
                </transcript>");

            // Act
            var actual = xml.ExtractCaptions();

            // Assert
            Assert.Single(actual, "Hi I’m John Green and this is Crash Course European History.");
        }

        [Fact]
        public void ExtractCaptions_ManyTexts()
        {
            // Arrange
            var xml = XElement.Parse(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <transcript>
                    <text start=""0.14"" dur=""3.05"">
                        Hi I’m John Green and this is Crash Course European History.
                    </text>
                    <text start=""3.19"" dur=""3.839"">
                        So, in the first episode of this series, we talked about the significance of the year
                    </text>
                    <text start=""7.029"" dur=""1"">
                        1431.
                    </text>
                    <text start=""8.029"" dur=""4.881"">
                        Remember, that was the year Joan of Arc was burned to death for heresy and witchcraft
                    </text>
                    <text start=""12.91"" dur=""4.24"">
                        because the English were so bewildered that a teenage peasant girl could lead the French
                    </text>
                    <text>
                        &quot;&quot;
                    </text>
                </transcript>");

            // Act
            var actual = xml.ExtractCaptions();

            // Assert
            var expected = new[]
            {
                "Hi I’m John Green and this is Crash Course European History.",
                "So, in the first episode of this series, we talked about the significance of the year",
                "1431.",
                "Remember, that was the year Joan of Arc was burned to death for heresy and witchcraft",
                "because the English were so bewildered that a teenage peasant girl could lead the French",
                "\"\"",
            };

            Assert.Equal(expected, actual);
        }
    }
}